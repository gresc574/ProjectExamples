using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Inventory
{
    public class SlotEquipable : Slot
    {
        private LocalEquip _currentEquip;

        public LocalEquip CurrentEquip
        {
            get { return _currentEquip; }
            set
            {
                _currentEquip = value;

            }

        }

        public SlotEquipable(Transform slotTransform, ItemSlotData slotData, int slotIndex) : base(slotTransform, slotData, slotIndex)
        {

        }

        public override void OnSlotChange()
        {
            base.OnSlotChange();
            Debug.Log("Changed SlotEquipable");
        }
    }
}
