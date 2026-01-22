using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class ChooseArrowTesting : MonoBehaviour
{
    public TextMeshProUGUI display;
    int _posX = 6600;
    const int CONST_MaxArrowCount = 19;
    BowShooting _bowShooting;
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)  _posX *= -1;
        _bowShooting = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerGame>().shootingCurrent;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(_posX, 0);
        display.text = _bowShooting._arrowIndex.ToString();
    }

    public void ButtonMethodPrevious()
    {
        _bowShooting._arrowIndex--;
        if (_bowShooting._arrowIndex < 0) _bowShooting._arrowIndex = CONST_MaxArrowCount - 1;
        display.text = _bowShooting._arrowIndex.ToString();
    }
    public void ButtonMethodNext()
    {
        _bowShooting._arrowIndex++;
        if (_bowShooting._arrowIndex >= CONST_MaxArrowCount) _bowShooting._arrowIndex = 0;
        display.text = _bowShooting._arrowIndex.ToString();
     }
    
}
