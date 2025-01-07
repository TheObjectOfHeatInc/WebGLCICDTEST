using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    private bool _isOverDropZone = false;
    private GameObject _currentDraggable;
    [SerializeField] private GameObject ButtonButMeat;
    [SerializeField] private GameObject SellZone;

    public void SetDraggingState(bool isDragging)
    {
        ButtonButMeat.SetActive(!isDragging);
        SellZone.SetActive(isDragging);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Draggable"))
        {
            _isOverDropZone = true;
            _currentDraggable = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Draggable"))
        {
            _isOverDropZone = false;
            _currentDraggable = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_isOverDropZone && _currentDraggable != null)
        {
            Destroy(_currentDraggable);
            _isOverDropZone = false;
            _currentDraggable = null;
        }
    }
}