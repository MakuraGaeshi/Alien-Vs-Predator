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

        // Token: 0x0600037C RID: 892 RVA: 0x00022F80 File Offset: 0x00021380
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.Takee;
            Job job = this.job;
            bool result;
            if (pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
            {
                pawn = this.pawn;
                target = this.DropBed;
                job = this.job;
                int sleepingSlotsCount = this.DropBed.SleepingSlotsCount;
                int stackCount = 0;
                result = pawn.Reserve(target, job, sleepingSlotsCount, stackCount, null, errorOnFailed);
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600037D RID: 893 RVA: 0x00022FF8 File Offset: 0x000213F8
        protected override IEnumerable<Toil> MakeNewToils()
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
                    Thing thing;
                    this.pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out thing, null);
                    if (!this.DropBed.Destroyed && (this.DropBed.owners.Contains(this.Takee) || (this.DropBed.Medical && this.DropBed.AnyUnoccupiedSleepingSlot) || this.Takee.ownership == null))
                    {
                        this.Takee.jobs.Notify_TuckedIntoBed(this.DropBed);
                        if (this.Takee.RaceProps.Humanlike && this.job.def != JobDefOf.Arrest && !this.Takee.IsPrisonerOfColony)
                        {
                            this.Takee.relations.Notify_RescuedBy(this.pawn);
                        }
                        this.Takee.mindState.Notify_TuckedIntoBed();
                    }
                    if (this.Takee.IsPrisonerOfColony)
                    {
                        LessonAutoActivator.TeachOpportunity(ConceptDefOf.PrisonerTab, this.Takee, OpportunityType.GoodToKnow);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
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
