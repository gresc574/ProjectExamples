using System.Collections.Generic;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects
{
    public class RenderInformationList : MonoBehaviour
    {
        public List<RenderInformation> RenderInformations = new List<RenderInformation>();

        void Awake()
        {
            SetupRenderInformationList();
        }

        private void SetupRenderInformationList()
        {
            var renderersList = new List<Renderer>();
            var renderersChildren = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderersChildren.Length; i++)
            {
                renderersList.Add(renderersChildren[i]);
            }
            for (int i = 0; i < renderersList.Count; i++)
            {
                RenderInformations.Add(new RenderInformation(renderersList[i], renderersList[i].sharedMaterials));
            }
        }

        public void SetMaterials(Material[] newMaterials)
        {
            for (int i = 0; i < RenderInformations.Count; i++)
            {
                RenderInformations[i].SetMaterials(newMaterials);
            }
        }

        public void Resetmaterials()
        {
            for (int i = 0; i < RenderInformations.Count; i++)
            {
                RenderInformations[i].ResetMaterials();
            }
        }
    }
}
