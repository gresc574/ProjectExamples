using System;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects
{
    [Serializable]
    public class RenderInformation
    {
        public Renderer Renderer;
        public Material[] BaseMaterials;

        public RenderInformation(Renderer renderer, Material[] baseMaterials)
        {
            Renderer = renderer;
            BaseMaterials = baseMaterials;
        }

        public void SetMaterials(Material[] newMaterials)
        {
            Renderer.sharedMaterials = newMaterials;
        }

        public void ResetMaterials()
        {
            Renderer.sharedMaterials = BaseMaterials;
        }
    }
}
