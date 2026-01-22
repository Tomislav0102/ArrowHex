using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupUiDifficulty : TabGroupUi
{

    public override void InitializeMe()
    {
        base.InitializeMe();
        singleTabs[PlayerPrefs.GetInt(Launch.Instance.persistentDataManager.gameData.difficulty_Int)/2].Clicked();
    }

    protected override void TabClicked(int index)
    {
        base.TabClicked(index);
        PlayerPrefs.SetInt(Launch.Instance.persistentDataManager.gameData.difficulty_Int, index * 2);
    }
}
