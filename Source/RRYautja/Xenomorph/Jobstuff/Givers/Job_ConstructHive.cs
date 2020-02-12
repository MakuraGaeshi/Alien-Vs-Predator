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

        private int MiningRange;
        public List<IntVec3> HiveStruct(IntVec3 HiveCenter)
        {
            List<IntVec3> HiveStruct = new List<IntVec3>()
            {
                // Support Collums
                // Cardinals
                new IntVec3
                {
                    x = HiveCenter.x + 3,
                    z = HiveCenter.z
                },
                new IntVec3
                {
                    x = HiveCenter.x - 3,
                    z = HiveCenter.z
                },
                new IntVec3
                {
                    x = HiveCenter.x,
                    z = HiveCenter.z + 3
                },
                new IntVec3
                {
                    x = HiveCenter.x,
                    z = HiveCenter.z - 3
                },
                // Corners
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x - 2,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z - 2
                },
                new IntVec3
                {
                    x = HiveCenter.x - 2,
                    z = HiveCenter.z - 2
                }

            };
            return HiveStruct;
        }

        public List<IntVec3> HiveWalls(IntVec3 HiveCenter)
        {
            List<IntVec3> HiveWalls = new List<IntVec3>()
            {
                // Exterior Walls
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 1
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 3
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + 4
                },
                new IntVec3
                {
                    x = HiveCenter.x + 5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + -1
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + -2
                },
                new IntVec3
                {
                    x = HiveCenter.x + 6,
                    z = HiveCenter.z + -3
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -4
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + -5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 1
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 2
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 3
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + 4
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -1
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -2
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -3
                },
                new IntVec3
                {
                    x = HiveCenter.x + -6,
                    z = HiveCenter.z + -4
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + -5
                },
                // 
                new IntVec3
                {
                    x = HiveCenter.x + 1,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 3,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 4,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + -1,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -2,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -3,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -4,
                    z = HiveCenter.z + 6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + 5
                },
                //
                new IntVec3
                {
                    x = HiveCenter.x + 1,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 2,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 3,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + 4,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -1,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -2,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -3,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -4,
                    z = HiveCenter.z + -6
                },
                new IntVec3
                {
                    x = HiveCenter.x + -5,
                    z = HiveCenter.z + -5
                },
                //

            };
            return HiveWalls;
        }

        // Token: 0x0600041E RID: 1054 RVA: 0x0002CA54 File Offset: 0x0002AE54
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.def != XenomorphRacesDefOf.RRY_Xenomorph_Queen && pawn.def != XenomorphRacesDefOf.RRY_Xenomorph_Drone)
            {
                return null;
            }
            Map map = pawn.Map;
            IntVec3 pos = pawn.Position;
            Comp_Xenomorph _Xenomorph = pawn.xenomorph();
            IntVec3 HiveCenter = _Xenomorph.HiveLoc == IntVec3.Invalid || _Xenomorph.HiveLoc == IntVec3.Zero ? pawn.mindState.duty.focus.Cell : _Xenomorph.HiveLoc;
            
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            if (region == null)
            {
                return null;
            }
            if (HiveCenter == pawn.InteractionCell)
            {
                return null;
            }
            bool centerNode = HiveCenter.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Xenomorph_Hive) != null;
            bool centerChild = HiveCenter.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Xenomorph_Hive_Child) != null;
            bool centerSlime = HiveCenter.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Xenomorph_Hive_Slime) != null;
            bool centerFilled = HiveCenter.Filled(pawn.Map);
            if (pawn.GetLord() is Lord L && L != null)
            {
                if ((L.CurLordToil is LordToil_DefendAndExpandHiveLike || L.CurLordToil is LordToil_DefendHiveLikeAggressively) && L.CurLordToil is LordToil_HiveLikeRelated THL)
                {
                    if (THL != null)
                    {
                        if (THL.Data != null)
                        {
                            HiveLike hive = THL.Data.assignedHiveLikes.TryGetValue(pawn);
                            if (hive != null)
                            {
                                centerNode = hive.def == XenomorphDefOf.RRY_Xenomorph_Hive;
                                centerChild = hive.def == XenomorphDefOf.RRY_Xenomorph_Hive_Child;
                            }
                        }
                    }
                }
            }
            if (centerChild)
            {
                return null;
            }
            if ((!centerNode && !centerSlime))
            {
                if (!centerFilled && pawn.def == XenomorphRacesDefOf.RRY_Xenomorph_Drone)
                {
                    IntVec3 c = HiveCenter;
                    if (c.InBounds(pawn.Map) && c.Roofed(pawn.Map) && pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.Deadly))
                    {
                        return new Job(XenomorphDefOf.RRY_Job_Xenomorph_Construct_Hive_Slime, c)
                        {
                            ignoreDesignations = false
                        };
                    }
                }
            }
            else
            {
                if (pawn.def==XenomorphRacesDefOf.RRY_Xenomorph_Queen)
                {
                    if (centerSlime && !centerFilled)
                    {
                        IntVec3 c = HiveCenter;
                        if (c.InBounds(pawn.Map) && c.Roofed(pawn.Map) && pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.Deadly))
                        {
                            return new Job(XenomorphDefOf.RRY_Job_Xenomorph_Construct_Hive_Node, c)
                            {
                                ignoreDesignations = false
                            };
                        }
                    }
                    return null;
                }
            }
            if (centerSlime)
            {
                MiningRange = 5;
            }
            if (centerNode)
            {
                MiningRange = 7;
            }
            if (centerChild)
            {
                MiningRange = 3;
            }
            
            if (centerNode)
            {
                Predicate<IntVec3> validator = delegate (IntVec3 y)
                {
                    bool pillarloc = HiveStruct(HiveCenter).Contains(y) || HiveWalls(HiveCenter).Contains(y);
                    bool Node = y.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Xenomorph_Hive) == null;
                    bool Child = y.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Xenomorph_Hive_Child) == null;
                    bool Wall = y.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Xenomorph_Hive_Wall) == null;

                    bool result = Node && Child && Wall && pillarloc /* && HiveCenter.DistanceTo(y) <= MiningRange */ && pawn.CanReserveAndReach(y, PathEndMode.OnCell, Danger.Deadly, layer: ReservationLayerDefOf.Floor);
                    return result;
                };

                if (RCellFinder.TryFindRandomCellNearWith(HiveCenter, validator, pawn.Map, out IntVec3 c1, MiningRange, MiningRange+1))
                {
                    if (c1.InBounds(pawn.Map) && pawn.CanReserveAndReach(c1, PathEndMode.OnCell, Danger.Deadly, layer:ReservationLayerDefOf.Floor))
                    {
                        Building edifice = c1.GetEdifice(pawn.Map);
                        if (edifice == null)
                        {
                            return new Job(XenomorphDefOf.RRY_Job_Xenomorph_Construct_Hive_Wall, c1)
                            {
                                ignoreDesignations = false
                            };
                        }
                    }
                }
                
            }
            Predicate<IntVec3> validator2 = delegate (IntVec3 y)
            {
                bool roofed = (y.Roofed(pawn.Map));
                bool result = !roofed && XenomorphUtil.DistanceBetween(y, HiveCenter) <= MiningRange && pawn.CanReserveAndReach(y, PathEndMode.OnCell, Danger.Deadly, layer: ReservationLayerDefOf.Ceiling);
            return result;
            };
            if (RCellFinder.TryFindRandomCellNearWith(HiveCenter, validator2, pawn.Map, out IntVec3 c2, MiningRange, MiningRange))
            {
                if (c2.InBounds(pawn.Map) && pawn.CanReserveAndReach(c2, PathEndMode.OnCell, Danger.Deadly))
                {
                    return new Job(XenomorphDefOf.RRY_Job_Xenomorph_Construct_Hive_Roof, c2)
                    {
                        ignoreDesignations = false
                    };
                }
            }
            return null;
        }

    }

    // Token: 0x02000089 RID: 137
    public class JobDriver_ConstructHive_Wall : JobDriver
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
            return pawn.Reserve(targetA, job, 1, -1, ReservationLayerDefOf.Floor, errorOnFailed);
        }

        public ThingDef MyDef = XenomorphDefOf.RRY_Xenomorph_Hive_Wall;

        // Token: 0x06000393 RID: 915 RVA: 0x000245C8 File Offset: 0x000229C8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Log.Message("MakeNewToils");
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            Log.Message("Manipulation capable");
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Log.Message("Going to");
            Toil prepare = Toils_General.Wait(this.useDuration, TargetIndex.A);
            prepare.NPCWithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            prepare.WithEffect(EffecterDefOf.Vomit, TargetIndex.A);
            prepare.PlaySustainerOrSound(() => SoundDefOf.Vomit);
            yield return prepare;
            Log.Message("Preparing");
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                Thing thing = ThingMaker.MakeThing(MyDef);
                GenSpawn.Spawn(thing, TargetA.Cell, actor.Map, Rot4.South, WipeMode.FullRefund, false);
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            Log.Message("use");
            yield break;
        }

        // Token: 0x0400024D RID: 589
        private int useDuration = 600;
    }

    // Token: 0x02000089 RID: 137
    public class JobDriver_ConstructHive_Node : JobDriver
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

        public ThingDef MyDef = XenomorphDefOf.RRY_Xenomorph_Hive;

        // Token: 0x06000393 RID: 915 RVA: 0x000245C8 File Offset: 0x000229C8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(this.useDuration, TargetIndex.None);
            prepare.NPCWithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            prepare.WithEffect(EffecterDefOf.Vomit, TargetIndex.A);
            prepare.PlaySustainerOrSound(() => SoundDefOf.Vomit);
            yield return prepare;
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                Thing thing = ThingMaker.MakeThing(MyDef);
                HiveLike hive = (HiveLike)thing;
                hive.hasQueen = false;
                GenSpawn.Spawn(thing, TargetA.Cell, actor.Map, Rot4.South, WipeMode.FullRefund, false);
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }

        // Token: 0x0400024D RID: 589
        private int useDuration = 2400;
    }

    // Token: 0x02000089 RID: 137
    public class JobDriver_ConstructHive_Child : JobDriver
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

        public ThingDef MyDef = XenomorphDefOf.RRY_Xenomorph_Hive_Child;

        // Token: 0x06000393 RID: 915 RVA: 0x000245C8 File Offset: 0x000229C8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(this.useDuration, TargetIndex.None);
            prepare.NPCWithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            prepare.WithEffect(EffecterDefOf.Vomit, TargetIndex.A);
            prepare.PlaySustainerOrSound(() => SoundDefOf.Vomit);
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
        private int useDuration = 2400;
    }

    // Token: 0x02000089 RID: 137
    public class JobDriver_ConstructHive_Slime : JobDriver
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

        public ThingDef MyDef = XenomorphDefOf.RRY_Xenomorph_Hive_Slime;

        // Token: 0x06000393 RID: 915 RVA: 0x000245C8 File Offset: 0x000229C8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(this.useDuration, TargetIndex.None);
            prepare.NPCWithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            prepare.WithEffect(EffecterDefOf.Vomit, TargetIndex.A);
            prepare.PlaySustainerOrSound(() => SoundDefOf.Vomit);
            yield return prepare;
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                if (!TargetA.Cell.GetTerrain(actor.Map).acceptFilth)
                {
                    MyDef = XenomorphDefOf.RRY_Xenomorph_Hive;
                }
                Thing thing = ThingMaker.MakeThing(MyDef);
                if (!TargetA.Cell.GetTerrain(actor.Map).acceptFilth)
                {
                    HiveLike hive = (HiveLike)thing;
                    hive.hasQueen = false;
                }
                GenSpawn.Spawn(thing, TargetA.Cell, actor.Map, Rot4.South, WipeMode.FullRefund, false);
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }

        // Token: 0x0400024D RID: 589
        private int useDuration = 60;
    }
 
    // Token: 0x02000089 RID: 137
    public class JobDriver_ConstructHive_Roof : JobDriver
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
            return true;
        }
        

        // Token: 0x06000393 RID: 915 RVA: 0x000245C8 File Offset: 0x000229C8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(this.useDuration, TargetIndex.None);
            prepare.NPCWithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            prepare.WithEffect(EffecterDefOf.Vomit, TargetIndex.A);
            prepare.PlaySustainerOrSound(() => SoundDefOf.Vomit);
            yield return prepare;
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                base.Map.roofGrid.SetRoof(TargetA.Cell, RoofDefOf.RoofConstructed);
                MoteMaker.PlaceTempRoof(TargetA.Cell, base.Map);
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }

        // Token: 0x0400024D RID: 589
        private int useDuration = 60;
    }
}
