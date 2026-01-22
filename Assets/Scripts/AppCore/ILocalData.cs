using Cysharp.Threading.Tasks;

public interface ILocalData
{
    public UniTask<(bool Success, string ErrorMessage)> HandleLogin(string username);
    public UniTask<(bool Success, string ErrorMessage)> HandleCampaignMatchCompleted(int level, int sublevel, 
        int experienceGained, int currencyGoldGained, int currencyDiamondGained);

    public UniTask<(bool Success, string ErrorMessage)> HandleMultiplayerMatchCompleted(string matchToken,
        int playerOtherId, int playerOtherScore, int playerUserScore,
        int playerOtherLeague, int playerUserLeague, int playerUserNextLeague, string botName,
        int experienceGained, int currencyGoldGained, int currencyDiamondGained);
}
