using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000099 RID: 153
    public class JobDriver_Xenomorph_Ingest : JobDriver
    {
        // Token: 0x170000C7 RID: 199
        // (get) Token: 0x060003DD RID: 989 RVA: 0x00027E7C File Offset: 0x0002627C
        private Thing IngestibleSource
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x170000C8 RID: 200
        // (get) Token: 0x060003DE RID: 990 RVA: 0x00027EA0 File Offset: 0x000262A0
        private float ChewDurationMultiplier
        {
            get
            {
                Thing ingestibleSource = this.IngestibleSource;
                if (ingestibleSource.def.ingestible != null && !ingestibleSource.def.ingestible.useEatingSpeedStat)
                {
                    return 1f;
                }
                return 1f / this.pawn.GetStatValue(StatDefOf.EatingSpeed, true);
            }
        }

        // Token: 0x060003DF RID: 991 RVA: 0x00027EF6 File Offset: 0x000262F6
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.usingNutrientPasteDispenser, "usingNutrientPasteDispenser", false, false);
            Scribe_Values.Look<bool>(ref this.eatingFromInventory, "eatingFromInventory", false, false);
        }

        // Token: 0x060003E0 RID: 992 RVA: 0x00027F24 File Offset: 0x00026324
        public override string GetReport()
        {
            if (this.usingNutrientPasteDispenser)
            {
                return this.job.def.reportString.Replace("TargetA", ThingDefOf.MealNutrientPaste.label);
            }
            Thing thing = this.job.targetA.Thing;
            if (thing != null && thing.def.ingestible != null)
            {
                if (!thing.def.ingestible.ingestReportStringEat.NullOrEmpty() && (thing.def.ingestible.ingestReportString.NullOrEmpty() || this.pawn.RaceProps.intelligence < Intelligence.ToolUser))
                {
                    return string.Format(thing.def.ingestible.ingestReportStringEat, this.job.targetA.Thing.LabelShort);
                }
                if (!thing.def.ingestible.ingestReportString.NullOrEmpty())
                {
                    return string.Format(thing.def.ingestible.ingestReportString, this.job.targetA.Thing.LabelShort);
                }
            }
            return base.GetReport();
        }

        // Token: 0x060003E1 RID: 993 RVA: 0x00028048 File Offset: 0x00026448
        public override void Notify_Starting()
        {
            base.Notify_Starting();
            this.usingNutrientPasteDispenser = false;//(this.IngestibleSource is Building_NutrientPasteDispenser);
            this.eatingFromInventory = false;//
        }

        // Token: 0x060003E2 RID: 994 RVA: 0x000280A0 File Offset: 0x000264A0
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (this.pawn.Faction != null && !(this.IngestibleSource is Building_NutrientPasteDispenser))
            {
                Thing ingestibleSource = this.IngestibleSource;
                if (ingestibleSource == null)
                {
                    return false;
                }
                if (ingestibleSource.def == null)
                {
                    return false;
                }
                if (ingestibleSource.GetType()!=typeof(Corpse))
                {
                    return false;
                }
                try
                {
                    int num = FoodUtility.WillIngestStackCountOf(this.pawn, ingestibleSource.def, ingestibleSource.GetStatValue(StatDefOf.Nutrition, true) + 1);
                    if (num >= ingestibleSource.stackCount && ingestibleSource.Spawned)
                    {
                        Pawn pawn = this.pawn;
                        LocalTargetInfo target = ingestibleSource;
                        Job job = this.job;
                        if (!pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
                        {
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    Log.Message(string.Format("pawn: {0}", pawn));
                    Log.Message(string.Format("ingestibleSource.def: {0}", ingestibleSource.def));
                    Log.Message(string.Format("Nutrition: {0}", ingestibleSource.GetStatValue(StatDefOf.Nutrition, true)));
                    return false;
                }
            }
            return true;
        }

        // Token: 0x060003E3 RID: 995 RVA: 0x00028138 File Offset: 0x00026538
        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (!this.usingNutrientPasteDispenser)
            {
                this.FailOn(() => !this.IngestibleSource.Destroyed && !this.IngestibleSource.IngestibleNow);
            }
            Toil chew = Toils_Ingest.ChewIngestible(this.pawn, this.ChewDurationMultiplier, TargetIndex.A, TargetIndex.B).FailOn((Toil x) => !this.IngestibleSource.Spawned/* && (this.pawn.carryTracker == null || this.pawn.carryTracker.CarriedThing != this.IngestibleSource)*/).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            foreach (Toil toil in this.PrepareToIngestToils(chew))
            {
                yield return toil;
            }
            yield return chew;
            yield return Toils_Ingest.FinalizeIngest(this.pawn, TargetIndex.A);
            yield return Toils_Jump.JumpIf(chew, () => this.job.GetTarget(TargetIndex.A).Thing is Corpse && this.pawn.needs.food.CurLevelPercentage < 0.9f);
            yield break;
        }

        // Token: 0x060003E4 RID: 996 RVA: 0x0002815B File Offset: 0x0002655B
        private IEnumerable<Toil> PrepareToIngestToils(Toil chewToil)
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield break;
        }
        
        // Token: 0x04000260 RID: 608
        private bool usingNutrientPasteDispenser;

        // Token: 0x04000261 RID: 609
        private bool eatingFromInventory;

        // Token: 0x04000262 RID: 610
        public const float EatCorpseBodyPartsUntilFoodLevelPct = 0.9f;

        // Token: 0x04000263 RID: 611
        public const TargetIndex IngestibleSourceInd = TargetIndex.A;

        // Token: 0x04000264 RID: 612
        private const TargetIndex TableCellInd = TargetIndex.B;

        // Token: 0x04000265 RID: 613
        private const TargetIndex ExtraIngestiblesToCollectInd = TargetIndex.C;
    }
}
