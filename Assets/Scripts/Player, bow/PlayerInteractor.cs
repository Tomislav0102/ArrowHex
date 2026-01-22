using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteractor : MonoBehaviour
{
    public bool testMode_grab, testMode_draw;
    GameManager gm;
    public Transform myTransform;
    public Transform bowGrabPoint, stringDrawPoint;
    public PlayerGame playerGame;
    [SerializeField] GenSide side;
    bool _inBow, _inString;
    string _bow;
    string _string;
    [SerializeField] PlayerInteractor otherInteractor;
    public enum AnimationState { Idle, BowGrab, ArrowGrab }
    public AnimationState AnimState
    {
        get => _aState;
        set
        {
            _aState = value;
            switch (value)
            {
                case AnimationState.Idle:
                    break;
                case AnimationState.BowGrab:
                    break;
                case AnimationState.ArrowGrab:
                    break;
            }
            playerGame.handsAnim[(int)side].SetInteger("state", (int)value);
            playerGame.SyncHandAnims_NotMeRpc((byte)side, (byte)value);
        }
    }
    AnimationState _aState;

    public bool Selected //holding bow
    {
        get => _selected;
        set
        {
            if (gm.playerVictoriousNet.Value != PlayerFaction.Undefined ||
                AnimState == AnimationState.ArrowGrab) return;

            _selected = value;
            if (value)
            {
                if (playerGame.SideThatHoldsBow() == side)
                {
                    AnimState = AnimationState.Idle;
                    playerGame.bowCurrent.interactor = null;
                    playerGame.bowCurrent.Bstate = BowState.Free;
                }
                else
                {
                    AnimState = AnimationState.BowGrab;
                    if (_inBow && playerGame.bowCurrent.Bstate != BowState.RackMoving)
                    {
                        playerGame.bowCurrent.interactor = this;
                        playerGame.bowCurrent.Bstate = BowState.InHand;
                        if (otherInteractor.AnimState == AnimationState.BowGrab) otherInteractor.AnimState = AnimationState.Idle;
                    }
                }
                _inBow = false;
            }
            else
            {
                if (playerGame.SideThatHoldsBow() == side) return;
                AnimState = AnimationState.Idle;
            }
        }
    }
    bool _selected;
    public bool Activated //pulling string
    {
        get => _activated;
        set
        {
            if (playerGame.SideThatHoldsBow() == side ||
                !gm.MyTurn() ||
                gm.playerVictoriousNet.Value != PlayerFaction.Undefined ||
                AnimState == AnimationState.BowGrab) return;

            _activated = value;

            if (value)
            {
                AnimState = AnimationState.ArrowGrab;
                if (!_inString) return;
                playerGame.shootingCurrent.controllerPullingString = true;
                PlayerManager.Instance.playerRigRef.RemoveLines();
            }
            else
            {
                AnimState = AnimationState.Idle;
                playerGame.shootingCurrent.controllerPullingString = false;
            }
        }
    }
    bool _activated;

    private void Start()
    {
        gm = GameManager.Instance;
        _bow = Launch.Instance.persistentDataManager.gameData.tagBow;
        _string = Launch.Instance.persistentDataManager.gameData.tagString;

    }

    bool _testSwitch;
    void Update()
    {
        if(!Application.isEditor) return;
        if (testMode_grab)
        {
            if (Input.GetKeyDown(KeyCode.Space)) Selected = !Selected;
            
        }
        if (testMode_draw)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && !_testSwitch)
            {
                Activated = true;
                _testSwitch = true;
            } 
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Activated) return;

        if (other.CompareTag(_bow))
        {
            _inBow = true;
        }
        if (other.CompareTag(_string))
        {
            _inString = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_bow))
        {
            _inBow = false;
        }
        if (other.CompareTag(_string))
        {
            _inString = false;
        }

    }
}


// public class PlayerInteractor : MonoBehaviour
// {
//     GameManager gm;
//     public Transform myTransform;
//     public PlayerGame playerGame;
//     [SerializeField] GenSide side;
//     bool _inBow, _inString;
//
//     public bool Selected
//     {
//         set
//         {
//             if (gm == null ||
//                 gm.playerVictoriousNet.Value != PlayerColor.Undefined) return;
//
//             if (value)
//             {
//                 // if (playerControl.handsAnim[(int)side].GetFloat("Fist") == 0) playerControl.handsAnim[(int)side].SetFloat("Fist", 1);
//                 // else playerControl.handsAnim[(int)side].SetFloat("Fist", 0);
//                 if (playerGame.handsAnim[(int)side].GetFloat("Fist") == 0) SetHandAnim((int)side, 1);
//                 else SetHandAnim((int)side, 0);
//
//                 if (playerGame.SideThatHoldsBow() == side)
//                 {
//                     playerGame.bowCurrent.Bstate = BowState.Free;
//                 }
//                 else if (_inBow && playerGame.bowCurrent.Bstate != BowState.RackMoving)
//                 {
//                     playerGame.bowCurrent.interactor = this;
//                     playerGame.bowCurrent.Bstate = BowState.InHand;
//                    // playerControl.handsAnim[((int)side + 1) % 2].SetFloat("Fist", 0);
//                     SetHandAnim(((int)side + 1) % 2, 0);
//                 }
//                 _inBow = false;
//             }
//             else
//             {
//                 if (playerGame.SideThatHoldsBow() == side) return;
//               //  playerControl.handsAnim[(int)side].SetFloat("Fist", 0);
//                 SetHandAnim((int)side, 0);
//             }
//         }
//     }
//     public bool Activated
//     {
//         get => _activated;
//         set
//         {
//             if (gm == null ||
//                 playerGame.SideThatHoldsBow() == side ||
//                 !gm.MyTurn() ||
//                 gm.playerVictoriousNet.Value != PlayerColor.Undefined) return;
//
//             _activated = value;
//
//            // playerControl.handsAnim[(int)side].SetFloat("Fist", value ? 1 : 0);
//             SetHandAnim((int)side, value ? 1 : 0);
//
//             if (value)
//             {
//                 if (!_inString) return;
//                 playerGame.shootingCurrent.controllerPullingString = true;
//                 PlayerRigRef.instance.RemoveLines();
//             }
//             else
//             {
//                 playerGame.shootingCurrent.controllerPullingString = false;
//             }
//         }
//     }
//     bool _activated;
//
//     private void Start()
//     {
//         gm = GameManager.Instance;
//     }
//
//     void SetHandAnim(int handIndex, int value)
//     {
//         playerGame.handsAnim[handIndex].SetFloat("Fist", value);
//         playerGame.SyncHandAnims_NotMeRpc((byte)handIndex, (byte)value);
//     }
//
//     private void OnTriggerEnter(Collider other)
//     {
//         if (Activated) return;
//
//         if (other.CompareTag("Bow"))
//         {
//             _inBow = true;
//         }
//         if (other.CompareTag("String"))
//         {
//             _inString = true;
//         }
//     }
//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Bow"))
//         {
//             _inBow = false;
//         }
//         if (other.CompareTag("String"))
//         {
//             _inString = false;
//         }
//
//     }
// }
