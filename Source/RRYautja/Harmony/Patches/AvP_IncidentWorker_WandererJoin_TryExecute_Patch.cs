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

namespace RRYautja
{
    /*
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(IncidentWorker_WandererJoin), "TryExecute")]
    public static class AvP_IncidentWorker_WandererJoin_TryExecute_Patch
    {
        public static string stranger = "StrangerInBlack";
        [HarmonyPrefix]
        public static bool PreExecute(IncidentWorker_WandererJoin __instance, ref IncidentParms parms ,bool __result)
        { // request parms.faction.def

        //    Log.Message(string.Format("Original race: {0}", __instance.def.pawnKind.race));
        //    Log.Message(string.Format("Original faction: {0}", parms.faction.def));






            return true;
        }
    }
    */
}