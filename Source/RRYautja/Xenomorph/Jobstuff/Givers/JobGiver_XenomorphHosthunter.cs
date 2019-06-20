﻿using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000113 RID: 275
    public class JobGiver_XenomorphHosthunter : ThinkNode_JobGiver
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_XenomorphHosthunter jobGiver_XenomorphHosthunter = (JobGiver_XenomorphHosthunter)base.DeepCopy(resolve);
            jobGiver_XenomorphHosthunter.HuntingRange = this.HuntingRange;
            jobGiver_XenomorphHosthunter.MinMeleeChaseTicks = this.MinMeleeChaseTicks;
            jobGiver_XenomorphHosthunter.MaxMeleeChaseTicks = this.MaxMeleeChaseTicks;
            jobGiver_XenomorphHosthunter.Gender = this.Gender;
            return jobGiver_XenomorphHosthunter;
        }

        // Token: 0x040002FF RID: 767
        private int MinMeleeChaseTicks = 420;

        // Token: 0x04000300 RID: 768
        private int MaxMeleeChaseTicks = 900;

        private float HuntingRange = 9999f;

        // Token: 0x040002FD RID: 765
        private const float WaitChance = 0.75f;

        // Token: 0x040002FE RID: 766
        private const int WaitTicks = 90;

        // Token: 0x04000301 RID: 769
        private const int WanderOutsideDoorRegions = 9;

        private Gender Gender = Gender.None;
        // Token: 0x060005B7 RID: 1463 RVA: 0x00037A28 File Offset: 0x00035E28
        protected override Job TryGiveJob(Pawn pawn)
        {
            /*
            if (pawn.mindState.duty.def == OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike && pawn.mindState.duty.radius > 0)
            {
                HuntingRange = pawn.mindState.duty.radius;
            }
            */

            if (pawn.TryGetAttackVerb(null, false) == null)
            {
                return null;
            }
            Pawn pawn2 = this.FindPawnTarget(pawn);

            if (pawn2 != null && pawn.CanReach(pawn2, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                return this.MeleeAttackJob(pawn, pawn2);
            }
            if (pawn2 != null)
            {
                using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, pawn2.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors, true), PathEndMode.OnCell))
                {
                    if (!pawnPath.Found)
                    {
                        return null;
                    }
                    if (!pawnPath.TryFindLastCellBeforeBlockingDoor(pawn, out IntVec3 loc))
                    {
                        //    Log.Error(pawn + " did TryFindLastCellBeforeDoor but found none when it should have been one. Target: " + pawn2.LabelCap, false);
                        return null;
                    }
                    IntVec3 randomCell = CellFinder.RandomRegionNear(loc.GetRegion(pawn.Map, RegionType.Set_Passable), 9, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, true), null, null, RegionType.Set_Passable).RandomCell;
                    if (randomCell == pawn.Position)
                    {
                        return new Job(JobDefOf.Wait, 30, false);
                    }
                    return new Job(JobDefOf.Goto, randomCell);
                }
            }
            return null;
        }

        // Token: 0x060005B8 RID: 1464 RVA: 0x00037B7C File Offset: 0x00035F7C
        private Job MeleeAttackJob(Pawn pawn, Thing target)
        {
            return new Job(JobDefOf.AttackMelee, target)
            {
                maxNumMeleeAttacks = 1,
                expiryInterval = Rand.Range(MinMeleeChaseTicks, MaxMeleeChaseTicks),
                killIncappedTarget = false,
                attackDoorIfTargetLost = true
            };
        }

        // Token: 0x060005B9 RID: 1465 RVA: 0x00037BC0 File Offset: 0x00035FC0
        private Pawn FindPawnTarget(Pawn pawn)
        {
            bool selected = Find.Selector.SingleSelectedThing == pawn;
            List<Pawn> list = pawn.Map.mapPawns.AllPawns.Where((Pawn x) => !x.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) && !x.Downed && XenomorphUtil.isInfectablePawn(x) && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.NoPassClosedDoors) && !pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Anesthetic) && (this.Gender == Gender.None || (this.Gender!=Gender.None && x.gender == this.Gender))).ToList();
            if (list.NullOrEmpty())
            {
                return null;
            }
            return (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), HuntingRange, (x => x is Pawn p && list.Contains(p)));//(Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn p && XenomorphUtil.isInfectablePawn(p) && !p.Downed, 0f, 9999f, default(IntVec3), float.MaxValue, true, true);
        }

        // RimWorld.FoodUtility
        private static Pawn BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
        {
            if (predator.meleeVerbs.TryGetMeleeVerb(null) == null)
            {
                return null;
            }
            bool flag = false;
            float summaryHealthPercent = predator.health.summaryHealth.SummaryHealthPercent;
            if (summaryHealthPercent < 0.25f)
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
                        Pawn p = ((Pawn)list[j]);
                        if (XenomorphUtil.isInfectablePawn(p))
                        {
                            tmpPredatorCandidates.Add(p);
                        }
                        else
                        {
                            //    Log.Message(string.Format("{0} cannae hunt {2} XenoInfection:{1} IsXenos:{3}", predator.Label, XenomorphUtil.IsInfectedPawn(p), ((Pawn)list[j]).Label, XenomorphUtil.IsXenomorph(p)));
                        }
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
                            if (IsAcceptablePreyFor(predator, pawn2))
                            {
                                if (predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
                                {
                                    if (!pawn2.IsForbidden(predator))
                                    {
                                        if (!tutorialMode || pawn2.Faction != Faction.OfPlayer)
                                        {
                                            float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2);
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
            return pawn;
        }

        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey)
        {

            //    Log.Message(string.Format("{0} hunting {1}? XenoInfection:{2} IsXenos:{3}", predator.Label, prey.Label, XenomorphUtil.IsInfectedPawn(prey), XenomorphUtil.IsXenomorph(prey)));
            if (XenomorphUtil.IsInfectedPawn(prey))
            {
                //    Log.Message(string.Format("{0} cant hunt {1} cause XenoInfection:{2}", predator.Label, prey.Label, XenomorphUtil.IsInfectedPawn(prey)));
                return false;
            }
            if (XenomorphUtil.IsXenomorph(prey))
            {
                //    Log.Message(string.Format("{0} cant hunt {1} cause IsXenos:{2}", predator.Label, prey.Label, XenomorphUtil.IsXenomorph(prey)));
                return false;
            }
            if (!prey.RaceProps.IsFlesh)
            {
                return false;
            }
            if (prey.BodySize > predator.RaceProps.maxPreyBodySize)
            {
                return false;
            }
            if (prey.BodySize < 0.65f)
            {
                return false;
            }
            /*
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
            */
            //    Log.Message(string.Format("{0}@{1} can hunt {2}@{3}", predator.Label, predator.Position, prey.Label, prey.Position));
            return (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) && (predator.Faction == null || prey.HostFaction == null || predator.HostileTo(prey)) && (predator.Faction != Faction.OfPlayer || prey.Faction != Faction.OfPlayer) && (!predator.RaceProps.herdAnimal || predator.def != prey.def) && (!XenomorphUtil.IsInfectedPawn(prey));
        }

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

        private static List<Pawn> tmpPredatorCandidates = new List<Pawn>();

        public bool forceScanWholeMap = true;
        
    }
}
