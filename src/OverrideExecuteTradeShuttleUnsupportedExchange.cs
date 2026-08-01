using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;


namespace GlobalCurrency
{
    [HarmonyPatch(typeof(TradeSystem), nameof(TradeSystem.ExecuteTradeShuttleUnsupportedExchange))]
    public static class OverrideExecuteTradeShuttleUnsupportedExchange
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        //Need both prefix and postfix on this one
        static int temp_faction_currency_beforetrade = 0;
        static bool trade_procced = false;


        public static bool Prefix(MagnumProgression magnumProgression, Factions factions, ItemsPrices itemsPrices, Statistics statistics, Difficulty difficulty, Station station, ItemStorage inputStorage, List<BasePickupItem> accumulatedResult, List<BasePickupItem> stationResult, TradeShuttleMode mode, TradeShuttleBarterContext barterContext, TradeShuttleExecutionAudit audit, List<BasePickupItem> soldItems)
        {
            //Plugin.Logger.Log("beginning shuttle exchang" + temp_faction_currency_beforetrade);
            Faction faction = factions.Get(station.OwnerFactionId, true);
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return true;
            }
            else
            {

                //Plugin.Logger.Log("Trade?");
                if (!trade_procced)
                {
                    temp_faction_currency_beforetrade = faction.PlayerTradePoints;

                    //Plugin.Logger.Log("Trading??" + temp_faction_currency_beforetrade + station.OwnerFactionId);
                    trade_procced = true;
                }
                return true;
            }
        }


        public static void Postfix(MagnumProgression magnumProgression, Factions factions, ItemsPrices itemsPrices, Statistics statistics, Difficulty difficulty, Station station, ItemStorage inputStorage, List<BasePickupItem> accumulatedResult, List<BasePickupItem> stationResult, TradeShuttleMode mode, TradeShuttleBarterContext barterContext, TradeShuttleExecutionAudit audit, List<BasePickupItem> soldItems)
        {

            Faction faction = factions.Get(station.OwnerFactionId, true);
            //Plugin.Logger.Log("after shuttle exchang" + faction.PlayerTradePoints);
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return;
            }
            else
            {
                if (trade_procced)
                {

                    //Plugin.Logger.Log("Trade det?" + faction.PlayerTradePoints + station.OwnerFactionId);
                    //Plugin.Logger.Log("Trade FR?" + temp_faction_currency_beforetrade);
                    int trade_profit_delta = Math.Max(faction.PlayerTradePoints - temp_faction_currency_beforetrade, 0);

                    //Plugin.Logger.Log("Trade FR?" + trade_profit_delta);
                    Faction global_faction = factions.Get(Plugin.global_currency_faction, true);
                    global_faction.PlayerTradePoints += trade_profit_delta;
                    faction.PlayerTradePoints = temp_faction_currency_beforetrade;
                    trade_procced = false;
                }
                return;
            }
        }
    }
}
