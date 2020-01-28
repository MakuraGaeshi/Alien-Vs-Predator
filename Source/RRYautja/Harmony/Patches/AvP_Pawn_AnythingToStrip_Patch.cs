using RimWorld;
using Verse;
using Harmony;
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

namespace RRYautja
{
    // Disallows stripping of the Wristblade
    [HarmonyPatch(typeof(Pawn), "AnythingToStrip")]
    public static class AvP_Pawn_AnythingToStrip_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreWristblade(Pawn __instance, ref bool __result)
        {
            __result = __result && !(__instance.apparel != null && __instance.apparel.WornApparelCount == 1 && __instance.apparel.WornApparel.Any(x => x.def.defName.Contains("RRY_Equipment_HunterGauntlet")) && __instance.Faction != Faction.OfPlayerSilentFail) && !(__instance.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));

        }
    }
}