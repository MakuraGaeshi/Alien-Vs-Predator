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
    /*
    [HarmonyPatch(typeof(Hediff_Injury), "get_BleedingStoppedDueToAge")]
    public static class AvP_Hediff_Injury_BleedingStoppedDueToAge_Synth_Patch
    {
        [HarmonyPostfix]
        public static void RoyalEggSize(Hediff_Injury __instance, ref bool __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            if (pawn.def == USCMDefOf.RRY_Synth)
            {
                __result = true;
                Log.Message(string.Format("{0} {1}", pawn.Label, __result));
            }
        }
    }
    */
    
    [HarmonyPatch(typeof(HediffSet), "get_PainTotal")]
    public static class AvP_HediffSet_get_PainTotal_Synth_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(HediffSet __instance, ref float __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            if (pawn.def == USCMDefOf.RRY_Synth || pawn.isXenomorph())
            {
                __result = 0f;
            //    Log.Message(string.Format("{0} {1}", pawn.Label, __result));
            }
        }
    }
    
}