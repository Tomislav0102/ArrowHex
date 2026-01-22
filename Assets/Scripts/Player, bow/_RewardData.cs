
public class RewardData
{
    public GenResult matchResult;
    public CampaignFinishState campaignFinishState;
    public int xpPerMinute, xpWin, xpScoreDiff, xpDaily, xpFirstWinOfDay, xpCampaign;
    public int coinPlay, coinWin, coinDaily, coinWinPlanet;
    public GenChange? leagueChange = null;

    public int TotalXp() => xpPerMinute + xpWin + xpScoreDiff + xpDaily + xpFirstWinOfDay + xpCampaign;
    public int TotalCoins() => coinPlay + coinWin + coinDaily + coinWinPlanet;
    
    public void NoRewards() => xpPerMinute = xpWin = xpScoreDiff = xpDaily = xpFirstWinOfDay = xpCampaign = coinPlay = coinWin = coinDaily = coinWinPlanet = 0;
}