using System;
using Newtonsoft.Json;

[Serializable]
public class ComprehensiveUserData
{
    [JsonProperty("user_id")]                   public int UserId { get; set; }
    [JsonProperty("completed")]                 public LevelData[] CompletedLevels { get; set; }
    [JsonProperty("experience")]                public int Experience { get; set; }
    [JsonProperty("currency_gold")]             public int CurrencyGold { get; set; }
    [JsonProperty("currency_diamond")]          public int CurrencyDiamond { get; set; }
    [JsonProperty("overall_matches")]           public int MatchesOverall { get; set; }
    [JsonProperty("overall_wins")]              public int WinsOverall { get; set; }
    [JsonProperty("current_league_matches")]    public int MatchesLeague { get; set; }
    [JsonProperty("current_league_wins")]       public int WinsLeague { get; set; }
    [JsonProperty("current_league")]            public int LeagueCurrent { get; set; }
    [JsonProperty("matches")]                   public MultiplayerStatsMatchData[] Matches { get; set; }
    [JsonProperty("purchased_items")]               public int[] ItemsPurchased { get; set; }
    [JsonProperty("currently_equipped_items")]      public int[] ItemsEquipped { get; set; }
}
