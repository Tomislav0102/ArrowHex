using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCarrier : MonoBehaviour, IItemCarrier
{
    [field: SerializeField] public SoItem Item { get; set; }
    [field: SerializeField] public GameObject MyGameObject { get; set; }
}
