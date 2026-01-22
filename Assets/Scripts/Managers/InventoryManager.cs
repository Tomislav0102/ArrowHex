using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class InventoryManager : SerializedMonoBehaviour
{
    PersistentDataManager _dm;
    
    [SerializeField] FakeButtonUi prevFb, nextFb;
    [SerializeField] TabGroupUi tab;
    [SerializeField] RectTransform parSlots;
    InvItemUi[] _slots;
    const int CONST_SlotXoffset = 250;
    const float CONST_AnimSpeed = 0.5f;
    float _startXpos;
    
    List<SoItem> _inInv = new List<SoItem>();
    List<SoItem> _filteredList = new List<SoItem>();
    int _screenCounter;
    int _addedEdgeCase = 0;

    public CloudData? Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            _screenCounter = 0;
            print(value);
            RefreshItemDisplay();
            BtnMethodMove();
        }
    }
    CloudData? _filter = null;


    
    [Title("Left avatar")]
    [SerializeField] Dictionary<CloudData, Image> _avImages;

    void Awake()
    {
        _dm = Launch.Instance.persistentDataManager;
        
        List<SoItem> allItemsInGame = new List<SoItem>(); 
        allItemsInGame.AddRange(_dm.heads);
        allItemsInGame.AddRange(_dm.gloves);
        allItemsInGame.AddRange(_dm.bows);
        allItemsInGame.AddRange(_dm.arrows);
        allItemsInGame.AddRange(_dm.environments);
        _inInv = allItemsInGame;
    }

    void Start()
    {
        prevFb.InitializeMe(() => BtnMethodMove(GenSide.Left));
        nextFb.InitializeMe(() => BtnMethodMove(GenSide.Right));

        _slots = new InvItemUi[parSlots.childCount];
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = parSlots.GetChild(i).GetComponent<InvItemUi>();
            _slots[i].InitializeMe();
        }
        
        _startXpos = parSlots.anchoredPosition.x;
        EquipAllItems();
        tab.InitializeMe();
    }


    void EquipAllItems()
    {
        EquipItem(_dm.playerData.bowItem, false);
        EquipItem(_dm.playerData.headItem, false);
        EquipItem(_dm.playerData.glovesItem, false);
        EquipItem(_dm.playerData.arrowItem, false);
        EquipItem(_dm.playerData.envItem, false);
    }
    void BtnMethodMove(GenSide side = GenSide.Center)
    {
        switch (side)
        {
            case GenSide.Left:
                _screenCounter -= 1;
                break;
            case GenSide.Right:
                _screenCounter += 1;
                break;
        }

        
        ArrowsDisplay();
        if (_screenCounter <= 0)
        {
            _screenCounter = 0;
            ArrowsDisplay(GenSide.Left);
        }
        if (_screenCounter * 3 + 12> _filteredList.Count - _addedEdgeCase)
        {
            ArrowsDisplay(GenSide.Right);
        }
        
        parSlots.DOAnchorPosX(_startXpos - _screenCounter * CONST_SlotXoffset, CONST_AnimSpeed);
        
        
        void ArrowsDisplay(GenSide sideToHide = GenSide.Center)
        {
            switch (sideToHide)
            {
                case GenSide.Left:
                    Utils.Activation(prevFb.gameObject, GenActivation.Off);
                    break;
                case GenSide.Right:
                    Utils.Activation(nextFb.gameObject, GenActivation.Off);
                    break;
                case GenSide.Center:
                    Utils.Activation(prevFb.gameObject, GenActivation.On);
                    Utils.Activation(nextFb.gameObject, GenActivation.On);
                    break;
            }
        }

    }



    void RefreshItemDisplay()
    {
        _filteredList = new List<SoItem>();
        for (int i = 0; i < _inInv.Count; i++)
        {
            _filteredList.Add(_inInv[i]);
        }
        if (Filter != null) _filteredList = _filteredList.Where(n => n.dataType == Filter).ToList();

        _addedEdgeCase = 1;
        if (_filteredList.Count < 11)
        {
            for (int i = _filteredList.Count; i < 12; i++)
            {
                _filteredList.Add(null);
            }
        }
        else if (_filteredList.Count  % 3 == 1)
        {
            _filteredList.Add(null);
            _filteredList.Add(null);
        }
        else if (_filteredList.Count  % 3 == 2)
        {
            _filteredList.Add(_filteredList[^1]);
            _filteredList[^2] = null;
        }
        else if (_filteredList.Count  % 3 == 0)
        {
            _filteredList.Add(null);
            _filteredList[^1] = _filteredList[^2];  
            _filteredList[^2] = null;
            _filteredList.Add(null);
            _filteredList.Add(null);
            _addedEdgeCase = 3;
        }

        SlotVisibility(_filteredList.Count);
        for (int i = 0; i < _filteredList.Count; i++)
        {
            SoItem soItem = _filteredList[i];
            _slots[i].MyItem = soItem;
            if (soItem != null)
            {
                _slots[i].toggleEquipFt.IsOn = _dm.playerData.ItemsEquipped().Contains(soItem);
            }
        }

        void SlotVisibility(int totalVisible)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                Utils.Activation(_slots[i].gameObject, i < totalVisible ? GenActivation.On : GenActivation.Off);
            }
        }
    }



    public void EquipItem(SoItem item, bool callEvent = true)
    {
        _dm.playerData.EquipSoItem(item);
        _avImages[item.dataType].sprite = item.icon;
        for (int i = 0; i < _slots.Length; i++)
        {
            InvItemUi invItemUi = _slots[i];
            
            if (!invItemUi.gameObject.activeSelf) break;
            if (invItemUi.MyItem == null) continue;
            if (invItemUi.MyItem.dataType == item.dataType) invItemUi.toggleEquipFt.IsOn = invItemUi.MyItem == item;
        }
    }
    public void UnEquipItem_ReplaceWithDefault(CloudData? itemType)
    {
        if (itemType == null) return;
        SoItem item = _inInv.FirstOrDefault(n => n.dataType == itemType && n.defaultItem);
        EquipItem(item);
    }

}



