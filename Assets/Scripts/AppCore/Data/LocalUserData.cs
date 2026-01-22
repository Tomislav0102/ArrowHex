using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LocalUserData
{
    public static ApiAccessToken Token { get; set; }
    public static string Username { get; set; }
    public static int Id { get; private set; }
    public static int Experience { get; private set; }
    public static int CurrencyGold { get; private set; }
    public static int CurrencyDiamond { get; private set; }
    public static ProgressData SinglePlayerCampaignData { get; private set; }
    public static MultiplayerMatchData MultiplayerCurrentMatch { get; private set; }   // currently not being used
    public static MultiplayerStatsData MultiplayerStatsData { get; private set; }
    public static int[] ItemsPurchased { get; private set; }
    public static int[] ItemsEquipped { get; private set; }

    #region Inventory items
    internal static List<int> GetPurchasedItems(CloudData itemType)
    {
        return itemType switch
        {
            
            CloudData.Head => GetPurchasedItemsByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderHeads),
            CloudData.Gloves => GetPurchasedItemsByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderGloves),
            CloudData.Bow => GetPurchasedItemsByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderBows),
            CloudData.Arrow => GetPurchasedItemsByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderArrows),
            CloudData.Environment => GetPurchasedItemsByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderEnvironments),
            _ => new List<int>()
        };
    }
    
    internal static int GetEquippedItem(CloudData itemType)
    {
        return itemType switch
        {
            CloudData.Head => GetEquippedItemByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderHeads),
            CloudData.Gloves => GetEquippedItemByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderGloves),
            CloudData.Bow => GetEquippedItemByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderBows),
            CloudData.Arrow => GetEquippedItemByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderArrows),
            CloudData.Environment => GetEquippedItemByBracketOrder(Launch.Instance.persistentDataManager.gameData.ItemTypeBracketOrderEnvironments),
            _ => -1
        };
    }
    
    private static List<int> GetPurchasedItemsByBracketOrder(int itemTypeBracketOrder)
    {
        return ItemsPurchased.Where(purchasedItemId => purchasedItemId / Launch.Instance.persistentDataManager.gameData.ItemTypeBracketSize == itemTypeBracketOrder).ToList();
    }

    private static int GetEquippedItemByBracketOrder(int itemTypeBracketOrder)
    {
        foreach (int itemEquippedId in ItemsEquipped) 
        {
            if (itemEquippedId / Launch.Instance.persistentDataManager.gameData.ItemTypeBracketSize == itemTypeBracketOrder)
            {
                return itemEquippedId;
            }
        }
        return -1;
    }
    #endregion
    
    internal static void SetUserData(ComprehensiveUserData userData)
    {
        Id = userData.UserId;
        Experience = userData.Experience;
        CurrencyGold = userData.CurrencyGold;
        CurrencyDiamond = userData.CurrencyDiamond;

        SinglePlayerCampaignData = new ProgressData(userData.CompletedLevels);
        MultiplayerStatsData = new MultiplayerStatsData(userData.MatchesOverall, userData.WinsOverall, userData.MatchesLeague, 
                                                        userData.WinsLeague, userData.LeagueCurrent, userData.Matches);
        ItemsPurchased = userData.ItemsPurchased;
        ItemsEquipped = userData.ItemsEquipped;
    }

    internal static void CacheMultiplayerMatchData(string matchToken, int playerOtherId, int playerUserScore, 
        int playerOtherScore, int playerUserLeague, int playerOtherLeague, int playerUserNextLeague, string botName)
    {
        MultiplayerCurrentMatch = new MultiplayerMatchData(matchToken, playerOtherId, playerUserScore, playerOtherScore,  playerUserLeague, playerOtherLeague, playerUserNextLeague, botName);
    }

    internal static void UpdateMultiplayerMatchData(string matchToken, int playerOtherId, int playerUserScore,
        int playerOtherScore, int playerUserLeague, int playerOtherLeague, int playerUserNextLeague, string botName)
    {
        // currently not being used
        // todo
    }

    internal static void UpdateCampaignData(int level, int sublevel)
    {
        int sublevelsCompleted = 0;
        if (sublevel != 0)
        {
            sublevelsCompleted = SinglePlayerCampaignData.CompletedLevels.Length + 1;
        }
        else
        {
            for (int i = 0; i < SinglePlayerCampaignData.CompletedLevels.Length; i++)
            {
                if (SinglePlayerCampaignData.CompletedLevels[i].Level != level)
                {
                    sublevelsCompleted++;
                }
                else
                {
                    SinglePlayerCampaignData.CompletedLevels[i] = null;
                }
            }
        }
        ProgressData newCampaignData = new ProgressData(sublevelsCompleted);
        int removedSublevelsCounter = 0;
        for (int i = 0; i < sublevelsCompleted; i++)
        {
            if (sublevel != 0 && i == sublevelsCompleted - 1)
            {
                // add last level & sublevel completed to the end of an array
                newCampaignData.CompletedLevels[sublevelsCompleted - 1] = new LevelData(level, sublevel);
                break;
            }
            
            LevelData levelData = null;
            bool sublevelFound = false;
            while (!sublevelFound)
            {
                if (i + removedSublevelsCounter >= SinglePlayerCampaignData.CompletedLevels.Length + 1)
                {
                    Debug.Log("SHOULD NOT HAPPEN - Sublevel not found!");
                    break;
                }
                if (SinglePlayerCampaignData.CompletedLevels[i + removedSublevelsCounter] != null)
                {
                    levelData = SinglePlayerCampaignData.CompletedLevels[i + removedSublevelsCounter];
                    sublevelFound = true;
                }
                else
                {
                    removedSublevelsCounter++;
                }
            }
            if (levelData == null)
            {
                Debug.Log($"SHOULD NOT HAPPEN - LevelData is NULL! i={i} removedSublevelsCounter={removedSublevelsCounter}");
            }
            newCampaignData.CompletedLevels[i] = levelData;
        }
        SinglePlayerCampaignData = newCampaignData;
    }

    internal static void UpdatePlayerStatsMatchCompleted(int experienceGained, int currencyGoldGained, int currencyDiamondGained)
    {
        Experience += experienceGained;
        CurrencyGold += currencyGoldGained;
        CurrencyDiamond += currencyDiamondGained;
    }
    
    public static void UpdatePlayerStatsItemBought(int itemPriceGold, int itemPriceDiamond)
    {
        CurrencyGold -= itemPriceGold;
        CurrencyDiamond -= itemPriceDiamond;
    }

    public static void UpdateBoughtItems(int itemId)
    {
        int itemsPurchasedCount = ItemsPurchased.Length + 1;
        int[] newItemsPurchased = new int[itemsPurchasedCount];
        for (int i = 0; i < ItemsPurchased.Length; i++)
        {
            newItemsPurchased[i] = ItemsPurchased[i];
        }
        newItemsPurchased[itemsPurchasedCount - 1] = itemId;
        ItemsPurchased = newItemsPurchased;
    }

    public static void UpdateUsedItems(int itemId, bool isEquipped)
    {
        int itemsEquipedCount = ItemsEquipped.Length + (isEquipped ? 1 : -1);
        int[] newItemsEquipped = new int[itemsEquipedCount];

        if (isEquipped)
        {
            for (int i = 0; i < ItemsEquipped.Length; i++)
            {
                newItemsEquipped[i] = ItemsEquipped[i];
            }
            newItemsEquipped[itemsEquipedCount - 1] = itemId;
        }
        else
        {
            for (int i = 0; i < ItemsEquipped.Length; i++)
            {
                if (ItemsEquipped[i] != itemId)
                {
                    newItemsEquipped[i] = ItemsEquipped[i];
                    continue;
                }

                for (int j = i + 1; j < itemsEquipedCount; j++)
                {
                    newItemsEquipped[j - 1] = ItemsEquipped[j]; 
                }
                break;
            }
        }
        ItemsEquipped = newItemsEquipped;
    }
}
