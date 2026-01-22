using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class InvItemUi : MonoBehaviour
{
    InventoryManager _im;
    [field: SerializeField] public Vector2Int Pos { get; set; } //not being used except when generating slots on scene
    public SoItem MyItem
    {
        get => _myItem;
        set
        {
            _myItem = value;
            if (_myItem == null) //empty slot
            {
                Utils.Activation(visual, GenActivation.Off);
                toggleEquipFt.IsOn = false;
                Utils.Activation(toggleEquipFt.gameObject, GenActivation.Off);
                itemName.text = "";
                return;
            }
            
            Utils.Activation(visual, GenActivation.On); 
            Utils.Activation(toggleEquipFt.gameObject, GenActivation.On);
            img.sprite = value.icon;
            itemName.text = value.itemName;
        }
    }
    [ShowInInspector][ReadOnly] SoItem _myItem;

    [SerializeField] GameObject visual;
    [SerializeField] Image img;
    [SerializeField] TextMeshProUGUI itemName;
    public FakeToggleUi toggleEquipFt;


    void OnDisable()
    {
        MyItem = null;
    }
    
    public void InitializeMe()
    {
        _im = MainMenuManager.Instance.inventoryGrid;
        
        toggleEquipFt.InitializeMe((bool on) =>
        {
            if (MyItem == null) return;
            if (on)
            {
                _im.EquipItem(MyItem);
            }
            else
            {
                _im.UnEquipItem_ReplaceWithDefault(MyItem.dataType);
            }
        });

    }

}

