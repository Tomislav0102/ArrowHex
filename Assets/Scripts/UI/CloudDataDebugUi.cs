using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CloudDataDebugUi : MonoBehaviour
{
    PersistentDataManager _dm;
    SoPlayerData _data;
    public GameObject mainWindow;
    public TextMeshProUGUI id, nameField, imageUrl, xp, league, mpWins, mpTotal, gold, diamond, bowName, headName, glovesName, arrowName, envName,
        campCurrLevel, campCurrPlanet, campTotLevel, campTotPlanet;
    public Image[] leagueBars;
    // [Header("Scene References")]
    // public LeagueProgressionUi leagueProgressionUi;

    void Awake()
    {
        _dm = Launch.Instance.persistentDataManager;
        _data = _dm.playerData;
    }

    void Start()
    {
        Refresh();
        mainWindow.SetActive(false);
    }

    public void BtnMain()
    {
        mainWindow.SetActive(!mainWindow.activeSelf);
        Refresh();
    }

    void Refresh()
    {
        id.text = _data.id;
        nameField.text = _data.namePlayer;
        imageUrl.text = _data.imageUrl;
        xp.text = _data.xp.ToString();
        league.text = $"{_data.league} - {(League)_data.league}";
        mpTotal.text = _data.mpTotal.ToString();
        mpWins.text = _data.mpWins.ToString();
        gold.text = _data.gold.ToString();
        diamond.text = _data.diamonds.ToString();
        bowName.text = _data.bowItem.itemName;
        headName.text = _data.headItem.itemName;
        glovesName.text = _data.glovesItem.itemName;
        arrowName.text = _data.arrowItem.itemName;
        envName.text = _data.envItem.itemName;
        campCurrLevel.text = _dm.playerData.campProgressCurrent.subLevel.ToString();
        campCurrPlanet.text = _dm.playerData.campProgressCurrent.planet.ToString();
        campTotLevel.text = _dm.playerData.campProgressTotal.subLevel.ToString();
        campTotPlanet.text =  _dm.playerData.campProgressTotal.planet.ToString();

        for (int i = 0; i < leagueBars.Length; i++)
        {
            Utils.Activation(leagueBars[i], i < _dm.playerData.leagueProgress.Count ? GenActivation.On : GenActivation.Off);
        }
        for (int i = 0; i < _dm.playerData.leagueProgress.Count; i++)
        {
            leagueBars[i].color = _dm.playerData.leagueProgress[i] ? Color.green : Color.grey;
        }
    }
}
