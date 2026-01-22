using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class MyLobbyManager : MonoBehaviour
{
    PersistentDataManager _db;
    RelayManager _relayManager;
    Lobby _currentLobby;
    League _myLeague;

    float _timerHeartbeat, _timerUpdateLobbyData, _timerStartHost;
    const string CONST_LobbyName = "Lobby";
    const int CONST_MaxPlayers = 2;
    const string CONST_KeyJoinCode = "RelayJoinCode";
    const string CONST_RankingOperation = "RankingOperation";

    bool _activeLobbyMaintenance = true;
    bool _oneHitBtnReady;
    public bool beginCountdownForStartHost;
    TextMeshProUGUI _countdownText;

    #region INITIALIZATION
    void Awake()
    {
        _db = Launch.Instance.persistentDataManager;
        _relayManager = new RelayManager(CONST_MaxPlayers);
    }

    void Start()
    {
        DontDestroyOnLoad(this);

    }


    public void Init(FakeButtonUi readyFb, TextMeshProUGUI displayCountdown)
    {
        readyFb.InitializeMe(Btn_Ready);
        _countdownText = displayCountdown;
        _oneHitBtnReady = beginCountdownForStartHost = _activeLobbyMaintenance = false;

      //  Btn_Ready();
    }
    #endregion


    async void Btn_Ready()
    {
        if (_oneHitBtnReady) return;
        _oneHitBtnReady = true;

        await QuickJoinLobby();
        if (_currentLobby == null)
        {
            await CreateLobby();
        }
    }

    #region HEARTBEAT AND UPDATES
    private void Update()
    {
        if (beginCountdownForStartHost)
        {
            _timerStartHost += Time.deltaTime;
            int countdown = _db.gameData.waitTimeStartGame - (int)_timerStartHost;
            if ((int)_timerStartHost > 0) _countdownText.text = $"Starting host in {countdown} seconds";
            if (_timerStartHost >= _db.gameData.waitTimeStartGame)
            {
                beginCountdownForStartHost = false;
                StartCoroutine(_db.NewSceneAfterFadeIn("Update",MainGameType.Multi));
                _oneHitBtnReady = false;
            }
        }
        else
        {
            _timerStartHost = 0f;
        }
        
        if (!_activeLobbyMaintenance || _currentLobby == null) return;
        HandleHeartbeat();
        HandleLobbyUpdates();
    }


    async void HandleHeartbeat()
    {
        _timerHeartbeat += Time.deltaTime;
        if (_timerHeartbeat > 15)
        {
            _timerHeartbeat = 0f;
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
        }
    }

    async void HandleLobbyUpdates() //changes in lobby don't sync automatically, this method updates all changes
    {
        _timerUpdateLobbyData += Time.deltaTime;
        if (_timerUpdateLobbyData > 1.1f)
        {
            _timerUpdateLobbyData = 0f;
            _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
        }
    }
    #endregion

    #region LOBBY CONTROLS
    async Task CreateLobby()
    {
        try
        {
            _countdownText.text = $"Creating lobby, please wait...";
            Allocation allocation = await _relayManager.AllocateRelay();
            string relayCode = await _relayManager.GetRelayJoinCode(allocation);
            
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(CONST_LobbyName, CONST_MaxPlayers, options);
            _myLeague = (League)_db.playerData.league;
            await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { CONST_KeyJoinCode, new DataObject(DataObject.VisibilityOptions.Public, relayCode) },
                    { CONST_RankingOperation, new DataObject(DataObject.VisibilityOptions.Public, ((int)_myLeague).ToString()) },
                }
            });
            
            RelayHostData hostData = new RelayHostData
            {
                JoinCode = relayCode,
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                IPv4Address = allocation.RelayServer.IpV4,
                ConnectionData = allocation.ConnectionData,
            };
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(hostData.IPv4Address, hostData.Port, hostData.AllocationIDBytes, hostData.Key, hostData.ConnectionData);
            NetworkManager.Singleton.StartHost();
            _activeLobbyMaintenance = true;
            beginCountdownForStartHost = true;
            _countdownText.text = $"Created lobby: {_currentLobby.Name}  with  relay code: {relayCode}";
          //  print($"Created lobby: {_currentLobby.Name}  with  relay code: {relayCode}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }

    async Task QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions()
            {
                Player = GetPlayer(),
            };
            Lobby bufferLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

            bool CanJoinThisLobby()
            {
                // int myLeague = _db.GetValAndCastTo<byte>(CloudData.League);
                // int hostLeague = int.Parse(bufferLobby.Data[CONST_RankingOperation].Value);
                //
                // if (hostLeague == System.Enum.GetNames(typeof(League)).Length - 1 && hostLeague != myLeague) return false;
                // if (Mathf.Abs(hostLeague - myLeague) > 2)
                // {
                //     if ((League)hostLeague == League.Silver1 && (League)myLeague == League.Bronze1) return true;
                //     if ((League)myLeague == League.Silver1 && (League)hostLeague == League.Bronze1) return true;
                //     
                //     if ((League)hostLeague == League.Platinum3 && (League)myLeague == League.Diamond3) return true;
                //     if ((League)myLeague == League.Platinum3 && (League)hostLeague == League.Diamond3) return true;
                //     
                //     return false;
                // }

                return true;
            }

            if (CanJoinThisLobby())
            {
                _currentLobby = bufferLobby;
                string relayCode = _currentLobby.Data[CONST_KeyJoinCode].Value;
                JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(relayCode);
                RelayJoinData joinData = new RelayJoinData
                {
                    Key = allocation.Key,
                    Port = (ushort)allocation.RelayServer.Port,
                    AllocationID = allocation.AllocationId,
                    AllocationIDBytes = allocation.AllocationIdBytes,
                    IPv4Address = allocation.RelayServer.IpV4,
                    ConnectionData = allocation.ConnectionData,
                    HostConnectionData = allocation.HostConnectionData,
                    JoinCode = relayCode
                };
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(joinData.IPv4Address, joinData.Port, joinData.AllocationIDBytes, joinData.Key, joinData.ConnectionData, joinData.HostConnectionData);
                NetworkManager.Singleton.StartClient();
                Launch.Instance.persistentDataManager.gameData.gameType = MainGameType.Multi;
              //  print($"starting client with {relayCode}");
            }
            
            _activeLobbyMaintenance = false;
        }
        catch (LobbyServiceException e)
        {
          //  Debug.LogError("Failed to quick join lobby: " + e.Message);
        }
    }
    
    public async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(_currentLobby.Id);
            _currentLobby = null;
            _activeLobbyMaintenance = false;
            print("Lobby deleted");
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
        }
    }
    #endregion

    #region MISC
    [Rpc(SendTo.Server)]
    public async void ClientDisconnectingRpc(string clientID)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, clientID);
            _currentLobby = null;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    Player GetPlayer()
    {
        return new Player()
        {
        };
    }
    #endregion

    #region DEBUGS
    [ContextMenu("PrintPlayers")]
    void PrintPlayers() => PrintPlayers(_currentLobby);

    void PrintPlayers(Lobby lobby)
    {
        string numPlayers = lobby.Players.Count == 1 ? "" : "s";
        string textToDisplay = $"{lobby.Players.Count} player{numPlayers} in lobby {lobby.Name}" + "\n";
        foreach (Player item in lobby.Players)
        {
           // print($"{item.Data["PlayerName"].Value} with id {item.Id}");
          //  textToDisplay += $"{item.Data["PlayerName"].Value} with id {item.Id}" + "\n";
            textToDisplay += $"with id {item.Id}" + "\n";
        }
        print(textToDisplay);
    }

    [ContextMenu("Lobby data")]
    async void M1()
    {
        try
        {
            // await Launch.Instance.myAuthManager.Authenticate();
            QueryLobbiesOptions options = new QueryLobbiesOptions()
            {
                Count = 5,
            };
            QueryResponse qr = await LobbyService.Instance.QueryLobbiesAsync();
            string st = $"Num of lobbies: {qr.Results.Count}\n";
            foreach (Lobby item in qr.Results)
            {
                st += $"id: {item.Id}\n";
                st += $"code: {item.LobbyCode}\n";
                st += $"num of players: {item.Players.Count}\n";
                st += $"lobby name: {item.Name}\n";
                st += $"created: {item.Created}";
            }
            print(st);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }
    #endregion

}

// async void Btn_Create()
// {
//     if (_oneHitBtnCreate) return;
//     _oneHitBtnCreate = true;
//
//     // await Launch.Instance.myAuthManager.Authenticate();
//     await CreateLobby();
//     StartCoroutine(Launch.Instance.mySceneManager.NewSceneAfterFadeIn(MainGameType.Multi));
// }
//
// async void Btn_QuickJoin()
// {
//     if (_oneHitBtnJoin) return;
//     _oneHitBtnJoin = true;
//
//     Launch.Instance.myMyDatabaseManager.gameData.gameType = MainGameType.Multi;
//     //  await Launch.Instance.myAuthManager.Authenticate();
//     await QuickJoinLobby();
// }

// async void Btn_Join()
// {
//     if (_oneHitBtnJoin) return;
//     _oneHitBtnJoin = true;
//
//     Utils.FadeOut?.Invoke(false);
//     Utils.GameType = MainGameType.Multiplayer;
//   //  await Launch.Instance.myAuthManager.Authenticate();
//     await JoinWithRelayCode(inputJoinCode.text);
// }


// async Task JoinWithRelayCode(string codeFromRelay)
// {
//     try
//     {
//         relayCode = codeFromRelay;
//         JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
//         {
//             Player = GetPlayer()
//         };
//         QueryResponse qr = await LobbyService.Instance.QueryLobbiesAsync();
//         foreach (Lobby item in qr.Results)
//         {
//             if (item.Data[CONST_KeyJoinCode].Value == relayCode)
//             {
//                 _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(item.Id, options);
//                 break;
//             }
//         }
//
//         JoinAllocation joinAllocation = await _relayManager.JoinRelay(this.relayCode);
//         NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, ConnectionType()));
//         NetworkManager.Singleton.StartClient();
//         Launch.Instance.mySceneManager.SubscribeAll();
//         _activeUpdates = false;
//         print($"starting client with relay code {relayCode}");
//     }
//     catch (LobbyServiceException e)
//     {
//         Debug.Log(e);
//         MainMenuManager.Instance.joinCodeInfo.text = "No lobby with that code found...";
//         _oneHitBtnJoin = false;
//     }
// }


// public async void LeaveLobby()
// {
//     try
//     {
//         await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, AuthenticationService.Instance.PlayerId);
//         _currentLobby = null;
//         _activeUpdates = false;
//     }
//     catch (LobbyServiceException ex)
//     {
//         Debug.LogError(ex.Message);
//     }
// }


