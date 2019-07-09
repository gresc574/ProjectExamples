using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Grid;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Mirror;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Structures
{
    [RequireComponent(typeof(RenderInformationList))]
    public class Structure : NetworkBehaviour
    {
        public uint Id;
        [ReadOnly]public Transform StructureTransform;
        protected Collider OurCollider;

        [ReadOnly] public RenderInformationList RenderInformation;
        public List<Transform> GridPositionReferences = new List<Transform>();
        public List<GridCell> GridCells = new List<GridCell>();

        public virtual void Awake()
        {
            StructureTransform = transform;
            RenderInformation = GetComponent<RenderInformationList>();
            for (int i = 0; i < GridPositionReferences.Count; i++)
            {
                GridCells.Add(new GridCell(GridPositionReferences[i].position.RoundToGridCenter(), this));
                Destroy(GridPositionReferences[i].gameObject);
            }
            GridPositionReferences.Clear();
        }

        public virtual void DoInteract()
        {

        }

        void OnDestroy()
        {
            for (int i = 0; i < GridCells.Count; i++)
            {
                GridCells[i].Structure = null;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            foreach (var gridPosition in GridPositionReferences)
            {
                Gizmos.DrawSphere(gridPosition.position.RoundToGridCenter(), 0.2f);
            }
        }
    }
}


