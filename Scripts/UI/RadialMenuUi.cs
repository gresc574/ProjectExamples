using System.Collections.Generic;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.UI
{
    public class RadialMenuUi : MonoBehaviour
    {
        public bool DoRotation;
        public float OffsetFromCenter = 100f;
        public List<Transform> RadialMenuOptions = new List<Transform>();

        [ContextMenu("Setup Menu")]
        public void SetupMenu()
        {
            for (int i = 0; i < RadialMenuOptions.Count; i++)
            {
                RadialMenuOptions[i].rotation = DoRotation ? Quaternion.Euler(0, 0, -((360f / RadialMenuOptions.Count) * i)) : Quaternion.identity;
                float t = (2 * Mathf.PI / RadialMenuOptions.Count) * i;
                float x = Mathf.Sin(t);
                float y = Mathf.Cos(t);
                RadialMenuOptions[i].localPosition = new Vector3(x, y, 0) * OffsetFromCenter;
            }
        }
    }
}
