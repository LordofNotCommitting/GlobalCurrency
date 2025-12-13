using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace GlobalCurrency
{
    [HarmonyPatch(typeof(TradeSystem), nameof(TradeSystem.BuyStationItems))]
    public static class OverrideBuyStationItems
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        //Need both prefix and postfix on this one
        static int temp_buy_price;
        public static bool Prefix(MagnumProgression magnumProgression, Factions factions, ItemsPrices itemsPrices, Statistics statistics, Station station, Dictionary<string, int> itemsToBuy, ref List<BasePickupItem> __result)
        {

            Faction faction = factions.Get(station.OwnerFactionId, true);
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return true;
            }
            Faction global_faction = factions.Get(Plugin.global_currency_faction, true);
            List<BasePickupItem> list = new List<BasePickupItem>();
            if (itemsToBuy.Count == 0)
            {
                __result = list;
                return false;
            }
            //use global currency faction here to eliminate any issues.
            // since faction.PlayerTradePoints COULD play the role.
            int buyPrice = TradeSystem.GetBuyPrice(magnumProgression, factions, itemsPrices, station, itemsToBuy);
            if (global_faction.PlayerTradePoints < buyPrice)
            {
                __result = list;
                return false;
            }
            global_faction.PlayerTradePoints -= buyPrice;
            faction.AllTimeTradingPoints += buyPrice;
            statistics.IncreaseStatistic(StatisticType.TotalIncome, buyPrice);
            foreach (KeyValuePair<string, int> keyValuePair in itemsToBuy)
            {
                station.InternalStorage.RemoveSpecificItem(keyValuePair.Key, (short)keyValuePair.Value);
            }
            StationSystem.RefreshConsumablesPrices(station, itemsPrices);
            foreach (KeyValuePair<string, int> keyValuePair2 in itemsToBuy)
            {
                ItemInteractionSystem.CreateItem(list, keyValuePair2.Key, keyValuePair2.Value);
            }
            foreach (BasePickupItem basePickupItem in list)
            {
                basePickupItem.ExaminedItem = false;
            }

            __result = list;
            return false;
        }

    }
}
