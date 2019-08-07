using RRYautja;
using RRYautja.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

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
            jobGiver_XenomorphHosthunter.requireLOS = this.requireLOS;
            return jobGiver_XenomorphHosthunter;
        }

        public HiveLike hive = null;

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

        private bool requireLOS = true;
        // Token: 0x060005B7 RID: 1463 RVA: 0x00037A28 File Offset: 0x00035E28
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.GetLord() != null && pawn.GetLord() is Lord lord)
            {
                if (lord.LordJob is LordJob_DefendAndExpandHiveLike hivejob)
                {
                    if (lord.CurLordToil is LordToil_DefendAndExpandHiveLike hivetoil)
                    {
                        if (hivetoil.Data.assignedHiveLikes.TryGetValue(pawn) != null)
                        {
                            hive = hivetoil.Data.assignedHiveLikes.TryGetValue(pawn);
                        }
                    }
                }
                if (lord.CurLordToil is LordToil_DefendHiveLikeAggressively hivetoilA)
                {
                    if (hivetoilA.Data.assignedHiveLikes.TryGetValue(pawn) != null)
                    {
                        hive = hivetoilA.Data.assignedHiveLikes.TryGetValue(pawn);
                    }
                }
            }

            bool aggressive;
            Comp_Xenomorph _Xenomorph = pawn.TryGetComp<Comp_Xenomorph>();
            if (pawn.TryGetAttackVerb(null, false) == null)
            {
                return null;
            }


            Pawn pawn2 = null;
            if (_Xenomorph!=null && hive.hiveDormant)
            {
                pawn2 = _Xenomorph.BestPawnToHuntForPredator(pawn, false, true);
            }
            if (pawn2 == null)
            {
                pawn2 = this.FindPawnTarget(pawn);
            }
            if (pawn2 == null)
            {
                return null;
            }
            if (!pawn2.isPotentialHost())
            {
                return null;
            }
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
            List<Pawn> list = pawn.Map.mapPawns.AllPawns.Where((Pawn x) => !x.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) && !x.Downed && XenomorphUtil.isInfectablePawn(x) && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.NoPassClosedDoors) && !pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Anesthetic)&& ((requireLOS && pawn.CanSee(x)) || !requireLOS) && (this.Gender == Gender.None || (this.Gender!=Gender.None && x.gender == this.Gender))).ToList();
            if (list.NullOrEmpty())
            {
                return null;
            }
            return (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), HuntingRange, (x => x is Pawn p && list.Contains(p)));//(Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable, (Thing x) => x is Pawn p && XenomorphUtil.isInfectablePawn(p) && !p.Downed, 0f, 9999f, default(IntVec3), float.MaxValue, true, true);
        }
        
        public bool forceScanWholeMap = true;
        
    }
}
