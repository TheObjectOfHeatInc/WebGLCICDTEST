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
    [SerializeField]private TextMeshProUGUI scoreText;
    [SerializeField]private TextMeshProUGUI playerID;
    private const string AUTH_URL = "https://api.lehagigachad.ru/auth";
    private AuthRequest authData;
    
    public string currentToken;
    public int currentScore;

    // Метод для установки initData из JavaScript
    public void SetInitData(string initData)
    {
        if (!string.IsNullOrEmpty(initData))
        {
            authData = new AuthRequest { initData = initData };
            StartCoroutine(AuthenticateUser());
        }
        else
        {
            Debug.LogWarning("InitData is empty or null.");
        }
    }

    private IEnumerator AuthenticateUser()
    {
        if (authData == null)
        {
            Debug.LogError("Auth data is not set.");
            yield break;
        }

        string jsonData = JsonUtility.ToJson(authData);

        using (UnityWebRequest request = new UnityWebRequest(AUTH_URL, "POST"))
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
                    playerID.text =  currentToken.ToString();     
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
