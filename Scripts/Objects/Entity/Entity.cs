using Assets._TwoFatCatsAssets.Scripts.Objects.Items;
using Mirror;
using Unity.Collections;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Objects.Entity
{
    public class Entity : NetworkBehaviour
    {
        public float MovementSpeed = 0.1f;
        public float RotationSpeed = 3f;

        public Vector3 MoveDirection;
        public Vector3 LookPosition;
        [ReadOnly] public Vector3 Velocity;

        public Transform EntityTransform;
        protected NetworkIdentity NetworkIdentity;

        public Inventory.Inventory Inventory;

        public virtual void Awake()
        {
            EntityTransform = transform;
            NetworkIdentity = GetComponent<NetworkIdentity>();
            Inventory = GetComponent<Inventory.Inventory>();
        }

        public virtual void CharacterControllerMove(Vector3 moveDirection, Vector3 lookPosition)
        {

        }

        public virtual void FixedUpdate()
        {

        }
    }
}
