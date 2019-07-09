
using System;
using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Inventory;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Assets._TwoFatCatsAssets.Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._TwoFatCatsAssets.Scripts.UI
{
    public class EquipmentCanvas : MonoBehaviour
    {
        [Tooltip("Assign this in editor")] public GameObject SlotUiPrefab;
        [Space(20)]
        [ReadOnly] public ItemSlotUiDictionary SlotUis = new ItemSlotUiDictionary();
        [Tooltip("Assign this in the Inspector")] public Transform HelmetTransform;
        [Tooltip("Assign this in the Inspector")] public Transform TorsoTransform;
        [Tooltip("Assign this in the Inspector")] public Transform LegsTransform;
        [Tooltip("Assign this in the Inspector")] public Transform MainHandTransform;
        [Tooltip("Assign this in the Inspector")] public Transform OffHandTransform;
        [Space(10)]
        [Tooltip("Assign this in the Inspector")] public Sprite HelmetSprite;
        [Tooltip("Assign this in the Inspector")] public Sprite TorsoSprite;
        [Tooltip("Assign this in the Inspector")] public Sprite LegsSprite;
        [Tooltip("Assign this in the Inspector")] public Sprite MainHandSprite;
        [Tooltip("Assign this in the Inspector")] public Sprite OffHandSprite;


        private InventoryEquipment _inventoryEquipment;
        public InventoryEquipment InventoryEquipment
        {
            get { return _inventoryEquipment; }
            set
            {
                SlotUis.Clear();
                if (_inventoryEquipment) _inventoryEquipment.InventorySlotData.Callback -= OnSlotChanged;
                _inventoryEquipment = value;
                if (_inventoryEquipment) _inventoryEquipment.InventorySlotData.Callback += OnSlotChanged;

                for (int i = 0; i < _inventoryEquipment.InventorySlotData.Count; i++)
                {
                    if (_inventoryEquipment.InventorySlotData[i].SlotType == ItemSlotType.Any) continue; // we only want equipment slots
                    ItemSlotUi slotUi = null;

                    switch (_inventoryEquipment.InventorySlotData[i].SlotType)
                    {
                        case ItemSlotType.Helmet:
                            slotUi = Instantiate(SlotUiPrefab, HelmetTransform).GetComponent<ItemSlotUi>();
                            slotUi.SlotImage.sprite = HelmetSprite;
                            break;
                        case ItemSlotType.Torso:
                            slotUi = Instantiate(SlotUiPrefab, TorsoTransform).GetComponent<ItemSlotUi>();
                            slotUi.SlotImage.sprite = TorsoSprite;
                            break;
                        case ItemSlotType.Legs:
                            slotUi = Instantiate(SlotUiPrefab, LegsTransform).GetComponent<ItemSlotUi>();
                            slotUi.SlotImage.sprite = LegsSprite;
                            break;
                        case ItemSlotType.MainHand:
                            slotUi = Instantiate(SlotUiPrefab, MainHandTransform).GetComponent<ItemSlotUi>();
                            slotUi.SlotImage.sprite = MainHandSprite;
                            break;
                        case ItemSlotType.OffHand:
                            slotUi = Instantiate(SlotUiPrefab, OffHandTransform).GetComponent<ItemSlotUi>();
                            slotUi.SlotImage.sprite = OffHandSprite;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    slotUi.SlotImage.gameObject.SetActive(true);
                    slotUi.SlotData = _inventoryEquipment.InventorySlotData[i];
                    slotUi.SlotIndex = i;
                    slotUi.AttachedInventory = _inventoryEquipment;
                    SlotUis.Add(i, slotUi);
                }
            }
        }

        public void OnSlotChanged(SyncDictionaryItemSlot.Operation op, int inventoryIndex, ItemSlotData itemSlotData)
        {
            if (itemSlotData.SlotType == ItemSlotType.Any) return; // return if non equipment slot
            SlotUis.TryGetValue(inventoryIndex, out var slotUi);
            if (slotUi == null) return;
            slotUi.SlotData = itemSlotData;

            // handle slotImage
            slotUi.SlotImage.gameObject.SetActive(!itemSlotData.Occupant());
        }
    }
}
