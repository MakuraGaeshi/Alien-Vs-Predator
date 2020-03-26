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
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "GetLetterText")]
    public static class AvP_IncidentWorker_RaidEnemyPatch_GetLetterText_Patch
    {
        [HarmonyPostfix]
        public static void PostExecute(ref string __result, ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && ((parms.faction.leader != null && parms.faction.leader.kindDef.race == YautjaDefOf.RRY_Alien_Yautja) || (parms.faction.def.basicMemberKind != null && parms.faction.def.basicMemberKind.race == YautjaDefOf.RRY_Alien_Yautja)))
                {
#if DEBUG
                //    Log.Message(string.Format("PostGetLetterText Yautja Raid"));
#endif

                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {
                        string text = "El Diablo, cazador de hombre. Only in the hottest years this happens. And this year it grows hot.";
                        text += "\n\n";
                        text += __result;
                        __result = text;
                    }
                }
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
#if DEBUG
                    //    Log.Message(string.Format("PostGetLetterText Xenomorph Raid CurSkyGlow: {0}", (parms.target as Map).skyManager.CurSkyGlow));
#endif
                    if (parms.raidStrategy != XenomorphDefOf.RRY_PowerCut)
                    {
                        if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                        {
                            string text = "They mostly come at night......mostly.....";
                            text += "\n\n";
                            text += __result;
                            __result = text;

                        }
                    }
                }
            }
        }
    }
    /*
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "GetLetterText")]
    public static class AvP_IncidentWorker_RaidEnemy_Patch_GetLetterText_Patch
    {
        [HarmonyPostfix]
        public static void PostExecute(ref string __result, ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
#if DEBUG
                //    Log.Message(string.Format("PostGetLetterText Xenomorph Raid CurSkyGlow: {0}", (parms.target as Map).skyManager.CurSkyGlow));
#endif
              
                    if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                    {
                        string text = "They mostly come at night......mostly.....";
                        text += "\n\n";
                        text += __result;
                        __result = text;

                    }
         
                }
            }
        }
    }
    */
}