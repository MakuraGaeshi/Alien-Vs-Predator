using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000330 RID: 816
	public class IncidentWorker_PowerCutV2 : IncidentWorker
	{
		// Token: 0x06000E1B RID: 3611 RVA: 0x00069AE0 File Offset: 0x00067EE0
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			GameConditionManager gameConditionManager = parms.target.GameConditionManager;
			if (gameConditionManager == null)
			{
				Log.ErrorOnce(string.Format("Couldn't find condition manager for incident target {0}", parms.target), 70849667, false);
				return false;
			}
			if (gameConditionManager.ConditionIsActive(this.def.gameCondition))
			{
				return false;
			}
			List<GameCondition> activeConditions = gameConditionManager.ActiveConditions;
			for (int i = 0; i < activeConditions.Count; i++)
			{
				if (!this.def.gameCondition.CanCoexistWith(activeConditions[i].def))
				{
					return false;
				}
            }
            Map map = (Map)parms.target;
            if (map.listerBuildings.allBuildingsColonist.Any(x=> x.TryGetComp<CompPowerPlant>()!=null))
            {
                /*
                List<Building> list = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null);
                int seed = 47569362;
                float chance = 1f;
                foreach (var item in list.OrderBy(x => x.TryGetComp<CompPowerPlant>().EnergyOutputPerTick))
                {
                    CompPowerPlant powerPlant = item.TryGetComp<CompPowerPlant>();
                    CompBreakdownable breakdownable = item.TryGetComp<CompBreakdownable>();
                    if (Rand.ChanceSeeded(chance, seed))
                    {
                        if (breakdownable != null)
                        {
                            breakdownable.DoBreakdown();
                        }

                        chance -= 0.05f;
                    }
                }
                */
                IntVec3 intVec;
                Faction faction = Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph);
                return this.TryFindSpawnSpot(map, out intVec) && faction != null;
            }
            return false;
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x00069B78 File Offset: 0x00067F78
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			GameConditionManager gameConditionManager = parms.target.GameConditionManager;
			int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
			GameCondition cond = GameConditionMaker.MakeCondition(this.def.gameCondition, duration, 0);
			gameConditionManager.RegisterCondition(cond);
            Map map = (Map)parms.target;
            IntVec3 spawnSpot;
            if (!this.TryFindSpawnSpot(map, out spawnSpot))
            {
                return false;
            }
            Faction faction = Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph);
            if (faction == null)
            {
                return false;
            }
            int @int = Rand.Int;
            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            raidParms.forced = true;
            raidParms.faction = faction;
            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            raidParms.spawnCenter = spawnSpot;
            raidParms.generateFightersOnly = true;
            raidParms.points = Mathf.Max(raidParms.points * IncidentWorker_PowerCutV2.RaidPointsFactorRange.RandomInRange, faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat));
            raidParms.pawnGroupMakerSeed = new int?(@int);
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, raidParms, false);
            defaultPawnGroupMakerParms.points = IncidentWorker_Raid.AdjustedRaidPoints(defaultPawnGroupMakerParms.points, raidParms.raidArrivalMode, raidParms.raidStrategy, defaultPawnGroupMakerParms.faction, PawnGroupKindDefOf.Combat);
            IEnumerable<PawnKindDef> pawnKinds = PawnGroupMakerUtility.GeneratePawnKindsExample(defaultPawnGroupMakerParms);
            base.SendStandardLetter();
            QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms), 0, 0);
            Find.Storyteller.incidentQueue.Add(qi);

            @int = Rand.Int;
            raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            raidParms.forced = true;
            raidParms.faction = faction;
            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            raidParms.spawnCenter = spawnSpot;
            raidParms.generateFightersOnly = true;
            raidParms.points = Mathf.Max(raidParms.points * IncidentWorker_PowerCutV2.RaidPointsFactorRange.RandomInRange, faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat));
            raidParms.pawnGroupMakerSeed = new int?(@int);
            defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, raidParms, false);
            defaultPawnGroupMakerParms.points = IncidentWorker_Raid.AdjustedRaidPoints(defaultPawnGroupMakerParms.points, raidParms.raidArrivalMode, raidParms.raidStrategy, defaultPawnGroupMakerParms.faction, PawnGroupKindDefOf.Combat);
            pawnKinds = PawnGroupMakerUtility.GeneratePawnKindsExample(defaultPawnGroupMakerParms);
            base.SendStandardLetter();
            qi = new QueuedIncident(new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms), Find.TickManager.TicksGame + IncidentWorker_PowerCutV2.RaidDelay.RandomInRange, 0);
            Find.Storyteller.incidentQueue.Add(qi);


            List<Building> list = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null);
            int seed = 47569362;
            float chance = 1f;
            foreach (var item in list.OrderBy(x => x.TryGetComp<CompPowerPlant>().EnergyOutputPerTick))
            {
                CompPowerPlant powerPlant = item.TryGetComp<CompPowerPlant>();
                CompBreakdownable breakdownable = item.TryGetComp<CompBreakdownable>();
                if (Rand.ChanceSeeded(chance, seed))
                {
                    if (breakdownable != null)
                    {
                        breakdownable.DoBreakdown();
                    }

                    chance -= 0.05f;
                }
            }
            /*
            DiaNode diaNode = new DiaNode(text);
            DiaOption diaOption = new DiaOption("RefugeeChasedInitial_Accept".Translate());
            diaOption.action = delegate ()
            {
                QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms), Find.TickManager.TicksGame + IncidentWorker_PowerCut.RaidDelay.RandomInRange, 0);
                Find.Storyteller.incidentQueue.Add(qi);
            };
            diaOption.resolveTree = true;
            diaNode.options.Add(diaOption);
            string text2 = "RefugeeChasedRejected".Translate(refugee.LabelShort, refugee);
            DiaNode diaNode2 = new DiaNode(text2);
            DiaOption diaOption2 = new DiaOption("OK".Translate());
            diaOption2.resolveTree = true;
            diaNode2.options.Add(diaOption2);
            DiaOption diaOption3 = new DiaOption("RefugeeChasedInitial_Reject".Translate());
            diaOption3.action = delegate ()
            {
                Find.WorldPawns.PassToWorld(refugee, PawnDiscardDecideMode.Decide);
            };
            diaOption3.link = diaNode2;
            diaNode.options.Add(diaOption3);
            string title = "RefugeeChasedTitle".Translate(map.Parent.Label);
            Find.WindowStack.Add(new Dialog_NodeTreeWithFactionInfo(diaNode, faction, true, true, title));
            Find.Archive.Add(new ArchivedDialog(diaNode.text, title, faction));
            */
            return true;
        }

        // Token: 0x0600001B RID: 27 RVA: 0x000030DC File Offset: 0x000012DC
        public static Building FindBreakDownTargetFor(Pawn p)
        {
            Predicate<Thing> breakdownValidator = delegate (Thing t)
            {
                Building building2 = (Building)t;
                return ReservationUtility.CanReserveAndReach(p, t, (PathEndMode)1, (Danger)2, 1, -1, null, false) && !building2.GetComp<CompBreakdownable>().BrokenDown && !FireUtility.IsBurning(building2);
            };
            ThingDef thingDef = GenCollection.RandomElement<ThingDef>((from t in DefDatabase<ThingDef>.AllDefsListForReading
                                                                       where t.GetCompProperties<CompProperties_Breakdownable>() != null
                                                                       select t).ToList<ThingDef>());
            Predicate<Thing> predicate = (Thing b) => breakdownValidator(b);
            Building building = (Building)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(thingDef), (PathEndMode)1, TraverseParms.For(p, (Danger)3, 0, false), 9999f, predicate, null, 0, -1, false, (RegionType)6, false);
            if (building != null)
            {
                return building;
            }
            return null;
        }

        // Token: 0x06000E92 RID: 3730 RVA: 0x0006CD54 File Offset: 0x0006B154
        private bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            Predicate<IntVec3> validator = (IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }

        // Token: 0x06000E93 RID: 3731 RVA: 0x0006CD8D File Offset: 0x0006B18D
        private bool TryFindEnemyFaction(out Faction enemyFac)
        {
            return (from f in Find.FactionManager.AllFactions
                    where !f.def.hidden && !f.defeated && f.HostileTo(Faction.OfPlayer)
                    select f).TryRandomElement(out enemyFac);
        }

        // Token: 0x04000949 RID: 2377
        private static readonly IntRange RaidDelay = new IntRange(1000, 4000);

        // Token: 0x0400094A RID: 2378
        private static readonly FloatRange RaidPointsFactorRange = new FloatRange(1f, 1.6f);
    }
}
