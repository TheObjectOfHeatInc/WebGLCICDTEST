using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

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
    private const string AuthURL = "https://api.lehagigachad.ru/auth";
    private AuthRequest _authData;

    public string currentToken;
    public int currentScore;

    // Метод для запроса initData из JavaScript
    public void RequestInitData()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("requestInitDataFromTelegram");
        #endif
    }

    public void SetInitData(string initData)
    {
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
