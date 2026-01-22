using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Bow")]
public class SoBow : SoItem
{
    public int power;

    void Awake()
    {
        dataType = CloudData.Bow;
    }
}
