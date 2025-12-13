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
    [HarmonyPatch(typeof(MissionSystem), nameof(MissionSystem.MissionFinishedByPlayer))]
    public static class PostfixMissionFinishedByPlayer
    {
        static bool Disable_Terrorist_GlobalCurrency_On = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Disable_Terrorist_GlobalCurrency_On", false);

        public static void Postfix(Missions missions, Stations stations, MagnumCargo magnumCargo, MagnumProgression magnumSpaceship, SpaceTime spaceTime, PopulationDebugData populationDebugData, TravelMetadata travelMetadata, ref Factions factions, ItemsPrices itemsPrices, Difficulty difficulty, Mission mission)
        {

            if (Disable_Terrorist_GlobalCurrency_On && !Plugin.legit_faction_alliance.Contains(factions.Get(mission.BeneficiaryFactionId, true).CurrentAlliance))
            {
                return;
            }

            int num = mission.RemainsRewardPoints;
            //undo local currency bonus
            factions.Get(mission.BeneficiaryFactionId, true).PlayerTradePoints -= num;
            //add currency value to global faction
            factions.Get(Plugin.global_currency_faction, true).PlayerTradePoints += num;

        }

    }
}
