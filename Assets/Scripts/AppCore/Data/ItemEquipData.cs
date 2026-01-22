using System;
using Newtonsoft.Json;

[Serializable]
public class ItemEquipData
{
    [JsonProperty("item_id")]                public int Id { get; set; }
    [JsonProperty("is_equipped")]            public bool IsEquipped { get; set; }

    public ItemEquipData(int itemId, bool isEquipped)
    {
        Id = itemId;
        IsEquipped = isEquipped;
    }
}
