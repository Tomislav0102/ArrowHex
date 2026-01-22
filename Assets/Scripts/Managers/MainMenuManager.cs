using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Serialization;


public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    PersistentDataManager _dm;
    SoPlayerData _pData;
    bool _oneHitExitScene;
    public Transform canvasTransform;
    public Camera cameraPlayer;
    enum PanelVisible { Play, CampSelection, Inventory_Store}
    PanelVisible PanVisible
    {
        set => Utils.ActivateOneArrayElement(mainPanels, (int)value);
    }
    [SerializeField] GameObject[] mainPanels;
    public GameObject noInternetWindow, requiredLevelWindow;
    [SerializeField] FakeButtonUi playTutorialFb;
    public InventoryManager inventoryGrid;

    [Title("General/shared")]
    [SerializeField] TabGroupUi tabGroupUiNavigation;
    [SerializeField] Image[] levelIconImages;
    [SerializeField] TextMeshProUGUI[] levelTexts;
    [SerializeField] TextMeshProUGUI[] xpTexts;
    [SerializeField] TextMeshProUGUI xpTitleText;
    [SerializeField] Image[] leagueIconImages;
    [SerializeField] TextMeshProUGUI[] leagueTexts;
    [SerializeField] TextMeshProUGUI[] coinsTexts;
    [SerializeField] TextMeshProUGUI[] diamondsTexts;
    [SerializeField] TextMeshProUGUI leagueProgressText;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] RawImage playerIcon;
    [SerializeField] FakeButtonUi openInvFb, openStoreFb, openFeaturedItemStoreFb, buyCoinsFb, buyDiamondsFb;

    [Title("Singleplayer - campaign")]
    [SerializeField] Image[] planetImages;
    [SerializeField] TextMeshProUGUI[] planetNameTexts, planetSubLevelProgressTexts;
    [SerializeField] DotTrackerUi[] planetSubLevelTrackers;
    [SerializeField] FakeButtonUi playIntroFb;
    [SerializeField] FakeButtonUi playOpenCampaignWindowFb;
    [SerializeField] FakeButtonUi playCloseCampaignWindowFb;
    [SerializeField] FakeButtonUi playCampFb;
    int _numOfLevels;
    [SerializeField] Transform parCampProgress;
    [SerializeField] ScrollRect campProgressScrollRect;
    const float CONST_CampProgressContentHeight = 908f;
    FakeButtonUi[] _cProgressSelectCurrentFb; 
    GameObject[] _cProgressCurrents, _cProgressTotals;
    Image[] _cProgressCurrentImages;
    TextMeshProUGUI[] _cProgressPlanetNames;

    [Title("Singleplayer - campaign selection")]
    [SerializeField] TextMeshProUGUI[] planetDescriptionTexts;
    [SerializeField] TextMeshProUGUI[] planetSubLevelNameTexts;
    [SerializeField] TextMeshProUGUI subLevelDescriptionText;
    public RectTransform[] focusImages, focusNames;
    [SerializeField] RawImage botImg;
    [SerializeField] TextMeshProUGUI botNameText, botDescriptionText;

    [Title("Multiplayer")]
    [SerializeField] GameObject[] panelsMp;
    [SerializeField] LeagueProgressionUi leagueProgressionUi;
    [SerializeField] FakeButtonUi playMpFb;
    [SerializeField] TextMeshProUGUI displayCountdownToStartHost;

    [Title("Singleplayer - practice")]
    [SerializeField] GameObject[] panelsPractice;
    [SerializeField] FakeButtonUi playPracticeFb;
    [SerializeField] Slider windSlider;

    void Awake()
    {
        Instance = this;
        _dm = Launch.Instance.persistentDataManager;
        _pData = _dm.playerData;
    }

    void Start()
    {
        playerNameText.text = $"Hello, {_pData.namePlayer}!";
        playerIcon.texture = Launch.Instance.myAuthManager.texturePlayer;

        _dm.gameData.gameType = MainGameType.MainMenu;

        Launch.Instance.myLobbyManager.Init(playMpFb, displayCountdownToStartHost);
        if (NetworkManager.Singleton.IsListening)
        {
            print(("network manager is listening, shutting down network"));
            NetworkManager.Singleton.Shutdown();
        }

        DisplayXpLeagueCurrencyData();
        CampDataDisplay();
        tabGroupUiNavigation.InitializeMe();
        PanVisible = PanelVisible.Play;
        Utils.Activation(noInternetWindow, GenActivation.Off);
        Utils.Activation(requiredLevelWindow, GenActivation.Off);

        switch (Application.isEditor)
        {
            case true:
              //  BtnMethodSinglePlay(MainGameType.Practice);
                break;
            case false:
                canvasTransform.position = new Vector3(0f, 1f, 1f);
                break;
        }
    }

    private void OnEnable()
    {
        openInvFb.InitializeMe(() => PanVisible = PanelVisible.Inventory_Store);
        openStoreFb.InitializeMe(() => PanVisible = PanelVisible.Inventory_Store);
        openFeaturedItemStoreFb.InitializeMe(() => PanVisible = PanelVisible.Inventory_Store);
        buyCoinsFb.InitializeMe(() => PanVisible = PanelVisible.Inventory_Store);
        buyDiamondsFb.InitializeMe(() => PanVisible = PanelVisible.Inventory_Store);
        
        playTutorialFb.InitializeMe(BtnMethodTutorial);

        playIntroFb.InitializeMe(() => BtnMethodSinglePlay(MainGameType.Intro));
        playPracticeFb.InitializeMe(() => BtnMethodSinglePlay(MainGameType.Practice));
        
        playCampFb.InitializeMe(() =>
        {
            BtnMethodSinglePlay(_pData.campProgressCurrent.ShowIntro() ? MainGameType.Intro : MainGameType.Campaign);
        });
        playOpenCampaignWindowFb.InitializeMe(() => PanVisible = PanelVisible.CampSelection);
        playCloseCampaignWindowFb.InitializeMe(() => PanVisible = PanelVisible.Play);

        windSlider.value = PlayerPrefs.GetFloat(_dm.gameData.windAmount_Fl);
        windSlider.onValueChanged.AddListener((float val) => { PlayerPrefs.SetFloat(_dm.gameData.windAmount_Fl, val); });
        
        NetworkManager.Singleton.OnConnectionEvent += CallEv_OnConnection;


    }
    private void OnDisable()
    {
        windSlider.onValueChanged.RemoveAllListeners();
        NetworkManager.Singleton.OnConnectionEvent -= CallEv_OnConnection;

    }

    void DisplayXpLeagueCurrencyData()
    {
        PlayerRewards.CalculateLevelFromXp(out int level, out _);
        League l = (League)_pData.league;

        for (int i = 0; i < levelIconImages.Length; i++)
        {
            levelIconImages[i].sprite = _dm.gameData.LevelIcon(level);
        }
        for (int i = 0; i < levelTexts.Length; i++)
        {
            levelTexts[i].text = (level + 1).ToString();
        }
        for (int i = 0; i < xpTexts.Length; i++)
        {
            xpTexts[i].text = Utils.ThousandsSeparator(_pData.xp) + " XP";
        }
        xpTitleText.text = _dm.gameData.XpTitle(level);
        for (int i = 0; i < leagueIconImages.Length; i++)
        {
            leagueIconImages[i].sprite = _dm.gameData.leagueSprites[_pData.league];
        }
        for (int i = 0; i < leagueTexts.Length; i++)
        {
            leagueTexts[i].text = _dm.gameData.LeagueName(_pData.league);
        }
        for (int i = 0; i < coinsTexts.Length; i++)
        {
            coinsTexts[i].text = Utils.ThousandsSeparator(_pData.gold);
        }
        for (int i = 0; i < diamondsTexts.Length; i++)
        {
            diamondsTexts[i].text = Utils.ThousandsSeparator(_pData.diamonds);
        }

        leagueProgressText.text = $"{leagueProgressionUi.Wins()}/{_dm.gameData.leagueProgressionTotalStayPromote[l].x} WON";
        LayoutRebuilder.ForceRebuildLayoutImmediate(leagueProgressText.GetComponent<RectTransform>());
        
        leagueProgressionUi.InitializeMe();
        
    }

    void CampDataDisplay()
    {
        if (_pData.campProgressCurrent.IsTutorial()) return;//tutorial
        
        RefreshCampDisplay(); 

        _numOfLevels = parCampProgress.childCount;
        _cProgressSelectCurrentFb = new FakeButtonUi[_numOfLevels];
        _cProgressCurrents = new GameObject[_numOfLevels];
        _cProgressCurrentImages = new Image[_numOfLevels];
        _cProgressPlanetNames = new TextMeshProUGUI[_numOfLevels];
        _cProgressTotals = new GameObject[_numOfLevels];
        for (int i = 0; i < _numOfLevels; i++)
        {
            _cProgressSelectCurrentFb[i] = parCampProgress.GetChild(i).GetChild(2).GetComponent<FakeButtonUi>();
            int index = i;
            _cProgressSelectCurrentFb[i].InitializeMe(() => SetCampProgress(index));
            
            _cProgressCurrents[i] = parCampProgress.GetChild(i).GetChild(0).gameObject;
            _cProgressCurrentImages[i] = _cProgressCurrents[i].transform.GetChild(0).GetChild(0).GetComponent<Image>();
            _cProgressPlanetNames[i] = _cProgressCurrents[i].transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            _cProgressTotals[i] = parCampProgress.GetChild(i).GetChild(3).gameObject;
        }
        int currentIndex = CampaignManager.IndexFromGetCampProgress(_pData.campProgressCurrent);
        SetCampProgress(currentIndex);
        if (currentIndex > 8)  campProgressScrollRect.content.anchoredPosition = new Vector2(0, -((float)currentIndex / (float)_numOfLevels) * CONST_CampProgressContentHeight);
    }

    void RefreshCampDisplay()
    {
        int planet = (int)_pData.campProgressCurrent.planet;
        int subLevel = _pData.campProgressCurrent.subLevel;
        SoPlanet currentPlanet = _dm.planets[planet];
        
        for (int i = 0; i < planetImages.Length; i++)
        {
            planetImages[i].sprite = currentPlanet.planetSprite;
        }
        for (int i = 0; i < planetNameTexts.Length; i++)
        {
            planetNameTexts[i].text = currentPlanet.PlanetName();
            LayoutRebuilder.ForceRebuildLayoutImmediate(planetNameTexts[i].rectTransform);
        }
        
        int currentIndex = CampaignManager.IndexFromGetCampProgress(new CampProgress(_pData.campProgressCurrent.planet, 0));
        int totalIndex = CampaignManager.IndexFromGetCampProgress(_pData.campProgressTotal);
        for (int i = 0; i < planetSubLevelProgressTexts.Length; i++)
        {
            planetSubLevelProgressTexts[i].text = $"{Mathf.Max(subLevel, Mathf.Min(currentPlanet.subLevels.Length, totalIndex - currentIndex + 1))}/{currentPlanet.subLevels.Length} WON";
            LayoutRebuilder.ForceRebuildLayoutImmediate(planetSubLevelProgressTexts[i].rectTransform);
        }
        for (int i = 0; i < planetSubLevelTrackers.Length; i++)
        {
            planetSubLevelTrackers[i].SetImages(currentPlanet.subLevels.Length, totalIndex - currentIndex + 1, subLevel);
        }
        
        for (int i = 0; i < planetSubLevelNameTexts.Length; i++)
        {
            planetSubLevelNameTexts[i].text = currentPlanet.subLevels[subLevel].environment.Name();
            LayoutRebuilder.ForceRebuildLayoutImmediate(planetSubLevelNameTexts[i].rectTransform);
        }
        for (int i = 0; i < planetDescriptionTexts.Length; i++)
        {
            planetDescriptionTexts[i].text = currentPlanet.descriptions[i];
            LayoutRebuilder.ForceRebuildLayoutImmediate(planetDescriptionTexts[i].rectTransform);
        }
        
        if (_pData.campProgressCurrent.IsTutorial()) return;
        subLevelDescriptionText.text = currentPlanet.subLevels[subLevel].environment.description;
        botImg.texture = currentPlanet.subLevels[subLevel].environment.bot.botTexture;
        botNameText.text = currentPlanet.subLevels[subLevel].environment.bot.botName;
        botDescriptionText.text = currentPlanet.subLevels[subLevel].environment.bot.description;
    }

    void SetCampProgress(int chosenCurrent)
    {
        int total = CampaignManager.IndexFromGetCampProgress(_pData.campProgressTotal);

        if (chosenCurrent - 1 <= total)
        {
            _pData.campProgressCurrent = CampaignManager.CampProgressFromIndex(chosenCurrent);
            
            Utils.ActivateOneArrayElement(_cProgressCurrents, chosenCurrent);
            _cProgressCurrentImages[chosenCurrent].sprite = _dm.planets[(int)_pData.campProgressCurrent.planet].planetSprite;
            _cProgressPlanetNames[chosenCurrent].text = _dm.planets[(int)_pData.campProgressCurrent.planet].PlanetName();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_cProgressPlanetNames[chosenCurrent].rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_cProgressCurrents[chosenCurrent].GetComponent<RectTransform>());

            for (int i = 0; i < _numOfLevels; i++)
            {
                Utils.Activation(_cProgressTotals[i], total < i ? GenActivation.Off : GenActivation.On);
            }
            
            RefreshCampDisplay();
        }
        else
        {
            print("cant skip levels");
        }
    }



    /// <summary>
    /// host waits for client to join. when he does, this triggers game scene loading
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    void CallEv_OnConnection(NetworkManager arg1, ConnectionEventData arg2)
    {
       // print($"OnConnection, network manager is listening: {arg1.IsListening}");
        if (_dm.gameData.gameType == MainGameType.MainMenu &&
            arg2.ClientId != NetworkManager.Singleton.LocalClient.ClientId &&
            arg1.IsListening)
        {
            StartCoroutine(_dm.NewSceneAfterFadeIn("CallEv_OnConnection", MainGameType.Multi));
            Launch.Instance.myLobbyManager.beginCountdownForStartHost = false;
        }
    }

    #region BUTTONS

    void BtnMethodSinglePlay(MainGameType gameType)
    {
        if (_oneHitExitScene) return;
        _oneHitExitScene = true;

        _dm.gameData.gameType = gameType;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(Unity.Networking.Transport.NetworkEndPoint.AnyIpv4);
        NetworkManager.Singleton.StartHost();
        StartCoroutine(_dm.NewSceneAfterFadeIn("BtnMethodSinglePlay", _dm.gameData.gameType));
    }

    void BtnMethodTutorial()
    {
        if (_oneHitExitScene) return;
        _oneHitExitScene = true;

        _dm.gameData.gameType = MainGameType.Tutorial;
        StartCoroutine(_dm.NewSceneAfterFadeIn("BtnMethodTutorial", _dm.gameData.gameType));
    }
    #endregion

    #region DEBUGS
    [ContextMenu("Print all playerprefs")]
    void Metoda3()
    {
        Utils.DisplayAllPlayerPrefs();
    }
    #endregion

}
