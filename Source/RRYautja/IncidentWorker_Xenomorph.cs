using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x02000352 RID: 850
    public class IncidentWorker_TransportPodCrashXenomorph : IncidentWorker
    {
        // Token: 0x06000EB1 RID: 3761 RVA: 0x0006D5F4 File Offset: 0x0006B9F4
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Thing> things = ThingSetMakerDefOf.RefugeePod.root.Generate();
            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            Pawn pawn = this.FindPawn(things);
            pawn.guest.getRescuedThoughtOnUndownedBecauseOfPlayer = true;
            pawn.health.AddHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
            string label = "LetterLabelRefugeePodCrash".Translate();
            string text = "RefugeePodCrash".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            text += "\n\n";
            if (pawn.Faction == null)
            {
                text += "RefugeePodCrash_Factionless".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            }
            else if (pawn.Faction.HostileTo(Faction.OfPlayer))
            {
                text += "RefugeePodCrash_Hostile".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            }
            else
            {
                text += "RefugeePodCrash_NonHostile".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            }
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(intVec, map, false), null, null);
            ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
            activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(things, true, false);
            activeDropPodInfo.openDelay = 180;
            activeDropPodInfo.leaveSlag = true;
            DropPodUtility.MakeDropPodAt(intVec, map, activeDropPodInfo);
            return true;
        }

        // Token: 0x06000EB2 RID: 3762 RVA: 0x0006D77C File Offset: 0x0006BB7C
        private Pawn FindPawn(List<Thing> things)
        {
            for (int i = 0; i < things.Count; i++)
            {
                Pawn pawn = things[i] as Pawn;
                if (pawn != null)
                {
                    return pawn;
                }
                Corpse corpse = things[i] as Corpse;
                if (corpse != null)
                {
                    return corpse.InnerPawn;
                }
            }
            return null;
        }
    }
 
    // Token: 0x02000353 RID: 851
    public class IncidentWorker_WandererJoinXenomorph : IncidentWorker 
    {
        // Token: 0x06000EB4 RID: 3764 RVA: 0x0006D7D8 File Offset: 0x0006BBD8
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return this.TryFindEntryCell(map, out intVec);
        }

        // Token: 0x06000EB5 RID: 3765 RVA: 0x0006D808 File Offset: 0x0006BC08
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 loc;
            if (!this.TryFindEntryCell(map, out loc))
            {
                return false;
            }
            Gender? gender = null;
            if (this.def.pawnFixedGender != Gender.None)
            {
                gender = new Gender?(this.def.pawnFixedGender);
            }
            PawnKindDef pawnKind = this.def.pawnKind;
            Faction ofPlayer = Faction.OfPlayer;
            bool pawnMustBeCapableOfViolence = this.def.pawnMustBeCapableOfViolence;
            Gender? fixedGender = gender;
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKind, ofPlayer, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, pawnMustBeCapableOfViolence, 20f, false, true, true, false, false, false, false, null, null, null, null, null, fixedGender, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            pawn.health.AddHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
            GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
            string text = this.def.letterText.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            string label = this.def.letterLabel.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, pawn, null, null);
            return true;
        }

        // Token: 0x06000EB6 RID: 3766 RVA: 0x0006D95C File Offset: 0x0006BD5C
        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out cell);
        }

        // Token: 0x04000951 RID: 2385
        private const float RelationWithColonistWeight = 20f;
    }

    // Token: 0x0200034B RID: 843
    public class IncidentWorker_RefugeeChasedXenomorph : IncidentWorker 
    {
        // Token: 0x06000E90 RID: 3728 RVA: 0x0006C988 File Offset: 0x0006AD88
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            Faction faction;
            return this.TryFindSpawnSpot(map, out intVec) && this.TryFindEnemyFaction(out faction);
        }

        // Token: 0x06000E91 RID: 3729 RVA: 0x0006C9C8 File Offset: 0x0006ADC8
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 spawnSpot;
            if (!this.TryFindSpawnSpot(map, out spawnSpot))
            {
                return false;
            }
            Faction faction;
            if (!this.TryFindEnemyFaction(out faction))
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
            raidParms.points = Mathf.Max(raidParms.points * IncidentWorker_RefugeeChasedXenomorph.RaidPointsFactorRange.RandomInRange, faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat));
            raidParms.pawnGroupMakerSeed = new int?(@int);
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, raidParms, false);
            defaultPawnGroupMakerParms.points = IncidentWorker_Raid.AdjustedRaidPoints(defaultPawnGroupMakerParms.points, raidParms.raidArrivalMode, raidParms.raidStrategy, defaultPawnGroupMakerParms.faction, PawnGroupKindDefOf.Combat);
            IEnumerable<PawnKindDef> pawnKinds = PawnGroupMakerUtility.GeneratePawnKindsExample(defaultPawnGroupMakerParms);
            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, null, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 20f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
            Pawn refugee = PawnGenerator.GeneratePawn(request);
            refugee.relations.everSeenByPlayer = true;
            refugee.health.AddHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
            string text = "RefugeeChasedInitial".Translate(refugee.Name.ToStringFull, refugee.story.Title, faction.def.pawnsPlural, faction.Name, refugee.ageTracker.AgeBiologicalYears, PawnUtility.PawnKindsToCommaList(pawnKinds, true), refugee.Named("PAWN"));
            text = text.AdjustedFor(refugee, "PAWN");
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, refugee);
            DiaNode diaNode = new DiaNode(text);
            DiaOption diaOption = new DiaOption("RefugeeChasedInitial_Accept".Translate());
            diaOption.action = delegate ()
            {
                GenSpawn.Spawn(refugee, spawnSpot, map, WipeMode.Vanish);
                refugee.SetFaction(Faction.OfPlayer, null);
                CameraJumper.TryJump(refugee);
                QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms), Find.TickManager.TicksGame + IncidentWorker_RefugeeChasedXenomorph.RaidDelay.RandomInRange, 0);
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
            return true;
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

        // Token: 0x0400094B RID: 2379
        private const float RelationWithColonistWeight = 20f;
    }

    // Token: 0x02000354 RID: 852
    public class IncidentWorker_WildManWandersInXenomorph : IncidentWorker
    {
        // Token: 0x06000EB8 RID: 3768 RVA: 0x0006D9D0 File Offset: 0x0006BDD0
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Faction faction;
            if (!this.TryFindFormerFaction(out faction))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && map.mapTemperature.SeasonAcceptableFor(ThingDefOf.Human) && this.TryFindEntryCell(map, out intVec);
        }

        // Token: 0x06000EB9 RID: 3769 RVA: 0x0006DA40 File Offset: 0x0006BE40
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 loc;
            if (!this.TryFindEntryCell(map, out loc))
            {
                return false;
            }
            Faction faction;
            if (!this.TryFindFormerFaction(out faction))
            {
                return false;
            }
            Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.WildMan, faction);
            pawn.SetFaction(null, null);
            pawn.health.AddHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
            GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
            string label = this.def.letterLabel.Formatted(pawn.LabelShort, pawn.Named("PAWN"));
            string text = this.def.letterText.Formatted(pawn.LabelShort, pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
            Find.LetterStack.ReceiveLetter(label, text, this.def.letterDef, pawn, null, null);
            return true;
        }

        // Token: 0x06000EBA RID: 3770 RVA: 0x0006DB24 File Offset: 0x0006BF24
        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
        }

        // Token: 0x06000EBB RID: 3771 RVA: 0x0006DB5B File Offset: 0x0006BF5B
        private bool TryFindFormerFaction(out Faction formerFaction)
        {
            return Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out formerFaction, false, true, TechLevel.Undefined);
        }
    }
 
    // Token: 0x02000360 RID: 864
    public class IncidentWorker_QuestDownedRefugeeXenomorph : IncidentWorker
    {
        // Token: 0x06000F14 RID: 3860 RVA: 0x00070184 File Offset: 0x0006E584
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            int num;
            Faction faction;
            return base.CanFireNowSub(parms) && this.TryFindTile(out num) && SiteMakerHelper.TryFindRandomFactionFor(SiteCoreDefOf.DownedRefugee, null, out faction, true, null);
        }

        // Token: 0x06000F15 RID: 3861 RVA: 0x000701C0 File Offset: 0x0006E5C0
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            int tile;
            if (!this.TryFindTile(out tile))
            {
                return false;
            }
            Site site = SiteMaker.TryMakeSite_SingleSitePart(SiteCoreDefOf.DownedRefugee, (!Rand.Chance(0.3f)) ? "DownedRefugeeQuestThreat" : null, tile, null, true, null, true, null);
            if (site == null)
            {
                return false;
            }
            site.sitePartsKnown = true;
            Pawn pawn = DownedRefugeeQuestUtility.GenerateRefugee(tile);
            pawn.health.AddHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
            site.GetComponent<DownedRefugeeComp>().pawn.TryAdd(pawn, true);
            int randomInRange = SiteTuning.QuestSiteRefugeeTimeoutDaysRange.RandomInRange;
            site.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
            Find.WorldObjects.Add(site);
            string text = this.def.letterLabel;
            string text2 = this.def.letterText.Formatted(randomInRange, pawn.ageTracker.AgeBiologicalYears, pawn.story.Title, SitePartUtility.GetDescriptionDialogue(site, site.parts.FirstOrDefault<SitePart>()), pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
            Pawn mostImportantColonyRelative = PawnRelationUtility.GetMostImportantColonyRelative(pawn);
            if (mostImportantColonyRelative != null)
            {
                PawnRelationDef mostImportantRelation = mostImportantColonyRelative.GetMostImportantRelation(pawn);
                if (mostImportantRelation != null && mostImportantRelation.opinionOffset > 0)
                {
                    pawn.relations.relativeInvolvedInRescueQuest = mostImportantColonyRelative;
                    text2 = text2 + "\n\n" + "RelatedPawnInvolvedInQuest".Translate(mostImportantColonyRelative.LabelShort, mostImportantRelation.GetGenderSpecificLabel(pawn), mostImportantColonyRelative.Named("RELATIVE"), pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
                }
                else
                {
                    PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text2, pawn);
                }
                text = text + " " + "RelationshipAppendedLetterSuffix".Translate();
            }
            if (pawn.relations != null)
            {
                pawn.relations.everSeenByPlayer = true;
            }
            Find.LetterStack.ReceiveLetter(text, text2, this.def.letterDef, site, null, null);
            return true;
        }

        // Token: 0x06000F16 RID: 3862 RVA: 0x000703CC File Offset: 0x0006E7CC
        private bool TryFindTile(out int tile)
        {
            IntRange downedRefugeeQuestSiteDistanceRange = SiteTuning.DownedRefugeeQuestSiteDistanceRange;
            return TileFinder.TryFindNewSiteTile(out tile, downedRefugeeQuestSiteDistanceRange.min, downedRefugeeQuestSiteDistanceRange.max, true, false, -1);
        }
    }


    // Token: 0x02000366 RID: 870
    public class IncidentWorker_QuestPrisonerRescueXenomorph : IncidentWorker
    {
        // Token: 0x06000F31 RID: 3889 RVA: 0x00070C9C File Offset: 0x0006F09C
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            int num;
            SitePartDef sitePartDef;
            Faction faction;
            return base.CanFireNowSub(parms) && Find.AnyPlayerHomeMap != null && this.TryFindTile(out num) && SiteMakerHelper.TryFindSiteParams_SingleSitePart(SiteCoreDefOf.PrisonerWillingToJoin, "PrisonerRescueQuestThreat", out sitePartDef, out faction, null, true, null);
        }

        // Token: 0x06000F32 RID: 3890 RVA: 0x00070CF0 File Offset: 0x0006F0F0
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            int tile;
            if (!this.TryFindTile(out tile))
            {
                return false;
            }
            Site site = SiteMaker.TryMakeSite_SingleSitePart(SiteCoreDefOf.PrisonerWillingToJoin, "PrisonerRescueQuestThreat", tile, null, true, null, true, null);
            if (site == null)
            {
                return false;
            }
            site.sitePartsKnown = true;
            Pawn pawn = PrisonerWillingToJoinQuestUtility.GeneratePrisoner(tile, site.Faction);
            pawn.health.AddHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
            site.GetComponent<PrisonerWillingToJoinComp>().pawn.TryAdd(pawn, true);
            int randomInRange = SiteTuning.QuestSiteTimeoutDaysRange.RandomInRange;
            site.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
            Find.WorldObjects.Add(site);
            string text;
            string label;
            this.GetLetterText(pawn, site, site.parts.FirstOrDefault<SitePart>(), randomInRange, out text, out label);
            Find.LetterStack.ReceiveLetter(label, text, this.def.letterDef, site, site.Faction, null);
            return true;
        }

        // Token: 0x06000F33 RID: 3891 RVA: 0x00070DC8 File Offset: 0x0006F1C8
        private bool TryFindTile(out int tile)
        {
            IntRange prisonerRescueQuestSiteDistanceRange = SiteTuning.PrisonerRescueQuestSiteDistanceRange;
            return TileFinder.TryFindNewSiteTile(out tile, prisonerRescueQuestSiteDistanceRange.min, prisonerRescueQuestSiteDistanceRange.max, false, false, -1);
        }

        // Token: 0x06000F34 RID: 3892 RVA: 0x00070DF4 File Offset: 0x0006F1F4
        private void GetLetterText(Pawn prisoner, Site site, SitePart sitePart, int days, out string letter, out string label)
        {
            letter = this.def.letterText.Formatted(site.Faction.Name, prisoner.ageTracker.AgeBiologicalYears, prisoner.story.Title, SitePartUtility.GetDescriptionDialogue(site, sitePart), prisoner.Named("PAWN")).AdjustedFor(prisoner, "PAWN").CapitalizeFirst();
            if (PawnUtility.EverBeenColonistOrTameAnimal(prisoner))
            {
                letter = letter + "\n\n" + "PawnWasFormerlyColonist".Translate(prisoner.LabelShort, prisoner);
            }
            string text;
            PawnRelationUtility.Notify_PawnsSeenByPlayer(Gen.YieldSingle<Pawn>(prisoner), out text, true, false);
            label = this.def.letterLabel;
            if (!text.NullOrEmpty())
            {
                string text2 = letter;
                letter = string.Concat(new string[]
                {
                    text2,
                    "\n\n",
                    "PawnHasTheseRelationshipsWithColonists".Translate(prisoner.LabelShort, prisoner),
                    "\n\n",
                    text
                });
                label = label + " " + "RelationshipAppendedLetterSuffix".Translate();
            }
            letter = letter + "\n\n" + "PrisonerRescueTimeout".Translate(days, prisoner.LabelShort, prisoner.Named("PRISONER"));
        }
    }
}
