using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Includes Xp, League and Currencies
/// </summary>
public class PlayerRewards
{
    GameManager gm;
    PersistentDataManager _dm;
    bool _gameOver;
    float _xpTimer;
    //from _dm
    bool XpDailyBonus() => true;
    bool XpFirstWinOfDay() => true;

    public static RewardData Data;
    
    public PlayerRewards()
    {
        Data = new RewardData();
        gm = GameManager.Instance;
        _dm = Launch.Instance.persistentDataManager;
    }

    public void UpdateLoop()
    {
        if (_gameOver || Data.xpPerMinute >= _dm.gameData.xpPerMinuteCap) return;
        _xpTimer += Time.deltaTime;
        if (_xpTimer >= 60f)
        {
            _xpTimer = 0f;
            Data.xpPerMinute += _dm.gameData.xpPerMinuteOfPlaying;
        }
    }
    public static void CalculateLevelFromXp(out int lv, out int toNext)
    {
        PersistentDataManager dm = Launch.Instance.persistentDataManager;
        int[] xpMilestones = dm.gameData.xpMilestones;
        int xp = dm.playerData.xp;

        for (int i = 0; i < xpMilestones.Length; i++)
        {
            if (xp < xpMilestones[i])
            {
                lv = i;
                toNext = xpMilestones[i] - xp;
                return;
            }
        }
        
        //max level reached
        lv =  xpMilestones.Length;
        toNext = 0;
    }

    public void RewardsCalculation(GenResult matchResult, CampaignFinishState campaignFinishState, bool isRepeatingCampaign)
    {
        _gameOver = true;
        Data.matchResult = matchResult;
        Data.campaignFinishState = campaignFinishState;

        if (matchResult == GenResult.Win)
        {
            int scoreDiff = (int)gm.scoresNet[0] - (int)gm.scoresNet[1];
            Data.xpWin = _dm.gameData.xpWin;
            Data.xpScoreDiff = Mathf.Abs(scoreDiff) * _dm.gameData.xpWinDifferencePerScorePoint;
            Data.xpFirstWinOfDay = XpFirstWinOfDay() ? _dm.gameData.xpFirstWinOfDay : 0;
            
            Data.coinWin = _dm.gameData.coinsWin;

            CalculateLevelFromXp(out int lv, out _);
            switch (campaignFinishState)
            {
                case CampaignFinishState.PlanetDone:
                    Data.xpCampaign = _dm.gameData.xpWinPlanet * (lv + 1);
                    Data.coinWinPlanet = _dm.gameData.coinsWinPlanet;
                    break;
            }
        }
        
        
        Data.xpDaily = XpDailyBonus() ? _dm.gameData.xpFirstThreePlaysOfDay : 0;

        Data.coinPlay = _dm.gameData.coinsPlay;
        Data.coinDaily = XpDailyBonus() ? _dm.gameData.coinsDailyBonus : 0;

        if (isRepeatingCampaign && _dm.gameData.gameType == MainGameType.Campaign)
        {
            Debug.Log("repeating campaign, no rewards");
            Data.NoRewards();
        }
        
        if (_dm.gameData.gameType == MainGameType.Multi) gm.leagueProgressionUi.EndMatch(matchResult == GenResult.Win);
        
        gm.uiManager.EndMatch();
    }



    #region DEBUG
    public static void GetMeToLevel(int targetLevel)
    {
        CalculateLevelFromXp(out int lv, out _);
        if (targetLevel <= lv || targetLevel > Launch.Instance.persistentDataManager.gameData.xpMilestones.Length - 1) return;
        Launch.Instance.persistentDataManager.playerData.xp = Launch.Instance.persistentDataManager.gameData.xpMilestones[targetLevel - 1];
    }
    #endregion
    
    
}

