using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Basic")]
public class SoItem : ScriptableObject
{
    public CloudData dataType;
    public bool defaultItem;
    public int id;
    public string itemName;
    public int priceGold;
    public int priceDiamond;
    public Sprite icon;
}
