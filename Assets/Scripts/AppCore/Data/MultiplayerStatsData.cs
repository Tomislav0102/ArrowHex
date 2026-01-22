using Newtonsoft.Json;
using System;

[Serializable]
public class MultiplayerStatsData
{
    [JsonProperty("overall_matches")]           public int MatchesOverall { get; set; }
    [JsonProperty("overall_wins")]              public int WinsOverall { get; set; }
    [JsonProperty("current_league_matches")]    public int MatchesLeague { get; set; }
    [JsonProperty("current_league_wins")]       public int WinsLeague { get; set; }
    [JsonProperty("current_league")]            public int LeagueCurrent { get; set; }
    [JsonProperty("matches")]                   public MultiplayerStatsMatchData[] Matches { get; set; }

    public MultiplayerStatsData (int matchesOverall, int winsOverall, int matchesLeague, int winsLeague, int leagueCurrent, MultiplayerStatsMatchData[] matches)
    {
        MatchesOverall = matchesOverall;
        WinsOverall = winsOverall;
        MatchesLeague = matchesLeague;
        WinsLeague = winsLeague;
        LeagueCurrent = leagueCurrent;
        Matches = matches;
    }   
}

[Serializable]
public class MultiplayerStatsMatchData
{
    [JsonProperty("user_score")]        public string ScorePlayer { get; set; }
    [JsonProperty("user_league")]       public string LeaguePlayer { get; set; }
    [JsonProperty("opponent")]          public string OpponentName { get; set; }
    [JsonProperty("opponent_score")]    public string ScoreOpponent { get; set; }
    [JsonProperty("opponent_league")]   public string LeagueOpponent { get; set; }
    [JsonProperty("datetime")]          public string Datetime { get; set; }
}
