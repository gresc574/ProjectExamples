using Assets._TwoFatCatsAssets.Scripts.Inventory;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._TwoFatCatsAssets.Scripts.UI
{
    public class ItemSlotUi : MonoBehaviour
    {
        public GameObject ItemUiPrefab;
        public Image SlotImage;
        public int SlotIndex;
        public Inventory.Inventory AttachedInventory;
        private ItemUi _itemUi;
        public Item ItemReference; // ideally won't need this

        private ItemSlotData _slotData;
        public ItemSlotData SlotData
        {
            get { return _slotData; }
            set
            {
                if (_itemUi) Destroy(_itemUi.gameObject);
                _slotData = value;
                if (!SlotData.Occupant()) return;
                InstantiateItemUi(_slotData);
            }
        }

        public static ItemSlotUi HoverItemSlot { get; set; }

        private void InstantiateItemUi(ItemSlotData slotData)
        {
            _itemUi = Instantiate(ItemUiPrefab, transform).GetComponent<ItemUi>();
            _itemUi.ItemData = _slotData.OccupantItemData;
            _itemUi.AttachedItemSlotUi = this;

            GameManager.ItemDictionary.TryGetValue(SlotData.OccupantItemData.ItemId, out var itemObject);
            if (itemObject == null) return;
            ItemReference = itemObject.GetComponent<Item>();
            _itemUi.ItemReference = ItemReference;
        }
    }
}
