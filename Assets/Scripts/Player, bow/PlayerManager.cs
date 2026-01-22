using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;


public class PlayerManager : NetworkBehaviour
{
    PersistentDataManager _dm;

    public static PlayerManager Instance;
    [SerializeField] GameObject prefabPlayer;
    public PlayerRigRef playerRigRef;
    public Animator[] standAnimators;
    int _rise = Animator.StringToHash("rise");
    public Transform[] bowSpawnPoints;
    [SerializeField] MeshRenderer[] playerMeshMarker;
    [SerializeField] ParticleSystem[] psSpawns;
    [SerializeField] ParticleSystem psWaitForPlayer;
    [SerializeField] ParticleSystem psDisconnect;

    /// <summary>
    /// Can start playing (pick up bow).
    /// True - practice starts, campaign intro finishes or mp 2nd player joins.
    /// False - end any match, campaign outro or mp 2nd player leaves.
    /// </summary>
    public NetworkVariable<bool> canPlay = new NetworkVariable<bool>(false);
    public NetworkList<NetPlayerEquipment> equipmentNet;
    public NetworkList<NetPlayerDisplay> playerDisplayNet;


    void Awake()
    {
        Instance = this;
        _dm = Launch.Instance.persistentDataManager;
        equipmentNet = new NetworkList<NetPlayerEquipment>(new List<NetPlayerEquipment>() { new NetPlayerEquipment(), new NetPlayerEquipment() });
        playerDisplayNet = new NetworkList<NetPlayerDisplay>(new List<NetPlayerDisplay>() { new NetPlayerDisplay(), new NetPlayerDisplay() });
        NetworkManager.Singleton.SceneManager.OnSceneEvent += CallEv_SceneEvent;
    }



    public void InitializeMe()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += CallEv_ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += CallEv_ClientDisconnected;
            GameManager.Instance.playerTurnNet.OnValueChanged += NetVarEv_PlayerTurnChange;
            ChangeVisualMarkers_EveryoneRpc(PlayerFaction.First_left);

            if (_dm.gameData.gameType == MainGameType.Multi && NetworkManager.Singleton.ConnectedClientsList.Count <= 1) psWaitForPlayer.Play();
        }
        else
        {
            ChangeVisualMarkers_EveryoneRpc(GameManager.Instance.playerTurnNet.Value);
        }
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            if (!GameManager.Instance.isRestarting.Value) Launch.Instance.myLobbyManager.DeleteLobby();
            NetworkManager.Singleton.OnClientConnectedCallback -= CallEv_ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= CallEv_ClientDisconnected;
            GameManager.Instance.playerTurnNet.OnValueChanged -= NetVarEv_PlayerTurnChange;
        }
        else
        {
            if (!GameManager.Instance.isRestarting.Value) Launch.Instance.myLobbyManager.ClientDisconnectingRpc(AuthenticationService.Instance.PlayerId);
        }

        if (!GameManager.Instance.isRestarting.Value) StartCoroutine(_dm.NewSceneAfterFadeIn("OnNetworkDespawn from playerManager", MainGameType.MainMenu));
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= CallEv_SceneEvent;
    }



    #region CALL EVENTS
    void CallEv_SceneEvent(SceneEvent sceneEvent)
    {
        bool showDebug = false;
        SceneEventType tip = sceneEvent.SceneEventType;
        switch (tip)
        {
            case SceneEventType.LoadEventCompleted:
                if (showDebug) print($"LoadEventCompleted: {sceneEvent.SceneName}");
                if (sceneEvent.SceneName == _dm.gameData.scene_CompleteGame)
                {
                    AudioManager.Instance.GameSceneStarted();
                    SpawnPlayers_ServerRpc(NetworkManager.Singleton.LocalClientId);
                }
                break;

            case SceneEventType.SynchronizeComplete:
                if (showDebug) print($"SynchronizeComplete: {sceneEvent.SceneName}");
                if (!IsServer)
                {
                    AudioManager.Instance.GameSceneStarted();
                    SpawnPlayers_ServerRpc(NetworkManager.Singleton.LocalClientId);
                }
                break;

            #region LESS IMPORTANT
            case SceneEventType.LoadComplete:
                if (showDebug) print($"LoadComplete: {sceneEvent.SceneName}");
                break;

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
            case SceneEventType.UnloadComplete:
                if (showDebug) print($"UnloadComplete: {sceneEvent.SceneName}");
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

    void NetVarEv_PlayerTurnChange(PlayerFaction previousvalue, PlayerFaction newvalue)
    {
        ChangeVisualMarkers_EveryoneRpc(newvalue);
    }

    void CallEv_ClientConnected(ulong obj)
    {
        print($"client {obj} connected, num of clients is {NetworkManager.Singleton.ConnectedClients.Count}");
        psWaitForPlayer.Stop();
        canPlay.Value = true;
    }

    void CallEv_ClientDisconnected(ulong obj)
    {
        print($"client {obj} disconnected, num of clients is {NetworkManager.Singleton.ConnectedClients.Count}");
        canPlay.Value = false;
        standAnimators[1].SetBool("rise", false);
        // if (equipmentNet.Count == 2) equipmentNet.RemoveAt(1);
        // if (playerDisplayNet.Count == 2) playerDisplayNet.RemoveAt(1);
        psDisconnect.Play();
        psWaitForPlayer.Play();
    }

    #endregion

    

    #region REGISTRATIONS
    [Rpc(SendTo.Server)]
    public void RegisterPlayerDisplay_ServerRpc(int index, FixedString128Bytes playerName, FixedString512Bytes imgUrl, uint level, byte league)
    {
        NetPlayerDisplay netPlayerDisplay = new NetPlayerDisplay()
        {
            name = playerName,
            imageUrl = imgUrl,
            level = level,
            league = league,
        };
        playerDisplayNet[index] = netPlayerDisplay; 
        UpdatePlayerImages_EveryoneRpc();
    }

    [Rpc(SendTo.Everyone)]
    void UpdatePlayerImages_EveryoneRpc()
    {
        for (int i = 0; i < playerDisplayNet.Count; i++)
        {
            UpdatePlayerImages(i);
        }

        async void UpdatePlayerImages(int index)
        {
            if (GameManager.Instance.uiManager.playerTextures[index] != null)
            {
                Texture2D texture2D = await Utils.GetRemoteTexture(playerDisplayNet[index].imageUrl.ToString());
                GameManager.Instance.uiManager.playerTextures[index] = texture2D;
                GameManager.Instance.uiManager.NetVar_NextTurn(PlayerFaction.Undefined, GameManager.Instance.playerTurnNet.Value);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void RegisterPlayerEquipment_ServerRpc(int index, byte[] equipmentIndices)
    {
        NetPlayerEquipment netPlayerEquipment = new NetPlayerEquipment()
        {
            bowIndex = equipmentIndices[0],
            headIndex = equipmentIndices[1],
            glovesIndex = equipmentIndices[2],
            arrowIndex = equipmentIndices[3],
        };
        equipmentNet[index] = netPlayerEquipment;
    }
    #endregion


    [Rpc(SendTo.Server)]
    void SpawnPlayers_ServerRpc(ulong obj)
    {
        bool hostIsSpawned = NetworkManager.Singleton.LocalClient.ClientId == obj;
        psSpawns[hostIsSpawned ? 0 : 1].Play();
        if (!hostIsSpawned) psWaitForPlayer.Stop();

        GameObject go = Instantiate(prefabPlayer);
        go.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj, true);
    }

    [Rpc(SendTo.Everyone)]
    void ChangeVisualMarkers_EveryoneRpc(PlayerFaction newValue)
    {
        for (int i = 0; i < playerMeshMarker.Length; i++)
        {
            playerMeshMarker[i].material = GameManager.Instance.factionData[(int)newValue].matMain;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void StandAnimators_EveryoneRpc(byte index, bool rise) => standAnimators[index].SetBool(_rise, rise);



    #region DEBUG
    [ContextMenu("SpawnPlayerAndRegisterRpc")]
    void Metoda6() => SpawnPlayers_ServerRpc(NetworkManager.Singleton.LocalClientId);

    [ContextMenu("Player equipment information")]
    void Metoda7()
    {
        string st = "";
        st += $"count is {equipmentNet.Count}\n";
        foreach (NetPlayerEquipment item in equipmentNet)
        {
            st += $"{item.bowIndex}: {item.headIndex}: {item.glovesIndex}: {item.arrowIndex}\n";
        }
        print(st);
    }

    [ContextMenu("Despawn players")]
    void Metoda8()
    {
        foreach (NetworkClient item in NetworkManager.Singleton.ConnectedClientsList)
        {
           if (item.PlayerObject != null) item.PlayerObject.Despawn();
        }
        foreach (NetworkClient item in NetworkManager.Singleton.ConnectedClientsList)
        {
           print(item.PlayerObject == null);
        }
    }
    #endregion


}
