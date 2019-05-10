using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000113 RID: 275
    public class JobGiver_FacehuggerHunt : ThinkNode_JobGiver
    {
        // Token: 0x060005B7 RID: 1463 RVA: 0x00037A28 File Offset: 0x00035E28
        protected override Job TryGiveJob(Pawn pawn)
        {
            bool selected = Find.Selector.SingleSelectedThing == pawn;
            if (pawn.TryGetAttackVerb(null, false) == null)
            {
                return null;
            }
            Pawn pawn2 = FindPawnTarget(pawn);
        //    Pawn pawn2 = BestPawnToHuntForPredator(pawn, forceScanWholeMap);
            if (pawn2 != null && pawn.CanReach(pawn2, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {

#if DEBUG
                if (selected) Log.Message(string.Format("{0}@{1} hunting {2}@{3}", pawn.Label, pawn.Position, pawn2.Label, pawn2.Position));
#endif
                return this.MeleeAttackJob(pawn, pawn2);
            }
            if (pawn2 != null)
            {
                using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, pawn2.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
                {
                    if (!pawnPath.Found)
                    {
                        return null;
                    }
                    IntVec3 loc;
                    if (!pawnPath.TryFindLastCellBeforeBlockingDoor(pawn, out loc))
                    {
                        Log.Error(pawn + " did TryFindLastCellBeforeDoor but found none when it should have been one. Target: " + pawn2.LabelCap, false);
                        return null;
                    }
                    IntVec3 randomCell = CellFinder.RandomRegionNear(loc.GetRegion(pawn.Map, RegionType.Set_Passable), 9, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), null, null, RegionType.Set_Passable).RandomCell;
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
                expiryInterval = Rand.Range(420, 900),
                attackDoorIfTargetLost = false,
                killIncappedTarget=true
            };
        }

        // Token: 0x060005B9 RID: 1465 RVA: 0x00037BC0 File Offset: 0x00035FC0
        private Pawn FindPawnTarget(Pawn pawn)
        {
            Pawn pawn2 = (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn && XenomorphUtil.isInfectablePawn((Pawn)x), 0f, 10f, default(IntVec3), float.MaxValue, true, true);
            if (pawn2 == null) pawn2 = (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn && XenomorphUtil.isInfectablePawn((Pawn)x), 0f, 20f, default(IntVec3), float.MaxValue, true, true);
            if (pawn2 == null) pawn2 = (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn && XenomorphUtil.isInfectablePawn((Pawn)x), 0f, 30f, default(IntVec3), float.MaxValue, true, true);
            if (pawn2 == null) pawn2 = (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn && XenomorphUtil.isInfectablePawn((Pawn)x), 0f, 40f, default(IntVec3), float.MaxValue, true, true);
            if (pawn2 == null) pawn2 = (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn && XenomorphUtil.isInfectablePawn((Pawn)x), 0f, 50f, default(IntVec3), float.MaxValue, true, true);
            if (pawn2 == null) pawn2 = (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn && XenomorphUtil.isInfectablePawn((Pawn)x), 0f, 9999f, default(IntVec3), float.MaxValue, true, true);
            if (pawn2 == null) pawn2 = BestPawnToHuntForPredator(pawn, forceScanWholeMap);
#if DEBUG
            bool selected = Find.Selector.SingleSelectedThing == pawn;
            if (selected)
            { if (pawn2 != null) Log.Message(string.Format("{0}@{1} hunting {2}@{3}", pawn.Label, pawn.Position, pawn2.Label, pawn2.Position)); }                
#endif
            return pawn2;
        }

        // Token: 0x060005BA RID: 1466 RVA: 0x00037C14 File Offset: 0x00036014
        private Building FindTurretTarget(Pawn pawn)
        {
            return (Building)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat, (Thing t) => t is Building, 0f, 70f, default(IntVec3), float.MaxValue, false, true);
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
                        bool XenoInfection = (((Pawn)list[j]).health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection) || ((Pawn)list[j]).health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation) || ((Pawn)list[j]).health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation)) ? true : false;
                        bool IsXenos = (((Pawn)list[j]).kindDef == XenomorphDefOf.RRY_Xenomorph_Drone || ((Pawn)list[j]).kindDef == XenomorphDefOf.RRY_Xenomorph_FaceHugger || ((Pawn)list[j]).kindDef == XenomorphDefOf.RRY_Xenomorph_Queen || ((Pawn)list[j]).kindDef == XenomorphDefOf.RRY_Xenomorph_Runner || ((Pawn)list[j]).kindDef == XenomorphDefOf.RRY_Xenomorph_Warrior) ? true : false;

                        if (!XenoInfection && !IsXenos)
                        {
                            tmpPredatorCandidates.Add((Pawn)list[j]);
                        }
                        else
                        {
                        //    Log.Message(string.Format("{0} cannae hunt {2} XenoInfection:{1} IsXenos:{3}", predator.Label, XenoInfection, ((Pawn)list[j]).Label, IsXenos));
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
                                    if (!pawn2.IsForbidden(predator) && !(XenoInfection(pawn2)))
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

        public static bool XenoInfection(Pawn pawn)
        {
            bool selected = Find.Selector.SingleSelectedThing == pawn;
            if (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection))
            {
#if DEBUG
                if (selected) Log.Message(string.Format("{0}@{1} infected: {2}", pawn.Label, pawn.Position, pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection)));
#endif
                return true;
            }
            if (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation))
            {
#if DEBUG
                if (selected) Log.Message(string.Format("{0}@{1} infected: {2}", pawn.Label, pawn.Position, pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation)));
#endif
                return true;
            }
            if (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation))
            {
#if DEBUG
                if (selected) Log.Message(string.Format("{0}@{1} infected: {2}", pawn.Label, pawn.Position, pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation)));
#endif
                return true;
            }
            return false;
        }
        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey)
        {
            bool XenoInfection = XenomorphUtil.IsInfectedPawn(prey);
            bool IsXenos = XenomorphUtil.IsXenomorph(prey);

        //    Log.Message(string.Format("{0} hunting {1}? XenoInfection:{2} IsXenos:{3}", predator.Label, prey.Label, XenoInfection, IsXenos));
            if (XenoInfection)
            {
            //    Log.Message(string.Format("{0} cant hunt {1} cause XenoInfection:{2}", predator.Label, prey.Label, XenoInfection));
                return false;
            }
            if (IsXenos)
            {
            //    Log.Message(string.Format("{0} cant hunt {1} cause IsXenos:{2}", predator.Label, prey.Label, IsXenos));
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
            if (prey.BodySize < 0.7f)
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
            //Log.Message(string.Format("{0}@{1} can hunt {2}@{3}", predator.Label, predator.Position, prey.Label, prey.Position));
            return (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) && (predator.Faction == null || prey.HostFaction == null || predator.HostileTo(prey)) && (predator.Faction != Faction.OfPlayer || prey.Faction != Faction.OfPlayer) && (!predator.RaceProps.herdAnimal || predator.def != prey.def) && (!prey.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection) && !prey.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation) && !prey.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation));
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

        // Token: 0x040002FD RID: 765
        private const float WaitChance = 0.75f;

        // Token: 0x040002FE RID: 766
        private const int WaitTicks = 90;

        // Token: 0x040002FF RID: 767
        private const int MinMeleeChaseTicks = 420;

        // Token: 0x04000300 RID: 768
        private const int MaxMeleeChaseTicks = 900;

        // Token: 0x04000301 RID: 769
        private const int WanderOutsideDoorRegions = 9;
    }
}
