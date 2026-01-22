using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SoPlayerData : SerializedScriptableObject
{
    public string id, namePlayer, imageUrl;
    public int xp, league, mpTotal, mpWins, gold, diamonds;
    public SoBow bowItem;
    public SoHead headItem;
    public SoGloves glovesItem;
    public SoArrow arrowItem;
    public SoEnvironment envItem;

    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    public string[] matchHistory;
    
    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    public Dictionary<CloudData, List<int>> ownedItems;

    [HorizontalGroup("Campaign")] [BoxGroup("Campaign/Current")]
    public CampProgress campProgressCurrent; //saved in playerprefs
    [HorizontalGroup("Campaign")][BoxGroup("Campaign/Total - completed")]
    public CampProgress campProgressTotal; //saved in backend
    
    [Tooltip("True is win, false is lose/draw, count is played so far.")]
    [Space]
    public List<bool> leagueProgress;

    #region INVENTORY
    public HashSet<SoItem> ItemsEquipped()
    {
        HashSet<SoItem> items = new HashSet<SoItem>();
        items.Add(bowItem);
        items.Add(headItem);
        items.Add(glovesItem);
        items.Add(arrowItem);
        items.Add(envItem);
        return items;
    }

    public void EquipSoItem(SoItem soItem)
    {
        if (soItem == null) return;
        
        switch (soItem.GetType().Name)
        {
            case nameof(SoBow):
                bowItem = (SoBow)soItem;
                break;
            case nameof(SoHead):
                headItem = (SoHead)soItem;
                break;
            case nameof(SoGloves):
                glovesItem = (SoGloves)soItem;
                break;
            case nameof(SoArrow):
                arrowItem = (SoArrow)soItem;
                break;
            case nameof(SoEnvironment):
                envItem = (SoEnvironment)soItem;
                break;
        }

    }
    #endregion


}
