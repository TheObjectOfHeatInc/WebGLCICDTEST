using UnityEngine;
using UnityEngine.UI;

public class Product : MonoBehaviour
{
    [SerializeField] private Image productImage;
    [SerializeField] private float colorChangeSpeed = 0.5f;
    
    private Color _originalColor;
    private readonly Color _targetColor = Color.red;
    private float _currentColorProgress = 0f;
    [SerializeField]private Plate _currentPlate;

    private bool _isOverPlate = false;

    void Start()
    {
        _originalColor = productImage.color;
    }
    
    void Update()
    {
        if (_isOverPlate && _currentPlate != null && _currentPlate.IsReadyForCooking())
        {
            _currentColorProgress += colorChangeSpeed * Time.deltaTime;
            _currentColorProgress = Mathf.Clamp01(_currentColorProgress);
            productImage.color = Color.Lerp(_originalColor, _targetColor, _currentColorProgress);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Plate")) return;
        _currentPlate = other.GetComponent<Plate>();
        _isOverPlate = true;
    }

  

    void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Plate")) return;
        _currentPlate = null;
        _isOverPlate = false;
    }
}




