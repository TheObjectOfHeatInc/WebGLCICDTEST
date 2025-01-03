using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    [SerializeField]private Image imageComponent;
    private bool isBlue = false;

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
        imageComponent.color = isBlue ? Color.red : Color.blue;

        isBlue = !isBlue;
    }
}