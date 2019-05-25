using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000082 RID: 130
    public class JobDriver_XenoTakeAndCocoon : JobDriver
    {
        // Token: 0x170000B6 RID: 182
        // (get) Token: 0x06000371 RID: 881 RVA: 0x0001EEFC File Offset: 0x0001D2FC
        protected Thing Item
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        protected IntVec3 loc
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Cell;
            }
        }

        protected Building_XenomorphCocoon DropBed
        {
            get
            {

                return (Building_XenomorphCocoon)this.job.GetTarget(TargetIndex.B).Thing;
            }
        }


        // Token: 0x06000372 RID: 882 RVA: 0x0001EF20 File Offset: 0x0001D320
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.Item;
            Job job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x06000373 RID: 883 RVA: 0x0001EF58 File Offset: 0x0001D358
        protected override IEnumerable<Toil> MakeNewToils()
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
            Toil prepare = Toils_General.Wait(300, TargetIndex.None);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return prepare;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    if (Item is Pawn takee)
                    {
                        Thing thing = ThingMaker.MakeThing(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon);
                        GenPlace.TryPlaceThing(thing, Item.Position, base.Map,ThingPlaceMode.Near);

                        takee.jobs.Notify_TuckedIntoBed((Building_XenomorphCocoon)thing);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }

        // Token: 0x0400023D RID: 573
        private const TargetIndex ItemInd = TargetIndex.A;

        // Token: 0x0400023E RID: 574
        private const TargetIndex ExitCellInd = TargetIndex.B;
    }
}
