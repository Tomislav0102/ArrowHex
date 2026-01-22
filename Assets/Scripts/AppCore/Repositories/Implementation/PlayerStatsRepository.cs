using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerStatsRepository : RepositoryBase, IPlayerStatsRepository
{
    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public PlayerStatsRepository() { }

    public async UniTask<(bool Success, string DataAsText)> UpdateStatsAsync(PlayerStatsData playerStatsData)
    {
        string repositoryName = nameof(PlayerStatsRepository);
        string methodName = nameof(UpdateStatsAsync);

        #region Request
        string payload = JsonConvert.SerializeObject(playerStatsData);
        var request = CreatePutRequest(payload, apiConfig.PlayerStatsUrl, true, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }

    public async UniTask<(bool Success, string DataAsText)> GetStatsAsync()
    {
        string repositoryName = nameof(PlayerStatsRepository);
        string methodName = nameof(GetStatsAsync);

        #region Request
        var request = CreateGetRequest(apiConfig.PlayerStatsUrl, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }
}
