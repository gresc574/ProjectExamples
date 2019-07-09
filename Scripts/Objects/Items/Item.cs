using System;
using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Inventory;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Mirror;
using Smooth;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Items
{
    // networked data
    [Serializable]
    public struct ItemData
    {
        public uint ItemId;
        public uint Quantity;
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(RenderInformationList))]
    [RequireComponent(typeof(SmoothSyncMirror))]
    public class Item : NetworkBehaviour
    {
        [SyncVar(hook = "OnItemDataChanged")]
        public ItemData ItemData;

        [ReadOnly] public RenderInformationList RenderInformation;
        [ReadOnly] public Transform ItemTransform;
        public ItemSlotType SlotType;

        void Awake()
        {
            RenderInformation = GetComponent<RenderInformationList>();
            ItemTransform = transform;
            gameObject.layer = 11; // item layer
        }

        public virtual void UseItem(ItemData itemData, uint inventoryIndex, Inventory.Inventory inventory)
        {
            Debug.Log("Item.cs Use");
        }

        [Server]
        public virtual void FinishedUseItem(ItemData itemData)
        {

        }

        #region  Networking
        // ItemData hook
        public virtual void OnItemDataChanged(ItemData itemData)
        {
            ItemData = itemData;
        }

        // need to call this on items pre-placed in scene. otherwise hook doesn't trigger client side on load
        // and the itemData client side will be incorrect
        public virtual void ItemInitilizeServer()
        {
            var itemData = ItemData;
            ItemData = itemData;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            // for objects pre placed in scene
            ItemInitilizeServer();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) ItemInitilizeServer();
            // Call the hook function to update as it doesn't on join in progress
            OnItemDataChanged(ItemData);
        }

        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            if (PlayerManager.Instance == null) return; // to prevent nulls popping when server/level shutdowns
            if (PlayerManager.Instance.Interactor.ItemsInRange.Contains(this)) PlayerManager.Instance.Interactor.ItemsInRange.Remove(this);
        }
        #endregion
    }

}
