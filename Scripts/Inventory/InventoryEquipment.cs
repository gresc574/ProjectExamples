using System;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Assets._TwoFatCatsAssets.Scripts.Tools;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Inventory
{
    [Serializable]
    public class EquipmentSlotReference
    {
        public Transform SlotTransform;
    }

    public class InventoryEquipment : Inventory
    {
        public EquipmentSlotReferenceDictionary SlotReferences = new EquipmentSlotReferenceDictionary();

        protected override void OnSlotChanged(SyncDictionaryItemSlot.Operation op, int key, ItemSlotData itemSlotData)
        {
            base.OnSlotChanged(op, key, itemSlotData);
            if (itemSlotData.SlotType == ItemSlotType.Any) return;
            SlotReferences.TryGetValue(itemSlotData.SlotType, out var equipmentSlotReference);
            InventorySlots.TryGetValue(key, out var itemSlot);
            var equipableSlot = itemSlot as SlotEquipable;
            switch (op)
            {
                case SyncDictionaryItemSlot.Operation.OP_ADD:
                    InventorySlots.Add(key, new SlotEquipable(equipmentSlotReference.SlotTransform, itemSlotData, key));
                    return;
                case SyncDictionaryItemSlot.Operation.OP_REMOVE:
                    InventorySlots.Remove(key);
                    return;
            }
            if (equipableSlot == null) return;
            equipableSlot.SlotData = itemSlotData;
            equipableSlot.OnSlotChange();

            EquipmentSlotChanged(itemSlotData, equipableSlot);
        }

        private void EquipmentSlotChanged(ItemSlotData itemData, SlotEquipable equipableSlot)
        {
            // destroy localEquip if Exists
            if (equipableSlot.CurrentEquip)
            {
                Destroy(equipableSlot.CurrentEquip.gameObject);
                equipableSlot.CurrentEquip = null;
            }
            var equipableItem = equipableSlot.ItemReference as ItemEquipable;
            if (equipableItem == null) return;
            equipableSlot.CurrentEquip = Instantiate(equipableItem.EquipItem, equipableSlot.SlotTransform);
            equipableSlot.CurrentEquip.transform.localPosition = Vector3.zero;
        }

        protected override void InitilizeInventorySlots()
        {
            base.InitilizeInventorySlots();
            var count = 0;
            foreach (var equipmentSlot in SlotReferences)
            {
                var equipableSlot = new SlotEquipable(equipmentSlot.Value.SlotTransform, InventorySlotData[count + Capacity], count + Capacity);
                InventorySlots.Add(count + Capacity, equipableSlot);
                count++;
                // call the hook function or it won't initilize the LocalEquips
                if (equipableSlot.SlotData.Occupant()) EquipmentSlotChanged(equipableSlot.SlotData, equipableSlot);
            }
        }

        protected override void InitilizeItemSlots()
        {
            base.InitilizeItemSlots();
            var count = 0;
            foreach (var equipmentSlot in SlotReferences)
            {
                var itemslotData = new ItemSlotData
                {
                    ParentNetId = AttachedNetworkIdentity.netId,
                    SlotIndex = InventorySlotData.Count + count,
                    SlotType = equipmentSlot.Key
                };
                InventorySlotData.Add(count + Capacity, itemslotData);
                count++;
            }
        }
    }
}
