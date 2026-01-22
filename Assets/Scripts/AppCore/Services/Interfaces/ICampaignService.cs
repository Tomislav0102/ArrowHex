using Cysharp.Threading.Tasks;

/// <summary>
/// Interface for campaign services on backend.
/// </summary>
public interface ICampaignService
{
    UniTask<(bool Success, RequestErrorInfo ErrorInfo)> PostProgressAsync(CampaignData campaignData);
    UniTask<ProgressData> GetProgressAsync();
}
