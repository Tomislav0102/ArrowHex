using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using UnityEngine;
using VContainer;

public class CampaignService : BaseService, ICampaignService
{
    [Inject] private ICampaignRepository campaignRepository;

    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public CampaignService() { }

    public async UniTask<(bool Success, RequestErrorInfo ErrorInfo)> PostProgressAsync(CampaignData campaignData)
    {
        string serviceName = nameof(CampaignService);
        string methodName = nameof(PostProgressAsync);

        Debug.Log($"{serviceName}::{methodName} data= {campaignData}");

        var (success, dataAsText) = await campaignRepository.PostProgressAsync(campaignData);
        var errorInfo = RequestErrorInfo.GeneralError;
        try
        {
            errorInfo = serializationService.Deserialize<RequestErrorInfo>(dataAsText);
        }
        catch (Exception e)
        {
            // ignored
            Debug.LogWarning($"{serviceName}::{methodName} " +
                             $"parsing error info failed! {e.Message}"
            );
        }
        return (Success: success, ErrorInfo: errorInfo);
    }

    public async UniTask<ProgressData> GetProgressAsync()
    {
        return await DoRequest<ProgressData>(
            request: campaignRepository.GetProgressAsync(),
            requestName: nameof(GetProgressAsync)
        );
    }
}
