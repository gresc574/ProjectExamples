using System.Collections.Generic;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Extensions
{
    public static class Extensions
    {
        public static Vector3 RoundToGridCenter(this Vector3 vector)
        {
            return new Vector3
            {
                x = Mathf.Round(vector.x),
                y = Mathf.Round(vector.y - 0.1f),
                z = Mathf.Round(vector.z)
            };
        }

        public static Vector3 CeilToGridCenter(this Vector3 vector)
        {
            return new Vector3
            {
                x = Mathf.Ceil(vector.x),
                y = Mathf.Round(vector.y - 0.1f),
                z = Mathf.Ceil(vector.z)
            };

        }

        public static Vector3 ToClampedGrid(this Vector3 vector)
        {
            var newVector = new Vector3
            {
                x = Mathf.Ceil(vector.x),
                y = Mathf.Round(vector.y - 0.1f),
                z = Mathf.Ceil(vector.z)
            };
            return newVector;
        }

        public static List<Vector3> GetFaceGrids(this Vector3 vector)
        {
            return new List<Vector3>
            {
                (vector + (Vector3.forward + Vector3.left) * 0.5f).RoundToGridCenter(),
                (vector + (Vector3.forward + Vector3.right) * 0.5f).RoundToGridCenter(),
                (vector + (Vector3.back + Vector3.left) * 0.5f).RoundToGridCenter(),
                (vector + (Vector3.back + Vector3.right) * 0.5f).RoundToGridCenter()
            };

        }


    }
}
