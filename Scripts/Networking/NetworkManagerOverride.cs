using System.Linq;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Mirror;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Networking
{
    public class NetworkManagerOverride : NetworkManager
    {
#if UNITY_EDITOR
        [ContextMenu("Add Spawnable Items")]
        private void AddSpawnableItems()
        {
            FindObjectOfType<NetworkManagerOverride>().spawnPrefabs = GameManager.SetupItemPrefabs();
        }
#endif
    }
}
