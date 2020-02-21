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

namespace RRYautja
{
    // DoComplexCalcs
    /*
    [HarmonyPatch(typeof(Fire), "DoComplexCalcs")]
    public static class AvP_Fire_DoComplexCalcs_Patch
    {
        [HarmonyPostfix]
        public static void DoComplexCalcsPostfix(Fire __instance)
        {
            Map map = __instance.Map != null ? __instance.Map : __instance.MapHeld;
            IntVec3 center = __instance.Position != null ? __instance.Position : __instance.PositionHeld;
            float radius = __instance.fireSize * 3f;
            MapComponent_HiveGrid _HiveGrid = map.GetComponent<MapComponent_HiveGrid>();
            if (_HiveGrid != null)
            {
                HiveUtility.AddHiveRadial(center, map, radius, -(__instance.fireSize * 0.1f));
            }
        }
    }
    */
}