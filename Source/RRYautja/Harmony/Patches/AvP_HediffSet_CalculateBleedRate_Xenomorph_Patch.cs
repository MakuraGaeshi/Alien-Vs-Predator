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
using AvP.settings;
using AvP.ExtensionMethods;

namespace AvP.HarmonyInstance
{
    [HarmonyPatch(typeof(HediffSet), "CalculateBleedRate")]
    public static class AvP_HediffSet_CalculateBleedRate_Xenomorph_Patch
    {
        [HarmonyPostfix]
        public static void postfix(HediffSet __instance, ref float __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            if (pawn.def.defName.Contains("Xenomorph"))
            {
                if (pawn.def.defName.Contains("Queen"))
                {
                    __result = __result * 0.25f;
                }
                else __result = __result*0.5f;

            }
        }
    }
    
}