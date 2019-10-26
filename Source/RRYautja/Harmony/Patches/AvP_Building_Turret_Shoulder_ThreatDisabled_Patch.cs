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
    // Plasmacasters ignore Cloaked things
    [HarmonyPatch(typeof(Building_Turret_Shoulder), "ThreatDisabled")]
    public static class AvP_Building_Turret_Shoulder_ThreatDisabled_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreShoulderTurret(Building_Turret_Shoulder __instance, ref bool __result, IAttackTargetSearcher disabledFor)
        {
            bool selected__instance = Find.Selector.SelectedObjects.Contains(__instance);
            bool shouldturret = false;
            if (__instance != null)
            {
                if (__instance is Building_Turret_Shoulder)
                {
                    shouldturret = true;
                }
            }
            __result = (__result || shouldturret);

        }
    }
}