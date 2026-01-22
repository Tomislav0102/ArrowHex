using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace TempSetup
{
    public class TempPlayer : NetworkBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] Material[] matsMain;
        
        // public override void OnNetworkSpawn()
        // {
        //     base.OnNetworkSpawn();
        //     if (IsOwnedByServer)
        //     {
        //         transform.position = new Vector3(-2f, 0f, 0f);
        //         meshRenderer.material = matsMain[0];
        //     }
        //     else
        //     {
        //         transform.position = new Vector3(2f, 0f, 0f);
        //         meshRenderer.material = matsMain[1];
        //     }
        // }

    }
}
