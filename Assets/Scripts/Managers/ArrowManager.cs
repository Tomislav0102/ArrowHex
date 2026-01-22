using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowManager : NetworkBehaviour
{
    public GameObject prefabArrowReal;
    public GameObject prefabArrowShadow;
    [Sirenix.OdinInspector.ReadOnly] public ArrowMain arrowReal;
    [Sirenix.OdinInspector.ReadOnly] public ArrowMain arrowShadow;
    [Sirenix.OdinInspector.ReadOnly] public float forceArrow;


    public void SpawnRealArrow(Vector3 pos, Quaternion rot, byte arrowIndex)
    {
        if (arrowReal != null)  Utils.DestroyGo(arrowReal.gameObject);
        arrowReal = Instantiate(prefabArrowReal, pos, rot).GetComponent<ArrowMain>();
        arrowReal.SetArrow(arrowIndex);
        SpawnShadowArrow_NotMeRpc(pos, rot, arrowIndex);
    }

    [Rpc(SendTo.NotMe)]
    void SpawnShadowArrow_NotMeRpc(Vector3 pos, Quaternion rot, byte arrowIndex)
    {
        if (arrowShadow != null)  Utils.DestroyGo(arrowShadow.gameObject);
        GameObject go = Instantiate(prefabArrowShadow, pos, rot);
        arrowShadow = go.GetComponent<ArrowMain>();
        arrowShadow.SetArrow(arrowIndex);
    }

    [Rpc(SendTo.NotMe)]
    public void PositioningShadowArrow_NotMeRpc(Vector3 pos, Quaternion rot)
    {
        if (arrowShadow == null) return;
        arrowShadow.myTransform.SetPositionAndRotation(pos, rot);
    }

    [Rpc(SendTo.Everyone)]
    public void Trails_EveryoneRpc()
    {
        if (arrowReal != null)  arrowReal.SetTrail();
        if (arrowShadow != null)  arrowShadow.SetTrail();
    }

    [Rpc(SendTo.Everyone)]
    public void RemoveShadows_EveryoneRpc()
    {
        if (arrowShadow != null)  Utils.DestroyGo(arrowShadow.gameObject);
    }

}
