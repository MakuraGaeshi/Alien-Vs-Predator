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
    // Stop Doctors trying to feed cocooned Pawns
    [HarmonyPatch(typeof(FeedPatientUtility), "ShouldBeFed")]
    public static class AvP_FeedPatientUtility_ShouldBeFed_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreCocooned(Pawn p, ref bool __result)
        {
            __result = __result && !(p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));
        }
    }
}