using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LocalDataManager : MonoBehaviour, ILocalData
{
    [SerializeField] private string testUsername;
    [Space(10)]
    [SerializeField] private int testLevel;
    [SerializeField] private int testSublevel;
    [Space(10)]
    [SerializeField] private int testExperience;
    [SerializeField] private int testGold;
    [SerializeField] private int testDiamond;
    [Space(10)]
    [SerializeField] private string testMatchToken;
    [SerializeField] private int testPlayer2;
    [SerializeField] private int testPlayer1Score;
    [SerializeField] private int testPlayer2Score;
    [SerializeField] private int testPlayer1League;
    [SerializeField] private int testPlayer2League;
    [SerializeField] private int testNextLeague;
    [SerializeField] private string testBotName;
    [Space(10)]
    [SerializeField] private int testItemId;
    [SerializeField] private int testItemCostGold;
    [SerializeField] private int testItemCostDiamond;
    [Space(10)]
    [SerializeField] private bool testEquipped;
    
    private static readonly string managerName = nameof(LocalDataManager);
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    #region Testing
    [Button("Login")]
    public async UniTaskVoid TestLogin()
    {
        var (success, errorMessage) = await HandleLogin(testUsername);
        Debug.Log($"HandleLogin {success.ToString().ToUpper()} {errorMessage}");
    }
    
    [Button("Campaign")]
    public async UniTaskVoid TestCampaign()
    {
        var (success, errorMessage) = await HandleCampaignMatchCompleted(testLevel, testSublevel, testExperience, testGold, testDiamond);
        Debug.Log($"HandleCampaignMatchCompleted {success.ToString().ToUpper()} {errorMessage}");
    }
    
    [Button("Multiplayer")]
    public async UniTaskVoid TestMultiplayer()
    {
        var (success, errorMessage) = await HandleMultiplayerMatchCompleted(testMatchToken, testPlayer2, testPlayer1Score, testPlayer2Score, testPlayer1League, testPlayer2League, testNextLeague, testBotName, testExperience, testGold, testDiamond);
        Debug.Log($"HandleCampaignMatchCompleted {success.ToString().ToUpper()} {errorMessage}");
    }
    
    [Button("Buy Item")]
    public async UniTaskVoid TestBuyItem()
    {
        var (success, errorMessage) = await HandleBuyItem(testItemId, testItemCostGold, testItemCostDiamond);
        Debug.Log($"HandleBuyItem {success.ToString().ToUpper()} {errorMessage}");
    }    
    
    [Button("Equip Item")]
    public async UniTaskVoid TestEquipItem()
    {
        var (success, errorMessage) = await HandleEquipItem(testItemId, testEquipped);
        Debug.Log($"HandleEquipItem {success.ToString().ToUpper()} {errorMessage}");
    } 
    #endregion
    
    public async UniTask<(bool Success, string ErrorMessage)> HandleLogin(string username)
    {
        // local data
        LocalUserData.Username = username;
        
        // backend login
        var (success, errorMessage) = await Launch.Instance.apiClientManager.PostLoginAsync(username);
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::PostLoginAsync success={success.ToString().ToUpper()}");
        if (!success)
        {
            return (false, errorMessage);
        }

        // backend user data
        (success, errorMessage) = await HandleUserData();
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::GetUserDataAsync success={success.ToString().ToUpper()}");
        return (success, errorMessage);
    }

    public async UniTask<(bool Success, string ErrorMessage)> HandleCampaignMatchCompleted(int level, int sublevel, int experienceGained, int currencyGoldGained, int currencyDiamondGained)
    {
        // local data
        LocalUserData.UpdateCampaignData(level, sublevel);
        LocalUserData.UpdatePlayerStatsMatchCompleted(experienceGained, currencyGoldGained, currencyDiamondGained);
        
        // backend campaign
        var (success, errorMessage) = await Launch.Instance.apiClientManager.PostCampaignAsync(level, sublevel);
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::PostCampaignAsync success={success.ToString().ToUpper()}");
        if (!success)
        {
            return (false, errorMessage);
        }
            
        // backend player stats
        (success, errorMessage) = await Launch.Instance.apiClientManager.UpdatePlayerStatsAsync(LocalUserData.Experience, LocalUserData.CurrencyGold, LocalUserData.CurrencyDiamond);
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::UpdatePlayerStatsAsync success={success.ToString().ToUpper()}");
        return (success, errorMessage);
    }

    public async UniTask<(bool Success, string ErrorMessage)> HandleMultiplayerMatchCompleted(string matchToken, int playerOtherId, int playerUserScore, int playerOtherScore, 
                                                                                                int playerUserLeague, int playerOtherLeague, int playerUserNextLeague, string botName,
                                                                                                int experienceGained, int currencyGoldGained, int currencyDiamondGained)
    {
        // local data
        LocalUserData.CacheMultiplayerMatchData(matchToken, playerOtherId, playerUserScore, playerOtherScore, playerUserLeague, playerOtherLeague, playerUserNextLeague, botName);
        LocalUserData.UpdateMultiplayerMatchData(matchToken, playerOtherId, playerUserScore, playerOtherScore, playerUserLeague, playerOtherLeague, playerUserNextLeague, botName);
        LocalUserData.UpdatePlayerStatsMatchCompleted(experienceGained, currencyGoldGained, currencyDiamondGained);
        
        // backend multiplayer
        var (success, errorMessage) = await Launch.Instance.apiClientManager.PostMultiplayerMatchAsync(matchToken, playerOtherId, playerUserScore, playerOtherScore, playerUserLeague, playerOtherLeague, playerUserNextLeague, botName);            // add sublevel
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::PostMultiplayerMatchAsync success={success.ToString().ToUpper()}");
        if (!success)
        {
            return (false, errorMessage);
        }

        // backend player stats
        (success, errorMessage) = await Launch.Instance.apiClientManager.UpdatePlayerStatsAsync(LocalUserData.Experience, LocalUserData.CurrencyGold, LocalUserData.CurrencyDiamond);
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::UpdatePlayerStatsAsync success={success.ToString().ToUpper()}");
        return (success, errorMessage);
    }

    public async UniTask<(bool Success, string ErrorMessage)> HandleBuyItem(int itemId, int itemPriceGold, int itemPriceDiamond)
    {
        // local data
        LocalUserData.UpdateBoughtItems(itemId);
        LocalUserData.UpdatePlayerStatsItemBought(itemPriceGold, itemPriceDiamond);

        // backend buy item
        var (success, errorMessage) = await Launch.Instance.apiClientManager.PostItemBuyAsync(itemId, itemPriceGold, itemPriceDiamond);
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::PostItemBuyAsync success={success.ToString().ToUpper()}");
        return (success, errorMessage);
    }
    
    public async UniTask<(bool Success, string ErrorMessage)> HandleEquipItem(int itemId, bool isEquipped)
    {
        // local data
        LocalUserData.UpdateUsedItems(itemId, isEquipped);

        // backend equip item
        var (success, errorMessage) = await Launch.Instance.apiClientManager.UpdateItemEquippedAsync(itemId, isEquipped);
        if (Launch.Instance.showDebugBackend) Debug.Log($"{managerName}::UpdateItemEquippedAsync success={success.ToString().ToUpper()}");
        return (success, errorMessage);
    }

    private async UniTask<(bool Success, string ErrorMessage)> HandleUserData()
    {
        ComprehensiveUserData userData = await Launch.Instance.apiClientManager.GetUserDataAsync();
        if (userData != null)
        {
            LocalUserData.SetUserData(userData);
            return (true, "");
        }
        else
        {
            return (false, "Error fetching player data.");
        }
    }
}
