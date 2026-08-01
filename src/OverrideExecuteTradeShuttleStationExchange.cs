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
    [HarmonyPatch(typeof(TradeSystem), nameof(TradeSystem.ExecuteTradeShuttleStationExchange))]
    public static class OverrideExecuteTradeShuttleStationExchange
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        public static Faction global_faction_var;

        public static bool Prefix(MagnumProgression magnumProgression, Factions factions, ItemsPrices itemsPrices, Statistics statistics, Difficulty difficulty, Station station, ItemStorage inputStorage, List<BasePickupItem> accumulatedResult, List<BasePickupItem> stationResult, TradeShuttleMode mode, TradeShuttleBarterContext barterContext, TradeShuttleExecutionAudit audit, List<BasePickupItem> soldItems)
        {
            Faction faction = factions.Get(station.OwnerFactionId, true);
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return true;
            }
            else {
                List<BasePickupItem> list = new List<BasePickupItem>();
                TradeShuttleExecutionAuditEntry tradeShuttleExecutionAuditEntry = (audit != null) ? audit.BeginStationExchange(station, faction, false) : null;
                stationResult.Clear();
                bool flag;

                Faction global_faction = factions.Get(Plugin.global_currency_faction, true);
                global_faction_var = global_faction;
                TradeSystem.CollectTradeShuttleSoldItems(magnumProgression, faction, station, itemsPrices, statistics, difficulty, mode, soldItems, list, tradeShuttleExecutionAuditEntry, out flag);
                if (tradeShuttleExecutionAuditEntry != null)
                {
                    tradeShuttleExecutionAuditEntry.SetSoldItems(list);
                }
                if (flag)
                {
                    TradeSystem.AddExchangeQuestRewardItems(faction, stationResult);
                }
                int tradeShuttleAvailableCells = TradeSystem.GetTradeShuttleAvailableCells(inputStorage, list, accumulatedResult, stationResult);
                
                //Plugin.Logger.Log("the fuck is happening" + tradeShuttleAvailableCells);

                if (tradeShuttleAvailableCells > 0)
                {
                    TradeSystem.TryGetTradeShuttleBarterPriorityItems(magnumProgression, faction, station, itemsPrices, stationResult, barterContext, ref tradeShuttleAvailableCells, true);
                    List<BasePickupItem> list2 = new List<BasePickupItem>();
                    TradeSystem.GetBestTradeShuttleItemsFromStation(magnumProgression, faction, station, itemsPrices, list2, ref global_faction.PlayerTradePoints, tradeShuttleAvailableCells, null);

                    stationResult.InsertRange(0, list2);
                }
                TradeSystem.MoveSoldItemsToStation(faction, station, inputStorage, list);
                StationSystem.RefreshConsumablesPrices(station, itemsPrices);
                if (tradeShuttleExecutionAuditEntry != null)
                {
                    tradeShuttleExecutionAuditEntry.Complete(stationResult, global_faction.PlayerTradePoints, faction.PlayerReputation);
                }
                return false;
            }

                
        }


    }
}
