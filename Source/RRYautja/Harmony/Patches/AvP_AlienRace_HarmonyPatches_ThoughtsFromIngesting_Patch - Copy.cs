using RimWorld;
using Verse;
using Harmony;
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
    [HarmonyPatch(typeof(IncidentWorker_Disease), "CanAddHediffToAnyPartOfDef")]
    public static class AvP_IncidentWorker_Disease_CanAddHediffToAnyPartOfDef_Synth_Patch
    {
        [HarmonyPostfix]
        public static void CanAddHediffToAnyPartOfDefPrefix(Pawn pawn, HediffDef hediffDef, BodyPartDef partDef, bool __result)
        {
            Log.Message(string.Format("trying to give {1} to {0}",pawn.LabelShortCap, hediffDef.LabelCap));
            if (pawn.def == USCMDefOf.RRY_Synth)
            {
                Log.Message(string.Format("{0} is a synth", pawn.LabelShortCap, hediffDef.LabelCap));

                Log.Message(string.Format("is immune to {1}", pawn.LabelShortCap, hediffDef.LabelCap));


                __result = false;
                return;
            }
            Log.Message(string.Format("is Not immune to {1}", pawn.LabelShortCap, hediffDef.LabelCap));
        }
    }

}