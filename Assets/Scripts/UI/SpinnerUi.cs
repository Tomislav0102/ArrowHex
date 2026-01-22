using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinnerUi : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] Transform parImages;
    Image[] _images;
    [SerializeField] TextMeshProUGUI infoText;
    float _rotSpeed;
    const float CONST_MaxTime = 0.2f;
    Vector3 _forwardOffset = new Vector3(0, 1f, 2f);
    Transform _camPos;

    bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Utils.Activation(container, value ? GenActivation.On : GenActivation.Off);
            if (value && _camPos != null)
            {
                transform.position = new Vector3(_camPos.position.x, 0f, _camPos.position.z);
                transform.forward = Vector3.ProjectOnPlane(_camPos.forward, Vector3.up);
                transform.position += transform.TransformDirection(_forwardOffset);
            }
        }
    }
    bool _isActive;

    void Awake()
    {
        _images = Utils.AllChildren<Image>(parImages);
    }

    void Start()
    {
        _camPos = GetComponent<Canvas>().worldCamera.transform;
        Utils.SpinnerActive += CallEv_SpinnerActive;
    }
    
    
    void Update()
    {
        if (!IsActive) return;
    
        _rotSpeed += Time.deltaTime;
        if (_rotSpeed > CONST_MaxTime)
        {
            _rotSpeed = 0f;
            parImages.Rotate(0f, 0f, -360f / _images.Length);
        }
    }
    
    
    void CallEv_SpinnerActive(bool active, string message)
    {
        IsActive = active;
        infoText.text = message;
    }
    
    
    void OnDisable()
    {
        Utils.SpinnerActive -= CallEv_SpinnerActive;
    }

    void ColorMe()
    {
        _images = Utils.AllChildren<Image>(parImages);
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].color = Color.Lerp(Color.clear, Color.white, (float)i / (float)_images.Length);
        }
    }
}
