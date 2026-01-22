using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

public class TestDailyRewards : MonoBehaviour
{
    public int year, month, day, hour, minute, second;

    const string API_URL = "https://www.timeapi.io/api/time/current/zone?timeZone=Europe%2FLondon";


    void Start()
    {
        StartCoroutine(GetRealDateTimeFromApi());
    }

    IEnumerator GetRealDateTimeFromApi()
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            Debug.Log("Received: " + www.downloadHandler.text);
        }
    }
    
    bool Have24HoursPassed(string dateInString)
    {
        DateTime prevTimeWithRewards = DateTime.Parse(dateInString);
        TimeSpan timeSpan = DateTime.Now - prevTimeWithRewards;
        return timeSpan.Days > 0;
    }
    [Button]
    void DisplaySomething()
    {
        StartCoroutine(GetRealDateTimeFromApi());
        // DateTime prevTimeWithRewards = new DateTime(year, month, day, hour, minute, second);
        // print(Have24HoursPassed(prevTimeWithRewards.ToString()));
    }
}
