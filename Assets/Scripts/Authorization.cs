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
    private AuthRequest _authData;

    public string currentToken;
    public int currentScore;

    private void Awake()
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
        int httpsStartIndex = initData.IndexOf("https");
        if (httpsStartIndex == -1)
        {
            return null;
        }

        string photoUrlPart = initData.Substring(httpsStartIndex);
        int nextAmpersandIndex = photoUrlPart.IndexOf('&');
        string photoUrlEncoded = nextAmpersandIndex == -1 ? photoUrlPart : photoUrlPart.Substring(0, nextAmpersandIndex);
        string photoUrl = DecodeUrl(photoUrlEncoded);

        return photoUrl;
    }

    private string DecodeUrl(string encodedUrl)
    {
        string decodedUrl = UnityWebRequest.UnEscapeURL(encodedUrl);
        decodedUrl = decodedUrl.Replace("\\/", "/");
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
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
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
        yield return WebApiManager.Instance.AuthenticateUser(jsonData, OnAuthSuccess, OnAuthError);
    }

    private void OnAuthSuccess(string responseJson)
    {
        AuthResponse response = JsonUtility.FromJson<AuthResponse>(responseJson);

        if (response.success)
        {
            currentToken = response.token;
            currentScore = response.score;
            scoreText.text = currentScore.ToString();
            playerID.text = response.success ? "Подключился" : "Ошибка";
        }
        else
        {
            Debug.LogError("Ошибка авторизации: Сервер вернул success = false");
        }
    }

    private void OnAuthError(string error)
    {
        Debug.LogError($"Ошибка запроса: {error}");
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
