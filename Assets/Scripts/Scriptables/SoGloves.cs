using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gloves")]
public class SoGloves : SoItem
{
    public Material[] mats;
    
    void Awake()
    {
        dataType = CloudData.Gloves;
    }

}
