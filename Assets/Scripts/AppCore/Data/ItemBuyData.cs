using System;
using Newtonsoft.Json;

[Serializable]
public class ItemBuyData
{
    [JsonProperty("item_id")]                public int Id { get; set; }
    [JsonProperty("cost_gold")]              public int CostGold { get; set; }
    [JsonProperty("cost_diamond")]           public int CostDiamond { get; set; }

    public ItemBuyData(int itemId, int costGold, int costDiamond)
    {
        Id = itemId;
        CostGold = costGold;
        CostDiamond = costDiamond;
    }
}
