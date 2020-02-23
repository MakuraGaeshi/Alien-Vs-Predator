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
using AlienRace;

namespace RRYautja
{
    [HarmonyPatch(typeof(SickPawnVisitUtility), "CanVisit")]
    public static class AvP_SickPawnVisitUtility_CanVisit_Synth_Patch
    {
        [HarmonyPrefix]
        public static bool ThoughtsFromIngestingPostPrefix(Pawn pawn, Pawn sick, JoyCategory maxPatientJoy, bool __result)
        {
            if (sick.def == USCMDefOf.RRY_Synth)
            {
                __result = false;
                return __result;
            }
            return true;
        }
    }
}