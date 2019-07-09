using Assets._TwoFatCatsAssets.Scripts.Inventory;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._TwoFatCatsAssets.Scripts.UI
{
    public class ItemUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector2 _startPosition;
        private Transform _startSlot;
        public ItemSlotUi AttachedItemSlotUi;
        private bool _dragged; // this is cheaper then comparing the ItemUi class
        public Item ItemReference;
        public bool Dragged
        {
            get { return _dragged; }
            set
            {
                // about to be dragged
                if (value && !_dragged)
                {
                    _startPosition = transform.position;
                    _startSlot = transform.parent;
                    transform.SetParent(transform.root, false);
                }
                _dragged = value;
                if (_dragged) return;
                // dropped
                transform.SetParent(_startSlot, false);
                transform.position = _startPosition;
            }
        }
        private static ItemUi _draggedItemUi;
        public static ItemUi DraggedItemUi
        {
            get { return _draggedItemUi; }
            set
            {
                if (_draggedItemUi) _draggedItemUi.Dragged = false;
                _draggedItemUi = value;
                if (_draggedItemUi) _draggedItemUi.Dragged = true;

            }
        }

        private static ItemUi _selectedItemUi;
        public static ItemUi SelectedItemUi
        {
            get { return _selectedItemUi; }
            set
            {
                _selectedItemUi = value;
            }
        }

        private ItemData _itemData;
        public ItemData ItemData
        {
            get { return _itemData; }
            set
            {
                _itemData = value;
                GameManager.ItemIconDictionary.TryGetValue(_itemData.ItemId, out var sprite);
                if (sprite) Image.sprite = sprite;
            }
        }
        [Tooltip("Set this in the inspector")]
        public Image Image;

        void Awake()
        {
            if (!Image) Image = GetComponent<Image>();
            _startPosition = transform.position;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            SelectedItemUi = this;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!SelectedItemUi) return;
            SelectedItemUi.OnUse();
            SelectedItemUi = null;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (DraggedItemUi == null)
            {
                DraggedItemUi = this;
                SelectedItemUi = null; // deselect so we don't accidently trigger Use
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (DraggedItemUi != this) return;

            // move item
            if (DraggedItemUi && ItemSlotUi.HoverItemSlot && ItemSlotUi.HoverItemSlot != AttachedItemSlotUi)
            {
                if (ItemReference.SlotType == ItemSlotUi.HoverItemSlot.SlotData.SlotType || ItemSlotUi.HoverItemSlot.SlotData.SlotType == ItemSlotType.Any)
                {
                    PlayerManager.Instance.CurrentPlayerEntity.MoveInventorySlotsExternal(AttachedItemSlotUi.AttachedInventory, (uint)AttachedItemSlotUi.SlotIndex,
                        (uint)ItemSlotUi.HoverItemSlot.SlotIndex, ItemSlotUi.HoverItemSlot.AttachedInventory.netId);
                }
            }

            // dropping item 
            if (DraggedItemUi && !GenericUiPanel.CurrentGenericUiPanel)
            {
                PlayerManager.Instance.CurrentPlayerEntity.DropInventorySlot(AttachedItemSlotUi.AttachedInventory, (uint)AttachedItemSlotUi.SlotIndex);
            }
            if (DraggedItemUi) DraggedItemUi = null;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!_dragged) return;
            transform.position = Input.mousePosition;
        }

        public virtual void OnUse()
        {
            PlayerManager.Instance.CurrentPlayerEntity.UseItemLocal(AttachedItemSlotUi.AttachedInventory, (uint)AttachedItemSlotUi.SlotIndex);
        }

        public virtual void OnUseFinished()
        {
            PlayerManager.Instance.CurrentPlayerEntity.FinishedUseItem(AttachedItemSlotUi.AttachedInventory, (uint)AttachedItemSlotUi.SlotIndex);
        }
    }
}
