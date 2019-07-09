using System;
using Assets._TwoFatCatsAssets.Scripts.Inventory;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Assets._TwoFatCatsAssets.Scripts.UI;

namespace Assets._TwoFatCatsAssets.Scripts.Tools
{
    [Serializable]
    public class ItemSlotUiDictionary : SerializableDictionary<int, ItemSlotUi>
    {
    }

    [Serializable]
    public class ItemSlotDictionary : SerializableDictionary<int, Slot>
    {
    }

    [Serializable]
    public class EquipmentSlotReferenceDictionary : SerializableDictionary<ItemSlotType, EquipmentSlotReference>
    {
    }
}
