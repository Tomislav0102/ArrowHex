using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BowControl : MonoBehaviour, IItemCarrier
{
    [field: SerializeField] public SoItem Item { get; set; }
    [field: SerializeField] public GameObject MyGameObject { get; set; }

    PlayerGame _playerGame;
    bool _isInitialized;
    int _myIndex;
    [SerializeField] GameObject bowBody;
    [SerializeField] Transform myTransform;
    [SerializeField] Rigidbody myRigid;
    [SerializeField] Transform attachPoint;
    [SerializeField] BoxCollider myCollider;
    Transform _rackParent;
    Vector3 _diffPos, _lastPosition;
    const float CONST_MinY = -4f;
    const int CONST_ThrowVelocity = 80;

    public BowState Bstate
    {
        get => _bState;
        set
        {
            myRigid.isKinematic = true;
            _playerGame.shootingCurrent.myCollider.enabled = false;
            myCollider.enabled = true;
            _bState = value;
            switch (value)
            {
                case BowState.RackMoving:
                    interactor = null;
                    myCollider.enabled = false;
                    myTransform.SetPositionAndRotation(_rackParent.position, _rackParent.rotation);
                    GameManager.Instance.psBowsReadyToPickup[_myIndex].Play();
                    break;

                case BowState.RackDone:
                    break;

                case BowState.InHand:
                    PlayerManager.Instance.StandAnimators_EveryoneRpc((byte)_myIndex, false);
                    _playerGame.shootingCurrent.myCollider.enabled = true;
                    GameManager.Instance.psBowsReadyToPickup[_myIndex].Stop();
                    break;

                case BowState.Free:
                    _playerGame.shootingCurrent.ReleaseString();
                    interactor = null;
                    myRigid.isKinematic = false;
                    myRigid.velocity = CONST_ThrowVelocity * _diffPos;
                    break;
            }
        }
    }
    [ShowInInspector][ReadOnly] BowState _bState;
    bool _oneHitBowPickup;
    [ReadOnly] public PlayerInteractor interactor;



    public void InitializeMe(PlayerGame playerGame)
    {
        if (!_isInitialized)
        {
            _playerGame = playerGame;
            _playerGame.shootingCurrent.power = ((SoBow)Item).power;
            _myIndex = NetworkManager.Singleton.IsHost ? 0 : 1;
            _rackParent = PlayerManager.Instance.bowSpawnPoints[_myIndex];
            PlayerManager.Instance.canPlay.OnValueChanged += NetVarEv_ReturnBowToRack;
            if (Launch.Instance.persistentDataManager.gameData.gameType == MainGameType.Multi ||
                Launch.Instance.persistentDataManager.gameData.gameType == MainGameType.Practice) NetVarEv_ReturnBowToRack();
        }
        
        
        _isInitialized = true;
    }


    private void OnDisable()
    {
        if (!_isInitialized) return;
        PlayerManager.Instance.canPlay.OnValueChanged -= NetVarEv_ReturnBowToRack;
    }

    private void Update()
    {
        if (!_isInitialized) return;
        switch (Bstate)
        {
            case BowState.RackMoving:
                myTransform.SetPositionAndRotation(_rackParent.position, _rackParent.rotation);
                break;
            case BowState.InHand:
                if (interactor == null) return;
               // myTransform.SetPositionAndRotation(interactor.myTransform.position - attachPoint.localPosition, interactor.myTransform.rotation * Quaternion.Inverse(attachPoint.localRotation));
                myTransform.SetPositionAndRotation(interactor.bowGrabPoint.position, interactor.bowGrabPoint.rotation);
                _diffPos = myTransform.position - _lastPosition;
                _lastPosition = myTransform.position;
                break;
            case BowState.Free:
                if (myTransform.position.y < CONST_MinY) NetVarEv_ReturnBowToRack();
                break;
        }

        if (!_oneHitBowPickup && PlayerManager.Instance.standAnimators[_myIndex].GetCurrentAnimatorStateInfo(0).IsName("PlatformRise") &&
            PlayerManager.Instance.standAnimators[_myIndex].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            _oneHitBowPickup = true;
            Bstate = BowState.RackDone;
        }
        
        if (!PlayerManager.Instance.standAnimators[_myIndex].GetCurrentAnimatorStateInfo(0).IsName("PlatformRise")) _oneHitBowPickup = false;
    }
    void NetVarEv_ReturnBowToRack(bool prevValue = false, bool canPlay = true)
    {
        if (!canPlay) return;
        if (!myRigid.isKinematic) myRigid.velocity = Vector3.zero;
        Bstate = BowState.RackMoving;
        PlayerManager.Instance.StandAnimators_EveryoneRpc((byte)_myIndex, true);
    }


}
