using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupUiInvFilter : TabGroupUi
{
    CloudData?[] _cloudData = new CloudData?[6] {null, CloudData.Head, CloudData.Gloves, CloudData.Bow, CloudData.Arrow, CloudData.Environment };


    public override void InitializeMe()
    {
        base.InitializeMe();
        StartCoroutine(LateFilter());

    }

    IEnumerator LateFilter()
    {
        yield return null;
        singleTabs[0].Clicked();
    }

    protected override void TabClicked(int index)
    {
        base.TabClicked(index);
        MainMenuManager.Instance.inventoryGrid.Filter = _cloudData[index];
    }
}
