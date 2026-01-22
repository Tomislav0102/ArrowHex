using System;
using Newtonsoft.Json;

[Serializable]
public class MultiplayerMatchData
{
    [JsonProperty("match_token")]           public string MatchToken { get; set; }
    [JsonProperty("player_2")]              public int PlayerOtherId { get; set; }
    [JsonProperty("player_1_score")]        public int PlayerUserScore { get; set; }
    [JsonProperty("player_2_score")]        public int PlayerOtherScore { get; set; }
    [JsonProperty("player_1_league")]       public int PlayerUserLeague { get; set; }
    [JsonProperty("player_2_league")]       public int PlayerOtherLeague { get; set; }
    [JsonProperty("player_next_league")]    public int PlayerUserUserNextLeague { get; set; }                
    [JsonProperty("bot_name")]              public string BotName { get; set; }

    public MultiplayerMatchData (string matchToken, int playerOtherId, int playerUserScore, int playerOtherScore, int playerUserLeague, int playerOtherLeague, int playerUserNextLeague, string botName)
    {
        MatchToken = matchToken;
        PlayerOtherId = playerOtherId;
        PlayerUserScore = playerUserScore;
        PlayerOtherScore = playerOtherScore;
        PlayerUserLeague = playerUserLeague;
        PlayerOtherLeague = playerOtherLeague;
        PlayerUserUserNextLeague = playerUserNextLeague;
        BotName = botName;
    }
}
