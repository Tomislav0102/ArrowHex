using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FakeToggleUi : RequirementsUi, IPointerClickHandler
{
    //0-off, 1-on
    [SerializeField] Image myImage;
    [SerializeField] Sprite[] images;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] string[] textValues;
    System.Action<bool> _onClick;
    
    public bool IsOn
    {
        get => _isOn;
        set
        {
            _isOn = value;
            myImage.sprite = images[_isOn ? 1 : 0];
            text.text = textValues[_isOn ? 1 : 0];
            LayoutRebuilder.ForceRebuildLayoutImmediate(text.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(myImage.rectTransform);
        }
    }
    bool _isOn;
    
    public void InitializeMe(System.Action<bool> onClick, bool defaultState = false)
    {
        _onClick = onClick;
        IsOn = defaultState;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CheckRequirements()) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        IsOn = !IsOn;
        _onClick?.Invoke(IsOn);
    }
}
