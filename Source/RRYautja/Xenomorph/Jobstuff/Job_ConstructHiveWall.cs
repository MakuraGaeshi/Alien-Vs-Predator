using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000A4 RID: 164
    public class JobGiver_ConstructHive : ThinkNode_JobGiver
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_ConstructHive jobGiver_ConstructHive = (JobGiver_ConstructHive)base.DeepCopy(resolve);
            jobGiver_ConstructHive.MiningRange = this.MiningRange;
            return jobGiver_ConstructHive;
        }

        private int MiningRange = 5;

        // Token: 0x0600041E RID: 1054 RVA: 0x0002CA54 File Offset: 0x0002AE54
        protected override Job TryGiveJob(Pawn pawn)
        {
            List<IntVec3> HiveStruct = new List<IntVec3>()
            {
                // Cardinals
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 3,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 3,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z + 3
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z - 3
                },
                // Corners
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 2,
                    z = pawn.mindState.duty.focus.Cell.z + 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 2,
                    z = pawn.mindState.duty.focus.Cell.z + 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 2,
                    z = pawn.mindState.duty.focus.Cell.z - 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 2,
                    z = pawn.mindState.duty.focus.Cell.z - 2
                }
            };
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            if (region == null)
            {
                return null;
            }
            for (int i = 0; i < 40; i++)
            {
                IntVec3 randomCell = HiveStruct.RandomElement();
                for (int j = 0; j < 4; j++)
                {
                    IntVec3 c = randomCell;// + GenAdj.CardinalDirections[j];
                    if (c.InBounds(pawn.Map) && c.Roofed(pawn.Map))
                    {

                        Building edifice = c.GetEdifice(pawn.Map);
                        if (edifice == null && XenomorphUtil.DistanceBetween(c, pawn.mindState.duty.focus.Cell) <= MiningRange)
                        {
                            return new Job(XenomorphDefOf.RRY_Job_ConstructHiveWall, c)
                            {
                                ignoreDesignations = true
                            };
                        }
                        /*
                        else if (edifice==null)
                        {
                            if (!pillarLoc.Contains(edifice.Position))
                            {
                                return new Job(JobDefOf.Mine, edifice)
                                {
                                    ignoreDesignations = true
                                };
                            }
                        }
                        */
                    }
                }
            }
            return null;
        }

    }

    // Token: 0x02000089 RID: 137
    public class JobDriver_ConstructHiveWall : JobDriver
    {
        // Token: 0x06000390 RID: 912 RVA: 0x00024538 File Offset: 0x00022938
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.useDuration, "useDuration", 0, false);
        }

        // Token: 0x06000391 RID: 913 RVA: 0x00024554 File Offset: 0x00022954
        public override void Notify_Starting()
        {
            base.Notify_Starting();
        }

        // Token: 0x06000392 RID: 914 RVA: 0x00024590 File Offset: 0x00022990
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        public ThingDef MyDef = XenomorphDefOf.RRY_Xenomorph_HiveWall;

        // Token: 0x06000393 RID: 915 RVA: 0x000245C8 File Offset: 0x000229C8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(this.useDuration, TargetIndex.None);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return prepare;
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                Thing thing = ThingMaker.MakeThing(MyDef);
                GenSpawn.Spawn(thing, TargetA.Cell, actor.Map, Rot4.South, WipeMode.FullRefund, false);
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }

        // Token: 0x0400024D RID: 589
        private int useDuration = 600;
    }
}
