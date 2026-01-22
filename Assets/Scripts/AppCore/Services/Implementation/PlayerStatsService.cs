using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using UnityEngine;
using VContainer;

public class PlayerStatsService : BaseService, IPlayerStatsService
{
    [Inject] private IPlayerStatsRepository playerStatsRepository;

    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public PlayerStatsService() { }

    public async UniTask<(bool Success, RequestErrorInfo ErrorInfo)> UpdateStatsAsync(PlayerStatsData playerStatsData)
    {
        string serviceName = nameof(PlayerStatsService);
        string methodName = nameof(UpdateStatsAsync);

        Debug.Log($"{serviceName}::{methodName} data= {playerStatsData}");

        var (success, dataAsText) = await playerStatsRepository.UpdateStatsAsync(playerStatsData);
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

    public async UniTask<PlayerStatsData> GetStatsAsync()
    {
        return await DoRequest<PlayerStatsData>(
            request: playerStatsRepository.GetStatsAsync(),
            requestName: nameof(GetStatsAsync)
        );
    }
}
