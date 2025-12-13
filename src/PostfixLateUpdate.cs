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
    [HarmonyPatch(typeof(StationCargoTradePage), nameof(StationCargoTradePage.LateUpdate))]
    public static class PostfixLateUpdate
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        public static void Postfix(ref StationCargoTradePage __instance)
        {

            Faction faction = __instance._factions.Get(__instance._station.OwnerFactionId, true);
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return;
            }


            Faction global_faction = __instance._factions.Get(Plugin.global_currency_faction, true);

            int buyPrice = TradeSystem.GetBuyPrice(__instance._magnumProgression, __instance._factions, __instance._itemsPrices, __instance._station, __instance._tradeWallet);
            int num = __instance._tradeWallet.Sum((KeyValuePair<string, int> t) => t.Value);
            __instance._buyButton.SetInteractable(global_faction.PlayerTradePoints >= buyPrice && num > 0, true);
            __instance._exchangeView.RefreshValue(global_faction.PlayerTradePoints, -buyPrice);
        }

    }
}
