using AvP;
using AvP.ExtensionMethods;
using AvP.Xenomorph.Hives;
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
        // Token: 0x0600041E RID: 1054 RVA: 0x0002CA54 File Offset: 0x0002AE54

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.isXenomorph(out Comp_Xenomorph _Xenomorph))
            {
                return null;
            }
            if (pawn.def != XenomorphRacesDefOf.AvP_Xenomorph_Queen && pawn.def != XenomorphRacesDefOf.AvP_Xenomorph_Drone && pawn.def != XenomorphRacesDefOf.AvP_Xenomorph_Predalien)
            {
                return null;
            }
            Map map = pawn.Map;
            IntVec3 pos = pawn.Position;
            IntVec3 HiveCenter = _Xenomorph.HiveLoc;
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            if (region == null)
            {
                return null;
            }
            if (HiveCenter == pawn.InteractionCell)
            {
                return null;
            }
            XenomorphHive hive = null;
            XenomorphHive tunnel = null;
            hive = HiveCenter.GetFirstThing(pawn.Map, XenomorphDefOf.AvP_Xenomorph_Hive) as XenomorphHive;
            bool centerNode = hive != null;
            tunnel = HiveCenter.GetFirstThing(pawn.Map, XenomorphDefOf.AvP_Xenomorph_Hive_Child) as XenomorphHive;
            bool centerChild = tunnel != null;
            bool centerSlime = HiveCenter.GetFirstThing(pawn.Map, XenomorphDefOf.AvP_Xenomorph_Hive_Slime) != null;
            bool centerFilled = HiveCenter.Filled(pawn.Map);

            if (centerNode)
            {
                MiningRange = 6;
            }
            else
            {
                MiningRange = 3;
                if (!centerChild)
                {

                    if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Queen || pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Drone)
                    {
                        if (!centerNode)
                        {
                            if (HiveCenter.InBounds(pawn.Map) && HiveCenter.Roofed(pawn.Map) && pawn.CanReserveAndReach(HiveCenter, PathEndMode.OnCell, Danger.Deadly))
                            {
                                return new Job(XenomorphDefOf.AvP_Job_Xenomorph_Construct_Hive_Node, HiveCenter)
                                {
                                    ignoreDesignations = false
                                };
                            }
                        }
                        return null;
                    }
                }
            }
            if (!centerChild)
            {
                
                foreach (IntVec3 structure in HiveStructure.HiveStruct(HiveCenter).Where(x => x.GetThingList(map).Any(z => !z.def.defName.Contains("AvP_Xenomorph_Hive")) && x.DistanceTo(HiveCenter) <= MiningRange && pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Deadly, layer: ReservationLayerDefOf.Floor) && x.GetFirstBuilding(map) == null))
                {
                    return new Job(XenomorphDefOf.AvP_Job_Xenomorph_Construct_Hive_Wall, structure)
                    {
                        ignoreDesignations = false
                    };
                }
                
                
                if (centerNode)
                {
                    if (!hive.HiveWalls.NullOrEmpty())
                    {
                        PawnPath path = map.pathFinder.FindPath(pawn.Position, HiveCenter,pawn);

                        foreach (IntVec3 structure in hive.HiveWalls.OrderByDescending(x=> x.DistanceTo(HiveCenter)))
                        {
                            if (structure.GetFirstPawn(map) == null)
                            {
                                if (pawn.CanReserveAndReach(structure, PathEndMode.Touch, Danger.Deadly, layer: ReservationLayerDefOf.Floor) && !path.NodesReversed.Contains(structure))
                                {
                                    if (structure.GetFirstThing(map, XenomorphDefOf.AvP_Xenomorph_Hive_Wall) == null)
                                    {
                                        return new Job(XenomorphDefOf.AvP_Job_Xenomorph_Construct_Hive_Wall, structure)
                                        {
                                            ignoreDesignations = false
                                        };
                                    }
                                }
                            }
                        }
                        path.ReleaseToPool();
                     //   IntVec3 structure = hive.HiveWalls.RandomElement();

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
                        return new Job(XenomorphDefOf.AvP_Job_Xenomorph_Construct_Hive_Roof, c2)
                        {
                            ignoreDesignations = false
                        };
                    }
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

        public ThingDef MyDef = XenomorphDefOf.AvP_Xenomorph_Hive_Wall;

        // Token: 0x06000393 RID: 915 RVA: 0x000245C8 File Offset: 0x000229C8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            if (this.job.targetA.Cell.GetFirstPawn(this.pawn.Map)!=null)
            {
                yield break;
            }
            Toil prepare = Toils_General.Wait(this.useDuration, TargetIndex.A);
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
                /*
                XenomorphHive Hive = actor.xenomorph().HiveLoc.GetFirstThing(Map, XenomorphDefOf.AvP_Xenomorph_Hive) as XenomorphHive;
                if (Hive!=null)
                {
                    Hive.WallBuilt(TargetA.Cell);
                }
                */
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
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

        public ThingDef MyDef = XenomorphDefOf.AvP_Xenomorph_Hive;

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
                XenomorphHive hive = (XenomorphHive)thing;
                hive.active = false;
                hive.canSpawnPawns = false;
                hive.getsQueen = false;
                hive.InitialPawnsPoints = 0;
                hive.MaxSpawnedPawnsPoints = 0;
                GenSpawn.Spawn(thing, TargetA.Cell, actor.Map, Rot4.South, WipeMode.FullRefund, false);
                hive.Lord.AddPawn(actor);
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

        public ThingDef MyDef = XenomorphDefOf.AvP_Xenomorph_Hive_Child;

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

        public ThingDef MyDef = XenomorphDefOf.AvP_Xenomorph_Hive_Slime;

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
                MyDef = XenomorphDefOf.AvP_Xenomorph_Hive;
                Thing thing = ThingMaker.MakeThing(MyDef);
                XenomorphHive hive = (XenomorphHive)thing;
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
