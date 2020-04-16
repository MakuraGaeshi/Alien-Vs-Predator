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
    [HarmonyPatch(typeof(HealthUtility), "AdjustSeverity")]
    public static class AvP_HealthUtility_AdjustSeverity_Cocooned_Patch
    {
        public static bool Prefix(Pawn pawn, HediffDef hdDef, float sevOffset)
        { 
            bool preflag = hdDef != XenomorphDefOf.RRY_FaceHuggerInfection || hdDef != XenomorphDefOf.RRY_XenomorphImpregnation || hdDef != XenomorphDefOf.RRY_HiddenXenomorphImpregnation || hdDef != XenomorphDefOf.RRY_NeomorphImpregnation || hdDef != XenomorphDefOf.RRY_HiddenNeomorphImpregnation;
            bool flag = (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned)) && preflag;
            return !flag;
        }
    }
}