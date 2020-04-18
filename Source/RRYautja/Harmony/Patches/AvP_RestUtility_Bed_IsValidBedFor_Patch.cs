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
    // Ignore Cocoons as Beds
    [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
    internal static class AvP_RestUtility_Bed_IsValidBedFor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing bedThing, Pawn sleeper, Pawn traveler, ref bool __result)
        {
            bool flag = bedThing is Building_XenomorphCocoon;
            bool flag2 = traveler != null ? traveler.kindDef.race.defName.Contains("AvP_Xenomorph") : false;
            bool flag3 = sleeper.isPotentialHost();
            __result = __result && !flag || (__result && flag && flag2);
            //    Log.Message(string.Format("RestUtility_Bed_IsValidBedFor sleeper: {0} traveler: {1} result: {2} = !flag: {3} && flag2: {4}", sleeper, traveler, __result, !flag , flag2));
            return;
        }
    }
}