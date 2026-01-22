using Cysharp.Threading.Tasks;

public interface IPlayerStatsRepository
{
    UniTask<(bool Success, string DataAsText)> UpdateStatsAsync(PlayerStatsData playerStatsData);
    UniTask<(bool Success, string DataAsText)> GetStatsAsync();
}
