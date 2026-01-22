using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.Netcode;
using DG.Tweening;

public class UiManager : NetworkBehaviour
{
    GameManager gm;
    PersistentDataManager _dm;

    [Title("GENERAL")]
    [SerializeField] Transform missTr;
    TextMeshPro _missText;
    const int CONST_StartPos = -3;
    const int CONST_EndPos = 20;
    const float CONST_MoveTime = 5f;
    Tween _moveTween;
    [SerializeField] GameObject[] canvasElements;
    [ReadOnly] public Texture2D[] playerTextures = new Texture2D[2];
    
    [Title("CENTER MENU")]
    [SerializeField] Image circleImage;
    [SerializeField] Image[] outlinesMainImages, outlinesCircleImages;
    [SerializeField] TextMeshProUGUI circleName;
    [SerializeField] Transform[] scoresParTransform;
    [SerializeField] TextMeshProUGUI[] scoresText;
    [SerializeField] Image[] scoreLeftImages, scoreRightImages;
    [SerializeField] FakeButtonUi backToMainMenuFb, retryFb;
    
    [Title("ENDGAME")]
    [SerializeField] GameObject endUiPrefab;
    [SerializeField] Transform parEnd;
    [SerializeField] Transform[] endSpawnPositions;
    [SerializeField] Transform[] rewardTransforms;
    [SerializeField] TextMeshProUGUI coinsText, xpText;
    [SerializeField] TextMeshProUGUI endInfo;
    [SerializeField] FakeButtonUi playNextFb;
    
    [Title("OLD - Tutorial")]
    [SerializeField] Button[] buttonsTutorialDone;
    
    private void Awake()
    {
        gm = GameManager.Instance;
        _dm = Launch.Instance.persistentDataManager;
        _missText = missTr.GetComponent<TextMeshPro>();
    }


    public void InitializeMe()
    {
        _missText.text = "";
        
        backToMainMenuFb.InitializeMe(BtnMethodExit);
        retryFb.InitializeMe(BtnMethodRestart);
        playNextFb.InitializeMe(BtnMethodRestart);
        Utils.Activation(playNextFb.gameObject, GenActivation.Off);
        for (int i = 0; i < buttonsTutorialDone.Length; i++)
        {
            buttonsTutorialDone[i].onClick.AddListener(BtnMethodSpCanStart);
        }
        
        int canvasIndex = int.MaxValue;
        if (_dm.gameData.gameType == MainGameType.Practice || _dm.gameData.gameType == MainGameType.Multi) canvasIndex = 0;
        Utils.ActivateOneArrayElement(canvasElements, canvasIndex);
        
        PlayerManager.Instance.canPlay.OnValueChanged += NetVarEv_GameStarted;
        gm.playerTurnNet.OnValueChanged += NetVar_NextTurn;
        gm.scoresNet.OnListChanged += NetVarEv_ScoreChange;

        if (!IsHost)
        {
            Utils.Activation(retryFb.gameObject, GenActivation.Off);
            Invoke(nameof(ScoreDisplaying), 0.3f); //unity bug
        }
        
    }

    public override void OnNetworkDespawn()
    {
        for (int i = 0; i < buttonsTutorialDone.Length; i++)
        {
            buttonsTutorialDone[i].onClick.RemoveAllListeners();
        }

        gm.playerTurnNet.OnValueChanged -= NetVar_NextTurn;
        gm.scoresNet.OnListChanged -= NetVarEv_ScoreChange;
        PlayerManager.Instance.canPlay.OnValueChanged -= NetVarEv_GameStarted;

        base.OnNetworkDespawn();
    }

    void ScoreDisplaying()
    {
        for (int i = 0; i < 2; i++)
        {
            scoresText[i].text = gm.scoresNet[i].ToString();
        }
        
        if (gm.scoresNet[0] > gm.scoresNet[1])
        {
            Utils.ActivateOneArrayElement(scoreLeftImages, 0);
            Utils.ActivateOneArrayElement(scoreRightImages, 2);
        }
        else if (gm.scoresNet[1] > gm.scoresNet[0])
        {
            Utils.ActivateOneArrayElement(scoreLeftImages, 2);
            Utils.ActivateOneArrayElement(scoreRightImages, 1);
        }
        else
        {
            Utils.ActivateOneArrayElement(scoreLeftImages, 0);
            Utils.ActivateOneArrayElement(scoreRightImages, 0);
        }
    }
    public void PanelDisplaying(UiVisibleInGame visibleInGame) => Utils.ActivateOneArrayElement(canvasElements, (int)visibleInGame);
    public void EndMatch()
    {
        if (IsServer) PlayerManager.Instance.canPlay.Value = false;
        switch (PlayerRewards.Data.matchResult)
        {
            case GenResult.Win:
                endInfo.text = "Congratulations!\n<size=60%><color=#8482AF>You won this match!";
                if (_dm.gameData.gameType == MainGameType.Campaign) Utils.Activation(playNextFb.gameObject, GenActivation.On);
                break;
            case GenResult.Lose:
                endInfo.text = "Sorry!\n<size=60%><color=#8482AF>Looks like you lost this round!";
                break;
            case GenResult.Draw:
                endInfo.text = "It's a draw!\n<size=60%><color=#8482AF>You are equally awesome players!";
                break;
        }
        endInfo.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        for (int i = 0; i < 2; i++)
        {
            scoresParTransform[i].DOScale(0.85f * Vector3.one, 0.2f).SetEase(Ease.OutBounce);
        }
        
        if (_dm.gameData.gameType == MainGameType.Practice) return;
        
        int xp = PlayerRewards.Data.TotalXp();
        PlayerRewards.CalculateLevelFromXp(out int prevLevel, out _);
        _dm.playerData.xp += xp;
        PlayerRewards.CalculateLevelFromXp(out int currentLevel, out _);
        xpText.text = xp.ToString();
        int gold = PlayerRewards.Data.TotalCoins();
        _dm.playerData.gold += gold;
        coinsText.text = gold.ToString();
        for (int i = 0; i < 2; i++)
        {
            rewardTransforms[i].DOScale(3f * Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            for (int i = 0; i < endSpawnPositions.Length; i++)
            {
                endSpawnPositions[i].position = new Vector3(-endSpawnPositions[i].position.x, endSpawnPositions[i].position.y, endSpawnPositions[i].position.z);
            }
        }
        
        int counterSpawn = 0;
        if (PlayerRewards.Data.xpCampaign != 0) SpawnEndUi(UiVisibleEndGame.Campaign);
        if (currentLevel > prevLevel) SpawnEndUi(UiVisibleEndGame.Xp);
        if (PlayerRewards.Data.leagueChange != null)
        {
            int league = _dm.playerData.league;
            switch (PlayerRewards.Data.leagueChange)
            {
                case GenChange.Increase:
                    league++;
                    break;
                case GenChange.Decrease:
                    league--;
                    break;
                case GenChange.Reset:
                    break;
            }
            _dm.playerData.league = league;
            SpawnEndUi(UiVisibleEndGame.League);
        }

        if (_dm.gameData.gameType == MainGameType.Multi)
        {
            string matchSummary = $"{DateTime.Now.Date.ToShortDateString()}/" +
                                  $"{gm.scoresNet[IsServer ? 0 : 1]}/" +
                                  $"{gm.scoresNet[!IsServer ? 0 : 1]}/" +
                                  $"{PlayerManager.Instance.playerDisplayNet[!IsServer ? 0 : 1].name}";
            bool slotFound = false;
            for (int i = 0; i < _dm.playerData.matchHistory.Length; i++)
            {
                if (_dm.playerData.matchHistory[i] == String.Empty)
                {
                    _dm.playerData.matchHistory[i] = matchSummary;
                    slotFound = true;
                    break;
                }
            }
            if (!slotFound)
            {
                Queue<string> q = new Queue<string>();
                for (int i = 0; i < _dm.playerData.matchHistory.Length; i++)
                {
                    q.Enqueue(_dm.playerData.matchHistory[i]);
                }
                q.Enqueue(matchSummary);
                q.Dequeue();
            }
        }
        
        void SpawnEndUi(UiVisibleEndGame uiVisible)
        {
            GameObject endGo = Instantiate(endUiPrefab, parEnd);
            endGo.transform.position = endSpawnPositions[counterSpawn].position;
            endGo.GetComponent<EndUi>().InitializeMe(uiVisible);
            counterSpawn++;
        }
    }

    #region RPC
    
    [Rpc(SendTo.Everyone)]
    public void SetMissCounter_EveryoneRpc(int counter)
    {
        string st = "";
        switch (counter)
        {
            case 1:
                st = "Miss! 3 more left..";
                break;
            case 2:
                st = "Miss! 2 more left..";
                break;
            case 3:
                st = "Miss again! Only 1 more left..";
                break;
            case 4:
                st = "And another miss! This was last one...";
                break;
        }
        
        if (_moveTween != null && _moveTween.IsActive()) _moveTween.Kill();
        _moveTween = missTr.DOMoveY(CONST_EndPos, CONST_MoveTime)
                        .SetEase(Ease.OutExpo)
                        .From(CONST_StartPos)
                        .OnComplete(() => _missText.text = "");
        
        _missText.text = st;
    }
    #endregion

    #region BUTTONS/CALLS
    
    [ContextMenu("BtnMethodExit")]
    void BtnMethodExit()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        NetworkManager.Singleton.Shutdown();
    }

    [ContextMenu("BtnMethodRestart")]
    void BtnMethodRestart()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        gm.isRestarting.Value = true;
        foreach (NetworkClient item in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (item.PlayerObject != null) item.PlayerObject.Despawn();
        }
        StartCoroutine(_dm.NewSceneAfterFadeIn("RestartSequence", _dm.gameData.gameType));
    }

    public void BtnMethodSpCanStart()
    {
        gm.botManager.IsActive = true;
        PlayerManager.Instance.canPlay.Value = true; //host that calls this method, no need for RPC call
    }

    void NetVarEv_GameStarted(bool prevValue, bool canPlay)
    {
        print(canPlay ? "game start invoked" : "game stopped");
        if (canPlay) PanelDisplaying(UiVisibleInGame.Main);
    }

    private void NetVarEv_ScoreChange(NetworkListEvent<uint> changeevent) => ScoreDisplaying();
    public void NetVar_NextTurn(PlayerFaction previousValue, PlayerFaction newValue)
    {
        int index = (int)newValue;
        if (index > 1) return;

        if (index == 0)
        {
            circleImage.sprite = Utils.SpriteFromTexture(playerTextures[index]);
            circleName.text = PlayerManager.Instance.playerDisplayNet[index].name.ToString();
        }
        else if (_dm.gameData.gameType != MainGameType.Multi)
        {
            circleImage.sprite = Utils.SpriteFromTexture(_dm.gameData.botSp.botTexture);
            circleName.text = _dm.gameData.botSp.botName;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(circleName.rectTransform);
        Utils.ActivateOneArrayElement(outlinesMainImages, index);
        Utils.ActivateOneArrayElement(outlinesCircleImages, index);
    }

    #endregion



}

