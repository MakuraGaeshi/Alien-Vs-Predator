﻿using System;
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

        protected Pawn Takee
        {
            get
            {
                return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
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
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(300, TargetIndex.None);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return prepare;
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                this.Takee.health.AddHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
                this.Takee.mindState.Notify_TuckedIntoBed();
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }


        private int ticksLeft;

        // Token: 0x0400023D RID: 573
        private const TargetIndex ItemInd = TargetIndex.A;

        // Token: 0x0400023E RID: 574
        private const TargetIndex ExitCellInd = TargetIndex.B;
    }
}
