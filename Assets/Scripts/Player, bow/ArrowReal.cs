using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArrowReal : ArrowMain
{
    [SerializeField] Rigidbody myRigidbody;
    float _arrowLength;
    LayerMask _layerMaskTarget;
    const int CONST_EdgeSideways = 15;
    const int CONST_EdgeForward = 21;
    const int CONST_EdgeBack = -10;
    const int CONST_EdgeVertical = -2;
    Ray _ray;
    RaycastHit _hit;
    RaycastHit2D _hit2D;
    Collider2D _hitCollider;
    PlayerFaction _ownerFaction;

    void Start()
    {
        _arrowLength = Vector3.Distance(myTransform.position, tip.position);
        _ownerFaction = gm.playerTurnNet.Value;
        _layerMaskTarget = Launch.Instance.persistentDataManager.gameData.layTargets;
    }


    public override void Release(Vector3 vel)
    {
        base.Release(vel);
        myRigidbody.useGravity = true;
        myRigidbody.isKinematic = false;
        if (vel == Vector3.zero) myRigidbody.velocity = gm.arrowManager.forceArrow * myTransform.forward;
        else myRigidbody.velocity = vel;
        StartCoroutine(ArrowDirectionWhileFlying(myRigidbody.velocity.magnitude > 0f));
    }


    IEnumerator ArrowDirectionWhileFlying(bool rotateObject = true)
    {
        while (myArrowState == ArrowState.Flying && rotateObject)
        {
            Quaternion velRot = Quaternion.LookRotation(myRigidbody.velocity, Vector3.up);
            myTransform.rotation = velRot;
            yield return null;
        }
    }

    void OnDestroy()
    {
        gm.arrowManager.RemoveShadows_EveryoneRpc();
    }

    void Update()
    {
        gm.arrowManager.PositioningShadowArrow_NotMeRpc(myTransform.position, myTransform.rotation);

        if (myArrowState == ArrowState.Notched)
        {
            bool draw = gm.arrowManager.forceArrow > 0.05f;
            gm.drawTrajectory.Trajectory(myTransform, gm.playerTurnNet.Value, myRigidbody.mass, gm.arrowManager.forceArrow, draw);
            return;
        }
        gm.drawTrajectory.Trajectory(false);
        
        if (IsTooFarAway() && myArrowState == ArrowState.Flying)
        {
            gm.NextPlayer_ServerRpc(_ownerFaction, true, $"arrow update from pos {myTransform.position}");
            myArrowState = ArrowState.Done;
            return;
        }
        if (myTransform.position.y < CONST_EdgeVertical)
        {
            if (myArrowState == ArrowState.Flying) gm.NextPlayer_ServerRpc(_ownerFaction, true, "arrow fell too low");
            myArrowState = ArrowState.Done;
          //  print("arrow fell too low");
            Destroy(gameObject);
        }
        
        
        
        bool IsTooFarAway()
        {
            return myTransform.position.z > CONST_EdgeForward ||
                   myTransform.position.z < CONST_EdgeBack ||
                   myTransform.position.x > CONST_EdgeSideways ||
                   myTransform.position.x < -CONST_EdgeSideways;
        }
    }

    private void FixedUpdate()
    {
        if (myArrowState != ArrowState.Flying) return;
        CheckCollisions();
    }

    void CheckCollisions()
    {
        float extraLength = 4f; //need to compensate for fast moving arrows. Physic isn't accurate if speeds are too high
        _ray.origin = myTransform.position - extraLength * myTransform.forward;
        _ray.direction = myTransform.forward;

        _hit2D = Physics2D.GetRayIntersection(_ray, _arrowLength + extraLength, _layerMaskTarget);
        _hitCollider = _hit2D.collider;
        if (_hitCollider != null && _hitCollider.TryGetComponent(out ITargetForArrow tar))
        {
            tar.HitMe();
            myArrowState = ArrowState.Done;
            //  print("hit collider");
            AudioManager.Instance.PlaySfx(AudioManager.Instance.hexHit, true, myTransform.position);
            Destroy(gameObject);
        }
    }
    
}
