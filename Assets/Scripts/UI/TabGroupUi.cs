using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupUi : MonoBehaviour
{
    [SerializeField] protected bool iniAtStart = true;
    protected TabSingleElementUi[] singleTabs;

    void Start()
    {
        if (iniAtStart) InitializeMe();
    }

    public virtual void InitializeMe()
    {
        singleTabs = new TabSingleElementUi[transform.childCount];
        for (int i = 0; i < singleTabs.Length; i++)
        {
            singleTabs[i] = transform.GetChild(i).GetComponent<TabSingleElementUi>();
            int index = i;
            singleTabs[i].Init(() => TabClicked(index));
        }
    }

    protected virtual void TabClicked(int index)
    {
        for (int i = 0; i < singleTabs.Length; i++)
        {
            singleTabs[i].UnSelected();
        }

    }
}
