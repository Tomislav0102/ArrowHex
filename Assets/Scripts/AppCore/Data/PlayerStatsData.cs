using System;
using Newtonsoft.Json;

[Serializable]
public class PlayerStatsData
{
    [JsonProperty("experience")]        public int Experience { get; set; }
    [JsonProperty("currency_gold")]     public int CurrencyGold { get; set; }
    [JsonProperty("currency_diamond")]  public int CurrencyDiamond { get; set; }

    public PlayerStatsData (int experience, int currency_gold, int currency_diamond)
    {
        Experience = experience;
        CurrencyGold = currency_gold;
        CurrencyDiamond = currency_diamond;
    }  
}
