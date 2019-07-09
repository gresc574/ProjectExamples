using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Assets._TwoFatCatsAssets.Scripts.Objects.Structures;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Entity
{
    // this class only exists on the local player
    public class Interactor : MonoBehaviour
    {
        public List<Item> ItemsInRange = new List<Item>();
        public List<Structure> StructuresInRange = new List<Structure>();
        public Material[] HighlightMaterials;
        [ReadOnly] public Transform OurTransform;

        private Structure _closestStructure;
        public Structure ClosestStructure
        {
            get { return _closestStructure; }
            set
            {
                if (_closestStructure == value) return;
                if (_closestStructure)
                {
                    _closestStructure.RenderInformation.Resetmaterials();
                }
                _closestStructure = value;
                if (_closestStructure) _closestStructure.RenderInformation.SetMaterials(HighlightMaterials);
            }
        }

        private Item _closestItem;
        public Item ClosestItem
        {
            get { return _closestItem; }
            set
            {
                if (_closestItem == value) return;
                if (_closestItem)
                {
                    _closestItem.RenderInformation.Resetmaterials();
                }
                _closestItem = value;
                if (_closestItem) _closestItem.RenderInformation.SetMaterials(HighlightMaterials);
            }
        }

        void Awake()
        {
            HighlightMaterials = PlayerManager.Instance.HighlightMaterials;
            PlayerManager.Instance.Interactor = this;
            OurTransform = transform;
            if (GetComponent<SphereCollider>()) return;
            var newSphereCollider = gameObject.AddComponent<SphereCollider>();
            newSphereCollider.center = new Vector3(0f, 1f, 0f);
            newSphereCollider.isTrigger = true;
            newSphereCollider.radius = 3f;
        }

        void Update()
        {
            ClosestStructure = CloestestStructureInRange();
            if (ClosestStructure) return;
            ClosestItem = ClosestItemInRange();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 9) return; // terrain layer

            // check for buildings
            var buildingComponent = other.GetComponent<Structure>();
            if (buildingComponent)
            {
                StructuresInRange.Add(buildingComponent);
                return; // items can't exist on buildings. so return
            }
            // check for items
            var itemComponent = other.GetComponent<Item>();
            if (itemComponent) ItemsInRange.Add(itemComponent);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 9) return; // terrain layer

            // check for buildings
            var buildingComponent = other.GetComponent<Structure>();
            if (buildingComponent)
            {
                StructuresInRange.Remove(buildingComponent);
                return; // items can't exist on buildings. so return
            }
            // check for items
            var itemComponent = other.GetComponent<Item>();
            if (itemComponent) ItemsInRange.Remove(itemComponent);
        }

        public Item ClosestItemInRange()
        {
            Item item = null;
            var dist = 50f;
            if (ItemsInRange.Count < 1) return null;
            for (int i = 0; i < ItemsInRange.Count; i++)
            {
                // check if infront
                var dirToItem = ItemsInRange[i].ItemTransform.position - OurTransform.position;
                var dirOfMouse = PlayerManager.Instance.LookPosition - OurTransform.position;
                var dot = Vector3.Dot(dirOfMouse, dirToItem.normalized);
                if (dot < 0) continue;
                // check closest
                var newDist = Vector3.Distance(ItemsInRange[i].ItemTransform.position, OurTransform.position);
                if (newDist < dist)
                {
                    item = ItemsInRange[i];
                    dist = newDist;
                }
            }
            return item;
        }

        public Structure CloestestStructureInRange()
        {
            Structure structure = null;
            var dist = 50f;
            if (StructuresInRange.Count < 1) return null;
            for (int i = 0; i < StructuresInRange.Count; i++)
            {
                // check if infront
                var dirToItem = StructuresInRange[i].StructureTransform.position - OurTransform.position;
                var dirOfMouse = PlayerManager.Instance.LookPosition - OurTransform.position;
                var dot = Vector3.Dot(dirOfMouse, dirToItem.normalized);
                if (dot < 0) continue;
                // check closest
                var newDist = Vector3.Distance(StructuresInRange[i].StructureTransform.position, OurTransform.position);
                if (newDist < dist)
                {
                    structure = StructuresInRange[i];
                    dist = newDist;
                }
            }
            return structure;
        }
    }
}
