using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class AuthRequest
{
    public string initData;
}

[System.Serializable]
public class AuthResponse
{
    public bool success;
    public string token;
    public int score;
}


public class Authorization : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI playerID;
    [SerializeField] private Image userImage;
    private const string AuthURL = "https://api.lehagigachad.ru/auth";
    private AuthRequest _authData;

    public string currentToken;
    public int currentScore;

    // Метод для запроса initData из JavaScript

    private void Start()
    {
        Debug.Log("Authorization script started.");
        RequestInitData();
    }
    
    public void RequestInitData()
    {
        Debug.Log("RequestInitData");
        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("requestInitDataFromTelegram");
        #endif
    }

    public void SetInitData(string initData)
    { 
        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("alert", "Привет, это сообщение из Unity!", initData);
        #endif

        string photoUrl = ExtractPhotoUrl(initData);
        if (!string.IsNullOrEmpty(photoUrl))
        {
                StartCoroutine(LoadImageFromUrl(photoUrl));
        }

        if (!string.IsNullOrEmpty(initData))
        {
            _authData = new AuthRequest { initData = initData };
            StartCoroutine(AuthenticateUser());
        }
        else
        {
            Debug.LogWarning("InitData is empty or null.");
        }
    }

    private string ExtractPhotoUrl(string initData)
    {
        // Разделяем строку на пары ключ-значение
        var keyValuePairs = initData.Split('&');
        foreach (var pair in keyValuePairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue[0] == "photo_url")
            {
                // Декодируем URL-encoded значение
                return UnityWebRequest.UnEscapeURL(keyValue[1]);
            }
        }
        return null;
    }

     private IEnumerator LoadImageFromUrl(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Получаем текстуру из запроса
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                // Создаем спрайт из текстуры
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                // Устанавливаем спрайт в Image
                userImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image: " + request.error);
            }
        }
    }

    private IEnumerator AuthenticateUser()
    {
        if (_authData == null)
        {
            Debug.LogError("Auth data is not set.");
            yield break;
        }

        string jsonData = JsonUtility.ToJson(_authData);

        using (UnityWebRequest request = new UnityWebRequest(AuthURL, "POST"))
        {
            request.certificateHandler = new BypassCertificate();
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    currentToken = response.token;
                    currentScore = response.score;
                    Debug.Log($"Авторизация успешна! Токен: {currentToken}, Счёт: {currentScore}");
                    scoreText.text = currentScore.ToString();
                    playerID.text = currentToken.ToString();
                }
                else
                {
                    Debug.LogError("Ошибка авторизации: Сервер вернул success = false");
                }
            }
            else
            {
                Debug.LogError($"Ошибка запроса: {request.error}");
            }
        }
    }

    public string GetToken()
    {
        return currentToken;
    }

    public int GetScore()
    {
        return currentScore;
    }
}


public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
