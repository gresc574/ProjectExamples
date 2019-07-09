using System.Collections.Generic;
using Assets._TwoFatCatsAssets.Scripts.Inventory;
using Assets._TwoFatCatsAssets.Scripts.Managers;
using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using RootMotion.FinalIK;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Entity
{
    public class PlayerEntity : Entity
    {
        private CharacterController _characterController;

        private Vector3 _lastPosition;
        private bool _backMoving;
        private Transform _lookTransform;
        private LookAtIK _lookIk;
        private Animator _animator;
        private static int _speedHash = Animator.StringToHash("Speed");
        private static int _idleHash = Animator.StringToHash("Idle");


        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            PlayerManager.Instance.CurrentPlayerEntity = this;
            CameraManager.Instance.TrackedTransform = transform;
            PlayerManager.Instance.PlayerInventory = GetComponent<Inventory.Inventory>();
            PlayerManager.Instance.PlayerInventoryCanvas.Inventory = GetComponent<Inventory.Inventory>();
            PlayerManager.Instance.PlayerEquipmentCanvas.InventoryEquipment = GetComponent<InventoryEquipment>();
            gameObject.layer = 10; // player layer
            gameObject.AddComponent<Interactor>();
        }

        public override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _lookIk = GetComponent<LookAtIK>();
            _lookTransform = transform.Find("LookPosition");
            _characterController = GetComponent<CharacterController>();
        }

        public override void CharacterControllerMove(Vector3 moveDirection, Vector3 lookPosition)
        {
            base.CharacterControllerMove(moveDirection, lookPosition);
            // clamp to prevent diagonal movement increasing speed
            var moveVector = moveDirection.magnitude > 1f ? moveDirection.normalized : moveDirection;
            var gravity = _characterController.isGrounded ? 0f : 9f;
            var moveDir = new Vector3(moveVector.x, -gravity, moveVector.z);
            var targetPosition = moveDir * MovementSpeed;

            _characterController.Move(targetPosition);

            Velocity = (EntityTransform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = EntityTransform.position;

            var mouseDelta = LookPosition - EntityTransform.position;
            var angleFwdMouse = Vector3.Angle(EntityTransform.forward, mouseDelta);
            var angleVelMouse = Vector3.Angle(Velocity.normalized, mouseDelta);

            // calculate if we should be moving backwards
            _backMoving = Mathf.Abs(angleVelMouse) > 100f;
            // invert velocity if moving backwards
            Velocity = _backMoving ? Velocity * -1f : Velocity;
            // if not moving we cant use velocity. so use forward instead
            var rotAim = EntityTransform.position + (Velocity.normalized.magnitude < 0.1f ? EntityTransform.forward : Velocity.normalized);

            var rotVelocityDelta = rotAim - EntityTransform.position;

            rotVelocityDelta.y = 0f;
            mouseDelta.y = 0f;

            // always rotate towards mouseDelta first to prevent random 180s in wrong direction
            var finalRotation = angleFwdMouse > 100f ? Quaternion.LookRotation(mouseDelta) : Quaternion.LookRotation(rotVelocityDelta);
            EntityTransform.rotation = Quaternion.Lerp(EntityTransform.rotation, finalRotation, Time.deltaTime * RotationSpeed);

            // handle IKs
            var ikPosition = lookPosition - EntityTransform.position;
            // clamp it at a fixed distance away from the character so we're not looking down if too close to character
            ikPosition = ikPosition.magnitude < 1f ? Vector3.ClampMagnitude((lookPosition - EntityTransform.position) * 20f, 1f) : ikPosition;
            _lookTransform.position = Vector3.Lerp(_lookTransform.position, ikPosition + EntityTransform.position, Time.deltaTime * 10f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isLocalPlayer) return;
            CharacterControllerMove(MoveDirection, LookPosition);
        }

        void Update()
        {
            if (!isLocalPlayer) return;
            SetAnimationValues();
        }

        private void SetAnimationValues()
        {
            var speedMag = _backMoving ? Velocity.sqrMagnitude * -1f : Velocity.sqrMagnitude;
            speedMag = speedMag * 0.2f;
            _animator.SetFloat(_speedHash, speedMag);
            _animator.SetBool(_idleHash, Mathf.Abs(speedMag) < 0.2f);
        }


        public void MoveInventorySlotsInternal(Inventory.Inventory currentInventory, uint currentInventoryIndex, uint targetInventoryIndex)
        {
            currentInventory.CmdMoveItemInternal(currentInventoryIndex, targetInventoryIndex);
        }

        public void MoveInventorySlotsExternal(Inventory.Inventory currentInventory, uint currentInventoryIndex, uint targetInventoryIndex, uint targetInventoryNetId)
        {
            Inventory.CmdMoveItemExternal(currentInventoryIndex, targetInventoryIndex, currentInventory.netId, targetInventoryNetId);
        }

        public void DropInventorySlot(Inventory.Inventory currentInventory, uint currentInventoryIndex)
        {
            Inventory.CmdDropItem(currentInventory.netId, currentInventoryIndex);
        }

        public void UseItemLocal(Inventory.Inventory currentInventory, uint currentInventoryIndex)
        {
            currentInventory.LocalUseItem(currentInventoryIndex);
        }

        public void FinishedUseItem(Inventory.Inventory currentInventory, uint currentInventoryIndex)
        {
            Inventory.CmdFinishedUseItem(currentInventory.netId, currentInventoryIndex);
        }

        public void FinishedUseBuildableItem(Inventory.Inventory currentInventory, uint currentInventoryIndex)
        {
            Inventory.CmdFinishedUseBuildableItem(currentInventory.netId, currentInventoryIndex, PlayerManager.Instance.BuildCursor.position);
        }
    }
}
