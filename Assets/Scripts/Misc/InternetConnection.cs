using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InternetConnection : MonoBehaviour
{
    public static InternetConnection Instance;
    public bool hasInternet;
    Coroutine _checkInternet;
    const int CONST_WaitTime = 15;

    bool _internetCheckSwitch;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            switch (value)
            {
                case true:
                    _checkInternet = StartCoroutine(CheckInternetConnection((bul) =>
                    {
                        hasInternet = bul;
                    }));
                    break;
                case false:
                    StopCoroutine(_checkInternet);
                    break;
            }
        }
    }
    bool _isActive;
    void Awake()
    {
        Instance = this;
    }

    // void Start()
    // {
    //     IsActive = true;
    //     InvokeRepeating(nameof(RegularInternetConnectionCheck), 0f, 15f);
    //
    //   //  DontDestroyOnLoad(this);
    // }

    void RegularInternetConnectionCheck()
    {
        if (_internetCheckSwitch) return;
        _internetCheckSwitch = true;
        StartCoroutine(Utils.CheckInternetConnection((bul) =>
        {
            _internetCheckSwitch = false;
            hasInternet = bul;
        }));
    }

    IEnumerator CheckInternetConnection(System.Action<bool> isConnected)
    {
        WaitForSeconds wait = Utils.GetWait(CONST_WaitTime);
        UnityWebRequest request = new UnityWebRequest("https://google.com");
        yield return request.SendWebRequest();
        if (request.error == null)
        {
            // Debug.Log("success, connected to internet");
            isConnected?.Invoke(true);
        }
        else
        {
            // Debug.Log("no internet connection");
            isConnected?.Invoke(false);
        }

        yield return wait;
    }

}
