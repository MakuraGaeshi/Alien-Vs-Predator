﻿using RimWorld;
using RRYautja.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RRYautja
{
    public class CompProperties_Xenomorph : CompProperties
    {
        public CompProperties_Xenomorph()
        {
            this.compClass = typeof(Comp_Xenomorph);
        }

        public int healIntervalTicks = 60;
    }

    public class Comp_Xenomorph : ThingComp, IThoughtGiver
    {
        public PawnKindDef HuggerKindDef = XenomorphDefOf.RRY_Xenomorph_FaceHugger;
        public PawnKindDef RoyaleKindDef = XenomorphDefOf.RRY_Xenomorph_RoyaleHugger;

        public PawnKindDef QueenDef = XenomorphDefOf.RRY_Xenomorph_Queen;
        public CompProperties_Xenomorph Props
        {
            get
            {
                return (CompProperties_Xenomorph)this.props;
            }
        }

        public bool Hidden
        {
            get
            {
                if (pawn.)
                {

                }
                return hidden;
            }
            set
            {
                hidden = value;
            }
        }
        private bool hidden = false;
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceHeal, "ticksSinceHeal");
            Scribe_Values.Look<IntVec3>(ref this.HiveLoc, "HiveLoc", IntVec3.Invalid);
            Scribe_Defs.Look<PawnKindDef>(ref this.host, "hostRef");
            Scribe_Values.Look<bool>(ref this.hidden, "Hidden");
        }

        public IntVec3 HiveLoc;

        public Pawn pawn
        {
            get
            {
                return (Pawn)parent;
            }
        }

        public Map map
        {
            get
            {
                return pawn.Map != null ? pawn.Map : pawn.MapHeld;
            }
        }

        public float MinHideDist
        {
            get
            {
                return (10 * pawn.BodySize) * pawn.Map.glowGrid.GameGlowAt(pawn.Position, false);
            }
        }

        public Thought_Memory GiveObservedThought()
        {
            string concept = string.Format("RRY_Concept_{0}s", pawn.def.label);
            ConceptDef conceptDef = null;
            conceptDef = DefDatabase<ConceptDef>.GetNamedSilentFail(concept);
            if (conceptDef!=null)
            {
                if (!PlayerKnowledgeDatabase.IsComplete(conceptDef))
                {
                    string thought = string.Format("RRY_Concept_{0}s", pawn.def.label);
                    ThoughtDef thoughtDef = DefDatabase<ThoughtDef>.GetNamedSilentFail(thought);
                    if (thoughtDef!=null)
                    {
                        Thought_MemoryObservation observation =

                    }
                      
                 //   LessonAutoActivator.TeachOpportunity(conceptDef, OpportunityType.Important);
                }
                else
                {

                }
                if (this.StoringThing() == null)
                {
                    Thought_MemoryObservation thought_MemoryObservation;
                    if (this.IsNotFresh())
                    {
                        thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(ThoughtDefOf.ObservedLayingRottingCorpse);
                    }
                    else
                    {
                        thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(ThoughtDefOf.ObservedLayingCorpse);
                    }
                    thought_MemoryObservation.Target = this;
                    return thought_MemoryObservation;
                }
                return null;
            }
            throw new NotImplementedException();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_XenomorphFullyFormed)
            {
                hidden = true;
            }
        }

        public void XenoLordTick()
        {
            if (map != null)
            {
                IntVec3 c = IntVec3.Invalid;
                Lord lord = null;
                List<Lord> Hivelords = new List<Lord>();
                Lord Hivelord = null;
                LordJob Hivejob = null;
                Pawn Hivequeen = null;
                IEnumerable<Lord> lords = pawn.Map.lordManager.lords.Where(x => x.faction == pawn.Faction);
                if (lords.Count() != 0 && ((pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendPoint) || pawn.GetLord() == null))
                {
                    foreach (var l in lords)
                    {
                        if (l != null)
                        {
                            if (XenomorphUtil.HivelikesPresent(map))
                            {
                                if (l.LordJob is LordJob_DefendAndExpandHiveLike j)
                                {
                                    Hivelord = l;
                                    Hivejob = j;
                                    if (l.ownedPawns.Any(x => x.kindDef == QueenDef))
                                    {
                                        Hivequeen = l.ownedPawns.Find(x => x.kindDef == QueenDef);
                                    }
                                    if (pawn.kindDef != XenomorphDefOf.RRY_Xenomorph_Queen || (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen && Hivequeen != null))
                                    {
                                        Hivelords.Add(l);
                                    }
                                }
                            }
                            else if (XenomorphUtil.HiveSlimePresent(map))
                            {
                                if (l.LordJob is LordJob_DefendHiveLoc j)
                                {
                                    Hivelord = l;
                                    Hivejob = j;
                                    if (l.ownedPawns.Any(x => x.kindDef == QueenDef))
                                    {
                                        Hivequeen = l.ownedPawns.Find(x => x.kindDef == QueenDef);
                                    }
                                    if (pawn.kindDef != XenomorphDefOf.RRY_Xenomorph_Queen || (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen && Hivequeen != null))
                                    {
                                        Hivelords.Add(l);
                                    }
                                }
                            }
                            
                        }
                        else
                        {
                            /*
                            lord = l;
                            lord.AddPawn(pawn);
                            pawn.mindState.duty = lord.ownedPawns.FindAll(x => x.mindState.duty != null && x != pawn).RandomElement().mindState.duty;
                            break;
                            */
                        }
                    }
                }
                if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendPoint LordJob_DefendPoint)
                {
                    lord = pawn.GetLord();
                    if (lord.ownedPawns.Count == 0)
                    {
                        Log.Message(string.Format("got no pawns, wtf?"));
                    }
                    if (lord.ownedPawns.Count == 1)
                    {

                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.HivelikesPresent(map))
                    {
                        c = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull() ? XenomorphUtil.ClosestReachableHivelike(pawn).Position : IntVec3.Invalid;
                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.EggsPresent(map))
                    {
                        c = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull() ? XenomorphUtil.ClosestReachableEgg(pawn).Position : IntVec3.Invalid;
                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.CocoonsPresent(map, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon))
                    {
                        c = !XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon).DestroyedOrNull() ? XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon).Position : IntVec3.Invalid;
                    }
                    if (c == IntVec3.Invalid && XenomorphUtil.CocoonsPresent(map, XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon))
                    {
                        c = !XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon).DestroyedOrNull() ? XenomorphUtil.ClosestReachableCocoon(pawn, XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon).Position : IntVec3.Invalid;
                    }
                    if (c == IntVec3.Invalid)
                    {
                        if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map, false))
                        {
                            if (Prefs.DevMode)
                            {
                                ThingDef td = XenomorphDefOf.RRY_Filth_Slime;
                                GenSpawn.Spawn(td, c, pawn.Map);
                                Find.LetterStack.ReceiveLetter(string.Format("Lord Created"), string.Format("@: {0} ", c), LetterDefOf.NegativeEvent, c.GetFirstThing(pawn.Map, td), null, null);
                            }
                        }
                        if (pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly, true))
                        {
                                c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                        }
                        else
                        {
                            c = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 3f, null, Danger.Some);
                        }
                    }
                    if (c != IntVec3.Invalid)
                    {
                        LordJob newJob;
                        if (XenomorphUtil.HivelikesPresent(map))
                        {
                            newJob = new LordJob_DefendAndExpandHiveLike(false);
                        }
                        else
                        {
                            newJob = new LordJob_DefendHiveLoc(parent.Faction, c);
                        }
                        if (pawn.GetLord().LordJob is LordJob_DefendPoint)
                        {
                            if (!Hivelords.NullOrEmpty())
                            {
                                Hivelord = Hivelords.RandomElement();
                                SwitchToLord(Hivelord);
                                CreateNewLord(pawn, c, newJob);
                            }
                            else
                            {
                                CreateNewLord(pawn, c, newJob);
                            }
                            if (HiveLoc == IntVec3.Invalid) HiveLoc = c;
                        }
                    }
                }
                else if (c == IntVec3.Invalid && (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob LordJob))
                {
                    lord = pawn.GetLord();
                    c = LordJob.lord.Graph.StartingToil.FlagLoc;
                    if (c == IntVec3.Invalid)
                    {
                        c = LordJob.lord.CurLordToil.FlagLoc;
                    }
                }
                else if (pawn.GetLord() == null)
                {
                    if (!Hivelords.NullOrEmpty())
                    {
                        Hivelord = Hivelords.RandomElement();
                        SwitchToLord(Hivelord);
                    }
                }
                if (pawn.GetLord()!=null)
                {
                    List<Pawn> list = pawn.GetLord().ownedPawns.Where(x => x.mindState.duty != null).ToList();
                    if (pawn.GetLord() != null && pawn.mindState.duty == null && !list.NullOrEmpty())
                    {
                        pawn.mindState.duty = list.RandomElement().mindState.duty;
                    }
                }
            }
        }

        private Lord SwitchToLord(Lord lord)
        {
            if (pawn.GetLord() != null && pawn.GetLord() is Lord l)
            {
                if (l.ownedPawns.Count > 0)
                {
                    l.ownedPawns.Remove(pawn);
                }
                if (l.ownedPawns.Count == 0)
                {
                    l.lordManager.RemoveLord(l);
                }
            }
            lord.AddPawn(pawn);
            return lord;
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            bool selected = Find.Selector.SelectedObjects.Contains(pawn);
            Lord lord = pawn.GetLord();
            Faction xenos = Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph);
            
            if (pawn != null && pawn.Map != null && !pawn.Dead && pawn.kindDef != XenomorphDefOf.RRY_Xenomorph_FaceHugger && pawn.kindDef != XenomorphDefOf.RRY_Xenomorph_RoyaleHugger)
            {
                LifeStageDef stage = pawn.ageTracker.CurLifeStage;
                if (stage == pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def)
                {
                    if (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden))
                    {
                        string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_Matures",pawn.LabelCap);
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden);
                        MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, text, 3f);
                        pawn.health.RemoveHediff(hediff);
                    }
                    XenoLordTick();
                }
                else
                {
                    Thing thing = GenClosest.ClosestThingReachable(this.parent.Position, this.parent.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.Touch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 6f, x => ((Pawn)this.parent).HostileTo((Pawn)x)&&!((Pawn)x).health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned), null, 0, -1, false, RegionType.Set_Passable, false);
                    if (!((Pawn)this.parent).health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden) && thing==null)
                    {
                        string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_Hides");

                        MoteMaker.ThrowText(base.parent.Position.ToVector3(), base.parent.Map, text, 3f);
                        ((Pawn)this.parent).health.AddHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden);
                    }
                }
            }

        }

        private Lord CreateNewLord(Pawn pawn)
        {
            IntVec3 c;
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            if (thing != null)
            {
                c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 5f, null, Danger.Some);
            }
            else
            {
                if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map, false))
                {
                    if (Prefs.DevMode && Find.Selector.SelectedObjects.Contains(pawn))
                    {
                        ThingDef td = XenomorphDefOf.RRY_Filth_Slime;
                        GenSpawn.Spawn(td, c, pawn.Map);
                        Find.LetterStack.ReceiveLetter(string.Format("Lord Created"), string.Format("@: {0} ", c), LetterDefOf.NegativeEvent, c.GetFirstThing(pawn.Map, td), null, null);
                    }
                }
                if (pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly, true))
                {
                //    c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                }
                else
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 3f, null, Danger.Some);
                }
            }
            if (pawn.GetLord() != null && pawn.GetLord() is Lord l)
            {
                if (l.ownedPawns.Count > 0)
                {
                    l.ownedPawns.Remove(pawn);
                }
                if (l.ownedPawns.Count == 0)
                {
                    l.lordManager.RemoveLord(l);
                }
            }
            Lord lord = LordMaker.MakeNewLord(parent.Faction, new LordJob_DefendAndExpandHiveLike(false), parent.Map, null);
            lord.AddPawn(pawn);
            return lord;
        }

        private Lord CreateNewLord(Pawn pawn, IntVec3 loc, LordJob lordJob)
        {
            IntVec3 c = loc;
            if (pawn.GetLord() != null && pawn.GetLord() is Lord l)
            {
                if (l.ownedPawns.Count > 0)
                {
                    l.ownedPawns.Remove(pawn);
                }
                if (l.ownedPawns.Count == 0)
                {
                    l.lordManager.RemoveLord(l);
                }
            }
            Lord lord = LordMaker.MakeNewLord(parent.Faction, lordJob, map, null);
            lord.AddPawn(pawn);
            return lord;
        }

        public int healIntervalTicks = 60;
        public override void CompTick()
        {
            if (pawn.Faction==null)
            {
                if (Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph) != null)
                {
                    pawn.SetFaction(Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph));
                }
            }
            base.CompTick();
            this.ticksSinceHeal++;
            bool flag = this.ticksSinceHeal > this.healIntervalTicks;
            if (flag)
            {
                bool flag2 = ((Pawn)base.parent).health.hediffSet.HasNaturallyHealingInjury();
                if (flag2)
                {
                    this.ticksSinceHeal = 0;
                    float num = 8f;
                    Hediff_Injury hediff_Injury = GenCollection.RandomElement<Hediff_Injury>(from x in ((Pawn)base.parent).health.hediffSet.GetHediffs<Hediff_Injury>()
                                                                                             where HediffUtility.CanHealNaturally(x)
                                                                                             select x);
                    doClot(pawn);
                    hediff_Injury.Heal(num * ((Pawn)base.parent).HealthScale * 0.01f);
                    string text = string.Format("{0} healed.", ((Pawn)base.parent).LabelCap);
                }
            }
        }


        public static void doClot(Pawn pawn, BodyPartRecord part = null)
        {
            var i = 5;
            foreach (var hediff in pawn.health.hediffSet.hediffs.Where(x => x.Bleeding).OrderByDescending(x => x.BleedRate))
            {
                hediff.Tended(Math.Min(Rand.Value + Rand.Value + Rand.Value, 1f));
                i--;

                if (i <= 0) return;
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            Pawn other = null;
            Pawn pawn = base.parent as Pawn;
            if (dinfo.Instigator!=null)
            {
                other = dinfo.Instigator as Pawn;
            }
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            if (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden))
            {
                pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden));
            }


        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            bool acidburns = true;
            if (base.parent is Pawn pawn && pawn != null)
            {
                if (dinfo.Def!=null)
                {
                    if (dinfo.Def.hediff == DefDatabase<HediffDef>.GetNamedSilentFail("CP_CQCTakedownHediff"))
                    {
                        absorbed = true;
                        return;
                    }
                    if (!dinfo.Def.isRanged)
                    {
                        if (dinfo.Instigator is Pawn Instigator && Instigator != null && Instigator != pawn && Instigator.AdjacentTo8WayOrInside(pawn))
                        {
                            if (dinfo.Weapon is ThingDef WeaponDef && WeaponDef != null)
                            {
                                if (WeaponDef.IsWeapon)
                                {
                                    if (WeaponDef == Instigator.equipment.Primary.def && Instigator.equipment.Primary is ThingWithComps Weapon && Instigator.equipment.PrimaryEq is CompEquippable WeaponEQ)
                                    {
                                        if (WeaponDef.MadeFromStuff && Weapon.Stuff is ThingDef WeaponStuff)
                                        {
                                            if (WeaponStuff.defName.Contains("RRY_Xeno"))
                                            {
                                                acidburns = false;
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in Weapon.def.costList)
                                            {
                                                if (item.thingDef == XenomorphDefOf.RRY_Xenomorph_TailSpike || item.thingDef == XenomorphDefOf.RRY_Xenomorph_HeadShell)
                                                {
                                                    acidburns = false;
                                                }
                                            }
                                        }
                                        if (acidburns)
                                        {
                                            Weapon.HitPoints -= Rand.Range(0, 5);
                                            if (Weapon.HitPoints <= 0)
                                            {
                                                Weapon.Destroy();
                                            }
                                        }
                                    }
                                }
                            }
                            if (Rand.Chance(0.25f) && Instigator.Map!=null)
                            {
                                FilthMaker.MakeFilth(Instigator.Position, Instigator.Map, pawn.RaceProps.BloodDef, pawn.LabelIndefinite(), 1);
                            }
                        }
                    }
                    else if (dinfo.Def.isRanged)
                    {
#if DEBUG
                        Log.Message(string.Format("{0} is ranged: {1}", dinfo.Weapon.LabelCap, dinfo.Def.isRanged));
#endif
                    }
                    else
                    {
#if DEBUG
                        Log.Message(string.Format("{0} is unknown", dinfo.Weapon.LabelCap));
#endif
                    }
                }
            }
            base.PostPreApplyDamage(dinfo, out absorbed);
        }

        // Token: 0x06000186 RID: 390 RVA: 0x0000E940 File Offset: 0x0000CD40
        public Pawn BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap, bool findhost = false)
        {
            if (predator.meleeVerbs.TryGetMeleeVerb(null) == null)
            {
                return null;
            }
            bool flag = false;
            float summaryHealthPercent = predator.health.summaryHealth.SummaryHealthPercent;
            if (summaryHealthPercent < 0.5f)
            {
                flag = true;
            }
            tmpPredatorCandidates.Clear();
            int maxRegionsToScan = GetMaxRegionsToScan(predator, forceScanWholeMap);
            if (maxRegionsToScan < 0)
            {
                tmpPredatorCandidates.AddRange(predator.Map.mapPawns.AllPawnsSpawned);
            }
            else
            {
                TraverseParms traverseParms = TraverseParms.For(predator, Danger.Deadly, TraverseMode.ByPawn, false);
                RegionTraverser.BreadthFirstTraverse(predator.Position, predator.Map, (Region from, Region to) => to.Allows(traverseParms, true), delegate (Region x)
                {
                    List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                    for (int j = 0; j < list.Count; j++)
                    {
                        tmpPredatorCandidates.Add((Pawn)list[j]);
                    }
                    return false;
                }, 999999, RegionType.Set_Passable);
            }
            Pawn pawn = null;
            float num = 0f;
            bool tutorialMode = TutorSystem.TutorialMode;
            for (int i = 0; i < tmpPredatorCandidates.Count; i++)
            {
                Pawn pawn2 = tmpPredatorCandidates[i];
                if (predator.GetRoom(RegionType.Set_Passable) == pawn2.GetRoom(RegionType.Set_Passable))
                {
                    if (predator != pawn2)
                    {
                        if (!flag || pawn2.Downed)
                        {
                            if (IsAcceptablePreyFor(predator, pawn2, findhost))
                            {
                                if (predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
                                {
                                    if (!pawn2.IsForbidden(predator))
                                    {
                                        if (!tutorialMode || pawn2.Faction != Faction.OfPlayer)
                                        {
                                            float preyScoreFor = GetPreyScoreFor(predator, pawn2, findhost);
                                            if (preyScoreFor > num || pawn == null)
                                            {
                                                num = preyScoreFor;
                                                pawn = pawn2;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            tmpPredatorCandidates.Clear();
            bool selected = Find.Selector.SingleSelectedThing == predator && Prefs.DevMode;
            if (selected)
            {
                if (pawn != null)
                {
                    Log.Message(string.Format("{0} hunting: {1}, @: {2}", predator.LabelShortCap, pawn.LabelShortCap, pawn.Position));
                }
                else
                {
                    Log.Message(string.Format("{0} Could not find a suitable pawn to hunt", predator.LabelShortCap));
                }
            }
            return pawn;
        }

        // Token: 0x06000187 RID: 391 RVA: 0x0000EB14 File Offset: 0x0000CF14
        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey, bool findhost)
        {
            if (prey.isXenomorph())
            {
                return false;
            }
            if (prey.isCocooned())
            {
                return false;
            }
            if (findhost)
            {
                if (prey.Downed)
                {
                    return false;
                }
                if (!prey.isPotentialHost())
                {
                    return false;
                }
            }
            if (!findhost)
            {
                if (prey.isPotentialHost())
                {
                    return false;
                }
            }
            /*
            if (!prey.RaceProps.canBePredatorPrey)
            {
                return false;
            }
            */
            /*
            if (!prey.RaceProps.IsFlesh)
            {
                return false;
            }
            */
            /*
            if (!Find.Storyteller.difficulty.predatorsHuntHumanlikes && prey.RaceProps.Humanlike)
            {
                return false;
            }
            */
            if (prey.BodySize > predator.RaceProps.maxPreyBodySize)
            {
                return false;
            }
            if (!prey.Downed)
            {
                if (prey.kindDef.combatPower > 2f * predator.kindDef.combatPower)
                {
                    return false;
                }
                float num = prey.kindDef.combatPower * prey.health.summaryHealth.SummaryHealthPercent * prey.ageTracker.CurLifeStage.bodySizeFactor;
                float num2 = predator.kindDef.combatPower * predator.health.summaryHealth.SummaryHealthPercent * predator.ageTracker.CurLifeStage.bodySizeFactor;
                if (num >= num2)
                {
                    return false;
                }
            }
            return (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) && (predator.Faction == null || prey.HostFaction == null || predator.HostileTo(prey)) && (predator.Faction != Faction.OfPlayer || prey.Faction != Faction.OfPlayer) && (!predator.RaceProps.herdAnimal || predator.def != prey.def);
        }

        // Token: 0x06000180 RID: 384 RVA: 0x0000E414 File Offset: 0x0000C814
        private static int GetMaxRegionsToScan(Pawn getter, bool forceScanWholeMap)
        {
            if (getter.RaceProps.Humanlike)
            {
                return -1;
            }
            if (forceScanWholeMap)
            {
                return -1;
            }
            if (getter.Faction == Faction.OfPlayer)
            {
                return 100;
            }
            return 30;
        }

        // Token: 0x06000188 RID: 392 RVA: 0x0000ECA4 File Offset: 0x0000D0A4
        public static float GetPreyScoreFor(Pawn predator, Pawn prey, bool findhost)
        {
            float offset = XenomorphUtil.TotalSpawnedXenomorphPawnCount(prey.Map);
            float num = prey.kindDef.combatPower / predator.kindDef.combatPower;
            float num2 = prey.health.summaryHealth.SummaryHealthPercent;
            float bodySizeFactor = prey.ageTracker.CurLifeStage.bodySizeFactor / predator.ageTracker.CurLifeStage.bodySizeFactor;
            float lengthHorizontal = (predator.Position - prey.Position).LengthHorizontal;
            if (prey.Downed)
            {
                num2 = Mathf.Min(num2, 0.2f);
            }
            float num3 = -lengthHorizontal - 56f * num2 * num2 * num * bodySizeFactor;
            float num4 = -56f * num2 * num2 * num * bodySizeFactor;
            if (prey.isHost())
            {
                if (prey.isXenoHost())
                {
                    num3 -= 350f;
                }
                if (prey.isNeoHost() && findhost)
                {
                    num3 -= 350f;
                }
            }
            if (prey.isNeomorph())
            {
                num3 -= 350f;
            }
            if (prey.isPotentialHost())
            {
                num3 += 20f;
            }
            if (prey.RaceProps.Humanlike)
            {
                num3 -= 35f;
            }
            num3 += offset*3;
            bool selected = Find.Selector.SelectedObjects.Contains(predator) && Prefs.DevMode;
            if (selected)
            {
                Log.Message(string.Format("{0} found: {1} @: {2}\nPreyScore: {3}, BFPreyScore: {4}, isXenoHost: {5}, isNeoHost: {6}, isXenomorph: {7}, isNeomorph: {8}, isPotentialHost: {9}, Humanlike: {10}", predator.LabelShortCap, prey.LabelShortCap, prey.Position, num3, num4, prey.isXenoHost(), prey.isNeoHost(), prey.isXenomorph(), prey.isNeomorph(), prey.isPotentialHost(), prey.RaceProps.Humanlike));
            }
            return num3;
        }

        private static List<Pawn> tmpPredatorCandidates = new List<Pawn>();
        public PawnKindDef host;
        public int ticksSinceHeal;
    }
}
