using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RRYautja;

namespace RimWorld
{
    // Token: 0x02000340 RID: 832
    public class IncidentWorker_Hivelike : IncidentWorker
    {
        public IntVec3 intVec;
        // Token: 0x06000E63 RID: 3683 RVA: 0x0006B874 File Offset: 0x00069C74
        protected override bool CanFireNowSub(IncidentParms parms)
		{
            
			Map map = (Map)parms.target;
            bool result = base.CanFireNowSub(parms) && XenomorphHiveUtility.TotalSpawnedHiveLikesCount(map) < 30 && InfestationLikeCellFinder.TryFindCell(out intVec, out IntVec3 lc, map);
            ThingDef thing = DefDatabase<ThingDef>.GetNamedSilentFail("O21_AntiInfestationThumper");
            if (map.listerBuildings.ColonistsHaveBuildingWithPowerOn(thing) && result)
            {
                IncidentDef def;
                def = new IncidentDef
                {
                    defName = this.def.defName + "B",
                    workerClass = typeof(IncidentWorker_XenomorphHive),
                    category = this.def.category,
                    allowedBiomes = this.def.allowedBiomes,
                    baseChance = 0,
                    description = this.def.description,
                    letterDef = this.def.letterDef,
                    letterLabel = this.def.letterLabel,
                    letterText = this.def.letterText,
                    targetTags = this.def.targetTags,
                    tags = this.def.tags,
                    tale = this.def.tale,
                    refireCheckTags = this.def.refireCheckTags,
                    mechClusterBuilding = this.def.mechClusterBuilding
                };
                parms.points *= 3;
                QueuedIncident qi = new QueuedIncident(new FiringIncident(def, null, parms), Find.TickManager.TicksGame+10);
                Find.Storyteller.incidentQueue.Add(qi);
            //    return false;
            }
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
			int hivelikeCount = Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1);
            if (def.tags.Contains("TunnelLike"))
            {
                //Log.Message(string.Format("TunnelLike"));
                Thing t = this.SpawnTunnelLikeCluster(hivelikeCount, map);
                base.SendStandardLetter(null, new TargetInfo(intVec, map, false), Array.Empty<NamedArgument>());
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
                base.SendStandardLetter(null, new TargetInfo(intVec, map, false), Array.Empty<NamedArgument>());
            }
			Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;


        }

        private HiveLike SpawnHiveLikeCluster(int hiveCount, Map map, bool ignoreRoofedRequirement = false, bool allowUnreachable = false, float modifier = 1)
        {;
            IntVec3 loc = intVec;
            ThingDef_HiveLike thingDef = (ThingDef_HiveLike)this.def.mechClusterBuilding;
            HiveLike hivelike = (HiveLike)ThingMaker.MakeThing(thingDef, null);
            GenSpawn.Spawn(ThingMaker.MakeThing(hivelike.Def.TunnelDef, null), loc, map);
            hivelike.SetFaction(hivelike.OfFaction, null);
            IncidentWorker_Hivelike.SpawnItemInstantly(hivelike);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                CompSpawnerHiveLikes c = hivelike.GetComp<CompSpawnerHiveLikes>();
                if (hivelike.Spawned && hivelike.GetComp<CompSpawnerHiveLikes>().TrySpawnChildHiveLike(modifier, modifier, out HiveLike hivelike2, ignoreRoofedRequirement, allowUnreachable))
                {
                    IncidentWorker_Hivelike.SpawnItemInstantly(hivelike2);
                    hivelike = hivelike2;
                }
            }
            return hivelike;
        }

        private TunnelHiveLikeSpawner SpawnTunnelLikeCluster(int hiveCount, Map map, bool ignoreRoofedRequirement = false, bool allowUnreachable = false, float modifier = 1)
        {
            IntVec3 loc = intVec;
            ThingDef_HiveLike tD = (ThingDef_HiveLike)this.def.mechClusterBuilding;
            ThingDef_TunnelHiveLikeSpawner thingDef = (ThingDef_TunnelHiveLikeSpawner)tD.TunnelDef;
            TunnelHiveLikeSpawner hivelike = (TunnelHiveLikeSpawner)ThingMaker.MakeThing(thingDef, null);
            GenSpawn.Spawn(ThingMaker.MakeThing(hivelike.def, null), loc, map);
            //hivelike.SetFaction(hivelike.faction, null);
            IncidentWorker_Hivelike.SpawnItemInstantly(hivelike);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                CompSpawnerHiveLikes c = hivelike.GetComp<CompSpawnerHiveLikes>();
                if (hivelike.Spawned && hivelike.GetComp<CompSpawnerHiveLikes>().TrySpawnChildHiveLike(modifier, modifier,out TunnelHiveLikeSpawner hivelike2, ignoreRoofedRequirement, allowUnreachable))
                {
                    IncidentWorker_Hivelike.SpawnItemInstantly(hivelike2);
                    hivelike = hivelike2;
                    //Log.Message(string.Format("7 e"));
                }
                //Log.Message(string.Format("7 f"));
            }
            //Log.Message(string.Format("8"));
            return hivelike;
        }

        private static void SpawnItemInstantly(HiveLike hive)
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
        
        private static void SpawnItemInstantly(TunnelHiveLikeSpawner hive)
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
        

        // Token: 0x04000939 RID: 2361
        private const float HivePoints = 550f;
	}
}
