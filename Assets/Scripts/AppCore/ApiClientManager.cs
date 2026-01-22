using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

public class ApiClientManager : MonoBehaviour, IApiClient
{
    [Inject] private IAuthService authService;
    [Inject] private ICampaignService campaignService;
    [Inject] private IPlayerStatsService playerStatsService;
    [Inject] private IMultiplayerService multiplayerService;
    [Inject] private IInventoryService inventoryService;
    [Inject] private ApiAccessToken apiAccessToken;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    #region API - Auth
    public async UniTask<(bool Success, string ErrorMessage)> PostLoginAsync(string username)
    {
        LoginModel loginModel = new LoginModel(username);
        var (success, errorInfo) = await authService.LoginAsync(loginModel);
        return (success, errorInfo.DisplayMessage);
    }

    public async UniTask<ComprehensiveUserData> GetUserDataAsync()
    {
        return await authService.GetUserDataAsync();
    }
    #endregion

    #region API - Campaign
    public async UniTask<(bool Success, string ErrorMessage)> PostCampaignAsync(int levelCompleted, int subLevelCompleted)
    {
        CampaignData campaignData = new CampaignData(levelCompleted, subLevelCompleted);
        var (success, errorInfo) = await campaignService.PostProgressAsync(campaignData);
        return (success, errorInfo.DisplayMessage);
    }

    public async UniTask<ProgressData> GetCampaignAsync()
    {
        return await campaignService.GetProgressAsync();
    }
    #endregion

    #region API - Player Stats
    public async UniTask<(bool Success, string ErrorMessage)> UpdatePlayerStatsAsync(int playerExperience, int playerCurrencyGold, int playerCurrencyDiamond)
    {
        PlayerStatsData playerStatsData = new PlayerStatsData(playerExperience, playerCurrencyGold, playerCurrencyDiamond);
        var (success, errorInfo) = await playerStatsService.UpdateStatsAsync(playerStatsData);
        return (success, errorInfo.DisplayMessage);
    }

    public async UniTask<PlayerStatsData> GetPlayerStatsAsync()
    {
        return await playerStatsService.GetStatsAsync();
    }
    #endregion

    #region API - Multiplayer
    public async UniTask<(bool Success, string ErrorMessage)> PostMultiplayerMatchAsync(string gameToken, int enemyId, int myScore, int enemyScore, int myLeague, int enemyLeague, int myNextLeague, string botName)
    {
        MultiplayerMatchData multiplayerMatchData = new MultiplayerMatchData(gameToken, enemyId, myScore, enemyScore, myLeague, enemyLeague, myNextLeague, botName);
        var (success, errorInfo) = await multiplayerService.PostMultiplayerMatchAsync(multiplayerMatchData);
        return (success, errorInfo.DisplayMessage);
    }

    public async UniTask<MultiplayerStatsData> GetMultiplayerStatsAsync()
    {
        return await multiplayerService.GetMultiplayerStatsAsync();
    }
    #endregion

    #region API - Inventory
    public async UniTask<(bool Success, string ErrorMessage)> PostItemBuyAsync(int itemId, int itemPriceGold, int itemPriceDiamond) 
	{
        ItemBuyData itemBuyData = new ItemBuyData(itemId, itemPriceGold, itemPriceDiamond);
        var (success, errorInfo) = await inventoryService.PostItemBuyAsync(itemBuyData);
        return (success, errorInfo.DisplayMessage);
	}
    
    public async UniTask<(bool Success, string ErrorMessage)> UpdateItemEquippedAsync(int itemId, bool isEquipped) 
    {
        ItemEquipData itemBuyData = new ItemEquipData(itemId, isEquipped);
        var (success, errorInfo) = await inventoryService.UpdateItemEquipAsync(itemBuyData);
        return (success, errorInfo.DisplayMessage);
    }
    #endregion
}
