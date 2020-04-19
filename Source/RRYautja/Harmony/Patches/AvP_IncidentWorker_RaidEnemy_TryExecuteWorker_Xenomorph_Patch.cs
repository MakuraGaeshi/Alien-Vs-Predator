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
    public static class AvP_IncidentWorker_RaidEnemy_Xenomorph_TryExecuteWorker_Patch
    {
        [HarmonyPrefix]
        public static bool PreExecute(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.AvP_Xenomorph))
                {
                    if ((parms.target is Map map))
                    {
                        parms.generateFightersOnly = true;
                        bool extTunnels = !map.GetComponent<MapComponent_HiveGrid>().HiveChildlist.NullOrEmpty();
                        Rand.PushState();
                        int @int = Rand.Int;
                        bool chance = Rand.ChanceSeeded(0.05f, AvPConstants.AvPSeed);
                        Rand.PopState();
                        if (chance)
                        {
                            parms.forced = true;
                            parms.points = Mathf.Max(parms.points * new FloatRange(1f, 1.6f).RandomInRange, parms.faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat));
                            
                        }
                        if (parms.raidStrategy != XenomorphDefOf.AvP_PowerCut)
                        {
                            if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                            {
                                parms.points *= 2;
                                parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;
                                Rand.PushState();
                                if (Rand.Chance(0.05f))
                                {
                                    parms.forced = true;
                                    parms.points = Mathf.Max(parms.points * new FloatRange(1f, 1.6f).RandomInRange, parms.faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat));
                                }
                                Rand.PopState();
                            }
                            Rand.PushState();
                            bool chance1 = Rand.ChanceSeeded(0.10f + (map.GetComponent<MapComponent_HiveGrid>().HiveChildlist.Count / 100f), AvPConstants.AvPSeed);
                            Rand.PopState();
                            if (extTunnels && chance1)
                            {
                                parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                                parms.raidArrivalMode = XenomorphDefOf.AvP_RandomEnterFromTunnel;
                            }
                            else
                            {
                                bool Hive = !map.GetComponent<MapComponent_HiveGrid>().Hivelist.NullOrEmpty();
                                if (Hive && map.GetComponent<MapComponent_HiveGrid>().Hivelist.Any(x=> x as HiveLike is HiveLike hive && hive!=null && hive.caveColony))
                                {
                                    Rand.PushState();
                                    bool chance2 = Rand.ChanceSeeded(0.10f + (map.GetComponent<MapComponent_HiveGrid>().Hivelist.Count / 100f), AvPConstants.AvPSeed);
                                    Rand.PopState();
                                    if (chance2)
                                    {
                                        parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                                        parms.raidArrivalMode = XenomorphDefOf.AvP_EnterFromTunnel;
                                    }
                                }

                                if (parms.raidArrivalMode != XenomorphDefOf.AvP_RandomEnterFromTunnel && parms.raidArrivalMode != XenomorphDefOf.AvP_DropThroughRoofNearPower)
                                {
                                    parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}