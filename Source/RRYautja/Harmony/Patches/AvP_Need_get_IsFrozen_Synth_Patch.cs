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
    [HarmonyPatch(typeof(Need), "get_IsFrozen")]
    public static class AvP_Need_get_IsFrozen_Synth_Patch
    {
        [HarmonyPostfix]
        public static void get_IsFrozen_Postfix(Need __instance, ref bool __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.def == USCMDefOf.AvP_Synth)
            {
                if (__instance.def == NeedDefOf.Food && __instance.CurLevelPercentage<0.1f)
                {
                    __result = true;
                }
            }
        }
    }
}