using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class BotManager : MonoBehaviour
{
    GameManager gm;
    PersistentDataManager _dm; 
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Utils.Activation(botGo, _isActive ? GenActivation.On : GenActivation.Off);
        }
    }
    [ShowInInspector][ReadOnly] bool _isActive;

    [SerializeField] GameObject botGo;
    BotAnimationManager _botAnimationManager;
    SoBot _myBot;

    #region SHOOTING
    [ShowInInspector][ReadOnly] List<ParentHex> _finalListForBot = new List<ParentHex>(); //all hexes sorted by their value
    [ShowInInspector][ReadOnly] ParentHex _target; //chosen hex to target
    Dictionary<BotStrength, int> _maxNumOfBestHexes = new Dictionary<BotStrength, int>
    {
        { BotStrength.Noob , 10},
        { BotStrength.Beginner , 9},
        { BotStrength.Average , 7},
        { BotStrength.Skilled , 5},
        { BotStrength.Expert , 3},
        { BotStrength.Insane , 1},
    };
    float Precision()
    {
        switch (_myBot.difficulty)
        {
            case BotStrength.Noob:
                return Random.value + (Random.value > 0.5 ? 0.3f : 0f);
            case BotStrength.Beginner:
                return Random.value;
            case BotStrength.Average:
                return Mathf.Pow(Random.value, 2);
            case BotStrength.Skilled:
                return Mathf.Pow(Random.value, 3);
            case BotStrength.Expert:
                return Mathf.Pow(Random.value, 4);
            case BotStrength.Insane:
                return Mathf.Pow(Random.value, 5);
            default:
                return 0;
        }
    }
    [SerializeField] Transform targetPointer;
    #endregion


    
    private void Awake()
    {
        gm = GameManager.Instance;
        _botAnimationManager = botGo.GetComponent<BotAnimationManager>();
        _dm = Launch.Instance.persistentDataManager;
    }

    void Start()
    {
        _myBot = _dm.gameData.botSp;
    }

    private void OnEnable()
    {
        gm.playerTurnNet.OnValueChanged += NetVarEv_NextTurn;
        gm.playerVictoriousNet.OnValueChanged += NetVarEv_EndMatch;
    }

    private void OnDisable()
    {
        gm.playerTurnNet.OnValueChanged -= NetVarEv_NextTurn;
        gm.playerVictoriousNet.OnValueChanged -= NetVarEv_EndMatch;
    }

    private void NetVarEv_NextTurn(PlayerFaction previousValue, PlayerFaction newValue)
    {
        if (!IsActive) return;
        if (newValue == PlayerFaction.First_left) return;
        BotMethod();
    }
    void NetVarEv_EndMatch(PlayerFaction previousValue, PlayerFaction newValue)
    {
        if (!IsActive) return;
        if (newValue == PlayerFaction.Undefined) return;
        IsActive = false;
    }



    void BotMethod()
    {
        List<ParentHex> allFree = gm.gridManager.AllTilesByType(TileState.Free);
        if (allFree.Count == 0) return;

        _finalListForBot = HexSortingByTargetValue(allFree);

        int finalCount = Mathf.Min(_finalListForBot.Count, _maxNumOfBestHexes[_myBot.difficulty]);

        _target = _finalListForBot[Random.Range(0, finalCount)];
        
        Vector2 rdn = Random.insideUnitCircle * Precision();
        Vector3 tarPos = new Vector3(_target.center.position.x + rdn.x, _target.center.position.y + rdn.y, _target.center.position.z);
        targetPointer.position = tarPos;
        _botAnimationManager.BeginShootingSequence(tarPos);
    }

    List<ParentHex> HexSortingByTargetValue(List<ParentHex> allFree)
    {
        foreach (ParentHex item in allFree)
        {
            item.valueForBot = 0;
        }

        foreach (ParentHex item in allFree)
        {
            item.valueForBot += -10 + item.CurrentValue;
            List<ParentHex> neigh = gm.gridManager.AllNeighbours(item.pos);
            foreach (ParentHex n in neigh)
            {
                item.valueForBot += -10 + n.CurrentValue;
            }
        }

        return allFree.OrderBy(n => n.valueForBot).ToList();
    }
}



