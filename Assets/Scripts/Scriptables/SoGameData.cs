using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class SoGameData : ScriptableObject
{
    [Title("Dynamic")]
    public MainGameType gameType;
    public SoBot botSp;

    [Title("General")]
    public Color textColorGreySubtle;
    public int waitTimeStartGame = 2;
    public string apiTimeUrl = "https://www.timeapi.io/api/time/current/zone?timeZone=Europe%2FLondon";
    
    [Title("Currency awards")]
    public int coinsPlay;
    public int coinsWin;
    public int coinsWinPlanet;
    public int coinsDailyBonus;
    
    [Title("Layers & Tags")]
    public LayerMask layTargets;
    public LayerMask layForTrajectory;
    public string tagBow, tagString;

    [Title("League")]
    #region 
    
    public Sprite[] leagueSprites;

    public string LeagueName(int leagueIndex)
    {
        string league = ((League)leagueIndex).ToString();
        if (System.Enum.Parse<League>(league) == League.Challenger) return league;
        
        string lName = "";
        foreach (char item in league)
        {
            if (Utils.IsLetter(item)) lName += item;
            else
            {
                int num = int.Parse(item.ToString());
                return $"{lName} {RomanNumber(num)}.";
            }
        }
        
        return "";

        string RomanNumber(int val)
        {
            switch (val)
            {
                case 1: return "I";
                case 2: return "II";
                case 3: return "III";
            }
            return "";
        }
    }
    public Dictionary<League, Vector3Int> leagueProgressionTotalStayPromote = new Dictionary<League, Vector3Int>()
    {
        { League.Bronze1, new Vector3Int(5, 0, 2) },
        { League.Bronze2, new Vector3Int(10, 2, 5) },
        { League.Bronze3, new Vector3Int(10, 3, 6) },
        { League.Silver1, new Vector3Int(15, 5, 9) },
        { League.Silver2, new Vector3Int(15, 6, 10) },
        { League.Silver3, new Vector3Int(15, 7, 10) },
        { League.Gold1, new Vector3Int(20, 9, 13) },
        { League.Gold2, new Vector3Int(20, 9, 14) },
        { League.Gold3, new Vector3Int(20, 10, 14) },
        { League.Platinum1, new Vector3Int(25, 13, 17) },
        { League.Platinum2, new Vector3Int(25, 14, 17) },
        { League.Platinum3, new Vector3Int(25, 14, 18) },
        { League.Diamond1, new Vector3Int(30, 18, 24) },
        { League.Diamond2, new Vector3Int(30, 20, 26) },
        { League.Diamond3, new Vector3Int(30, 25, 30) },
        { League.Challenger, new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue) },
    };
    public Dictionary<League, float> windStrengthByLeague = new Dictionary<League, float>()
    {
        { League.Bronze1, 0f },
        { League.Bronze2, 0f },
        { League.Bronze3, 0f },
        { League.Silver1, 0.2f },
        { League.Silver2, 0.2f },
        { League.Silver3, 0.2f },
        { League.Gold1, 0.5f },
        { League.Gold2, 0.5f },
        { League.Gold3, 0.5f },
        { League.Platinum1, 0.5f },
        { League.Platinum2, 0.8f },
        { League.Platinum3, 0.8f },
        { League.Diamond1, 0.8f },
        { League.Diamond2, 0.8f },
        { League.Diamond3, 0.8f },
        { League.Challenger, 1f },
    };
    #endregion

    [Title("Xp")]
    #region
    
    public int[] xpMilestones; 
    [SerializeField] Sprite[] levelIcons;
    public Sprite LevelIcon(int level)
    {
        switch (level)
        {
            case <= 5:
                return levelIcons[0];
            case > 5 and <= 20:
                return levelIcons[1];
            case > 20 and <= 60:
                return levelIcons[2];
            default:
                return levelIcons[3];
        }
    }

    public string XpTitle(int level)
    {
        switch (level)
        {
            case < 10:
                return "Novice";
            case >= 10 and < 20:
                return "Rookie";
            case >= 20 and < 30:
                return "Semi-Pro";
            case >= 30 and < 40:
                return "Pro";
            case >= 40 and < 50:
                return "Veteran";
            case >= 50 and < 60:
                return "Expert";
            case >= 60 and < 70:
                return "Master";
            case >= 70:
                return "Legend";
        }
    }
    public int xpWin = 200;
    public int xpWinDifferencePerScorePoint = 5;
    public int xpFirstWinOfDay = 100;
    public int xpFirstThreePlaysOfDay = 100;
    public int xpPerMinuteOfPlaying = 20;
    public int xpPerMinuteCap = 200;
    public int xpWinPlanet = 1000; //should be multiplied with current player level
    #endregion

    [Title("Scene names strings")]
    public string scene_Launch;
    public string scene_MainMenu;
    public string scene_CompleteGame;
    public string scene_Intro;
    public string scene_Tutorial;
    
    [Title("PlayerPrefs strings")]
    // //suffix is Type
    public string difficulty_Int = "difficulty AI";
    public string size_Int = "size of grid";
    public string windAmount_Fl = "wind amount";
    public string trajectoryVisible_Int = "trajectory is visible";
    public string campCurrPlanet_Int = "camp current planet";
    public string campCurrSub_Int = "camp current sublevel";
    
    [Title("Backend - item type brackets")]
    public int ItemTypeBracketSize = 100;
    public int ItemTypeBracketOrderHeads = 0;
    public int ItemTypeBracketOrderGloves = 1;
    //public int ItemTypeBracketOrderHeadwears = 2;
    public int ItemTypeBracketOrderBows = 3;
    public int ItemTypeBracketOrderArrows = 4;
    //public int ItemTypeBracketOrderTrails = 5;
    public int ItemTypeBracketOrderEnvironments = 6;
    
    public Dictionary<CloudData, int> IndicesForScriptables = new Dictionary<CloudData, int>()
    {
        { CloudData.Bow, 300 },
        { CloudData.Head, 0 },
        { CloudData.Gloves, 100 },
        { CloudData.Arrow, 400 },
        { CloudData.Environment, 600 },
    };

}
