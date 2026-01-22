using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Head")]
public class SoHead : SoItem
{
    void Awake()
    {
        dataType = CloudData.Head;
    }

}
