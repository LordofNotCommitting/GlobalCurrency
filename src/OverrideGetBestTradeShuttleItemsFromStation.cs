using HarmonyLib;
using MGSC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;


namespace GlobalCurrency
{
    [HarmonyPatch(typeof(TradeSystem), nameof(TradeSystem.GetBestTradeShuttleItemsFromStation))]
    public static class OverrideGetBestTradeShuttleItemsFromStation
    {
       
        static bool Disable_Trash_Exchange = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Trash_Exchange", false);

        public static bool Prefix(MagnumProgression magnumProgression, Faction faction, Station station, ItemsPrices itemsPrices, List<BasePickupItem> result, ref int totalPoints, int availableCells = 2147483647, HashSet<ItemClass> allowedItemClasses = null)
        {
            //if buying trash
            if (allowedItemClasses == null)
            {
                //and if buy trash option is disabled. do not buy trash.
                return !Disable_Trash_Exchange;
            }
            //otherwise go ahead and do your thing
            return true;
        }


    }
}
