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
        SetInitData("query_id=AAHrjK8IAAAAAOuMrwi1lGj0&user=%7B%22id%22%3A145722603%2C%22first_name%22%3A%22%D0%95%D0%B3%D0%BE%D1%80%22%2C%22last_name%22%3A%22%D0%91%D0%BE%D0%BD%D0%B4%D0%B0%D1%80%D1%8C%22%2C%22username%22%3A%22GregCooper%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%2C%22allows_write_to_pm%22%3Atrue%2C%22photo_url%22%3A%22https%3A%5C%2F%5C%2Ft.me%5C%2Fi%5C%2Fuserpic%5C%2F320%5C%2F3ZsEAJfiHm2WGMYNHLONv9YHdGoLQkifYrXUYoM2sVI.svg%22%7D&auth_date=1738451246&signature=nH0x453t2dZ-aBVXMrVvf2Ng8Uj8pyXVzXUAysfTFhUrOa7ZxKrNtYEXrSABwhTOKgC-pXSxeSC88lydy8yGAA&hash=d6f1c0dd9ee5ecedc55edd4d66fd465d6aa60087919e5058b67be63b349cbf49");
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
