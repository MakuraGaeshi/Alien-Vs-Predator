using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000030 RID: 48
    public class JobDriver_PredatorHuntXenomorph : JobDriver
    {
        // Token: 0x1700005A RID: 90
        // (get) Token: 0x060001BD RID: 445 RVA: 0x000112D8 File Offset: 0x0000F6D8
        public Pawn Prey
        {
            get
            {
                Corpse corpse = this.Corpse;
                if (corpse != null)
                {
                    return corpse.InnerPawn;
                }
                return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x1700005B RID: 91
        // (get) Token: 0x060001BE RID: 446 RVA: 0x00011314 File Offset: 0x0000F714
        private Corpse Corpse
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing as Corpse;
            }
        }

        // Token: 0x060001BF RID: 447 RVA: 0x0001133A File Offset: 0x0000F73A
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.firstHit, "firstHit", false, false);
            Scribe_Values.Look<bool>(ref this.notifiedPlayerAttacking, "notifiedPlayerAttacking", false, false);
        }

        // Token: 0x060001C0 RID: 448 RVA: 0x00011366 File Offset: 0x0000F766
        public override string GetReport()
        {
            if (this.Corpse != null)
            {
                return base.ReportStringProcessed(JobDefOf.Ingest.reportString);
            }
            return base.GetReport();
        }

        // Token: 0x060001C1 RID: 449 RVA: 0x0001138A File Offset: 0x0000F78A
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x060001C2 RID: 450 RVA: 0x00011390 File Offset: 0x0000F790
        protected override IEnumerable<Toil> MakeNewToils()
        {
            base.AddFinishAction(delegate
            {
                this.Map.attackTargetsCache.UpdateTarget(this.pawn);
            });
            Toil prepareToEatCorpse = new Toil();
            prepareToEatCorpse.initAction = delegate ()
            {
                Pawn actor = prepareToEatCorpse.actor;
                Corpse corpse = this.Corpse;
                if (corpse == null)
                {
                    Pawn prey = this.Prey;
                    if (prey == null)
                    {
                        actor.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                        return;
                    }
                    corpse = prey.Corpse;
                    if (corpse == null || !corpse.Spawned)
                    {
                        actor.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                        return;
                    }
                }
                if (actor.Faction == Faction.OfPlayer)
                {
                    corpse.SetForbidden(false, false);
                }
                else
                {
                    corpse.SetForbidden(true, false);
                }
                actor.CurJob.SetTarget(TargetIndex.A, corpse);
            };
            yield return Toils_General.DoAtomic(delegate
            {
                this.Map.attackTargetsCache.UpdateTarget(this.pawn);
            });
            Action onHitAction = delegate ()
            {
                Pawn prey = this.Prey;
                bool surpriseAttack = this.firstHit && !prey.IsColonist;
                if (this.pawn.meleeVerbs.TryMeleeAttack(prey, this.job.verbToUse, surpriseAttack))
                {
                    if (!this.notifiedPlayerAttacked && PawnUtility.ShouldSendNotificationAbout(prey))
                    {
                        this.notifiedPlayerAttacked = true;
                        Messages.Message("MessageAttackedByPredator".Translate(prey.LabelShort, this.pawn.LabelIndefinite(), prey.Named("PREY"), this.pawn.Named("PREDATOR")).CapitalizeFirst(), prey, MessageTypeDefOf.ThreatSmall, true);
                    }
                    this.Map.attackTargetsCache.UpdateTarget(this.pawn);
                    this.firstHit = false;
                }
            };
            Toil followAndAttack = Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, onHitAction).JumpIfDespawnedOrNull(TargetIndex.A, prepareToEatCorpse).JumpIf(() => this.Corpse != null, prepareToEatCorpse).FailOn(() => Find.TickManager.TicksGame > this.startTick + 5000 && (float)(this.job.GetTarget(TargetIndex.A).Cell - this.pawn.Position).LengthHorizontalSquared > 4f);
            followAndAttack.AddPreTickAction(new Action(this.CheckWarnPlayer));
            yield return followAndAttack;
            yield return prepareToEatCorpse;
            Toil gotoCorpse = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return gotoCorpse;
            float durationMultiplier = 1f / this.pawn.GetStatValue(StatDefOf.EatingSpeed, true);
            yield return Toils_Ingest.ChewIngestible(this.pawn, durationMultiplier, TargetIndex.A, TargetIndex.None).FailOnDespawnedOrNull(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_Ingest.FinalizeIngest(this.pawn, TargetIndex.A);
            yield return Toils_Jump.JumpIf(gotoCorpse, () => this.pawn.needs.food.CurLevelPercentage < 0.9f);
            yield break;
        }

        // Token: 0x060001C3 RID: 451 RVA: 0x000113B4 File Offset: 0x0000F7B4
        public override void Notify_DamageTaken(DamageInfo dinfo)
        {
            base.Notify_DamageTaken(dinfo);
            if (dinfo.Def.ExternalViolenceFor(this.pawn) && dinfo.Def.isRanged && dinfo.Instigator != null && dinfo.Instigator != this.Prey && !this.pawn.InMentalState && !this.pawn.Downed)
            {
                this.pawn.mindState.StartFleeingBecauseOfPawnAction(dinfo.Instigator);
            }
        }

        // Token: 0x060001C4 RID: 452 RVA: 0x00011448 File Offset: 0x0000F848
        private void CheckWarnPlayer()
        {
            if (this.notifiedPlayerAttacking)
            {
                return;
            }
            Pawn prey = this.Prey;
            if (!prey.Spawned || prey.Faction != Faction.OfPlayer)
            {
                return;
            }
            if (Find.TickManager.TicksGame <= this.pawn.mindState.lastPredatorHuntingPlayerNotificationTick + 2500)
            {
                return;
            }
            if (!prey.Position.InHorDistOf(this.pawn.Position, 60f))
            {
                return;
            }
            if (prey.RaceProps.Humanlike)
            {
                Find.LetterStack.ReceiveLetter("LetterLabelPredatorHuntingColonist".Translate(this.pawn.LabelShort, prey.LabelDefinite(), this.pawn.Named("PREDATOR"), prey.Named("PREY")).CapitalizeFirst(), "LetterPredatorHuntingColonist".Translate(this.pawn.LabelIndefinite(), prey.LabelDefinite(), this.pawn.Named("PREDATOR"), prey.Named("PREY")).CapitalizeFirst(), LetterDefOf.ThreatBig, this.pawn, null, null);
            }
            else
            {
                Messages.Message("MessagePredatorHuntingPlayerAnimal".Translate(this.pawn.LabelIndefinite(), prey.LabelDefinite(), this.pawn.Named("PREDATOR"), prey.Named("PREY")).CapitalizeFirst(), this.pawn, MessageTypeDefOf.ThreatBig, true);
            }
            this.pawn.mindState.Notify_PredatorHuntingPlayerNotification();
            this.notifiedPlayerAttacking = true;
        }

        // Token: 0x040001B4 RID: 436
        private bool notifiedPlayerAttacked;

        // Token: 0x040001B5 RID: 437
        private bool notifiedPlayerAttacking;

        // Token: 0x040001B6 RID: 438
        private bool firstHit = true;

        // Token: 0x040001B7 RID: 439
        public const TargetIndex PreyInd = TargetIndex.A;

        // Token: 0x040001B8 RID: 440
        private const TargetIndex CorpseInd = TargetIndex.A;

        // Token: 0x040001B9 RID: 441
        private const int MaxHuntTicks = 5000;
    }
}
