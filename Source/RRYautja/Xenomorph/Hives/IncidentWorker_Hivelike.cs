using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using AvP;
using static RimWorld.InfestationLikeCellFinder;

namespace RimWorld
{
    // Token: 0x02000340 RID: 832
    public class IncidentWorker_Xenomorph_Hivelike : IncidentWorker
    {
        IntVec3 intVec = IntVec3.Invalid;
        LocationCandidate lc = new LocationCandidate();
        // Token: 0x06000E63 RID: 3683 RVA: 0x0006B874 File Offset: 0x00069C74
        protected override bool CanFireNowSub(IncidentParms parms)
		{
            if (!AvP.settings.SettingsHelper.latest.AllowXenomorphFaction)
            {
                Log.Error(string.Format("InfestationLikeCellFinder when Xeno disabled"));
                return false;
            }
            Map map = (Map)parms.target;
            intVec = IntVec3.Invalid;
            lc = new LocationCandidate();
            if (!InfestationLikeCellFinder.TryFindCell(out intVec, out lc, map, true, false, true, true))
            {
                if (!InfestationLikeCellFinder.TryFindCell(out intVec, out lc, map, true, true, true, true))
                {
                    Log.Warning(string.Format("InfestationLikeCellFinder Cant find any suitable location candidates"));
                    return false;
                }
                else
                {
                    Log.Message(string.Format("InfestationLikeCellFinder found suitable location candidate {0} {1} second try", intVec, lc));
                }
            }
            else
            {
                Log.Message(string.Format("InfestationLikeCellFinder found suitable location candidate {0} {1} first try", intVec, lc));
            }
            bool hiveflag = XenomorphHiveUtility.TotalSpawnedHiveLikesCount(map) < 1;
            if (!hiveflag)
            {
                Log.Warning(string.Format("Xeno hive already present"));
            }
            bool basecanfire = base.CanFireNowSub(parms);
            if (!basecanfire)
            {
                Log.Warning(string.Format("! base.CanFireNowSub(parms)"));
            }
            bool result = basecanfire && hiveflag;
            
			return result;
            
            /*
            Map map = (Map)parms.target;
            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            return base.CanFireNowSub(parms) && HiveLikeUtility.TotalSpawnedHiveLikesCount(map) < 100;
            */
        }
        // Token: 0x06000E64 RID: 3684 RVA: 0x0006B8B4 File Offset: 0x00069CB4
        protected override bool TryExecuteWorker(IncidentParms parms)
		{

            Map map = (Map)parms.target;

            if (!InfestationLikeCellFinder.TryFindCell(out intVec, out lc, map, true, false, true, true))
            {
                if (!InfestationLikeCellFinder.TryFindCell(out intVec, out lc, map, true, true, true, true))
                {
                    Log.Warning(string.Format("InfestationLikeCellFinder Cant find any suitable location candidates"));
                    return false;
                }
            }

            if (intVec == IntVec3.Invalid)
            {
                Log.Warning(string.Format("{0} intVec == IntVec3.Invalid",this));
                return false;
            }
            if (intVec.CloseToEdge(map, 10))
            {
                Log.Warning(string.Format("{0} intVec.CloseToEdge(map, 10)", this));
                return false;
            }
            int hivelikeCount = Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1);
            if (def.tags.Contains("TunnelLike"))
            {
                //Log.Message(string.Format("TunnelLike"));
                Thing t = this.SpawnTunnelLikeCluster(hivelikeCount, map, parms);
                base.SendStandardLetter(parms, new TargetInfo(intVec, map, false), Array.Empty<NamedArgument>());
                /*
                Map map = (Map)parms.target;
                int hiveCount = Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1);
                Thing t = this.SpawnTunnels(hiveCount, map);
                base.SendStandardLetter(t, null, new string[0]);
                Find.TickManager.slower.SignalForceNormalSpeedShort();
                return true;
                */
            }
            else
            {
                //Log.Message(string.Format("HiveLike"));

                Thing t = this.SpawnHiveLikeCluster(hivelikeCount, map);
                base.SendStandardLetter(parms, new TargetInfo(intVec, map, false), Array.Empty<NamedArgument>());
            }
			Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;


        }

        private XenomorphHive SpawnHiveLikeCluster(int hiveCount, Map map, bool ignoreRoofedRequirement = false, bool allowUnreachable = false, float modifier = 1)
        {;
            IntVec3 loc = intVec;
            ThingDef thingDef = this.def.mechClusterBuilding;
            XenomorphHive hivelike = (XenomorphHive)ThingMaker.MakeThing(thingDef, null);
            GenSpawn.Spawn(ThingMaker.MakeThing(hivelike.Ext.TunnelDef, null), loc, map);
            hivelike.SetFaction(hivelike.OfFaction, null);
            IncidentWorker_Xenomorph_Hivelike.SpawnItemInstantly(hivelike);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                CompSpawnerHiveLikes c = hivelike.GetComp<CompSpawnerHiveLikes>();
                if (hivelike.Spawned && hivelike.GetComp<CompSpawnerHiveLikes>().TrySpawnChildHiveLike(modifier, modifier, out XenomorphHive hivelike2, ignoreRoofedRequirement, allowUnreachable))
                {
                    IncidentWorker_Xenomorph_Hivelike.SpawnItemInstantly(hivelike2);
                    hivelike = hivelike2;
                }
            }
            return hivelike;
        }

        private XenomorphTunnelSpawner SpawnTunnelLikeCluster(int hiveCount, Map map, IncidentParms parms, bool ignoreRoofedRequirement = false, bool allowUnreachable = false, float modifier = 1)
        {
            IntVec3 loc = intVec;
            ThingDef tD = this.def.mechClusterBuilding;
            ThingDef thingDef = tD.GetModExtension<XenomorphHiveExtension>().TunnelDef;
            XenomorphTunnelSpawner hivelike = (XenomorphTunnelSpawner)ThingMaker.MakeThing(thingDef, null);
            hivelike.hivePoints = parms.points / hiveCount;
            GenSpawn.Spawn(ThingMaker.MakeThing(hivelike.def, null), loc, map);
            //hivelike.SetFaction(hivelike.faction, null);
            IncidentWorker_Xenomorph_Hivelike.SpawnItemInstantly(hivelike);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                CompSpawnerHiveLikes c = hivelike.GetComp<CompSpawnerHiveLikes>();
                if (hivelike.Spawned && hivelike.GetComp<CompSpawnerHiveLikes>().TrySpawnChildHiveLike(modifier, modifier,out XenomorphTunnelSpawner hivelike2, ignoreRoofedRequirement, allowUnreachable))
                {
                    IncidentWorker_Xenomorph_Hivelike.SpawnItemInstantly(hivelike2);
                    hivelike = hivelike2;
                    //Log.Message(string.Format("7 e"));
                }
                //Log.Message(string.Format("7 f"));
            }
            //Log.Message(string.Format("8"));
            return hivelike;
        }

        private static void SpawnItemInstantly(XenomorphHive hive)
        {
            CompXenomorph_SpawnerLike compSpawner = (CompXenomorph_SpawnerLike)hive.AllComps.Find(delegate (ThingComp x)
            {
                CompXenomorph_SpawnerLike compSpawner2 = x as CompXenomorph_SpawnerLike;
                return compSpawner2 != null && compSpawner2.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly;
            });
            if (compSpawner != null)
            {
                compSpawner.TryDoSpawn();
            }
        }
        
        private static void SpawnItemInstantly(XenomorphTunnelSpawner hive)
        {
            CompXenomorph_SpawnerLike compSpawner = (CompXenomorph_SpawnerLike)hive.AllComps.Find(delegate (ThingComp x)
            {
                CompXenomorph_SpawnerLike compSpawner2 = x as CompXenomorph_SpawnerLike;
                return compSpawner2 != null && compSpawner2.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly;
            });
            if (compSpawner != null)
            {
                compSpawner.TryDoSpawn();
            }
        }

        /*
        // Token: 0x06000E65 RID: 3685 RVA: 0x0006B914 File Offset: 0x00069D14
        private Thing SpawnTunnels(int hivelikeCount, Map map)
		{
            if (!InfestationLikeCellFinder.TryFindCell(out IntVec3 loc, out IntVec3 lc, map))
            {
                //    Log.Message(string.Format("TryFindCell: {0} From !InfestationLikeCellFinder.TryFindCell(out loc, map)", !InfestationLikeCellFinder.TryFindCell(out loc, map)));
                return null;
            }
            ThingDef_HiveLike thingDef = (ThingDef_HiveLike)this.def.mechClusterBuilding;
            Thing thing = GenSpawn.Spawn(ThingMaker.MakeThing(thingDef.TunnelDef, null), loc, map, WipeMode.FullRefund);
			for (int i = 0; i < hivelikeCount - 1; i++)
			{
				loc = CompSpawnerHiveLikes.FindChildHiveLocation(thing.Position, map, this.def.mechClusterBuilding, this.def.mechClusterBuilding.GetCompProperties<CompProperties_SpawnerHiveLikes>(), 1, 1, true, true);
            //    Log.Message(string.Format("loc: {0} to check", !InfestationLikeCellFinder.TryFindCell(out loc, map)));
                if (loc.IsValid)
                {
                    thing = GenSpawn.Spawn(ThingMaker.MakeThing(thingDef.TunnelDef, null), loc, map, WipeMode.FullRefund);
                //    Log.Message(string.Format("spawning: {0} @ {1}", thing.Label, loc));
                }
			}
			return thing;
		}
        */

        // Token: 0x04000939 RID: 2361
        private const float HivePoints = 550f;
	}
}
