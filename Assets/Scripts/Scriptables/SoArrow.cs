using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Arrow")]
public class SoArrow : SoItem
{
    void Awake()
    {
        dataType = CloudData.Arrow;
    }
}
