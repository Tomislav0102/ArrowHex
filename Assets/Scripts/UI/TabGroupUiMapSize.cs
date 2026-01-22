using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupUiMapSize : TabGroupUi
{

    public override void InitializeMe()
    {
        base.InitializeMe();
        singleTabs[PlayerPrefs.GetInt(Launch.Instance.persistentDataManager.gameData.size_Int)].Clicked();
    }

    protected override void TabClicked(int index)
    {
        base.TabClicked(index);
        PlayerPrefs.SetInt(Launch.Instance.persistentDataManager.gameData.size_Int, index);
    }
}
