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
    
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
    public static class AvP_Pawn_NeedsTracker_ShouldHaveNeed_Synth_Patch
    {
        [HarmonyPostfix]
        public static void RoyalEggSize(Pawn_NeedsTracker __instance, NeedDef nd, ref bool __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            if (pawn.def == USCMDefOf.AvP_Synth)
            {
                if (nd == NeedDefOf.Rest)
                {
                    __result = false;
                }
            }
        }
    }
    
}