using Assets._TwoFatCatsAssets.Scripts.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._TwoFatCatsAssets.Scripts.UI
{
    public class DragHeaderUi : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        public static DragHeaderUi CurrentDragHeaderUi;
        private Transform _parentTransform;
        private RectTransform _parentRectTransform;
        private RectTransform _canvasRectTransform;
        private Camera _parentCanvasCamera;

        void Awake()
        {
            _parentTransform = transform.parent; // panel
            _parentRectTransform = _parentTransform.GetComponent<RectTransform>();
            _canvasRectTransform = transform.root.GetComponent<RectTransform>();
            _parentCanvasCamera = _parentTransform.root.GetComponent<Canvas>().worldCamera;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(_canvasRectTransform, eventData.position)) return;
            _parentTransform.position = eventData.position;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, eventData.position, _parentCanvasCamera, out var localPoint);
            var normalizedPoint = Rect.PointToNormalized(_parentRectTransform.rect, localPoint);
            _parentRectTransform.pivot = normalizedPoint;
        }
    }
}
