using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupUiInvState : TabGroupUi
{
    public override void InitializeMe()
    {
        base.InitializeMe();
       // singleTabs[0].Clicked();
    }

    protected override void TabClicked(int index)
    {
        base.TabClicked(index);
       // MainMenuManager.Instance.inventoryGrid.InvState = (InventoryState)index;
    }
}
