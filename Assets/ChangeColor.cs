using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    [SerializeField]private Image imageComponent;
    [SerializeField]private TextMeshProUGUI _textComponent;
    private int _score = 0;
    private bool _isBlue = false;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        
    }

    void Update()
    {
        // Проверяем клик мышью для ПК
        if (Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }

        // Проверяем тач для мобильных устройств
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckClick();
        }
    }

    void CheckClick()
    {
        imageComponent.color = _isBlue ? Color.red : Color.blue;
        _score++;
        _textComponent.text = _score.ToString();
        _isBlue = !_isBlue;
    }
}