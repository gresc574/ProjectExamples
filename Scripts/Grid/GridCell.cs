using System;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Structures;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Grid
{
    [Serializable]
    public class GridCell
    {
        public Vector3 Position;

        private Structure _structure;
        public Structure Structure // one structure per cell
        {
            get { return _structure; }
            set
            {
                _structure = value;
                if (_structure == null) RemoveCell();
            }
        }

        public GridCell(Vector3 position, Structure structure)
        {
            Position = position;
            Structure = structure;
            AddCell();
        }

        public void AddCell()
        {
            Debug.Log("Add Cell");
            GridManager.CellDictionary.Add(Position, this);
        }

        public void RemoveCell()
        {
            Debug.Log("Remove Cell");
            GridManager.CellDictionary.Remove(Position);
        }
    }
}
