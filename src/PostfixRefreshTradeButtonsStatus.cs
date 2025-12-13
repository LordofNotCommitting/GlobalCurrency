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
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace GlobalCurrency
{
    [HarmonyPatch(typeof(StationExchangePage), nameof(StationExchangePage.RefreshTradeButtonsStatus))]
    public static class PostfixRefreshTradeButtonsStatus
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        public static void Postfix(ref StationExchangePage __instance)
        {

            Faction global_faction = __instance._factions.Get(Plugin.global_currency_faction, true);

            Faction faction = __instance._factions.Get(__instance._station.OwnerFactionId, true);
            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(faction.CurrentAlliance))
            {
                return;
            }


            bool flag = __instance._station.Stash.ContainsItem(Data.Global.TutorialAncomQuestItemId);

            ProxyCorpDepartment department = __instance._magnumProgression.GetDepartment<ProxyCorpDepartment>();
            int num = Mathf.RoundToInt((float)TradeSystem.GetSellPrice(__instance._magnumProgression, __instance._factions, __instance._itemsPrices, __instance._station, __instance._difficulty, true));
            bool flag2 = num > 0;
            if (department.ProxyFactionId == faction.Id)
            {
                num = 0;
            }
            __instance._exchangeView.RefreshValue(global_faction.PlayerTradePoints, num);
            if (__instance._canSell)
            {
                __instance._sellButton.gameObject.SetActive(true);
                __instance._exchangeButton.gameObject.SetActive(false);
                __instance._sellButton.SetInteractable(flag || num > 0 || (department.ProxyFactionId == faction.Id && flag2), true);
                return;
            }
            __instance._sellButton.gameObject.SetActive(false);
            __instance._exchangeButton.gameObject.SetActive(true);
            bool flag3 = flag || num > 0;
            int minItemPriceOnStation = TradeSystem.GetMinItemPriceOnStation(__instance._magnumProgression, __instance._itemsPrices, faction, __instance._station);
            bool flag4 = global_faction.PlayerTradePoints > minItemPriceOnStation && minItemPriceOnStation > 0;
            __instance._exchangeButton.SetInteractable(flag3 || flag4, true);
        }

    }
}
