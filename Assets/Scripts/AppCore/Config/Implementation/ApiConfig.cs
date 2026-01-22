using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Represents the backend API configuration.
/// Used as <see cref="ScriptableObject"/> to make it easier to change paths, urls etc.
/// </summary>
[CreateAssetMenu(
    fileName = nameof(ApiConfig)
    //menuName = TarMenuNames.Config + nameof(ApiConfig)
)]

public class ApiConfig : ScriptableObject, IApiConfig
{
    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Base")]
    [SerializeField] private string rootPath = "https://arrowhex-ane0ezdpbkc0erbk.westeurope-01.azurewebsites.net/api/v1/prod";

    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Auth")]
    [SerializeField] private string loginPath = "user/login/";

    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Auth")]
    [SerializeField] private string refreshPath = "user/token/refresh/";
    
    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Player")]
    [SerializeField] private string userDataPath = "user/data/";
    
    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Player")]
    [SerializeField] private string playerStatsPath = "campaign/stats/";
    
    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Inventory")]
    [SerializeField] private string itemBuyPath = "campaign/item/buy/";
    
    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Inventory")]
    [SerializeField] private string itemEquipPath = "campaign/item/equip/";

    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Campaign")]
    [SerializeField] private string campaignPath = "campaign/progress/";

    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Multiplayer")]
    [SerializeField] private string multiplayerMatchPath = "multiplayer/match/";

    [BoxGroup(FoldoutGroupNames.SetInInspector)]
    [TitleGroup(FoldoutGroupNames.SetInInspector + "/Multiplayer")]
    [SerializeField] private string multiplayerStatsPath = "multiplayer/stats/";

    public string LoginUrl => $"{rootPath}/{loginPath}";
    public string RefreshUrl => $"{rootPath}/{refreshPath}";
    public string UserDataUrl => $"{rootPath}/{userDataPath}";
    public string CampaignUrl => $"{rootPath}/{campaignPath}";
    public string PlayerStatsUrl => $"{rootPath}/{playerStatsPath}";
    public string MultiplayerMatchUrl => $"{rootPath}/{multiplayerMatchPath}";
    public string MultiplayerStatsUrl => $"{rootPath}/{multiplayerStatsPath}";
    public string ItemBuyUrl => $"{rootPath}/{itemBuyPath}";
    public string ItemEquipUrl => $"{rootPath}/{itemEquipPath}";
}
