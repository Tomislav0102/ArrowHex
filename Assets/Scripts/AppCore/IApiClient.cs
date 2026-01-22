using Cysharp.Threading.Tasks;

public interface IApiClient
{
    #region Authentication
    /// <summary>
    /// Logs in the user with their Meta ID to our 3rd party AWS backend.
    /// </summary>
    /// <param name="username">Oculus user id.</param>
    /// <returns>True if user logged in successfully, false with appropriate error message otherwise.</returns>
    public UniTask<(bool Success, string ErrorMessage)> PostLoginAsync(string username);
    
    public UniTask<ComprehensiveUserData> GetUserDataAsync();
    #endregion

    // -----------------------------------------------------------------------------------------------------------------------------------

    #region Single player campaign
    /// <summary>
    /// Saves single player campaign progress.
    /// </summary>
    /// <param name="levelCompleted">Level that the user just finished.</param>
    /// <returns>True if campaign progress has been saved successfully, false with appropriate error message otherwise.</returns>
    public UniTask<(bool Success, string ErrorMessage)> PostCampaignAsync(int levelCompleted, int subLevelCompleted);
    
    /// <summary>
    /// Fetches player's current progress on single player campaign.
    /// </summary>
    /// <returns>Data object that contains previously completed campaign levels.</returns>
    public UniTask<ProgressData> GetCampaignAsync();
    #endregion

    // -----------------------------------------------------------------------------------------------------------------------------------

    #region Experience, Gold, Diamonds
    /// <summary>
    /// Updates player's total experience earned, and the amount of gold and diamonds they currently have.
    /// </summary>
    /// <param name="playerExperience">Player's total experience earned.</param>
    /// <param name="playerCurrencyGold">Player's current amount of gold..</param>
    /// <param name="playerCurrencyDiamond">Player's current amount of diamonds.</param>
    /// <returns>True if player stats have been updated successfully, false with appropriate error message otherwise.</returns>
    public UniTask<(bool Success, string ErrorMessage)> UpdatePlayerStatsAsync(int playerExperience, int playerCurrencyGold, int playerCurrencyDiamond);

    /// <summary>
    /// Fetches player's current stats.
    /// </summary>
    /// <returns>Data object that contains total experience earned, and the amount of gold and diamonds they currently have.</returns>
    public UniTask<PlayerStatsData> GetPlayerStatsAsync();
    #endregion

    // -----------------------------------------------------------------------------------------------------------------------------------

    #region Multiplayer
    /// <summary>
    /// Saves a multiplayer match that player has just finished.
    /// </summary>
    /// <param name="gameToken">Match's indentifier for backend.</param>
    /// <param name="myScore">End game score of the player sending the request.</param>
    /// <param name="enemyScore">End game score of the other player.</param>
    /// <param name="myLeague">Current league of the player sending the request.</param>
    /// <param name="enemyLeague">Current league of the other player.</param>
    /// <param name="myNextLeague">Number of league player will be playing in in the next multiplayer match. (-1, 0, +1 for the current one)</param>
    /// <param name="botName">Name of the bot, if player was playing against bot until the end of the match.</param>
    /// <returns>True if the match has been saved successfully, false with appropriate error message otherwise.</returns>
    public UniTask<(bool Success, string ErrorMessage)> PostMultiplayerMatchAsync(string gameToken, int enemyId, int myScore, int enemyScore, int myLeague, int enemyLeague, int myNextLeague, string botName);

    /// <summary>
    /// Fetches player's stats of accomplishments and current state in multiplayer mode.
    /// </summary>
    /// <returns>Data object that contains info about multiplayer games the player has played, and status of current league.</returns>
    public UniTask<MultiplayerStatsData> GetMultiplayerStatsAsync();
    #endregion
    
    // -----------------------------------------------------------------------------------------------------------------------------------
    
    #region Inventory
    public UniTask<(bool Success, string ErrorMessage)> PostItemBuyAsync(int itemId, int itemPriceGold, int itemPriceDiamond);
    
    public UniTask<(bool Success, string ErrorMessage)> UpdateItemEquippedAsync(int itemId, bool isEquipped);
    #endregion
}
