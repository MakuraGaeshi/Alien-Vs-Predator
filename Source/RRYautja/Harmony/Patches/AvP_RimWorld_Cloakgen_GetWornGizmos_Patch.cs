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
    // Gets Gizmos from Cloakgen's Comps
    [HarmonyPatch(typeof(Cloakgen), "GetWornGizmos")]
    public static class AvP_RimWorld_Cloakgen_GetWornGizmos_Patch
    {
        [HarmonyPostfix]
        public static void ApparelGizmosFromComps(Cloakgen __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance == null)
            {
                //    Log.Warning("ApparelGizmosFromComps cannot access Apparel.");
                return;
            }
            if (__result == null)
            {
                //    Log.Warning("ApparelGizmosFromComps creating new list.");
                return;
            }

            // Find all comps on the apparel. If any have gizmos, add them to the result returned from apparel already (typically empty set).
            List<Gizmo> l2 = new List<Gizmo>(__result);
            foreach (CompWearable comp in __instance.GetComps<CompWearable>())
            {
                foreach (Gizmo gizmo in comp.CompGetGizmosWorn())
                {
                    l2.Add(gizmo);
                }
            }
            __result = l2;
        }
    }
}