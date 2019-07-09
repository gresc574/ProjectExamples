using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Objects;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Assets._TwoFatCatsAssets.Scripts.Objects.Structures;
using Mirror;
using UnityEditor;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public LayerMask AllButPlayer;
        public LayerMask TerrainOnly;
        public Transform StructuresCursorTransform;

        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<GameManager>();
                return _instance;
            }
            set { _instance = value; }
        }

        public static Dictionary<uint, GameObject> ItemDictionary = new Dictionary<uint, GameObject>();
        public static Dictionary<uint, Sprite> ItemIconDictionary = new Dictionary<uint, Sprite>();
        public static Dictionary<uint, GameObject> StructureDictionary = new Dictionary<uint, GameObject>();
        public static Dictionary<uint, StructureCursor> StructureCursorDictionary = new Dictionary<uint, StructureCursor>();


        void Update()
        {
            if (ItemDictionary.Count < 1) Debug.Log("Item Dictionary is Empty!");
            if (ItemIconDictionary.Count < 1) Debug.Log("Item Icon Dictionary is Empty!");
            if (StructureDictionary.Count < 1) Debug.Log("Structure Dictionary is Empty!");
        }


        private void Awake()
        {
            InitilizeItemDictionary();
            InitilizeStructureDictionary();
            Invoke("InitilizeStructureCursors", 0.5f); // delaying it or mirror complains about scene parity
        }

#if UNITY_EDITOR
        // This loads all items.cs in resources/items,
        // assigns them new Itemdata.ItemId and resaves prefab
        [ContextMenu("Load All Prefabbed Object Ids")]
        public void LoadItems()
        {
            SetupItemPrefabs();
            SetupStructurePrefabs();
        }

        public static void SetupStructurePrefabs()
        {
            StructureDictionary.Clear();
            uint idCount = 1;
            var structures = Resources.LoadAll("Structures", typeof(GameObject));

            Debug.Log("Found " + structures.Length + " Structures");
            foreach (var structureGameObject in structures)
            {
                var structureGo = PrefabUtility.InstantiatePrefab(structureGameObject) as GameObject;
                PrefabUtility.UnpackPrefabInstance(structureGo, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                var structure = structureGo.GetComponent<Structure>();
                if (structure == null)
                {
                    Debug.LogError("Structure Gameobject has no Structure script attached, Skipping", structureGo);
                    continue;
                }
                StructureDictionary.Add(idCount, structureGo);
                var path = AssetDatabase.GetAssetPath(structureGameObject);
                structure.Id = idCount;
                idCount++;
                PrefabUtility.SaveAsPrefabAsset(structureGo, path);
                DestroyImmediate(structureGo);
            }
            Debug.Log("StructureDictionary Count = " + StructureDictionary.Count);
        }

        public static List<GameObject> SetupItemPrefabs()
        {
            ItemDictionary.Clear();
            ItemIconDictionary.Clear();
            var list = new List<GameObject>();
            uint idCount = 1;
            var items = Resources.LoadAll("Items/Dropped/", typeof(GameObject));

            Debug.Log("Items Found : " + items.Length);
            foreach (var itemGameObject in items)
            {
                var itemGo = PrefabUtility.InstantiatePrefab(itemGameObject) as GameObject;
                PrefabUtility.UnpackPrefabInstance(itemGo, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                var item = itemGo.GetComponent<Item>();
                if (item == null)
                {
                    Debug.LogError("Item Gameobject has no Item Script attached, Skipping", itemGameObject);
                    continue;
                }
                // get icon
                var icon = Resources.Load("Items/Icons/" + item.name, typeof(Sprite)) as Sprite;
                if (icon)
                {
                    ItemIconDictionary.Add(idCount, icon);
                }
                else
                {
                    Debug.Log(item.name + " has no Icon!");
                }
                ItemDictionary.Add(idCount, itemGameObject as GameObject);
                var path = AssetDatabase.GetAssetPath(itemGameObject);
                Debug.Log(ItemDictionary[idCount] + " :  " + path);

                list.Add(itemGo);
                item.ItemData.ItemId = idCount;
                idCount++;
                PrefabUtility.SaveAsPrefabAsset(itemGo, path);
                DestroyImmediate(itemGo);
            }
            Debug.Log(ItemDictionary.Count + " Items added to Prefabed items Dictionary");
            return list;
        }

#endif
        public static void InitilizeStructureDictionary()
        {
            StructureDictionary.Clear();
            uint idCount = 1;
            var structures = Resources.LoadAll("Structures", typeof(GameObject));

            Debug.Log("Found " + structures.Length + " Structures");
            foreach (var structureGameObject in structures)
            {
                var go = structureGameObject as GameObject;
                StructureDictionary.Add(idCount, go);
                idCount++;
            }
            Debug.Log("StructureDictionary Count = " + StructureDictionary.Count);
        }

        public void InitilizeStructureCursors()
        {
            foreach (var structureGo in StructureDictionary.Values)
            {
                var instantiatedObject = Instantiate(structureGo, StructuresCursorTransform);
                instantiatedObject.transform.localPosition = Vector3.zero;

                var structure = instantiatedObject.GetComponent<Structure>();
                var networkIdentity = instantiatedObject.GetComponent<NetworkIdentity>();
                var col = instantiatedObject.GetComponent<Collider>();
                var inventory = instantiatedObject.GetComponent<Inventory.Inventory>();

                var structureCursor = instantiatedObject.AddComponent<StructureCursor>();

                structureCursor.Id = structure.Id;
                structureCursor.StructureGameObject = instantiatedObject;
                structureCursor.RenderInformation = instantiatedObject.GetComponent<RenderInformationList>();

                foreach (var positionReference in structure.GridPositionReferences)
                {
                    structureCursor.Positions.Add(positionReference.transform);
                }

                StructureCursorDictionary.Add(structure.Id, structureCursor);

                Destroy(structure);
                if (inventory) Destroy(inventory);
                Destroy(networkIdentity);
                Destroy(col);

                instantiatedObject.SetActive(false);
            }
            StructuresCursorTransform.gameObject.SetActive(true);
        }

        public static void InitilizeItemDictionary()
        {
            ItemDictionary.Clear();
            ItemIconDictionary.Clear();
            uint idCount = 1;
            var items = Resources.LoadAll("Items/Dropped/", typeof(GameObject));

            foreach (var o in items)
            {
                var go = o as GameObject;
                var item = go.GetComponent<Item>();
                if (!item)
                {
                    Debug.LogError("No Item Component on " + go.name);
                    continue;
                }
                var icon = Resources.Load("Items/Icons/" + item.name, typeof(Sprite)) as Sprite;
                if (icon)
                {
                    ItemIconDictionary.Add(idCount, icon);
                }
                else
                {
                    Debug.LogError(item.name + " has no Icon!");
                }
                ItemDictionary.Add(idCount, go);
                idCount++;
            }
        }
    }
}
