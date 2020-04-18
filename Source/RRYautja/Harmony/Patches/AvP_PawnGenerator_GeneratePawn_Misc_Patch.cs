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
using RRYautja.settings;
using RRYautja.ExtensionMethods;

namespace RRYautja.HarmonyInstance
{

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) })]
    public static class AvP_PawnGenerator_GeneratePawn_Misc_Patch
    {
        public static void Postfix(PawnGenerationRequest request, ref Pawn __result)
        {
            Rand.PushState();
            if (Rand.Chance(0.005f) && __result.isPotentialHost() && SettingsHelper.latest.AllowHiddenInfections)
            {
                HediffDef def = Rand.Chance(0.75f) || !SettingsHelper.latest.AllowNeomorphs ? XenomorphDefOf.AvP_HiddenXenomorphImpregnation : XenomorphDefOf.AvP_HiddenNeomorphImpregnation;
                __result.health.AddHediff(def, __result.RaceProps.body.corePart, null);
            }
            Rand.PopState();
            var hediffGiverSet = __result?.def?.race?.hediffGiverSets;
            if (hediffGiverSet == null) return;
            foreach (var item in hediffGiverSet)
            {
                var hediffGivers = item.hediffGivers;
                if (hediffGivers == null) return;
                if (hediffGivers.Any(y => y is HediffGiver_StartWithHediff))
                {
                    foreach (var hdg in hediffGivers.Where(x => x is HediffGiver_StartWithHediff))
                    {
                        HediffGiver_StartWithHediff hediffGiver_StartWith = (HediffGiver_StartWithHediff)hdg;
                        hediffGiver_StartWith.GiveHediff(__result);
                    }
                }
            }
        }
    }

}