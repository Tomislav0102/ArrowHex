using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using UnityEngine;
using VContainer;

public class MultiplayerService : BaseService, IMultiplayerService
{
    [Inject] private IMultiplayerRepository multiplayerRepository;

    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public MultiplayerService() { }

    public async UniTask<(bool Success, RequestErrorInfo ErrorInfo)> PostMultiplayerMatchAsync(MultiplayerMatchData multiplayerMatchData)
    {
        string serviceName = nameof(MultiplayerService);
        string methodName = nameof(PostMultiplayerMatchAsync);

        Debug.Log($"{serviceName}::{methodName} data= {multiplayerMatchData}");

        var (success, dataAsText) = await multiplayerRepository.PostMultiplayerMatchAsync(multiplayerMatchData);
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

    public async UniTask<MultiplayerStatsData> GetMultiplayerStatsAsync()
    {
        return await DoRequest<MultiplayerStatsData>(
            request: multiplayerRepository.GetMultiplayerStatsAsync(),
            requestName: nameof(GetMultiplayerStatsAsync)
        );
    }
}
