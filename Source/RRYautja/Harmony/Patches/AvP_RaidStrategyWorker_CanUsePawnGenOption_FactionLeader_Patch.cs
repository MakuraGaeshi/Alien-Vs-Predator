using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using AvP.settings;
using AvP.ExtensionMethods;

namespace AvP.HarmonyInstance
{

    [HarmonyPatch(typeof(RaidStrategyWorker), "CanUsePawnGenOption")]
    public static class AvP_RaidStrategyWorker_CanUsePawnGenOption_FactionLeader_Patch
    {
        [HarmonyPostfix]
        public static void CanUse_factionLeader_Postfix(RaidStrategyWorker __instance, PawnGenOption g, List<PawnGenOption> chosenGroups, ref bool __result)
        {
            if (g.kind.factionLeader && __result)
            {
                if (chosenGroups.Any(x => x.kind.factionLeader && x.kind == g.kind))
                {
                    __result = false;
                    return;
                }
            }
            /*
            if (__instance.)
            {

            }
            */
        }
    }
}