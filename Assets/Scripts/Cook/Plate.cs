using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Plate : MonoBehaviour
{
    [SerializeField] private Image _plateImage;
    [SerializeField] private float _heatingSpeed = 0.5f;
    [SerializeField] private Transform switchTransform;
    [SerializeField] private Button _powerButton;

    private const float OffRotation = 0f;
    private const float OnRotation = 90f;
    private const float RotationSpeed = 180f;

    private bool _isPowerOn = false;
    private float _currentHeat = 0f;
    private readonly Color _coldColor = Color.white;
    private readonly Color _hotColor = Color.red;
    private Coroutine _heatCoroutine;
    private Coroutine _rotationCoroutine;
    
    void Start()
    {
        _plateImage.color = _coldColor;
        _powerButton.onClick.AddListener(TogglePower);
    }
    
    private void TogglePower()
    {
        _isPowerOn = !_isPowerOn;

        if (_heatCoroutine != null)
            StopCoroutine(_heatCoroutine);
    
        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);

        _heatCoroutine = StartCoroutine(_isPowerOn ? HeatUp() : CoolDown());
        _rotationCoroutine = StartCoroutine(RotateKnob());
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
    
    private IEnumerator RotateKnob()
    {
        float targetRotation = _isPowerOn ? OnRotation : OffRotation;
    
        while (Mathf.Abs(switchTransform.rotation.eulerAngles.z - targetRotation) > 0.1f)
        {
            float newRotation = Mathf.MoveTowards(switchTransform.rotation.eulerAngles.z, targetRotation, RotationSpeed * Time.deltaTime);
            switchTransform.rotation = Quaternion.Euler(0f, 0f, newRotation);
            yield return null;
        }
    
        switchTransform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
    }
}