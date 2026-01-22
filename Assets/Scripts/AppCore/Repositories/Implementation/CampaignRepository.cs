using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class CampaignRepository : RepositoryBase, ICampaignRepository
{
    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public CampaignRepository() { }

    public async UniTask<(bool Success, string DataAsText)> PostProgressAsync(CampaignData campaignData)
    {
        string repositoryName = nameof(CampaignRepository);
        string methodName = nameof(PostProgressAsync);

        #region Request
        string payload = JsonConvert.SerializeObject(campaignData);
        var request = CreatePostRequest(payload, apiConfig.CampaignUrl, true, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }

    public async UniTask<(bool Success, string DataAsText)> GetProgressAsync()
    {
        string repositoryName = nameof(CampaignRepository);
        string methodName = nameof(GetProgressAsync);

        #region Request
        var request = CreateGetRequest(apiConfig.CampaignUrl, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }
}
