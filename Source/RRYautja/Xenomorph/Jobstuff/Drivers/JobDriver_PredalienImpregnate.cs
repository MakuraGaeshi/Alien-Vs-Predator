using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RRYautja;

namespace RimWorld
{
    // Token: 0x02000089 RID: 137
    public class JobDriver_PredalienImpregnate : JobDriver
    {
        // Token: 0x06000390 RID: 912 RVA: 0x00024538 File Offset: 0x00022938
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.useDuration, "useDuration", 0, false);
        }

        public HediffDef heDiffDeff = XenomorphDefOf.RRY_XenomorphImpregnation;
        public HediffDef heCocDeff = XenomorphDefOf.RRY_Hediff_Cocooned;

        // Token: 0x06000391 RID: 913 RVA: 0x00024554 File Offset: 0x00022954
        public override void Notify_Starting()
        {
            base.Notify_Starting();
            this.useDuration = 300;
        }

        // Token: 0x06000392 RID: 914 RVA: 0x00024590 File Offset: 0x00022990
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

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
                Pawn Infectable = (Pawn)actor.CurJob.targetA.Thing;
                ThingDef named = Infectable.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
                GenPlace.TryPlaceThing(XenomorphKidnapUtility.TryMakeCocoon(new CellRect(Infectable.Position.x, Infectable.Position.z, 1, 1), Infectable.Map, named), Infectable.Position, Infectable.Map, ThingPlaceMode.Direct);
                Infectable.health.AddHediff(heCocDeff);
                Infectable.health.AddHediff(heDiffDeff, Infectable.RaceProps.body.corePart);
                Hediff hediff = Infectable.health.hediffSet.GetFirstHediffOfDef(heDiffDeff);
                HediffComp_XenoSpawner _XenoSpawner = hediff.TryGetComp<HediffComp_XenoSpawner>();
                _XenoSpawner.countToSpawn = Rand.RangeInclusive(1, 4);
                Infectable.jobs.Notify_TuckedIntoBed((Building_XenomorphCocoon)XenomorphUtil.ClosestReachableEmptyCocoon(Infectable, named));
                Infectable.mindState.Notify_TuckedIntoBed();

            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }

        // Token: 0x0400024D RID: 589
        private int useDuration = -1;
    }
}
