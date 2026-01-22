using Cysharp.Threading.Tasks;

public interface IPlayerStatsService
{
    UniTask<(bool Success, RequestErrorInfo ErrorInfo)> UpdateStatsAsync(PlayerStatsData playerStatsData);
    UniTask<PlayerStatsData> GetStatsAsync();
}
