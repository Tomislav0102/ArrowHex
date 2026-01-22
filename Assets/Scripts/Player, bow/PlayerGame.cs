using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerGame : NetworkBehaviour
{
    PlayerManager _pm;
    PersistentDataManager _dm;
    PlayerRigRef _rig;
    [SerializeField] Transform parBows, parHeads, parHands;
    [ReadOnly] public BowControl bowCurrent;
    Transform _bowTransform;
    LineRenderer _lrShooting;
    [ReadOnly] public BowShooting shootingCurrent;
    [SerializeField] Transform head;
    [Title("Name tag")]
    [SerializeField] Transform canvasTr;
    [SerializeField] TextMeshProUGUI nameText, levelText, leagueText;
    [Title("Hands")]
    [SerializeField] Transform[] handsTransform;
    int _state = Animator.StringToHash("state");
    public Animator[] handsAnim;
    public PlayerInteractor[] handsInteractor;
    [SerializeField] SkinnedMeshRenderer[] handsMeshMat0;
    [SerializeField] SkinnedMeshRenderer[] handsMeshMat1;

    public GenSide SideThatHoldsBow() //GenSide.Center means no bow is being held
    {
        if (bowCurrent.interactor == handsInteractor[0]) return GenSide.Left;
        if (bowCurrent.interactor == handsInteractor[1]) return GenSide.Right;

        return GenSide.Center;
    }

    int Index() => NetworkObject.IsOwnedByServer ? 0 : 1;
    #region BODY ROTATION
    [SerializeField] Transform bodyTr;
    const int CONST_RotSpeed = 5;
    const int CONST_RotMinAngle = 30;
    const int CONST_RotMaxAngle = 40;
    bool _isCentering;
    #endregion

  #region INI
    
    void Awake()
    {
        _pm = PlayerManager.Instance;
        _dm = Launch.Instance.persistentDataManager;
        _rig = _pm.playerRigRef;
        _bowTransform  = parBows.GetChild(0);//to prevent error from Lateupdate
    }
    

    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    
        _pm.playerDisplayNet.OnListChanged += NetVar_PlayerData;
        _pm.equipmentNet.OnListChanged += NetVar_Equipment;
    
        handsAnim[0].SetInteger(_state, 0);
        handsAnim[1].SetInteger(_state, 0);
        
        canvasTr.GetComponent<Canvas>().worldCamera = _rig.mainCam;
        if (IsOwner)
        {
            GameManager.Instance.playerVictoriousNet.OnValueChanged += NetVarEv_End;
            
            PlayerRewards.CalculateLevelFromXp(out int level, out _);
            _pm.RegisterPlayerDisplay_ServerRpc(Index(), 
                _dm.playerData.namePlayer, 
                _dm.playerData.imageUrl,
                (uint)level, 
                (byte)_dm.playerData.league);
    
            int[] equipment = new int[4]
            {
                _dm.playerData.bowItem.id,
                _dm.playerData.headItem.id,
                _dm.playerData.glovesItem.id,
                _dm.playerData.arrowItem.id
            };
            byte[] equipmentBytes = new byte[4]
            {
                (byte)(equipment[0] - _dm.gameData.IndicesForScriptables[CloudData.Bow]),
                (byte)(equipment[1] - _dm.gameData.IndicesForScriptables[CloudData.Head]),
                (byte)(equipment[2] - _dm.gameData.IndicesForScriptables[CloudData.Gloves]),
                (byte)(equipment[3] - _dm.gameData.IndicesForScriptables[CloudData.Arrow]),
            };
            _pm.RegisterPlayerEquipment_ServerRpc(Index(), equipmentBytes);

            _rig.playerGame = this;
            _rig.leftController.selectAction.action.performed += InputSelectLeftOn;
            _rig.leftController.selectAction.action.canceled += InputSelectLeftOff;
            _rig.rightController.selectAction.action.performed += InputSelectRightOn;
            _rig.rightController.selectAction.action.canceled += InputSelectRightOff;
            _rig.leftController.activateAction.action.performed += InputActivateLeftOn;
            _rig.leftController.activateAction.action.canceled += InputActivateLeftOff;
            _rig.rightController.activateAction.action.performed += InputActivateRightOn;
            _rig.rightController.activateAction.action.canceled += InputActivateRightOff;
            _rig.root.SetPositionAndRotation(PlayerManager.Instance.standAnimators[NetworkManager.Singleton.IsHost ? 0 : 1].transform.position, Quaternion.identity);
    
            Utils.Activation(canvasTr, GenActivation.Off);
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            GameManager.Instance.playerVictoriousNet.OnValueChanged -= NetVarEv_End;
            _rig.leftController.selectAction.action.performed -= InputSelectLeftOn;
            _rig.leftController.selectAction.action.canceled -= InputSelectLeftOff;
            _rig.rightController.selectAction.action.performed -= InputSelectRightOn;
            _rig.rightController.selectAction.action.canceled -= InputSelectRightOff;
            _rig.leftController.activateAction.action.performed -= InputActivateLeftOn;
            _rig.leftController.activateAction.action.canceled -= InputActivateLeftOff;
            _rig.rightController.activateAction.action.performed -= InputActivateRightOn;
            _rig.rightController.activateAction.action.canceled -= InputActivateRightOff;
        }
        _pm.playerDisplayNet.OnListChanged -= NetVar_PlayerData;
        _pm.equipmentNet.OnListChanged -= NetVar_Equipment;
        base.OnNetworkDespawn();
    }
    #endregion
    



    [Rpc(SendTo.Everyone)]
    public void LineRendererLength_EveryoneRpc(Vector3 pos) => _lrShooting.SetPosition(1, pos);

    [Rpc(SendTo.NotMe)]
    void SyncBow_NotMeRpc(Vector3 pos, Quaternion rot) => _bowTransform.SetPositionAndRotation(pos, rot);

    [Rpc(SendTo.NotMe)]
    void SyncHands_NotMeRpc(Vector3[] pos, Quaternion[] rot)
    {
        for (int i = 0; i < 2; i++)
        {
            handsTransform[i].SetPositionAndRotation(pos[i], rot[i]);
        }
    }

    [Rpc(SendTo.NotMe)]
    public void SyncHandAnims_NotMeRpc(byte handIndex, byte value) => handsAnim[(int)handIndex].SetInteger(_state, (int)value);


    private void Update()
    {
        if (IsOwner) return;
        canvasTr.LookAt(_rig.camMainTransform.position);
        BodyRotation();
        
        void BodyRotation()
        {
            Vector3 projectedForward = Vector3.ProjectOnPlane(head.forward, Vector3.up);
            float currentAngle = Vector3.Angle(projectedForward, bodyTr.forward);
            
            if (currentAngle > CONST_RotMaxAngle) _isCentering = true;
            else if (currentAngle < 1f) _isCentering = false;
            
            if (currentAngle > CONST_RotMinAngle || _isCentering)
            {
                Quaternion targetRot = Quaternion.LookRotation(projectedForward);
                bodyTr.rotation = Quaternion.Slerp(bodyTr.rotation, targetRot, Time.deltaTime * CONST_RotSpeed);
            }
        }
    }


    void LateUpdate()
    {
        if (!IsOwner) return;
        
        transform.SetPositionAndRotation(_rig.root.position, _rig.root.rotation);
        head.SetPositionAndRotation(_rig.head.position, _rig.head.rotation);
        handsTransform[0].SetPositionAndRotation(_rig.leftHand.position, _rig.leftHand.rotation);
        handsTransform[1].SetPositionAndRotation(_rig.rightHand.position, _rig.rightHand.rotation);
        
        Vector3[] handPos = new Vector3[2];
        Quaternion[] handRot = new Quaternion[2];
        for (int i = 0; i < 2; i++)
        {
            handPos[i] = handsTransform[i].position;
            handRot[i] = handsTransform[i].rotation;
        }
        SyncHands_NotMeRpc(handPos, handRot);
        SyncBow_NotMeRpc(_bowTransform.position, _bowTransform.rotation);

    }


    #region CALL EVENTS & EVENTS
    void NetVar_PlayerData(NetworkListEvent<NetPlayerDisplay> changeevent)
    {
        int index = Index();
        string myName = "";
        string levelDisplay = "LV ";
        string leagueDisplay = "";
            
        myName = _pm.playerDisplayNet[index].name.ToString();
        levelDisplay += _pm.playerDisplayNet[index].level.ToString();
        leagueDisplay += ((League)_pm.playerDisplayNet[index].league).ToString();
        if ((League)_pm.playerDisplayNet[index].league != League.Challenger) leagueDisplay = leagueDisplay.Remove(leagueDisplay.Length - 1, 1);
            
        nameText.text = myName;
        levelText.text = levelDisplay ;
        leagueText.text = leagueDisplay;
        LayoutRebuilder.ForceRebuildLayoutImmediate(nameText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(leagueText.rectTransform);
        
        gameObject.name = $"Igrach {index}";
    
    }
    void NetVar_Equipment(NetworkListEvent<NetPlayerEquipment> changeevent)
    {
        int bowIndex = _pm.equipmentNet[Index()].bowIndex;
        Utils.ActivateOneArrayElement(Utils.AllChildrenGameObjects(parBows), bowIndex);
        bowCurrent = parBows.GetChild(bowIndex).GetComponent<BowControl>();
        _bowTransform = bowCurrent.transform;
        shootingCurrent = bowCurrent.transform.GetChild(0).GetComponent<BowShooting>();
        _lrShooting = shootingCurrent.GetComponent<LineRenderer>();
    
        int handIndex = _pm.equipmentNet[Index()].glovesIndex;
        for (int i = 0; i < 2; i++)
        {
            handsMeshMat0[i].material = _dm.gloves[handIndex].mats[0];
            handsMeshMat1[i].material = _dm.gloves[handIndex].mats[1];
        }
        
        if (IsOwner)
        {
            bowCurrent.InitializeMe(this);
            shootingCurrent.InitializeMe(this);
            
            for (int i = 0; i < 2; i++)
            {
                handsInteractor[i].playerGame = this;
            }
            Utils.Activation(bodyTr, GenActivation.Off);
        }
        else
        {
            bowCurrent.enabled = false;
            shootingCurrent.enabled = false;
            
            int headIndex = _pm.equipmentNet[Index()].headIndex;
            Utils.ActivateOneArrayElement(Utils.AllChildrenGameObjects(parHeads), headIndex);
            for (int i = 0; i < 2; i++)
            {
                parHeads.GetChild(headIndex).GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material = GameManager.Instance.factionData[Index()].matMain;
            }
            
            for (int i = 0; i < 2; i++)
            {
                handsInteractor[i].enabled = false;
            }
        }
    }
    private void NetVarEv_End(PlayerFaction previousValue, PlayerFaction newValue)
    {
        if (newValue != PlayerFaction.Undefined)
        {
            handsAnim[0].SetInteger(_state, 0);
            handsAnim[1].SetInteger(_state, 0);
            Utils.Activation(bowCurrent.gameObject, GenActivation.Off);
        }
    }

    
    private void InputSelectLeftOn(InputAction.CallbackContext context) => handsInteractor[0].Selected = true;
    private void InputSelectLeftOff(InputAction.CallbackContext context) => handsInteractor[0].Selected = false;
    private void InputSelectRightOn(InputAction.CallbackContext context) => handsInteractor[1].Selected = true;
    private void InputSelectRightOff(InputAction.CallbackContext context) => handsInteractor[1].Selected = false;
    private void InputActivateLeftOn(InputAction.CallbackContext context) => handsInteractor[0].Activated = true;
    private void InputActivateLeftOff(InputAction.CallbackContext context) => handsInteractor[0].Activated = false;
    private void InputActivateRightOn(InputAction.CallbackContext context) => handsInteractor[1].Activated = true;
    private void InputActivateRightOff(InputAction.CallbackContext context) => handsInteractor[1].Activated = false;


    #endregion
}
