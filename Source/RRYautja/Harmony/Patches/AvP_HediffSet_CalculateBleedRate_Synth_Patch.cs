﻿using RimWorld;
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
    public static class AvP_HediffSet_CalculateBleedRate_Synth_Patch
    {
        [HarmonyPostfix]
        public static void postfix(HediffSet __instance, ref float __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            if (pawn.def == USCMDefOf.AvP_Synth)
            {
                __result = 0f;
            }
        }
    }
    
}