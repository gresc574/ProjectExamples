using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public float Vertical;
        public float Horizontal;
        public bool Interact;
        public bool Inventory;
        public bool Equipment;

        private static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<InputManager>();
                return _instance;
            }
            set { _instance = value; }
        }

        void Update()
        {
            Vertical = Input.GetAxis("Vertical");
            Horizontal = Input.GetAxis("Horizontal");
            Interact = Input.GetButtonDown("Interact");
            Inventory = Input.GetButtonDown("Inventory");
            Equipment = Input.GetButtonDown("Equipment");
        }
    }
}
