using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public class PersistentDataManager : SerializedMonoBehaviour
{
    bool _oneShotSceneChange;

    [HideInInspector] public SoBow[] bows;
    [HideInInspector] public SoHead[] heads;
    [HideInInspector] public SoGloves[] gloves;
    [HideInInspector] public SoEnvironment[] environments;
    [HideInInspector] public SoArrow[] arrows;
    [HideInInspector] public SoPlanet[] planets;
    public static List<Planet> AllLevels;
    [HideInInspector] public SoBot[] bots;

    T[] SoGenericItem<T>() where T : SoItem
    {
        string path = string.Empty;
        if (typeof(T) == typeof(SoBow)) path = "SoBow";
        else if (typeof(T) == typeof(SoGloves)) path = "SoGloves";
        else if (typeof(T) == typeof(SoHead)) path = "SoHead";
        else if (typeof(T) == typeof(SoEnvironment)) path = "SoEnvironments";
        else if (typeof(T) == typeof(SoArrow)) path = "SoArrow";

        SoItem[] items = Resources.LoadAll<SoItem>(path).OrderBy(n => n.id).ToArray();
        T[] gens = new T[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            gens[i] = items[i] as T;
        }

        return gens;
    }

    public SoGameData gameData;
    public SoPlayerData playerData;
    public TeleType teleType;


    #region HELPERS
    [Title("Generating Ids for Scriptable items")]
    [SerializeField] SoItem[] itemsToGenerateIds;
    bool HasItems() => itemsToGenerateIds != null && itemsToGenerateIds.Length > 0;

    [Button]
    [ShowIf("HasItems")]
    void GenerateId()
    {
#if (UNITY_EDITOR)

        for (int i = 0; i < itemsToGenerateIds.Length; i++)
        {
            itemsToGenerateIds[i].id = i + gameData.IndicesForScriptables[itemsToGenerateIds[i].dataType];
            UnityEditor.EditorUtility.SetDirty(itemsToGenerateIds[i]);
        }
        itemsToGenerateIds = null;
#endif

    }
    #endregion

    private void Awake()
    {
        bows = SoGenericItem<SoBow>();
        heads = SoGenericItem<SoHead>();
        gloves = SoGenericItem<SoGloves>();
        environments = SoGenericItem<SoEnvironment>();
        arrows = SoGenericItem<SoArrow>();

        planets = Resources.LoadAll<SoPlanet>("SoPlanet").ToArray();
        AllLevels = new List<Planet>();
        for (int i = 0; i < planets.Length; i++)
        {
            for (int j = 0; j < planets[i].subLevels.Length; j++)
            {
                AllLevels.Add(planets[i].planet);
            }
        }

        bots = Resources.LoadAll<SoBot>("SoBot").ToArray();
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += CallEv_SceneLoaded;
        NetworkManager.Singleton.OnClientStarted += () => NetworkManager.Singleton.SceneManager.OnSceneEvent += CallEv_OnSceneEvent;

        //debug
        PlayerPrefs.SetInt(gameData.campCurrPlanet_Int, (int)playerData.campProgressCurrent.planet);
        PlayerPrefs.SetInt(gameData.campCurrSub_Int, playerData.campProgressCurrent.subLevel);
    }

    #region CALL EVENTS
    void CallEv_SceneLoaded(Scene arg0, LoadSceneMode arg1) => _oneShotSceneChange = false;

    private void CallEv_OnSceneEvent(SceneEvent sceneEvent)
    {
        _oneShotSceneChange = false;
        bool showDebug = false;
        SceneEventType tip = sceneEvent.SceneEventType;
        switch (tip)
        {
            case SceneEventType.LoadEventCompleted:
                if (showDebug) print($"LoadEventCompleted: {sceneEvent.SceneName}");
                break;

            #region LESS IMPORTANT
            case SceneEventType.Load:
                if (showDebug) print($"Load: {sceneEvent.SceneName}");
                break;
            case SceneEventType.Unload:
                if (showDebug) print($"Unload: {sceneEvent.SceneName}");
                break;
            case SceneEventType.UnloadEventCompleted:
                if (showDebug) print($"UnloadEventCompleted: {sceneEvent.SceneName}");
                break;
            case SceneEventType.Synchronize:
                if (showDebug) print($"Synchronize: {sceneEvent.SceneName}");
                break;
            case SceneEventType.ReSynchronize:
                if (showDebug) print($"ReSynchronize: {sceneEvent.SceneName}");
                break;
            case SceneEventType.LoadComplete:
                if (showDebug) print($"LoadComplete: {sceneEvent.SceneName}");
                break;
            case SceneEventType.UnloadComplete:
                if (showDebug) print($"UnloadComplete: {sceneEvent.SceneName}");
                break;
            case SceneEventType.SynchronizeComplete:
                if (showDebug) print($"SynchronizeComplete: {sceneEvent.SceneName}");
                break;
            case SceneEventType.ActiveSceneChanged:
                if (showDebug) print($"ActiveSceneChanged: {sceneEvent.SceneName}");
                break;
            case SceneEventType.ObjectSceneChanged:
                if (showDebug) print($"ObjectSceneChanged: {sceneEvent.SceneName}");
                break;
            #endregion

        }
    }
    #endregion

    public IEnumerator NewSceneAfterFadeIn(string origin, MainGameType gameType, bool useFade = true)
    {
        // print($"origin: {origin}");
        if (_oneShotSceneChange) yield break;
        _oneShotSceneChange = true;
        gameData.gameType = gameType;
        WaitForSeconds wait = Utils.GetWait(2);
        if (useFade)
        {
            Utils.FadeOut?.Invoke(GenMove.Exit);
            yield return wait;
        }

        switch (gameData.gameType)
        {
            case MainGameType.MainMenu:
                SceneManager.LoadScene(gameData.scene_MainMenu);
                break;
            case MainGameType.Intro:
                NetworkManager.Singleton.SceneManager.LoadScene(gameData.scene_Intro, LoadSceneMode.Single);
                break;
            case MainGameType.Tutorial:
                SceneManager.LoadScene(gameData.scene_Tutorial);
                break;
            default:
                NetworkManager.Singleton.SceneManager.LoadScene(gameData.scene_CompleteGame, LoadSceneMode.Single);
                break;
        }
    }

    public async void FirstDownloadFromCLoud()
    {
        Utils.SpinnerActive?.Invoke(true, "Downloading user data, please wait...");

        var (success, errorMessage) = await Launch.Instance.localDataManager.HandleLogin($"{playerData.id}_{playerData.namePlayer}");
        if (success)
        {
            if (Launch.Instance.showDebugBackend) print("success in HandleLogin");

            playerData.xp = LocalUserData.Experience;
            playerData.gold = LocalUserData.CurrencyGold;
            playerData.diamonds = LocalUserData.CurrencyDiamond;

            playerData.league = LocalUserData.MultiplayerStatsData.LeagueCurrent;
            playerData.mpTotal = LocalUserData.MultiplayerStatsData.MatchesOverall;
            playerData.mpWins = LocalUserData.MultiplayerStatsData.WinsOverall;

            playerData.leagueProgress = new List<bool>();
            foreach (MultiplayerStatsMatchData item in LocalUserData.MultiplayerStatsData.Matches)
            {
                playerData.leagueProgress.Add(int.Parse(item.ScorePlayer) > int.Parse(item.ScoreOpponent));
            }

            #region CAMP
            LevelData[] levelData = LocalUserData.SinglePlayerCampaignData.CompletedLevels;
            int totPlanet = 0;
            int totSub = -1;
            if (levelData.Length > 0)
            {
                totPlanet = levelData[^1].Level;
                totSub = levelData[^1].Sublevel;
            }
            playerData.campProgressTotal = new CampProgress((Planet)totPlanet, totSub);
            playerData.campProgressCurrent = new CampProgress((Planet)PlayerPrefs.GetInt(gameData.campCurrPlanet_Int), PlayerPrefs.GetInt(gameData.campCurrSub_Int));
            int curr = CampaignManager.IndexFromGetCampProgress(playerData.campProgressCurrent);
            int tot = CampaignManager.IndexFromGetCampProgress(playerData.campProgressTotal);
            if (curr > tot + 1)
            {
                //print("Current campaign playerpref is corrupted, reducing it to total value)");
                curr = tot == AllLevels.Count - 1 ? tot : tot + 1;
                playerData.campProgressCurrent = CampaignManager.CampProgressFromIndex(curr);
                PlayerPrefs.SetInt(gameData.campCurrPlanet_Int, (int)playerData.campProgressCurrent.planet);
                PlayerPrefs.SetInt(gameData.campCurrSub_Int, playerData.campProgressCurrent.subLevel);
            }
            #endregion

            #region ITEMS
            foreach (int item in LocalUserData.ItemsEquipped)
            {
                switch (item)
                {
                    case < 100:
                        playerData.headItem = heads.FirstOrDefault(n => n.id == item);
                        break;
                    case < 200:
                        playerData.glovesItem = gloves.FirstOrDefault(n => n.id == item);
                        break;
                    case >= 300 and < 400:
                        playerData.bowItem = bows.FirstOrDefault(n => n.id == item);
                        break;
                    case >= 600 and < 700:
                        playerData.envItem = environments.FirstOrDefault(n => n.id == item);
                        break;
                }
            }

            playerData.ownedItems = new Dictionary<CloudData, List<int>>()
            {
                { CloudData.Head, new List<int>() },
                { CloudData.Gloves, new List<int>() },
                { CloudData.Bow, new List<int>() },
                { CloudData.Environment, new List<int>() },
            };
            foreach (int item in LocalUserData.ItemsPurchased)
            {
                switch (item)
                {
                    case < 100:
                        playerData.ownedItems[CloudData.Head].Add(item);
                        break;
                    case < 200:
                        playerData.ownedItems[CloudData.Gloves].Add(item);
                        break;
                    case >= 300 and < 400:
                        playerData.ownedItems[CloudData.Bow].Add(item);
                        break;
                    case >= 600 and < 700:
                        playerData.ownedItems[CloudData.Environment].Add(item);
                        break;
                }
            }
            foreach (KeyValuePair<CloudData, List<int>> item in playerData.ownedItems) //default items
            {
                if (item.Value.Count == 0) item.Value.Add(gameData.IndicesForScriptables[item.Key]);
            }
            #endregion

            StartCoroutine(NewSceneAfterFadeIn("initial main menu, from dm", MainGameType.MainMenu, false));
        }
        else
        {
            if (Launch.Instance.showDebugBackend) Debug.LogError("error in PostLoginAsync");
            Utils.SpinnerActive(false, "");
        }
    }


    #region BACKEND
    async void Post_PlayerStats()
    {
        var (success, errorMessage) = await Launch.Instance.apiClientManager.UpdatePlayerStatsAsync(playerData.xp, playerData.gold, playerData.diamonds);
    }

    async void Post_EquippedItem()
    {
        var (success, errorMessage) = await Launch.Instance.apiClientManager.UpdateItemEquippedAsync(0, true);
    }

    async void Post_CampStats()
    {
        var (success, errorMessage) = await Launch.Instance.apiClientManager.PostCampaignAsync((int)playerData.campProgressTotal.planet, playerData.campProgressTotal.subLevel);
    }

    async void Post_MpMatchStats()
    {
        var (success, errorMessage) = await Launch.Instance.apiClientManager.PostMultiplayerMatchAsync("", 0, 0, 0, 0, 0, 0, "");
    }
    #endregion


    #region MAYBE REDUNDANT

    public void UpdateValue(CloudData dataType, string val)
    {
        switch (dataType)
        {
            case CloudData.Id:
                playerData.id = val;
                break;
            case CloudData.Name:
                playerData.namePlayer = val;
                break;
            case CloudData.ImageUrl:
                playerData.imageUrl = val;
                break;
            default:
                return;
        }
    }

    public void UpdateValue(CloudData dataType, int val, bool post = true)
    {
        switch (dataType)
        {
            case CloudData.Xp:
                playerData.xp = val;
                break;
            case CloudData.League:
                playerData.league = val;
                break;
            case CloudData.MpTotal:
                playerData.mpTotal = val;
                break;
            case CloudData.MpWins:
                playerData.mpWins = val;
                break;
            case CloudData.Currency_Gold:
                playerData.gold = val;
                break;
            case CloudData.Currency_Diamond:
                playerData.diamonds = val;
                break;
            default:
                return;
        }
        if (post) Post_PlayerStats();
    }

    public void UpdateValue(CloudData dataType, SoItem val, bool post = true) { }

    public async void UpdateValue(CampProgress valCurr = null, CampProgress valTot = null, bool post = true)
    {
        if (valCurr != null)
        {
            playerData.campProgressCurrent = valCurr;
            PlayerPrefs.SetInt(gameData.campCurrPlanet_Int, (int)valCurr.planet);
            PlayerPrefs.SetInt(gameData.campCurrSub_Int, valCurr.subLevel);
        }

        if (valTot != null)
        {
            playerData.campProgressTotal = valTot;
            var (success, errorMessage) = await Launch.Instance.apiClientManager.PostCampaignAsync((int)valTot.planet, valTot.subLevel);
        }

    }

    public void UpdateValue(bool val)
    {
        playerData.leagueProgress.Add(val);
    }

    void Up(CloudData dataType)
    {
        switch (dataType)
        {
            case CloudData.Id:
                break;
            case CloudData.Name:
                break;
            case CloudData.ImageUrl:
                break;
            case CloudData.Xp:
                break;
            case CloudData.League:
                break;
            case CloudData.MpTotal:
                break;
            case CloudData.MpWins:
                break;
            case CloudData.Bow:
                break;
            case CloudData.Head:
                break;
            case CloudData.Gloves:
                break;
            case CloudData.Arrow:
                break;
            case CloudData.Environment:
                break;
            case CloudData.Currency_Gold:
                break;
            case CloudData.Currency_Diamond:
                break;
            case CloudData.MatchHistory:
                break;
            default:
                return;
        }

    }
    #endregion


    #region DEBUGS

    [ContextMenu("Upload camp")]
    void M1()
    {
        Post_CampStats();
    }

    [ContextMenu("Upload xp, gold, diamonds")]
    void M3()
    {
        Post_PlayerStats();
    }
    #endregion



}