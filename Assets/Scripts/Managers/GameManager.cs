using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine.Serialization;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public bool MyTurn() => playerTurnNet.Value == factionData[NetworkManager.Singleton.IsHost ? 0 : 1].playerFaction;
    int CounterMisses
    {
        get => _counterMisses;
        set
        {
            _counterMisses = value;
            if (value != 0 ) uiManager.SetMissCounter_EveryoneRpc(value);
        }
    }
    int _counterMisses;
    PersistentDataManager _dm;
    PlayerRewards _playerRewards;

    [Title("References", TitleAlignment = TitleAlignments.Centered)]
    public SoFactionData[] factionData;
    public UiManager uiManager;
    public LeagueProgressionUi leagueProgressionUi;
    public ArrowManager arrowManager;
    public CampaignManager campaignManager;
    public BotManager botManager;
    public ParticleSystem[] psBowsReadyToPickup;
    
    [Title("Instances", TitleAlignment = TitleAlignments.Centered)]
    public GridManager gridManager;
    public PoolManager poolManager;
    public WindManager windManager;
    public DrawTrajectory drawTrajectory;

    NetworkVariable<float> _windAmountNet = new NetworkVariable<float>();
    NetworkVariable<bool> _trajectoryVisibleNet = new NetworkVariable<bool>();
    NetworkVariable<byte> _skyboxIndexNet = new NetworkVariable<byte>();

    [Title("Public network variables", TitleAlignment = TitleAlignments.Centered)]
    public NetworkVariable<bool> isRestarting = new NetworkVariable<bool>();
    public NetworkVariable<PlayerFaction> playerTurnNet = new NetworkVariable<PlayerFaction>();
    public NetworkVariable<PlayerFaction> playerVictoriousNet = new NetworkVariable<PlayerFaction>();
    public NetworkList<byte> gridTileStatesNet;
    public NetworkList<sbyte> gridValuesNet;
    public NetworkList<uint> scoresNet;

    
    private void Awake()
    {
        Instance = this;
        _dm = Launch.Instance.persistentDataManager;
        poolManager.Init();
        Physics.gravity = windManager.gravityVector;

        gridTileStatesNet = new NetworkList<byte>(new List<byte>());
        gridValuesNet = new NetworkList<sbyte>(new List<sbyte>());

        scoresNet = new NetworkList<uint>(new List<uint>() { 0, 0 });
        _playerRewards = new PlayerRewards();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayerManager.Instance.InitializeMe();
        uiManager.InitializeMe();
        gridManager.Init();
        uiManager.playerTextures[NetworkManager.Singleton.IsHost ? 0 : 1] = Launch.Instance.myAuthManager.texturePlayer;
        
        if (IsServer)
        {
            _trajectoryVisibleNet.Value = false;
            _windAmountNet.Value = 0f;
            GameObject level = null;
            switch (_dm.gameData.gameType)
            {
                case MainGameType.Campaign:
                    campaignManager.Init();
                    level = campaignManager.NextLevel();
                    _trajectoryVisibleNet.Value = _dm.playerData.campProgressTotal.IsTutorial();
                    SoEnvironment env = _dm.planets[(int)_dm.playerData.campProgressTotal.planet].subLevels[_dm.playerData.campProgressTotal.subLevel].environment;
                    _skyboxIndexNet.Value = (byte)(env.id - _dm.gameData.IndicesForScriptables[CloudData.Environment]);
                    break;
                case MainGameType.Practice:
                    if (PlayerPrefs.GetInt(_dm.gameData.trajectoryVisible_Int) == 1) _trajectoryVisibleNet.Value = true;
                    _windAmountNet.Value = PlayerPrefs.GetFloat(_dm.gameData.windAmount_Fl);
                    List<SoBot> bots = _dm.bots.Where(n => n.difficulty == (BotStrength)PlayerPrefs.GetInt(_dm.gameData.difficulty_Int)).ToList();
                    _dm.gameData.botSp = bots[Random.Range(0, bots.Count)];
                    _skyboxIndexNet.Value = (byte)Random.Range(0, _dm.environments.Length);
                    uiManager.BtnMethodSpCanStart();
                    break;
                case MainGameType.Multi:
                    int windDirection = Random.value < 0.5f ? -1 : 1;
                    _windAmountNet.Value = windDirection * _dm.gameData.windStrengthByLeague[(League)_dm.playerData.league];
                    _skyboxIndexNet.Value = (byte)Random.Range(0, _dm.environments.Length);
                    break;
            }
            gridManager.ChooseGrid(level);

            playerVictoriousNet.Value = PlayerFaction.Undefined;
            playerVictoriousNet.OnValueChanged += NetVarEv_PlayerVictorious;
            _trajectoryVisibleNet.Value = _dm.gameData.gameType == MainGameType.Practice && PlayerPrefs.GetInt(_dm.gameData.trajectoryVisible_Int) == 1;

        }
        else
        {
            gridManager.GridUseNetworkVariables();
        }

        AudioManager.Instance.PlaySfx(AudioManager.Instance.gameStarted);
        windManager.WindChange(float.MinValue, _windAmountNet.Value);
        _windAmountNet.OnValueChanged += windManager.WindChange;
        drawTrajectory.showTrajectory = _trajectoryVisibleNet.Value;
        
        Material chosenSkybox = _dm.environments[_skyboxIndexNet.Value].skyboxMaterial;
        RenderSettings.skybox = chosenSkybox;
        RenderSettings.customReflectionTexture = chosenSkybox.GetTexture("_Tex");

    }


    void Update()
    {
        if (_dm.gameData.gameType == MainGameType.Practice) return;
        
        _playerRewards.UpdateLoop();
    }


    #region CALL EVENTS

    private void NetVarEv_PlayerVictorious(PlayerFaction previousValue, PlayerFaction newValue)
    {
        PlayerVictorious_EveryoneRpc(newValue);
    }

    [Rpc(SendTo.Everyone)]
    void PlayerVictorious_EveryoneRpc(PlayerFaction victor)
    {
        GenResult result = GenResult.Lose;
        CampaignFinishState campaignFinishState = CampaignFinishState.None;
        bool isRepeatingCampaign = false;
        switch (victor)
        {
            case PlayerFaction.First_left:
                if (IsServer) Win();
                else AudioManager.Instance.PlaySfx(AudioManager.Instance.lose);
                break;
            case PlayerFaction.Second_right:
                if (!IsServer) Win();
                else AudioManager.Instance.PlaySfx(AudioManager.Instance.lose);
                break;
            case PlayerFaction.None:
                result = GenResult.Draw;
                AudioManager.Instance.PlaySfx(AudioManager.Instance.draw);
                break;

                void Win()
                {
                    result = GenResult.Win;
                    AudioManager.Instance.PlaySfx(AudioManager.Instance.win);
                    if (_dm.gameData.gameType == MainGameType.Campaign) campaignFinishState = campaignManager.PlayerVictorious(out isRepeatingCampaign);
                }
        }

        _playerRewards.RewardsCalculation(result, campaignFinishState, isRepeatingCampaign);
    }

    #endregion



    #region GAME FLOW
    [Rpc(SendTo.Server)]
    public void SetGridTileStateNet_ServerRpc(byte ord, byte value) => gridTileStatesNet[ord] = value;

    [Rpc(SendTo.Server)]
    public void SetGridValuesNet_ServerRpc(byte ord, sbyte value) => gridValuesNet[ord] = value;


    PlayerFaction _pc = PlayerFaction.Undefined;
    [Rpc(SendTo.Server)]
    public void NextPlayer_ServerRpc(PlayerFaction arrowOwner, bool countGridMisses = true, string caller = "")
    {
        if (playerVictoriousNet.Value != PlayerFaction.Undefined || _pc == arrowOwner) return;
        _pc = arrowOwner;
        
        if (countGridMisses)
        {
            CounterMisses++;
            if (CounterMisses >= 4)
            {
              //  print("4 misses in a row, match is over");
                DecideVictor();
                return;
            }
        }
        else CounterMisses = 0;
        
        int val = (int)arrowOwner;
        val = (1 + val) % 2;
        playerTurnNet.Value = (PlayerFaction)val; 
       // print($"next player is { playerTurnNet.Value }, called by {caller}");
    }

    [Rpc(SendTo.Server)]
    public void Scoring_ServerRpc()
    {
        if ((int)(playerVictoriousNet.Value) < 2) return; //is gameover
        bool useConsole = false;
        int[] tempScores = new int[2];
        List<ParentHex> takenHex = gridManager.AllTilesByType(TileState.Taken);
        if (useConsole) print($"taken hex {takenHex.Count}");
        foreach (ParentHex item in takenHex)
        {
            switch (item.CurrentValue)
            {
                case > 0:
                    tempScores[0] += item.CurrentValue;
                    if (useConsole) print("plus");
                    break;
                case < 0:
                    tempScores[1] -= item.CurrentValue;
                    if (useConsole) print("minus");
                    break;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            scoresNet[i] = (uint)Mathf.Abs(tempScores[i]);
        }

        //check game over
        int freePoints = gridManager.NumOfTilesByType(TileState.Free) * 10;
        if (freePoints == 0)
        {
            print("all tiles are taken");
            DecideVictor();
        }
        else if (scoresNet[0] > scoresNet[1] + freePoints || scoresNet[1] > scoresNet[0] + freePoints)
        {
            print("impossible to win");
            DecideVictor();
        }
    }

    void DecideVictor()
    {
        if (scoresNet[0] > scoresNet[1])
        {
            playerVictoriousNet.Value = PlayerFaction.First_left;
        }
        else if (scoresNet[0] < scoresNet[1])
        {
            playerVictoriousNet.Value = PlayerFaction.Second_right;
        }
        else playerVictoriousNet.Value = PlayerFaction.None;
    }

    #endregion

    #region DEBUGS

    [ContextMenu("ConnectedClients.Count")]
    void Metoda2() => print(NetworkManager.Singleton.ConnectedClients.Count);

    [ContextMenu("Print all playerprefs")]
    void Metoda3() => Utils.DisplayAllPlayerPrefs();

    [ContextMenu("Change wind")]
    void Metoda4()
    {
        _windAmountNet.Value = Random.Range(-0.5f, 0.5f);
        // print($"wind is {windAmountNet.Value * CONST_WINDSCALE}");
        print($"wind is {_windAmountNet.Value * 20}");
    }

    [ContextMenu("Next player")]
    public void Metoda5NextPlayer()
    {
        NextPlayer_ServerRpc(playerTurnNet.Value,false, "context menu");
    }


    [ContextMenu("First player wins")]
    void Metoda7() => playerVictoriousNet.Value = PlayerFaction.First_left;

    [ContextMenu("Second player wins")]
    void Metoda8() => playerVictoriousNet.Value = PlayerFaction.Second_right;

    [ContextMenu("hex data")]
    void Metoda11()
    {
        string st = "all states and values:\n";
        for (int i = 0; i < 100; i++)
        {
            if (gridTileStatesNet[i] == 0 && gridValuesNet[i] == 0) continue;
            st += $"state is {(TileState)gridTileStatesNet[i]}, value is {gridValuesNet[i]}, ordinal is {i}\n";
        }
        print(st);
    }

    [ContextMenu("Scores")]
    void Metoda12()
    {
        for (int i = 0; i < 2; i++)
        {
            print(scoresNet[i]);
        }
    }


    #endregion
}


