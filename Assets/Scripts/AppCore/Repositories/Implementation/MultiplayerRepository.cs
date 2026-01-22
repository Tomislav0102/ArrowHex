using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class MultiplayerRepository : RepositoryBase, IMultiplayerRepository
{
    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public MultiplayerRepository() { }

    public async UniTask<(bool Success, string DataAsText)> PostMultiplayerMatchAsync(MultiplayerMatchData multiplayerMatchData)
    {
        string repositoryName = nameof(MultiplayerRepository);
        string methodName = nameof(PostMultiplayerMatchAsync);

        #region Request
        string payload = JsonConvert.SerializeObject(multiplayerMatchData);
        var request = CreatePostRequest(payload, apiConfig.MultiplayerMatchUrl, true, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }

    public async UniTask<(bool Success, string DataAsText)> GetMultiplayerStatsAsync()
    {
        string repositoryName = nameof(MultiplayerRepository);
        string methodName = nameof(GetMultiplayerStatsAsync);

        #region Request
        var request = CreateGetRequest(apiConfig.MultiplayerStatsUrl, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }
}
