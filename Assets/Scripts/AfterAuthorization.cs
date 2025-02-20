using UnityEngine;
using UnityEngine.UI;

public class AfterAuthorization : MonoBehaviour
{
    [SerializeField] private GameObject uiRootObject;
    [SerializeField] private GameObject nextAfterAuthorization;
    [SerializeField] private GameObject failedAuthorization;
    [SerializeField] private Button nextButton;

    private void Start()
    {
        nextButton.onClick.AddListener(CloseRootObject);
        failedAuthorization.SetActive(false);
        nextAfterAuthorization.SetActive(false);
    }
    public void CloseRootObject()
    {
        uiRootObject.SetActive(false); 
    }

    public void OpenFail()
    {
        nextAfterAuthorization.SetActive(false);
        failedAuthorization.SetActive(true);
    }

    public void OpenAfterAuthorization()
    {
        nextAfterAuthorization.SetActive(true);
        failedAuthorization.SetActive(false);
    }



}
