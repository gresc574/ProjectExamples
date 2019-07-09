using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Structures
{
    // class used for building placement of structures
    public class StructureCursor : MonoBehaviour
    {
        public uint Id;
        public List<Transform> Positions = new List<Transform>();
        public GameObject StructureGameObject;
        public RenderInformationList RenderInformation;
        public Inventory.Inventory Inventory;
        public uint InventoryIndex;
    }
}
