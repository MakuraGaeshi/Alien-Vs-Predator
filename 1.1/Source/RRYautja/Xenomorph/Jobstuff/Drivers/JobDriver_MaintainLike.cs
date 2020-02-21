using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000074 RID: 116
    public class JobDriver_MaintainLike : JobDriver
    {
        // Token: 0x06000332 RID: 818 RVA: 0x0001FB54 File Offset: 0x0001DF54
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x06000333 RID: 819 RVA: 0x0001FB8C File Offset: 0x0001DF8C
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(180, TargetIndex.None);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return prepare;
            Toil maintain = new Toil();
            maintain.initAction = delegate ()
            {
                Pawn actor = maintain.actor;
                CompMaintainableLike compMaintainable = actor.CurJob.targetA.Thing.TryGetComp<CompMaintainableLike>();
                compMaintainable.Maintained();
            };
            maintain.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return maintain;
            yield break;
        }

        // Token: 0x04000223 RID: 547
        private const int MaintainTicks = 180;
    }
}
