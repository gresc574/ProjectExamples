using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Extensions;
using Assets._TwoFatCatsAssets.Scripts.Objects.Entity;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Assets._TwoFatCatsAssets.Scripts.Objects.Structures;
using Assets._TwoFatCatsAssets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._TwoFatCatsAssets.Scripts.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerEntity CurrentPlayerEntity;
        public Vector3 MoveDirection;
        public Vector3 LookPosition;

        [ReadOnly] public Interactor Interactor;
        [ReadOnly] public Inventory.Inventory PlayerInventory;
        public Transform InventoryCanvasParent;
        [ReadOnly] public InventoryCanvas PlayerInventoryCanvas;
        [ReadOnly] public InventoryCanvas ExternalInventoryCanvas;
        [ReadOnly] public EquipmentCanvas PlayerEquipmentCanvas;
        [Space(10)]
        public Material[] HighlightMaterials;
        public Material[] CursorGreenMaterials;
        public Material[] CursorRedMaterials;
        public GameObject InventoryCanvasPrefab;
        public GameObject EquipmentCanvasPrefab;
        #region static ref
        private static PlayerManager _instance;
        public static PlayerManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<PlayerManager>();
                return _instance;
            }
            set { _instance = value; }
        }
        #endregion

        // build cursor
        public Transform BuildCursor;
        public Transform StructuresBuildCursor;

        public ItemBuildable CurrentBuildableItem;
        private StructureCursor _buildStructure;
        public StructureCursor BuildStructure
        {
            get
            {
                return _buildStructure;
            }
            set
            {
                if (_buildStructure != null) _buildStructure.StructureGameObject.SetActive(false);
                if (value == null)
                {
                    _buildStructure = null;
                    return;
                }
                _buildStructure = value;
                _buildStructure.StructureGameObject.SetActive(true);
            }
        }
        private bool _buildMode;
        public bool BuildMode
        {
            get { return _buildMode; }
            set
            {
                _buildMode = value;
            }
        }

        private void Awake()
        {
            PlayerInventoryCanvas = Instantiate(InventoryCanvasPrefab).GetComponent<InventoryCanvas>();
            PlayerEquipmentCanvas = Instantiate(EquipmentCanvasPrefab).GetComponent<EquipmentCanvas>();
            ExternalInventoryCanvas = Instantiate(InventoryCanvasPrefab).GetComponent<InventoryCanvas>();
            PlayerInventoryCanvas.transform.SetParent(InventoryCanvasParent);
            PlayerEquipmentCanvas.transform.SetParent(InventoryCanvasParent);
            ExternalInventoryCanvas.transform.SetParent(InventoryCanvasParent); // set parent after initilization or canvas won't render correctly
            PlayerInventoryCanvas.gameObject.SetActive(false);
            PlayerEquipmentCanvas.gameObject.SetActive(false);
            ExternalInventoryCanvas.gameObject.SetActive(false);
        }

        void Update()
        {
            if (CurrentPlayerEntity == null) return;
            HandleUiRaycast();
            HandleBuildRaycast();

            #region Position & Rotation
            MoveDirection.x = Mathf.Lerp(CurrentPlayerEntity.MoveDirection.x, InputManager.Instance.Horizontal, Time.deltaTime * 5f);
            MoveDirection.z = Mathf.Lerp(CurrentPlayerEntity.MoveDirection.z, InputManager.Instance.Vertical, Time.deltaTime * 5f);

            CurrentPlayerEntity.MoveDirection.x = MoveDirection.x;
            CurrentPlayerEntity.MoveDirection.z = MoveDirection.z;

            // add height if looking at the terrain so we don't stare down
            LookPosition = CameraManager.Instance.HitTerain ? CameraManager.Instance.MousePosition + (Vector3.up * 0.5f) : CameraManager.Instance.MousePosition;
            CurrentPlayerEntity.LookPosition = LookPosition;
            #endregion

            if (InputManager.Instance.Interact)
            {
                if (Interactor.ClosestStructure)
                {
                    Interactor.ClosestStructure.DoInteract();
                }
                if (Interactor.ClosestItem)
                {
                    AddItemToNextEmptySlot(Interactor.ClosestItem);
                }
            }

            if (InputManager.Instance.Inventory)
            {
                PlayerInventoryCanvas.gameObject.SetActive(!PlayerInventoryCanvas.isActiveAndEnabled);
            }
            if (InputManager.Instance.Equipment)
            {
                PlayerEquipmentCanvas.gameObject.SetActive(!PlayerEquipmentCanvas.isActiveAndEnabled);
            }
        }

        public void AddItemToNextEmptySlot(Item item)
        {
            for (var i = 0; i < PlayerInventory.Capacity; i++)
            {
                if (PlayerInventory.InventorySlotData[i].Occupant()) continue;
                CurrentPlayerEntity.Inventory.CmdAddItem(item.ItemData, (uint)i, item.netId);
                return;
            }
        }

        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();
        private void HandleUiRaycast()
        {
            if (ItemUi.DraggedItemUi == null) return;
            _pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
            var foundSlot = false;
            for (int i = 0; i < _raycastResults.Count; i++)
            {
                GenericUiPanel.CurrentGenericUiPanel = _raycastResults[i].gameObject.GetComponent<GenericUiPanel>();
                if (!foundSlot)
                {
                    var slotUi = _raycastResults[i].gameObject.GetComponent<ItemSlotUi>();
                    ItemSlotUi.HoverItemSlot = slotUi;
                    if (slotUi) foundSlot = true;
                }
            }
        }

        private RaycastHit _terrainHit;
        private void HandleBuildRaycast()
        {
            if (!BuildMode) return;
            var ray = CameraManager.Instance.PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _terrainHit, 50f, GameManager.Instance.TerrainOnly))
            {
                BuildCursor.transform.position = _terrainHit.point.RoundToGridCenter();
                var canPlace = true;
                for (int i = 0; i < BuildStructure.Positions.Count; i++)
                {
                    if (!GridManager.CheckCellEmpty(BuildStructure.Positions[i].position.RoundToGridCenter()))
                        canPlace = false;
                }
                BuildStructure.RenderInformation.SetMaterials(canPlace ? CursorGreenMaterials : CursorRedMaterials);
                if (Input.GetMouseButtonDown(1))
                {
                    BuildMode = false;
                    BuildStructure = null;
                    return;
                }

                if (canPlace && Input.GetMouseButtonDown(0))
                {
                    CurrentPlayerEntity.FinishedUseBuildableItem(BuildStructure.Inventory, BuildStructure.InventoryIndex);
                }
            }
        }

        public void SetBuildMode(bool active, uint id, ItemBuildable item, uint inventoryIndex, Inventory.Inventory inventory)
        {
            BuildMode = active;
            CurrentBuildableItem = item;
            GameManager.StructureCursorDictionary.TryGetValue(id, out var structureCursor);
            structureCursor.InventoryIndex = inventoryIndex;
            structureCursor.Inventory = inventory;
            BuildStructure = structureCursor;
        }
    }
}
