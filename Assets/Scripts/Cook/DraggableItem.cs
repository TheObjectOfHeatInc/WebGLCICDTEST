using UnityEngine;
using UnityEngine.EventSystems;

namespace Cook
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]private RectTransform _rectTransform;
        private Vector2 _originalPosition;


        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalPosition = _rectTransform.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / GetCanvasScaleFactor();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Вернуть объект на исходную позицию, если нужно
            // _rectTransform.anchoredPosition = _originalPosition;
        }

        private float GetCanvasScaleFactor()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            return canvas != null ? canvas.scaleFactor : 1f;
        }
    }
}