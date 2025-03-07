using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private Authorization _authorization;
    private int _clickCount = 0; // Серверное количество кликов
    private int _localClickCount = 0; // Локальное количество кликов
    private int _pendingClicks = 0; // Количество кликов, ожидающих подтверждения от сервера
    [SerializeField] private TextMeshProUGUI clickCountText; // Текстовый элемент для отображения серверных кликов
    [SerializeField] private TextMeshProUGUI clickCountLocalText; // Текстовый элемент для локальных кликов
    [SerializeField] private GameObject targetObject; // Объект для анимации
    [SerializeField] private Button clickZoneButton; //Кнопка для обработки клика
    private Coroutine _scaleAnimationCoroutine; // Корутина для анимации масштабирования
    private Coroutine _countAnimationCoroutine; // Корутина для анимации числа кликов

    private void Start()
    {
        clickZoneButton.onClick.AddListener(HandleClick);
        if (_authorization == null)
        {
            Debug.LogError("Authorization component not found!");
            return;
        }
    }


    private void HandleClick()
    {
        if(_clickCount == 0)
        {
            //костыль
            bool crutch = int.TryParse(clickCountText.text, out _clickCount);
        }
        _clickCount++; // Увеличиваем счетчики кликов
        _localClickCount++;
        _pendingClicks++;

        StartCoroutine(SendClickToServer());
        clickCountLocalText.text = _localClickCount.ToString(); // Записываем локальный счетчик кликов

        // Запускаем анимацию масштабирования
        if (_scaleAnimationCoroutine != null)
        {
            StopCoroutine(_scaleAnimationCoroutine); // Останавливаем текущую анимацию, если она активна
        }
        _scaleAnimationCoroutine = StartCoroutine(ScaleAnimation());
    }

    private IEnumerator ScaleAnimation()
    { 
        targetObject.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);

        yield return new WaitForSeconds(0.1f);

        // Возвращаем масштаб к 1
        targetObject.transform.localScale = Vector3.one;
    }

    private IEnumerator SendClickToServer()
    {
        string token = _authorization.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("Token is not available!");
            _pendingClicks--; // Уменьшаем счетчик ожидающих кликов при ошибке
            yield break;
        }

        string jsonData = "{}"; // Если нужно отправить дополнительные данные, их можно добавить сюда
        yield return WebApiManager.Instance.AddScore(jsonData, token, OnScoreAdded, OnScoreError);
    }

    private void OnScoreAdded(string responseJson)
    {
        // Парсим ответ от сервера
        ScoreResponse response = JsonUtility.FromJson<ScoreResponse>(responseJson);

        if (response.success)
        {
            _pendingClicks--; // Уменьшаем счетчик ожидающих кликов
            // Синхронизируем локальный счетчик с серверным значением
            if (_pendingClicks == 0 && _clickCount != response.totalScore)
            {
                if (_countAnimationCoroutine != null)
                {
                    StopCoroutine(_countAnimationCoroutine); // Останавливаем текущую анимацию, если она активна
                }
                _countAnimationCoroutine = StartCoroutine(AnimateCountChange(_clickCount, response.totalScore));
            }
            _clickCount = response.totalScore; // Обновляем локальный счетчик
            UpdateClickCountText(_clickCount); // Обновляем текст на экране            
        }
        else
        {
            _pendingClicks--; // Уменьшаем счетчик даже при ошибке
            Debug.LogError("Failed to add score: Server returned success = false");
        }
    }

    private void OnScoreError(string error)
    {
        _pendingClicks--; // Уменьшаем счетчик даже при ошибке
        Debug.LogError($"Failed to add score: {error}");
    }

    private IEnumerator AnimateCountChange(int from, int to)
    {
        float duration = 0.5f; // Длительность анимации
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentValue = (int)Mathf.Lerp(from, to, elapsed / duration);
            UpdateClickCountText(currentValue); // Плавно обновляем текст
            yield return null;
        }

        UpdateClickCountText(to); // Убедимся, что текст установлен в конечное значение
    }

    private void UpdateClickCountText(int value)
    {
        if (clickCountText != null)
        {
            clickCountText.text = value.ToString();
        }
    }
}

[System.Serializable]
public class ScoreResponse
{
    public bool success;
    public int totalScore;
}
