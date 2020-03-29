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
    /*
    [HarmonyPatch(typeof(RaceProperties), "get_isFlesh")]
    public static class AvP_RaceProperties_isFlesh_Synth_Patch
    {
        [HarmonyPostfix]
        public static void isFlesh_Synth_Postfix(RaceProperties __instance,ref bool __result)
        {
            if (__instance!=null && __result)
            {
                if (__instance.FleshType!=null)
                {
                    __result = __instance.FleshType != USCMDefOf.RRY_Synth.race.FleshType;
                }
            }
        }
    }
    */
}