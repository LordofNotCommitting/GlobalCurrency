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
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.BuildFactionReputationTooltip))]
    public static class PostfixBuildFactionReputationTooltip
    {

        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        //display should also work
        public static void Postfix(Faction faction, ref TooltipFactory __instance)
        {
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return;
            }
            Factions factions = __instance._state.Get<Factions>();
            Faction global_faction = factions.Get(Plugin.global_currency_faction, true);

            //append currency display value to global faction

            __instance.AddPanelToTooltip().Initialize(ItemPropertyType.Credit, "Global Currency: " + global_faction.PlayerTradePoints.ToString(), TooltipProperty.ComprasionType.None, null);
        }

    }
}
