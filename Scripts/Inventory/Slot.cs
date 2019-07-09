using System;
using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Mirror;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Inventory
{
    [Serializable]
    public struct ItemSlotData
    {
        public ItemSlotType SlotType;
        public ItemData OccupantItemData;
        public uint ParentNetId;
        public int SlotIndex;

        public bool Occupant()
        {
            return OccupantItemData.ItemId > 0;
        }
    }

    [Serializable]
    public class SyncDictionaryItemSlot : SyncDictionary<int, ItemSlotData> { }

    [Serializable]
    public enum ItemSlotType
    {
        Any,
        Helmet,
        Torso,
        Legs,
        MainHand,
        OffHand
    }

    [Serializable]
    public class Slot
    {
        [ReadOnly] public Transform SlotTransform;
        [ReadOnly] public ItemSlotData SlotData;
        [ReadOnly] public int SlotIndex;
        [ReadOnly] public Item ItemReference;

        public Slot(Transform slotTransform, ItemSlotData slotData, int slotIndex)
        {
            SlotTransform = slotTransform;
            SlotData = slotData;
            SlotIndex = slotIndex;
            if (!SlotData.Occupant()) return;
            GameManager.ItemDictionary.TryGetValue(SlotData.OccupantItemData.ItemId, out var itemObject);
            if (itemObject == null) return;
            ItemReference = itemObject.GetComponent<Item>();
        }

        public virtual void OnSlotChange()
        {
            Debug.Log("OnSlotChanged");
            if (!SlotData.Occupant())
            {
                ItemReference = null;
                return;
            }
            GameManager.ItemDictionary.TryGetValue(SlotData.OccupantItemData.ItemId, out var itemObject);
            if (itemObject == null) return;
            ItemReference = itemObject.GetComponent<Item>();
        }
    }
}