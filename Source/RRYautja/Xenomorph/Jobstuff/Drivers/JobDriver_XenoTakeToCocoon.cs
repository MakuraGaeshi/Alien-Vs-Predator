using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x02000082 RID: 130
    public class JobDriver_XenoTakeToCocoon : JobDriver
    {
        // Token: 0x170000B9 RID: 185
        // (get) Token: 0x0600037A RID: 890 RVA: 0x00022F30 File Offset: 0x00021330
        protected Pawn Takee
        {
            get
            {
                return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x170000BA RID: 186
        // (get) Token: 0x0600037B RID: 891 RVA: 0x00022F58 File Offset: 0x00021358
        protected Building_XenomorphCocoon DropBed
        {
            get
            {

                return (Building_XenomorphCocoon)this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        public bool eggsPresent;
        public bool eggsReachable;
        public Thing closestReachableEgg;
        public Thing closestReachableCocoontoEgg;

        public bool cocoonsPresent;
        public bool cocoonsReachable;
        public bool cocoonOccupied;
        public Thing closestReachableCocoon;

        public bool hivelikesPresent;
        public bool hivelikesReachable;
        public Thing closestReachableHivelike;

        // Token: 0x0600037C RID: 892 RVA: 0x00022F80 File Offset: 0x00021380
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) : false;
            LocalTargetInfo target = this.Takee;
            Job job = this.job;
            bool result;
            if (this.DropBed!=null && pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
            {
                pawn = this.pawn;
                target = this.DropBed;;
                job = this.job;
                if (selected) Log.Message(string.Format("JobDriver_XenoTakeToCocoon TryMakePreToilReservations this.DropBed: {0}", this.DropBed));
                int sleepingSlotsCount = this.DropBed.SleepingSlotsCount;
                int stackCount = 0;
                result = pawn.Reserve(target, job, sleepingSlotsCount, stackCount, null, errorOnFailed);
            }
            else
            {
                return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
            }
            return result;
        }

        // Token: 0x0600037D RID: 893 RVA: 0x00022FF8 File Offset: 0x000213F8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (this.DropBed!=null)
            {
                this.FailOnDestroyedOrNull(TargetIndex.A);
                this.FailOnDestroyedOrNull(TargetIndex.B);
                this.FailOnAggroMentalStateAndHostile(TargetIndex.A);
                this.FailOn(delegate ()
                {
                    return false;
                });
                yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.B, TargetIndex.A);
                base.AddFinishAction(delegate
                {
                    if (this.job.def.makeTargetPrisoner && this.Takee.ownership.OwnedBed == this.DropBed && this.Takee.Position != RestUtility.GetBedSleepingSlotPosFor(this.Takee, this.DropBed))
                    {
                        this.Takee.ownership.UnclaimBed();
                    }
                });
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOn(() => this.job.def == JobDefOf.Arrest && !this.Takee.CanBeArrestedBy(this.pawn)).FailOn(() => !this.pawn.CanReach(this.DropBed, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn)).FailOn(() => this.job.def == JobDefOf.Rescue && !this.Takee.Downed).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        if (this.job.def.makeTargetPrisoner)
                        {
                            Pawn pawn = (Pawn)this.job.targetA.Thing;
                            Lord lord = pawn.GetLord();
                            if (lord != null)
                            {
                                lord.Notify_PawnAttemptArrested(pawn);
                            }
                            GenClamor.DoClamor(pawn, 10f, ClamorDefOf.Harm);
                            if (this.job.def == JobDefOf.Arrest && !pawn.CheckAcceptArrest(this.pawn))
                            {
                                this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                            }
                        }
                    }
                };
                Toil startCarrying = Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false).FailOnNonMedicalBedNotOwned(TargetIndex.B, TargetIndex.A);
                startCarrying.AddPreInitAction(new Action(this.CheckMakeTakeeGuest));
                yield return startCarrying;
                yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        this.CheckMakeTakeePrisoner();
                        if (this.Takee.playerSettings == null)
                        {
                            this.Takee.playerSettings = new Pawn_PlayerSettings(this.Takee);
                        }
                    }
                };
                yield return Toils_Reserve.Release(TargetIndex.B);
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        IntVec3 position = this.DropBed.Position;
                        this.pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out Thing thing, null);
                        this.Takee.jobs.Notify_TuckedIntoBed(this.DropBed);
                        this.Takee.mindState.Notify_TuckedIntoBed();
                    },
                    defaultCompleteMode = ToilCompleteMode.Instant
                };
                yield break;
            }
            else
            {

                this.FailOnDestroyedOrNull(TargetIndex.A);
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
                yield return Toils_Construct.UninstallIfMinifiable(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
                yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
                Toil gotoCell = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
                gotoCell.AddPreTickAction(delegate
                {
                    if (base.Map.exitMapGrid.IsExitCell(this.pawn.Position))
                    {
                        this.pawn.ExitMap(true, CellRect.WholeMap(base.Map).GetClosestEdge(this.pawn.Position));
                    }
                });
                yield return gotoCell;
                /*
                Toil prepare = Toils_General.Wait(300, TargetIndex.None);
                prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
                prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
                prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
                prepare.tickAction();
                yield return prepare;
                */
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        if (this.pawn.Position.OnEdge(this.pawn.Map) || this.pawn.Map.exitMapGrid.IsExitCell(this.pawn.Position))
                        {
                            this.pawn.ExitMap(true, CellRect.WholeMap(base.Map).GetClosestEdge(this.pawn.Position));
                        }
                        else
                        {
                            eggsPresent = XenomorphUtil.EggsPresent(pawn.Map);
                            eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
                            closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);

                            hivelikesPresent = XenomorphUtil.HivelikesPresent(pawn.Map);
                            hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull();
                            closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(pawn);

                            int radius = 1;
                            IntVec3 position; if (hivelikesPresent && hivelikesReachable)
                            {
                                position = closestReachableHivelike.Position;
                            }
                            else if (eggsPresent && eggsReachable)
                            {
                                position = closestReachableEgg.Position;
                            }
                            else
                            {
                                position = Takee.PositionHeld;
                            }


                            Log.Message(string.Format("position: {0}", position));
                            ThingDef named = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;
                            Log.Message(string.Format("named: {0}", named));
                            int num = (named.Size.x > named.Size.z) ? named.Size.x : named.Size.z;
                            Log.Message(string.Format("num: {0}", num));
                            CellRect mapRect;
                            IntVec3 intVec = CellFinder.RandomClosewalkCellNear(position, this.Map, radius, null);
                            Log.Message(string.Format("intVec: {0}", intVec));
                            mapRect = new CellRect(intVec.x, intVec.z, num, num);
                            while (!IsMapRectClear(mapRect, this.Map))
                            {
                                intVec = CellFinder.RandomClosewalkCellNear(position, this.Map, radius, null);
                                Log.Message(string.Format("intVec: {0}", intVec));
                                mapRect = new CellRect(intVec.x, intVec.z, num, num);
                                radius++;
                            }
                            Log.Message(string.Format("mapRect: {0}", mapRect));
                            //    GenPlace.TryPlaceThing(TryMakeCocoon(mapRect, this.Map, named), intVec, this.Map, ThingPlaceMode.Near);
                           // this.pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out Thing thing, null);
                           // this.Takee.jobs.Notify_TuckedIntoBed(this.DropBed);
                            this.Takee.mindState.Notify_TuckedIntoBed();
                        }
                    },
                    defaultCompleteMode = ToilCompleteMode.Instant
                };
                yield break;
            }
        }


        // Token: 0x0600000B RID: 11 RVA: 0x00002C9C File Offset: 0x00000E9C
        private static bool IsMapRectClear(CellRect mapRect, Map map)
        {
            foreach (IntVec3 intVec in mapRect)
            {
                bool flag = !map.pathGrid.WalkableFast(intVec);
                if (flag)
                {
                    return false;
                }
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag2 = thingList[i].def.category == (ThingCategory)3 || thingList[i].def.category == (ThingCategory)1 || thingList[i].def.category == (ThingCategory)10;
                    if (flag2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002D80 File Offset: 0x00000F80
        private static void ClearMapRect(CellRect mapRect, Map map)
        {
            foreach (IntVec3 intVec in mapRect)
            {
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    thingList[i].Destroy(0);
                }
            }
        }
        // Token: 0x0600000D RID: 13 RVA: 0x00002DF8 File Offset: 0x00000FF8
        private static Building_XenomorphCocoon TryMakeCocoon(CellRect mapRect, Map map, ThingDef thingDef)
        {
            mapRect.ClipInsideMap(map);
            CellRect cellRect;
            cellRect = new CellRect(mapRect.BottomLeft.x + 1, mapRect.BottomLeft.z + 1, 2, 1);
            cellRect.ClipInsideMap(map);
            IsMapRectClear(cellRect, map);
            foreach (IntVec3 intVec in cellRect)
            {
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag = !thingList[i].def.destroyable;
                    if (flag)
                    {
                        return null;
                    }
                }
            }
            Building_XenomorphCocoon building_XenomorphCocoon = (Building_XenomorphCocoon)ThingMaker.MakeThing(thingDef, null);
            building_XenomorphCocoon.SetPositionDirect(cellRect.BottomLeft);
            bool flag2 = Rand.Value < 0.5f;
            if (flag2)
            {
                flag2 = Rand.Value < 0.5f;
                if (flag2)
                {
                    building_XenomorphCocoon.Rotation = Rot4.West;
                }
                else
                {
                    building_XenomorphCocoon.Rotation = Rot4.East;
                }
            }
            else
            {
                flag2 = Rand.Value < 0.5f;
                if (flag2)
                {
                    building_XenomorphCocoon.Rotation = Rot4.South;
                }
                else
                {
                    building_XenomorphCocoon.Rotation = Rot4.North;
                }
            }
            return building_XenomorphCocoon;
        }


        // Token: 0x0600037E RID: 894 RVA: 0x0002301C File Offset: 0x0002141C
        private void CheckMakeTakeePrisoner()
        {
            if (this.job.def.makeTargetPrisoner)
            {
                if (this.Takee.guest.Released)
                {
                    this.Takee.guest.Released = false;
                    this.Takee.guest.interactionMode = PrisonerInteractionModeDefOf.NoInteraction;
                }
                if (!this.Takee.IsPrisonerOfColony)
                {
                    this.Takee.guest.CapturedBy(Faction.OfPlayer, this.pawn);
                }
            }
        }

        // Token: 0x0600037F RID: 895 RVA: 0x000230A4 File Offset: 0x000214A4
        private void CheckMakeTakeeGuest()
        {
            if (!this.job.def.makeTargetPrisoner && this.Takee.Faction != Faction.OfPlayer && this.Takee.HostFaction != Faction.OfPlayer && this.Takee.guest != null && !this.Takee.IsWildMan())
            {
                this.Takee.guest.SetGuestStatus(Faction.OfPlayer, false);
            }
        }

        // Token: 0x04000243 RID: 579
        private const TargetIndex TakeeIndex = TargetIndex.A;

        // Token: 0x04000244 RID: 580
        private const TargetIndex BedIndex = TargetIndex.B;
    }
}
