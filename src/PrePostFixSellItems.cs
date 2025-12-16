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
    public static class PrePostFixSellItems
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);
        static int total_trade_point;
        static bool internal_disable;
        static bool internal_exchange_flag;

        //need to override this one as well since EXCHANGE is a thing that uses ref.... damn.

        public static bool Prefix(MagnumProgression magnumProgression, ref Factions factions, ItemsPrices itemsPrices, Statistics statistics, Station station, ref List<BasePickupItem> result, ItemStorage inputStorage, Difficulty difficulty, ref bool exchangeItems, int bonusPoints, bool exchangeQuestItem)
        {
            total_trade_point = 0;
            internal_disable = false;
            internal_exchange_flag = false;
            Faction global_faction = factions.Get(Plugin.global_currency_faction, true);
            Faction faction = factions.Get(station.OwnerFactionId, true);

            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                internal_disable = true;

            }
            if (internal_disable)
            {
                return true;
            }

            foreach (BasePickupItem basePickupItem in inputStorage.Items)
            {
                if (!basePickupItem.Id.Equals(Data.Global.TutorialAncomQuestItemId) && TradeSystem.IsValidItem(faction, station, basePickupItem.Id))
                {
                    int num = Mathf.RoundToInt((float)(TradeSystem.GetItemSellPrice(magnumProgression, faction, station, itemsPrices, basePickupItem.Id, false) * (int)basePickupItem.StackCount) * difficulty.Preset.BarterValue);
                    total_trade_point += num;
                }
            }
            if (exchangeItems)
            {
                exchangeItems = false;
                internal_exchange_flag = true;
            }
            return true;

        }

        public static void Postfix(MagnumProgression magnumProgression, ref Factions factions, ItemsPrices itemsPrices, Statistics statistics, Station station, ref List<BasePickupItem> result, ItemStorage inputStorage, Difficulty difficulty, ref bool exchangeItems, int bonusPoints, bool exchangeQuestItem)
        {

            Faction global_faction = factions.Get(Plugin.global_currency_faction, true);
            Faction faction = factions.Get(station.OwnerFactionId, true);

            //recalculate factional trade val
            global_faction.PlayerTradePoints += total_trade_point;
            faction.PlayerTradePoints -= total_trade_point;

            if (internal_exchange_flag)
            {
                global_faction.PlayerTradePoints += bonusPoints;
                internal_exchange_flag = false;
                exchangeItems = true;
                TradeSystem.GetRandomItemsFromStation(magnumProgression, faction, station, itemsPrices, result, ref global_faction.PlayerTradePoints);
            }

        }
    }
}
