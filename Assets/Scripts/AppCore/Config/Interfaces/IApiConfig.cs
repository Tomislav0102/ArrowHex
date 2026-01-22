/// <summary>
/// Interface for backend API configuration.
/// </summary>
public interface IApiConfig
{
    string LoginUrl { get; }
    string RefreshUrl { get; }
    string UserDataUrl { get; }
    string CampaignUrl { get; }
    string PlayerStatsUrl { get; }
    string MultiplayerMatchUrl { get; }
    string MultiplayerStatsUrl { get; }
    string ItemBuyUrl { get; }
    string ItemEquipUrl { get; }
}
