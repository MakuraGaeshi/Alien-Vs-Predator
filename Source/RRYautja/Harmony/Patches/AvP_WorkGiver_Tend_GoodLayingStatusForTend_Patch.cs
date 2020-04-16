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
    // Doctors Ignore Cocooned Pawns that need tending
    [HarmonyPatch(typeof(WorkGiver_Tend), "GoodLayingStatusForTend")]
    public static class AvP_WorkGiver_Tend_GoodLayingStatusForTend_Patch
    {
        [HarmonyPostfix]
        public static void PawnInCocoon(WorkGiver_Tend __instance, Pawn patient, Pawn doctor, ref bool __result)
        {
            __result = __result && (!patient.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) && !(patient.CurrentBed() is Building_XenomorphCocoon));
            //    Log.Message(string.Format("WorkGiver_Tend_GoodLayingStatusForTend_Patch patient: {0}, doctor: {1}, __Result: {2}", patient, doctor, __result));

        }
    }
}