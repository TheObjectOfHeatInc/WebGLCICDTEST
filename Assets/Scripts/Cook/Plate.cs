using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Plate : MonoBehaviour
{
    [SerializeField] private Image _plateImage;
    [SerializeField] private float _heatingSpeed = 0.5f;
    [SerializeField] private Button _powerButton;
    
    private bool _isPowerOn = false;
    private float _currentHeat = 0f;
    private readonly Color _coldColor = Color.white;
    private readonly Color _hotColor = Color.red;
    private Coroutine _heatCoroutine;

    void Start()
    {
        _plateImage.color = _coldColor;
        _powerButton.onClick.AddListener(TogglePower);
    }

    private void TogglePower()
    {
        _isPowerOn = !_isPowerOn;

        if (_heatCoroutine != null)
        {
            StopCoroutine(_heatCoroutine);
        }

        _heatCoroutine = StartCoroutine(_isPowerOn ? HeatUp() : CoolDown());
    }

    private IEnumerator HeatUp()
    {
        while (_currentHeat < 1f)
        {
            _currentHeat += _heatingSpeed * Time.deltaTime;
            _plateImage.color = Color.Lerp(_coldColor, _hotColor, _currentHeat);
            yield return null;
        }
    }

    private IEnumerator CoolDown()
    {
        while (_currentHeat > 0f)
        {
            _currentHeat -= _heatingSpeed * Time.deltaTime;
            _plateImage.color = Color.Lerp(_coldColor, _hotColor, _currentHeat);
            yield return null;
        }
    }

    public bool IsReadyForCooking()
    {
        return _currentHeat >= 0.7f;
    }
}