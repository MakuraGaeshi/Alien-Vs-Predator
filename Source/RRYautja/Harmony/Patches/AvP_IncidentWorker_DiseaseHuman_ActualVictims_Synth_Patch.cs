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
using AlienRace;

namespace AvP.HarmonyInstance
{
    [HarmonyPatch(typeof(IncidentWorker_DiseaseHuman), "ActualVictims")]
    public static class AvP_IncidentWorker_DiseaseHuman_ActualVictims_Synth_Patch
    {
        // protected override IEnumerable<Pawn> ActualVictims(IncidentParms parms)
        [HarmonyPostfix]
        public static void CanAddHediffToAnyPartOfDefPrefix(IncidentWorker_DiseaseHuman __instance, IncidentParms parms, ref IEnumerable<Pawn> __result)
        {
            if (__instance != null)
            {
                if (__instance.def != null)
                {
                    if (__instance.def.diseaseIncident != null)
                    {
                        __result = __result.Where(x => x.def != USCMDefOf.AvP_Synth || (x.TryGetComp<CompSynthProps>() != null && x.TryGetComp<CompSynthProps>().AllowedDiseases.Contains(__instance.def.diseaseIncident)));
                    }
                }
            }
        }
    }

}