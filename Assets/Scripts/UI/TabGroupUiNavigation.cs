using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupUiNavigation : TabGroupUi
{
    [SerializeField] GameObject[] panels; 

    public override void InitializeMe()
    {
        base.InitializeMe();
        singleTabs[0].Clicked();
    }

    protected override void TabClicked(int index)
    {
        base.TabClicked(index);
        
        Utils.ActivateOneArrayElement(panels, index);
        Transform currentPanel = panels[index].transform;
        if (index == 0) //campaign
        {
            bool isTutorial = Launch.Instance.persistentDataManager.playerData.campProgressCurrent.IsTutorial();
            currentPanel.GetChild(0).gameObject.SetActive(isTutorial);
            currentPanel.GetChild(1).gameObject.SetActive(!isTutorial);
        }
        else
        {
            currentPanel.GetChild(0).gameObject.SetActive(true);
            currentPanel.GetChild(1).gameObject.SetActive(false);
        }
        currentPanel.GetChild(2).gameObject.SetActive(false);

    }
}
