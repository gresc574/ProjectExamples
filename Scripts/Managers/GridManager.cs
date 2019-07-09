using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Grid;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Managers
{
    public class GridManager : MonoBehaviour
    {
        public static Dictionary<Vector3, GridCell> CellDictionary = new Dictionary<Vector3, GridCell>();

        public Vector2 TestGridSize;
        public bool DebugTestGrid;
        public bool DebugCells = false;
        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            if (DebugCells)
            {
                foreach (var gridCell in CellDictionary)
                {
                    Gizmos.DrawSphere(gridCell.Key, 0.2f);
                }
            }

            if (!DebugTestGrid) return;
            Gizmos.color = Color.blue;
            for (int i = (int)-TestGridSize.x / 2; i < TestGridSize.x / 2f; i++)
            {
                for (int j = (int)-TestGridSize.y / 2; j < TestGridSize.y / 2; j++)
                {
                    Gizmos.DrawWireCube(new Vector3(i, 0f, j).RoundToGridCenter(), new Vector3(1f, 0.01f, 1f));
                }
            }
        }

        public static bool CheckCellEmpty(Vector3 gridPosition)
        {
            CellDictionary.TryGetValue(gridPosition, out var gridCell);
            return gridCell == null;
        }


    }
}
