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
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
    public static class AvP_IncidentWorker_RaidEnemy_Yautja_TryExecute_Patch
    {
        // Token: 0x06000017 RID: 23 RVA: 0x00002CD0 File Offset: 0x00000ED0
        [HarmonyPrefix]
        public static bool PreExecute(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && ((parms.faction.leader != null && parms.faction.leader.kindDef.race == YautjaDefOf.AvP_Alien_Yautja) || (parms.faction.def.basicMemberKind != null && parms.faction.def.basicMemberKind.race == YautjaDefOf.AvP_Alien_Yautja)))
                {
                    parms.generateFightersOnly = true;
                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {
                        parms.points *= 2;
                        parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;
                    }
                }
            }
            return true;
        }

        /*
        [HarmonyPostfix]
        public static void PostExecute(bool __result, ref IncidentParms parms)
        {
            if (__result && parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && parms.faction.leader.kindDef.race == YautjaDefOf.AvP_Alien_Yautja)
                {

                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {

                    }
                }
            }
        }
        */
    }
}