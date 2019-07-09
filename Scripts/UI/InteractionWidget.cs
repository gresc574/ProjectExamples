using Assets._TwoFatCatsAssets.Scripts.Managers;
using UnityEngine;

namespace Assets._TwoFatCatsAssets.Scripts.UI
{
    public class InteractionWidget : MonoBehaviour
    {
        private Transform _transform;
        private Transform _letterTransform;

        void Awake()
        {
            _transform = transform;
            _letterTransform = transform.Find("LetterMesh");
        }

        void Update()
        {
            if (PlayerManager.Instance.CurrentPlayerEntity == null) return;
            _letterTransform.LookAt(CameraManager.Instance.CameraTransform);
        }
    }
}
