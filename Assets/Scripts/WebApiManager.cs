using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WebApiManager : MonoBehaviour
{
    public static WebApiManager Instance { get; private set; }

    // API URLs
    private const string AuthURL = "https://api.lehagigachad.ru/auth";
    private const string AddScoreURL = "https://api.lehagigachad.ru/api/addScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator AuthenticateUser(string jsonData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        yield return PostRequest(AuthURL, jsonData, null, onSuccess, onError);
    }

    public IEnumerator AddScore(string jsonData, string token, System.Action<string> onSuccess, System.Action<string> onError)
    {
        yield return PostRequest(AddScoreURL, jsonData, token, onSuccess, onError);
    }

    private IEnumerator GetRequest(string url, System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new BypassCertificate();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    private IEnumerator PostRequest(string url, string jsonData, string token, System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.certificateHandler = new BypassCertificate();
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
