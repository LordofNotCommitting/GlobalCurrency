using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GlobalCurrency
{
    [HarmonyPatch(typeof(FactionSystem), nameof(FactionSystem.WipeTradeRelations))]
    public static class DisableWipeTradeRelations
    {
        //no more trade break baby

        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        public static bool Prefix(Stations stations, Faction faction)
        {
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance)) {
                return true;
            }
            return false;
        }
    }
}
