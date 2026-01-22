using Cysharp.Threading.Tasks;

public interface ICampaignRepository
{
    UniTask<(bool Success, string DataAsText)> PostProgressAsync(CampaignData campaignData);
    UniTask<(bool Success, string DataAsText)> GetProgressAsync();
}
