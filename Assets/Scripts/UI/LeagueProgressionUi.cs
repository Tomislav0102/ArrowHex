using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class LeagueProgressionUi : MonoBehaviour
{
    PersistentDataManager _dm;
    [SerializeField] Sprite[] spritesBars;
    Image[] _bars;
    float _barWidth;
    bool _isInitialized;

    int _total, _demote, _promote;
    List<bool> _progress = new List<bool>();
    public int Wins()
    {
        int wins = 0;
        for (int i = 0; i < _progress.Count; i++)
        {
            if (_progress[i]) wins++;
        }
        return wins;
    }
    int NoWins()
    {
        int noWins = 0;
        for (int i = 0; i < _progress.Count; i++)
        {
            if (!_progress[i]) noWins++;
        }
        return noWins;
    }
    
    [Title("Ref")]
    [SerializeField] RectTransform parProgress;
    [SerializeField] Image barPrefab;
    [SerializeField] RectTransform[] pointers;
    [SerializeField] Image[] pointersLeagueImages;
    [SerializeField] RectTransform[] pointersFrames;
    [SerializeField] TextMeshProUGUI[] pointerTexts;
    [SerializeField] FakeButtonUi fakeButtonWin, fakeButtonLoseDraw;
    [ReadOnly] [ShowInInspector] League _league;
    [SerializeField] TMP_Dropdown dropdown;
    

    public void InitializeMe()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        
        _dm = Launch.Instance.persistentDataManager;
        _league = (League)_dm.playerData.league;
        _progress = _dm.playerData.leagueProgress;
        CreateNewProgressBar();
        SetValues();
    }

    public void EndMatch(bool win)
    {
        InitializeMe();
        _progress.Add(win);
        _dm.playerData.leagueProgress = _progress;
        PlayerRewards.Data.leagueChange = null;
        SetValues();
    }


    void CreateNewProgressBar()
    {
        Utils.Activation(parProgress, _league == League.Challenger ? GenActivation.Off : GenActivation.On);
      //  _progress = new List<bool>();
        for (int i = 0; i < parProgress.childCount; i++)
        {
            Destroy(parProgress.GetChild(i).gameObject);
        }

        _total = _dm.gameData.leagueProgressionTotalStayPromote[_league].x;
        _demote = _dm.gameData.leagueProgressionTotalStayPromote[_league].y;
        _promote = _dm.gameData.leagueProgressionTotalStayPromote[_league].z;
        
        _bars = new Image[_total];
        for (int i = 0; i < _total; i++)
        {
            _bars[i] = Instantiate(barPrefab, parProgress);
        }
        _barWidth = parProgress.sizeDelta.x/(float)_total;
        
        Utils.Activation(pointers[0], _league == League.Bronze1 ? GenActivation.Off: GenActivation.On);
        Utils.Activation(pointers[1], GenActivation.On);
        if (_league != League.Bronze1) pointersLeagueImages[0].sprite = _dm.gameData.leagueSprites[(int)_league - 1];
        if (_league != League.Challenger) pointersLeagueImages[1].sprite = _dm.gameData.leagueSprites[(int)_league + 1];
        pointers[0].anchoredPosition = _barWidth * _demote * Vector2.right;
        pointerTexts[0].text = $"{_demote} LOSSES";
        pointers[1].anchoredPosition = _barWidth * _promote * Vector2.right;
        pointerTexts[1].text = $"{_promote} WINS"; 
        LayoutRebuilder.ForceRebuildLayoutImmediate(pointersFrames[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(pointersFrames[1]);
    }

    void SetValues()
    {
        if (_league == League.Challenger) return;
        
        for (int i = 0; i < _progress.Count; i++)
        {
            _bars[i].sprite = _progress[i] ? spritesBars[0] : spritesBars[1];
            _bars[i].transform.GetChild(_progress[i] ? 0 : 1).GetComponent<Image>().enabled = true;
        }
        
        int remain = _total - _progress.Count;
        
        int demotePos = _demote + NoWins();
        int promotePos = _promote + NoWins();
        if (promotePos > _total)
        {
            //cant promote
            Utils.Activation(pointers[1], GenActivation.Off);
        }

        int demoteRemain = Mathf.Max(_demote - Wins(), 0);
        int promoteRemain = Mathf.Max(_promote - Wins(), 0);
        
        if (_progress.Count >= _total)
        {
            if (promoteRemain == 0) ChangeLeague(GenChange.Increase, "0");
            else
            {
                if (demoteRemain > 0) ChangeLeague(GenChange.Decrease, "1");
                else ChangeLeague(GenChange.Reset, "2");
            }
            return;
        }

        if (demoteRemain > 0)
        {
            pointers[0].anchoredPosition = _barWidth * demotePos * Vector2.right;
            pointerTexts[0].text = $"{Wins()}/{demoteRemain} LOSSES";
            if (promoteRemain > remain)
            {
                if (demoteRemain > remain)
                {
                    ChangeLeague(GenChange.Decrease, "3");
                    return;
                }
            }
        }
        else
        {
            if (promoteRemain > remain)
            {
                ChangeLeague(GenChange.Reset, "4");
                return;
            }
            pointerTexts[0].text = "";
        }
        
        if (promoteRemain > 0)
        {
            pointers[1].anchoredPosition = _barWidth * promotePos * Vector2.right;
            pointerTexts[1].text = $"{Wins()}/{promoteRemain} WINS";
        }
        else
        {
            pointerTexts[1].text = "";
            ChangeLeague(GenChange.Increase, "5");
        }
    }
    
    void ChangeLeague(GenChange change, string st = "")
    {
        print($"{change} {st} ");
        PlayerRewards.Data.leagueChange = change;
        int lig = (int)_league;
        switch (change)
        {
            case GenChange.Increase:
                if (_league != League.Challenger) lig++;
                break;
            case GenChange.Decrease:
                if (_league != League.Bronze1) lig--;
                break;
        }
        _progress = _dm.playerData.leagueProgress = new List<bool>();
        _league = (League)lig;
        dropdown.value = (int)_league;
        CreateNewProgressBar();
    }

    #region DEBUG
    void DropdownValueChanged(int arg0)
    {
        _league = (League)arg0;
        CreateNewProgressBar();
    }
    
    void ButtonMethodWin()
    {
        _progress.Add(true);
        SetValues();
    }
    void ButtonMethodNoWin()
    {
        _progress.Add(false);
        SetValues();
    }
    #endregion

}
