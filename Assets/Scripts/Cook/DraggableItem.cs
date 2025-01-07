using UnityEngine;
using UnityEngine.EventSystems;

namespace Cook
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        private Vector2 _originalPosition;
        private DropZone _dropZone;

        void Start()
        {
            _dropZone = GameObject.Find("Bottom").GetComponent<DropZone>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalPosition = _rectTransform.anchoredPosition;
            _canvasGroup.blocksRaycasts = false;
            if (_dropZone != null)
            {
                _dropZone.SetDraggingState(true);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / GetCanvasScaleFactor();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            if (_dropZone != null)
            {
                _dropZone.SetDraggingState(false);
            }
        }

        private float GetCanvasScaleFactor()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            return canvas != null ? canvas.scaleFactor : 1f;
        }
    }
}