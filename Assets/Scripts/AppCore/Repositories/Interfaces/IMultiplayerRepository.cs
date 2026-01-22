using Cysharp.Threading.Tasks;

public interface IMultiplayerRepository
{
    UniTask<(bool Success, string DataAsText)> PostMultiplayerMatchAsync(MultiplayerMatchData multiplayerMatchData);
    UniTask<(bool Success, string DataAsText)> GetMultiplayerStatsAsync();
}
