using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    PersistentDataManager _dm;
    [SerializeField] FakeButtonUi backFb, restartFb;
    [SerializeField] Button[] doneButtons;

    void Awake()
    {
        Instance = this;
        _dm = Launch.Instance.persistentDataManager;
    }

    void Start()
    {
        backFb.InitializeMe(BtnMethodExit);
        restartFb.InitializeMe(BtnMethodRestart);
        for (int i = 0; i < doneButtons.Length; i++)
        {
            doneButtons[i].onClick.AddListener(EndTutorial);
        }
    }

    void EndTutorial()
    {
        _dm.playerData.campProgressCurrent = CampaignManager.CampProgressFromIndex(1);
        int total = CampaignManager.IndexFromGetCampProgress(_dm.playerData.campProgressTotal);
        if (total < 0) _dm.playerData.campProgressTotal = CampaignManager.CampProgressFromIndex(0);
        BtnMethodExit();
    }

    #region BUTTONS/CALLS
    void BtnMethodExit()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        StartCoroutine(_dm.NewSceneAfterFadeIn("Back to main from tut", MainGameType.MainMenu));
    }

    void BtnMethodRestart()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        StartCoroutine(_dm.NewSceneAfterFadeIn("RestartSequence from tut", MainGameType.Tutorial));
    }
    #endregion

}
