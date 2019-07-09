using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Structures
{
    public class StructureChest : Structure
    {
        [ReadOnly] public Inventory.Inventory Inventory;

        public override void Awake()
        {
            base.Awake();
            Inventory = GetComponent<Inventory.Inventory>();
        }

        public override void DoInteract()
        {
            base.DoInteract();
            PlayerManager.Instance.ExternalInventoryCanvas.Inventory = Inventory;
            PlayerManager.Instance.ExternalInventoryCanvas.gameObject.SetActive(true);
        }
    }
}
