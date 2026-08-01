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
    [HarmonyPatch(typeof(TradeSystem), nameof(TradeSystem.TryGetTradeShuttleBarterPriorityItems))]
    public static class OverrideTryGetTradeShuttleBarterPriorityItems
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        //Need both prefix and postfix on this one
        static int temp_faction_currency_beforetrade = 0;
        static bool trade_procced = false;


        static bool Disable_Min_Threshold = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Min_Threshold", false);

        public static bool Prefix(MagnumProgression magnumProgression, Faction faction, Station station, ItemsPrices itemsPrices, List<BasePickupItem> stationResult, TradeShuttleBarterContext barterContext, ref int availableCells, bool insertAtStart)
        {


            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return true;
            }
            else
            {
                //Plugin.Logger.Log("does buy not occur if no item?");
                // barterContext is null if 0 item is sold by trade shuttle....
                if (Disable_Min_Threshold && barterContext != null)
                {
                    barterContext.PriorityTradePointsBudget = OverrideExecuteTradeShuttleStationExchange.global_faction_var.PlayerTradePoints;
                }
                if (barterContext == null || barterContext.PriorityTradePointsBudget <= 0 || availableCells <= 0)
                {
                    return false;
                }
                int num = Mathf.Min(barterContext.PriorityTradePointsBudget, OverrideExecuteTradeShuttleStationExchange.global_faction_var.PlayerTradePoints);
                if (num <= 0)
                {
                    return false;
                }
                List<BasePickupItem> list = new List<BasePickupItem>();
                int num2 = num;
                TradeSystem.GetBestTradeShuttleItemsFromStation(magnumProgression, faction, station, itemsPrices, list, ref num, availableCells, barterContext.PriorityItemClasses);
                
                //Plugin.Logger.Log("you buy?" + station.Id);
                //Plugin.Logger.Log("buy what?" + list);

                int num3 = num2 - num;
                if (num3 <= 0)
                {
                    return false;
                }
                OverrideExecuteTradeShuttleStationExchange.global_faction_var.PlayerTradePoints -= num3;
                barterContext.PriorityTradePointsBudget -= num3;
                availableCells = Mathf.Max(0, availableCells - TradeSystem.GetItemsUsedCells(list));
                if (insertAtStart)
                {
                    stationResult.InsertRange(0, list);
                    return false;
                }
                stationResult.AddRange(list);
                return false;
            }


        }
    }
}
