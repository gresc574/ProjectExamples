using System.Collections;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Assets._TwoFatCatsAssets.Scripts.Tools;
using Mirror;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Inventory
{
    public class Inventory : NetworkBehaviour
    {
        public int Capacity = 12;

        public SyncDictionary<int, ItemSlotData> InventorySlotData = new SyncDictionaryItemSlot();
        public ItemSlotDictionary InventorySlots = new ItemSlotDictionary();
        private NetworkIdentity _attachedNetworkIdentity;
        public NetworkIdentity AttachedNetworkIdentity
        {
            get
            {
                if (_attachedNetworkIdentity == null) _attachedNetworkIdentity = GetComponent<NetworkIdentity>();
                return _attachedNetworkIdentity;
            }
            set { _attachedNetworkIdentity = value; }
        }
        protected Transform _inventoryTransform;

        public override void OnStartClient()
        {
            base.OnStartClient();
            InventorySlotData.Callback += OnSlotChanged;
            _inventoryTransform = transform;

            // init item slots
            if (isServer) InitilizeItemSlots();
            if (!isServer) StartCoroutine(WaitForNetworkSlots());
        }

        private IEnumerator WaitForNetworkSlots()
        {
            var wait = new WaitForEndOfFrame();
            while (InventorySlotData.Count < Capacity)
            {
                yield return wait;
            }
            InitilizeInventorySlots();
        }

        [Server]
        protected virtual void InitilizeItemSlots()
        {
            for (int i = 0; i < Capacity; i++)
            {
                var itemSlotData = new ItemSlotData
                {
                    ParentNetId = AttachedNetworkIdentity.netId,
                    SlotIndex = i,
                    SlotType = ItemSlotType.Any
                };
                InventorySlotData.Add(i, itemSlotData);
            }
        }

        protected virtual void InitilizeInventorySlots()
        {
            for (int i = 0; i < Capacity; i++)
            {
                InventorySlots.Add(i, new Slot(_inventoryTransform, InventorySlotData[i], i));
            }
        }

        protected virtual void OnSlotChanged(SyncDictionaryItemSlot.Operation op, int key, ItemSlotData itemSlotData)
        {
            if (itemSlotData.SlotType != ItemSlotType.Any) return;
            InventorySlots.TryGetValue(key, out var itemSlot);
            switch (op)
            {
                case SyncDictionaryItemSlot.Operation.OP_ADD:
                    InventorySlots.Add(key, new Slot(_inventoryTransform, itemSlotData, key));
                    return;
                case SyncDictionaryItemSlot.Operation.OP_REMOVE:
                    InventorySlots.Remove(key);
                    return;
            }
            if (itemSlot == null) return;
            itemSlot.SlotData = itemSlotData;
            itemSlot.OnSlotChange();
        }

        [Command]
        public void CmdAddItem(ItemData data, uint index, uint itemNetId)
        {
            var intIndex = (int)index;

            var changedSlot = InventorySlotData[intIndex];
            changedSlot.OccupantItemData = data;
            InventorySlotData[intIndex] = changedSlot;

            NetworkIdentity.spawned.TryGetValue(itemNetId, out var itemIdentity);
            if (itemIdentity != null) NetworkServer.Destroy(itemIdentity.gameObject);
        }

        [Command]
        public void CmdDropItem(uint currentInventoryId, uint currentIndex)
        {
            var currentIndexInt = (int)currentIndex;

            NetworkIdentity.spawned.TryGetValue(currentInventoryId, out var currentNetworkIdentity);
            if (currentNetworkIdentity == null) return;
            var currentInventory = currentNetworkIdentity.GetComponent<Inventory>();
            if (currentInventory == null) return;


            if (!currentInventory.InventorySlotData[currentIndexInt].Occupant()) return;
            var changedSlot = currentInventory.InventorySlotData[currentIndexInt];
            var data = changedSlot.OccupantItemData;
            changedSlot.OccupantItemData = new ItemData();
            currentInventory.InventorySlotData[currentIndexInt] = changedSlot;

            GameManager.ItemDictionary.TryGetValue(data.ItemId, out var itemToSpawn);

            if (!itemToSpawn) return;
            itemToSpawn = Instantiate(itemToSpawn);
            itemToSpawn.GetComponent<Item>().ItemData = data;
            NetworkServer.Spawn(itemToSpawn);
        }

        [Command]
        public void CmdMoveItemInternal(uint currentIndex, uint newIndex)
        {
            var currentIndexInt = (int)currentIndex;
            var newIndexInt = (int)newIndex;

            var currentSlot = InventorySlotData[currentIndexInt];
            var newSlot = InventorySlotData[newIndexInt];
            var currentSlotOccupant = currentSlot.OccupantItemData;

            // swap items around
            var newSlotOccupant = newSlot.OccupantItemData;
            currentSlot.OccupantItemData = newSlotOccupant;
            newSlot.OccupantItemData = currentSlotOccupant;

            InventorySlotData[currentIndexInt] = currentSlot;
            InventorySlotData[newIndexInt] = newSlot;
        }

        [Command]
        public void CmdMoveItemExternal(uint currentIndex, uint newIndex, uint currentNetId, uint targetNetId)
        {
            var currentIndexInt = (int)currentIndex;
            var newIndexInt = (int)newIndex;

            NetworkIdentity.spawned.TryGetValue(targetNetId, out var targetNetworkIdentity);
            NetworkIdentity.spawned.TryGetValue(currentNetId, out var currentNetworkIdentity);
            if (targetNetworkIdentity == null) return;
            if (currentNetworkIdentity == null) return;
            var currentInventory = currentNetworkIdentity.GetComponent<Inventory>();
            var targetInventory = targetNetworkIdentity.gameObject.GetComponent<Inventory>();
            if (targetInventory == null) return;
            if (currentInventory == null) return;

            var currentSlot = currentInventory.InventorySlotData[currentIndexInt];
            var targetSlot = targetInventory.InventorySlotData[newIndexInt];
            var currentSlotOccupant = currentSlot.OccupantItemData;

            // swap items around
            var newSlotOccupant = targetSlot.OccupantItemData;
            currentSlot.OccupantItemData = newSlotOccupant;
            targetSlot.OccupantItemData = currentSlotOccupant;

            currentInventory.InventorySlotData[currentIndexInt] = currentSlot;
            targetInventory.InventorySlotData[newIndexInt] = targetSlot;
        }

        public void LocalUseItem(uint currentIndex)
        {
            var index = (int)currentIndex;
            var slot = InventorySlots[index];
            var item = slot.ItemReference;
            item.UseItem(InventorySlotData[index].OccupantItemData, currentIndex, this);
        }

        [Command]
        public void CmdFinishedUseItem(uint currentInventoryId, uint currentIndex)
        {
            var index = (int)currentIndex;
            NetworkIdentity.spawned.TryGetValue(currentInventoryId, out var currentNetworkIdentity);
            if (currentNetworkIdentity == null) return;
            var currentInventory = currentNetworkIdentity.GetComponent<Inventory>();
            if (currentInventory == null) return;
            var slot = currentInventory.InventorySlots[index];
            var item = slot.ItemReference;
            item.FinishedUseItem(InventorySlotData[index].OccupantItemData);
        }

        [Command]
        public void CmdFinishedUseBuildableItem(uint currentInventoryId, uint currentIndex, Vector3 buildPosition)
        {
            var index = (int)currentIndex;
            NetworkIdentity.spawned.TryGetValue(currentInventoryId, out var currentNetworkIdentity);
            if (currentNetworkIdentity == null) return;
            var currentInventory = currentNetworkIdentity.GetComponent<Inventory>();
            if (currentInventory == null) return;

            var slot = currentInventory.InventorySlots[index];
            var item = slot.ItemReference;
            var buildableItem = item as ItemBuildable;
            buildableItem.FinishedBuildItem(InventorySlotData[index].OccupantItemData, buildPosition);
        }
    }
}