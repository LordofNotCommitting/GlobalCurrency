using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using static System.Collections.Specialized.BitVector32;

namespace GlobalCurrency
{
    [HarmonyPatch(typeof(TradeSystem), nameof(TradeSystem.SellItems))]
    public static class OverrideSellItems
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        //need to override this one as well since EXCHANGE is a thing that uses ref.... damn.

        public static bool Prefix(MagnumProgression magnumProgression, ref Factions factions, ItemsPrices itemsPrices, Statistics statistics, Station station, ref List<BasePickupItem> result, ItemStorage inputStorage, Difficulty difficulty, bool exchangeItems, int bonusPoints, bool exchangeQuestItem)
        {
            Faction global_faction = factions.Get(Plugin.global_currency_faction, true);
            Faction faction = factions.Get(station.OwnerFactionId, true);

            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return true;
            }

            exchangeQuestItem = false;
            result.Clear();
            List<BasePickupItem> list = new List<BasePickupItem>();
            foreach (BasePickupItem basePickupItem in inputStorage.Items)
            {
                if (basePickupItem.Id.Equals(Data.Global.TutorialAncomQuestItemId))
                {
                    exchangeQuestItem = true;
                    list.Add(basePickupItem);
                }
                else if (TradeSystem.IsValidItem(faction, station, basePickupItem.Id))
                {
                    int num = Mathf.RoundToInt((float)(TradeSystem.GetItemSellPrice(magnumProgression, faction, station, itemsPrices, basePickupItem.Id, false) * (int)basePickupItem.StackCount) * difficulty.Preset.BarterValue);
                    global_faction.PlayerTradePoints += num;
                    faction.AllTimeTradingPoints += num;
                    statistics.IncreaseStatistic(StatisticType.TotalIncome, num);
                    list.Add(basePickupItem);
                }
            }
            if (exchangeItems)
            {
                global_faction.PlayerTradePoints += bonusPoints;
                TradeSystem.GetRandomItemsFromStation(magnumProgression, faction, station, itemsPrices, result, ref global_faction.PlayerTradePoints);
            }
            foreach (BasePickupItem basePickupItem2 in list)
            {
                inputStorage.Remove(basePickupItem2, true);
                if (basePickupItem2.Id != Data.Global.TutorialAncomQuestItemId)
                {
                    station.InternalStorage.AddItemAndReshuffleOptional(basePickupItem2);
                    faction.StratProgData.ReceiveItem(basePickupItem2);
                }
            }
            if (exchangeQuestItem)
            {
                List<string> list2 = CollectionPool<List<string>, string>.Get();
                list2.AddRange(Data.Global.TutorialAncomRewardItems);
                foreach (AnComDataRewardRecord anComDataRewardRecord in Data.AnComDataRewards)
                {
                    if (anComDataRewardRecord.Id == faction.Id)
                    {
                        list2.Clear();
                        list2.AddRange(anComDataRewardRecord.Items);
                        break;
                    }
                }
                foreach (string itemId in list2)
                {
                    BasePickupItem basePickupItem3 = SingletonMonoBehaviour<ItemFactory>.Instance.CreateForInventory(itemId, false);
                    basePickupItem3.ExaminedItem = false;
                    result.Add(basePickupItem3);
                }
                CollectionPool<List<string>, string>.Release(list2);
            }
            StationSystem.RefreshConsumablesPrices(station, itemsPrices);
            return false;
        }

    }
}
