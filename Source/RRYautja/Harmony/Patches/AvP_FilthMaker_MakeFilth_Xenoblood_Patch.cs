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
    
    // FilthMaker.TryMakeFilth
    [HarmonyPatch(typeof(Pawn_HealthTracker), "DropBloodFilth")]
    public static class AvP_Pawn_HealthTracker_DropBloodFilth_Xenoblood_Patch
    {
        [HarmonyPrefix]
        public static bool Patch_Pawn_HealthTracker_DropBloodFilth(Pawn_HealthTracker __instance)
        {
            Pawn pawn = HarmonyPatches.Pawn_HealthTracker_GetPawn(__instance);
            bool flag = pawn.isXenomorph() && (pawn.Spawned || pawn.ParentHolder is Pawn_CarryTracker) && pawn.SpawnedOrAnyParentSpawned && pawn.RaceProps.BloodDef != null;
            bool result;
            if (flag)
            {
                FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.MapHeld, XenomorphDefOf.RRY_FilthBloodXenomorph_Active, pawn.LabelIndefinite(), 1);
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