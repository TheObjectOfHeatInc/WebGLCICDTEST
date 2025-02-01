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

        RequestInitData();
    }
    
    public void RequestInitData()
    {

        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("requestInitDataFromTelegram");
        #endif
    }

    public void SetInitData(string initData)
    { 

        string photoUrl = ExtractPhotoUrl(initData);
        Debug.Log(photoUrl);
        if (!string.IsNullOrEmpty(photoUrl))
        {
                StartCoroutine(LoadImageFromUrl(photoUrl));
        }

        if (!string.IsNullOrEmpty(initData))
        {
            _authData = new AuthRequest { initData = initData.ToString() };
            StartCoroutine(AuthenticateUser());
        }
        else
        {
            Debug.LogWarning("InitData is empty or null.");
        }
    }

    private string ExtractPhotoUrl(string initData)
    {
    // Ищем начало строки "https"
    int httpsStartIndex = initData.IndexOf("https");
    if (httpsStartIndex == -1)
    {
        return null; // Если "https" не найден
    }

    // Извлекаем часть строки, начиная с "https"
    string photoUrlPart = initData.Substring(httpsStartIndex);

    // Ищем следующий разделитель & или конец строки
    int nextAmpersandIndex = photoUrlPart.IndexOf('&');
    string photoUrlEncoded = nextAmpersandIndex == -1 ? photoUrlPart : photoUrlPart.Substring(0, nextAmpersandIndex);

    // Декодируем URL и убираем лишние символы
    string photoUrl = DecodeUrl(photoUrlEncoded);

    return photoUrl;
    }   

    private string DecodeUrl(string encodedUrl)
    {
    // Декодируем URL
    string decodedUrl = UnityWebRequest.UnEscapeURL(encodedUrl);

    // Заменяем обратные слэши на обычные слэши
    decodedUrl = decodedUrl.Replace("\\/", "/");

    // Убираем лишние символы в конце
    if (decodedUrl.EndsWith("\"}"))
    {
        decodedUrl = decodedUrl.Substring(0, decodedUrl.Length - 2);
    }

    return decodedUrl;
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
                    currentScore = response.score;
                    Debug.Log($"Авторизация успешна! Токен: {currentToken}, Счёт: {currentScore}");
                    scoreText.text = currentScore.ToString();
                    playerID.text = response.success? "Подключился" : "Ошибка";
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
