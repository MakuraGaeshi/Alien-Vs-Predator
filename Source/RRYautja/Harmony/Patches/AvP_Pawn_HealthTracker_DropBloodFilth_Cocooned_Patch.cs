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
    
    [HarmonyPatch(typeof(Pawn_HealthTracker), "DropBloodFilth")]
    public static class AvP_Pawn_HealthTracker_DropBloodFilth_Cocooned_Patch
    {
        public static bool Prefix(Pawn_HealthTracker __instance)
        { 
            Pawn pawn = Main.Pawn_HealthTracker_GetPawn(__instance);
            bool flag = (pawn.health.hediffSet.HasHediff(XenomorphDefOf.AvP_Hediff_Cocooned));
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
    
}