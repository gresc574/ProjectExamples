using Assets._TwoFatCatsAssets.Scripts.Inventory;
using Assets._TwoFatCatsAssets.Scripts.Tools;
using Unity.Collections;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.UI
{
    public class InventoryCanvas : MonoBehaviour
    {
        [Tooltip("Assign this in editor")] public GameObject SlotUiPrefab;
        [Tooltip("Assign this in editor")] public Transform SlotsTransform;
        [ReadOnly] public ItemSlotUiDictionary SlotUis = new ItemSlotUiDictionary();

        private Inventory.Inventory _inventory;
        public Inventory.Inventory Inventory
        {
            get { return _inventory; }
            set
            {
                foreach (Transform slotImage in SlotsTransform)
                {
                    Destroy(slotImage.gameObject);
                }
                SlotUis.Clear();
                if (_inventory) _inventory.InventorySlotData.Callback -= OnSlotChanged;
                _inventory = value;
                if (_inventory) _inventory.InventorySlotData.Callback += OnSlotChanged;

                for (int i = 0; i < _inventory.InventorySlotData.Count; i++)
                {
                    if (_inventory.InventorySlotData[i].SlotType != ItemSlotType.Any) continue;
                    var slotUi = Instantiate(SlotUiPrefab, SlotsTransform).GetComponent<ItemSlotUi>();
                    slotUi.SlotData = _inventory.InventorySlotData[i];
                    slotUi.SlotIndex = i;
                    slotUi.AttachedInventory = Inventory;
                    SlotUis.Add(i, slotUi);
                }
            }
        }

        public void OnSlotChanged(SyncDictionaryItemSlot.Operation op, int inventoryIndex, ItemSlotData itemSlotData)
        {
            if (itemSlotData.SlotType != ItemSlotType.Any) return; // return if equipment slot
            SlotUis.TryGetValue(inventoryIndex, out var slotUi);
            if (slotUi == null) return;
            slotUi.SlotData = itemSlotData;
        }
    }
}
