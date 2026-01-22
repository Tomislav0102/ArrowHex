using Cysharp.Threading.Tasks;

public interface IMultiplayerService
{
    UniTask<(bool Success, RequestErrorInfo ErrorInfo)> PostMultiplayerMatchAsync(MultiplayerMatchData multiplayerMatchData);
    UniTask<MultiplayerStatsData> GetMultiplayerStatsAsync();
}
