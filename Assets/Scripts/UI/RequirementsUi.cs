using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RequirementsUi : MonoBehaviour
{
    [SerializeField]
    [BoxGroup("Requirements")]
    protected bool internetConnection;
    [SerializeField] 
    [BoxGroup("Requirements")]
    protected bool minimumLevel;
    [SerializeField] 
    [BoxGroup("Requirements")]
    [ShowIf(nameof(minimumLevel))]
    protected int minLevelValue = 5;

    protected bool CheckRequirements()
    {
        if (internetConnection)
        {
            if (InternetConnection.Instance.hasInternet) return true;
            
            Utils.Activation(MainMenuManager.Instance.noInternetWindow, GenActivation.On);
            return false;
        }
        if (minimumLevel)
        {
            PlayerRewards.CalculateLevelFromXp(out int lv, out _);
            if (lv >= minLevelValue) return true;
            
            Utils.Activation(MainMenuManager.Instance.requiredLevelWindow, GenActivation.On);
            return false;
        }

        return true;
    }
}
