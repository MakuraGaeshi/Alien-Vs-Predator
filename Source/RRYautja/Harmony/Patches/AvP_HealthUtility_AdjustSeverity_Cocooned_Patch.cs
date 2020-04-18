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
    [HarmonyPatch(typeof(HealthUtility), "AdjustSeverity")]
    public static class AvP_HealthUtility_AdjustSeverity_Cocooned_Patch
    {
        public static bool Prefix(Pawn pawn, HediffDef hdDef, float sevOffset)
        { 
            bool preflag = hdDef != XenomorphDefOf.AvP_FaceHuggerInfection || hdDef != XenomorphDefOf.AvP_XenomorphImpregnation || hdDef != XenomorphDefOf.AvP_HiddenXenomorphImpregnation || hdDef != XenomorphDefOf.AvP_NeomorphImpregnation || hdDef != XenomorphDefOf.AvP_HiddenNeomorphImpregnation;
            bool flag = (pawn.health.hediffSet.HasHediff(XenomorphDefOf.AvP_Hediff_Cocooned)) && preflag;
            return !flag;
        }
    }
}