using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class BowShooting : MonoBehaviour
{
    GameManager gm;
    BowControl _bow;
    PlayerGame _playerGame;
    bool _isInitialized;
    AudioSource _audioSource;
    bool _oneHitAudioDraw;
    public BoxCollider myCollider;
    [SerializeField] Transform end;
    float _maxLength, _offsetStart;
    [SerializeField] Transform notch;
    float _pullAmount;
    Transform _myTransform;

    [ReadOnly] public float power;
    bool _oneHitArrowNotched;
    [ReadOnly] public bool controllerPullingString;

    [SerializeField] bool vRSimulation = true; //only for keyboard control
    
    public int _arrowIndex; //testing

    public void InitializeMe(PlayerGame playerGame)
    {
        _oneHitArrowNotched = _oneHitAudioDraw = controllerPullingString = false;
        gm = GameManager.Instance; 
        gm.playerTurnNet.OnValueChanged += NetVarEv_PlayerTurn;

        if (!_isInitialized)
        {
            _playerGame = playerGame;
            _bow = playerGame.bowCurrent;
            _audioSource = _bow.GetComponent<AudioSource>();
            _myTransform = transform;
            _offsetStart = Mathf.Abs(_myTransform.localPosition.z);
            //_maxLength = (end.localPosition - notch.localPosition).magnitude;
            _maxLength =  Mathf.Abs(end.localPosition.z - _myTransform.localPosition.z);
            _arrowIndex = Launch.Instance.persistentDataManager.playerData.arrowItem.id - Launch.Instance.persistentDataManager.gameData.IndicesForScriptables[CloudData.Arrow];
        }
        _isInitialized = true;
    }



    void OnDisable()
    {
        if (!_isInitialized) return;
        gm.playerTurnNet.OnValueChanged -= NetVarEv_PlayerTurn;
    }
    private void NetVarEv_PlayerTurn(PlayerFaction previousValue, PlayerFaction newValue)
    {
        _oneHitArrowNotched = false;
    }

    private void Update()
    {
        if (!_isInitialized || !gm.MyTurn()) return;

        if (controllerPullingString)
        {
            if (!_oneHitArrowNotched)
            {
                _oneHitArrowNotched = true;

               gm.arrowManager.SpawnRealArrow(_myTransform.position, _myTransform.rotation, (byte)_arrowIndex);
            }
            
            //audio
            if (_oneHitArrowNotched && _pullAmount > 0.3f && !_oneHitAudioDraw)
            {
                _oneHitAudioDraw = true;
                AudioManager.Instance.PlayOnSpecificAudioSource(_audioSource, AudioManager.Instance.bowDraw);
            }

        }
        else
        {
            if (!Application.isEditor || !vRSimulation) ReleaseString();
        }
        
        if (Input.GetKeyDown(KeyCode.K) && Application.isEditor && vRSimulation) TestShooting();

    }
    private void LateUpdate()
    {
        if (!_isInitialized || !controllerPullingString || !gm.MyTurn() || gm.arrowManager.arrowReal == null) return;
        ProcessNotchedArrow();
    }

    void ProcessNotchedArrow()
    {
        int handThaPullsString = ((int)_playerGame.SideThatHoldsBow() + 1) % 2;
        Vector3 handPos = _playerGame.handsInteractor[handThaPullsString].myTransform.position;
        Vector3 pullDir = notch.position - handPos;

        Vector3 endPoint;
        Quaternion rot;
        if (_myTransform.InverseTransformPoint(handPos).z > 0f)
        {
            endPoint = _myTransform.position;
            rot = Quaternion.LookRotation(notch.position - _myTransform.position);
        }
        else if (pullDir.magnitude < _maxLength + _offsetStart)
        {
            endPoint = handPos;
            rot = Quaternion.LookRotation(pullDir.normalized);
        }
        else
        {
            endPoint = notch.position - (_maxLength + _offsetStart) * pullDir.normalized;
            rot = Quaternion.LookRotation(pullDir.normalized);
        }

        gm.arrowManager.arrowReal.myTransform.SetPositionAndRotation(endPoint, rot);
        _pullAmount = Mathf.Abs(_myTransform.InverseTransformPoint(endPoint).z) / _maxLength;
        gm.arrowManager.forceArrow = _pullAmount * power; 

        _playerGame.LineRendererLength_EveryoneRpc(_myTransform.InverseTransformPoint(endPoint));
    }

    public void ReleaseString()
    {
        if (gm.arrowManager.arrowReal == null) return;
        gm.arrowManager.arrowReal.Release(Vector3.zero);
        _pullAmount = 0.0f;
        controllerPullingString = false;
       // _oneHitArrowNotched = false;
        gm.arrowManager.arrowReal = null;
        _playerGame.LineRendererLength_EveryoneRpc(Vector3.zero);

        AudioManager.Instance.PlayOnSpecificAudioSource(_audioSource, AudioManager.Instance.bowRelease);
        _oneHitAudioDraw = false;
    }

    void TestShooting()
    {
        _oneHitArrowNotched = true;
        gm.arrowManager.SpawnRealArrow(_myTransform.position, _myTransform.rotation, (byte)_arrowIndex);
        gm.arrowManager.forceArrow = 1 * power;
        gm.arrowManager.arrowReal.Release(Vector3.zero);
    }



}


