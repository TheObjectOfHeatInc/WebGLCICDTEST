using UnityEngine;

public class SpawnMeat : MonoBehaviour
{
    [SerializeField] private GameObject meatPrefab;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private float _spawnOffset = 300f;

    private Camera _mainCamera;
    private float _screenBoundary;

    void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null) _screenBoundary = _mainCamera.orthographicSize;
    }

    void SpawnNewMeat()
    {
        // Создаем мясо как дочерний объект Canvas
        GameObject meat = Instantiate(meatPrefab, targetCanvas.transform);

        // Устанавливаем случайную позицию в пределах Canvas
        RectTransform meatRect = meat.GetComponent<RectTransform>();
        Vector2 canvasSize = targetCanvas.GetComponent<RectTransform>().sizeDelta;

        meatRect.anchoredPosition = new Vector2(
            Random.Range(-canvasSize.x / 2, canvasSize.x / 2),
            canvasSize.y / 2 - _spawnOffset
        );
    }
}


// И использовать в методе:



