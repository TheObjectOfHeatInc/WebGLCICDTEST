using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSwaper : MonoBehaviour 
{
    [SerializeField] private float colorChangeSpeed = 2f; // Скорость смены цвета в секундах
    [SerializeField] private float hueShiftSpeed = 0.5f; // Скорость перехода по цветам
    [SerializeField] private TextMeshProUGUI textComponent;
   
    private float hue;

    void Start()
    {
        hue = 0f;
    }

    void Update()
    {
        // Плавно меняем оттенок (hue) со временем
        hue += hueShiftSpeed * Time.deltaTime;
        if (hue >= 1f) hue -= 1f;

        // Конвертируем HSV в RGB с полной насыщенностью и яркостью
        Color newColor = Color.HSVToRGB(hue, 1f, 1f);
        
        // Плавно интерполируем между текущим и новым цветом
        textComponent.color = Color.Lerp(
            textComponent.color, 
            newColor, 
            Time.deltaTime * colorChangeSpeed
        );
    }
}
