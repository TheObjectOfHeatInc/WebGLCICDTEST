using System.Collections;
using UnityEngine;

public class Material : MonoBehaviour
{
    private Color _originalColor;
    private SpriteRenderer _spriteRenderer;
    private Plate _plate;
    private Coroutine _colorChangeCoroutine;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }

        GameObject plateObject = GameObject.Find("Plate");
        if (plateObject != null)
        {
            _plate = plateObject.GetComponent<Plate>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      
        if (_plate == null || !_plate.IsReadyForCooking()) return;

        if (!other.CompareTag($"Plate")) return;
        
        if (_colorChangeCoroutine != null)
        {
            StopCoroutine(_colorChangeCoroutine);
        }

        _colorChangeCoroutine = StartCoroutine(ChangeColorToCooked());
    }

    
    private IEnumerator ChangeColorToCooked()
    {
        Color targetColor = new Color(0.6f, 0.3f, 0.2f, 0.8f); // Красно-коричневый с прозрачностью
        float duration = 1.0f; // Длительность изменения цвета
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _spriteRenderer.color = targetColor; // Убедиться, что цвет точно установлен
    }

   
}