using Assets._TwoFatCatsAssets.Scripts.Managers;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._TwoFatCatsAssets.Scripts.Managers
{
    public class CameraManager : MonoBehaviour
    {
        public Transform TrackedTransform;
        public Transform CameraTransform;
        public Transform MouseHitTransform;

        public Vector3 CameraOffset;
        [HideInInspector] public Vector3 MousePosition;
        public float MovementSpeed;
        public float RotationSpeed;

        [ReadOnly]
        public bool HitTerain;
        [ReadOnly] public Camera PlayerCamera;
        public RaycastHit Hit;

        private static CameraManager _instance;
        public static CameraManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<CameraManager>();
                return _instance;
            }
            set { _instance = value; }
        }

        void Awake()
        {
            PlayerCamera = CameraTransform.GetComponent<Camera>();
        }

        void Update()
        {
            var ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out Hit, 50f, GameManager.Instance.AllButPlayer))
            {
                MousePosition = Hit.point;
                MouseHitTransform.position = Hit.point;
                HitTerain = Hit.transform.gameObject.layer == 9;
            }
            else
            {
                HitTerain = false;
            }
        }

        void FixedUpdate()
        {
            if (TrackedTransform == null) return;

            var rot = Quaternion.LookRotation(TrackedTransform.position - CameraTransform.position);
            CameraTransform.rotation = Quaternion.Lerp(CameraTransform.rotation, rot, Time.deltaTime * RotationSpeed);
            CameraTransform.position = Vector3.Lerp(CameraTransform.position, TrackedTransform.position + CameraOffset, Time.deltaTime * MovementSpeed);
        }
    }
}
