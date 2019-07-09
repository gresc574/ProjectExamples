using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Structures;
using Mirror;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Items
{
    public class ItemBuildable : Item
    {
        public Structure BuildableStructure;

        public override void UseItem(ItemData itemData, uint inventoryIndex, Inventory.Inventory inventory)
        {
            base.UseItem(itemData, inventoryIndex, inventory);
            // place grid
            PlayerManager.Instance.SetBuildMode(true, BuildableStructure.Id, this, inventoryIndex, inventory);
        }

        [Server]
        public override void FinishedUseItem(ItemData itemData)
        {
            base.FinishedUseItem(itemData);
        }

        [Server]
        public void FinishedBuildItem(ItemData itemData, Vector3 buildPosition)
        {
            Debug.Log("Finished Building");
            var buildableStructure = Instantiate(BuildableStructure.gameObject, buildPosition, Quaternion.identity);
            NetworkServer.Spawn(buildableStructure);
        }

    }
}
