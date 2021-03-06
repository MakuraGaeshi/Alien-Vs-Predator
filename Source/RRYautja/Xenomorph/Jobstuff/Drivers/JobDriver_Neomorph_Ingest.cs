﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000099 RID: 153
    public class JobDriver_Neomorph_Ingest : JobDriver
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
                //    Log.Message(string.Format("pawn: {0}", pawn));
                //    Log.Message(string.Format("ingestibleSource.def: {0}", ingestibleSource.def));
                //    Log.Message(string.Format("Nutrition: {0}", ingestibleSource.GetStatValue(StatDefOf.Nutrition, true)));
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
            // yield return this.ReserveFoodIfWillIngestWholeStack();
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield break;
            /*
            if (this.usingNutrientPasteDispenser)
            {
                return this.PrepareToIngestToils_Dispenser();
            }
            if (this.pawn.RaceProps.ToolUser)
            {
                return this.PrepareToIngestToils_ToolUser(chewToil);
            }
            */
        //    return this.PrepareToIngestToils_NonToolUser();
        }

        // Token: 0x060003E5 RID: 997 RVA: 0x00028194 File Offset: 0x00026594
        private IEnumerable<Toil> PrepareToIngestToils_Dispenser()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Ingest.TakeMealFromDispenser(TargetIndex.A, this.pawn);
            yield return Toils_Ingest.CarryIngestibleToChewSpot(this.pawn, TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.B, TargetIndex.A);
            yield break;
        }

        // Token: 0x060003E6 RID: 998 RVA: 0x000281B8 File Offset: 0x000265B8
        private IEnumerable<Toil> PrepareToIngestToils_ToolUser(Toil chewToil)
        {
            if (this.eatingFromInventory)
            {
                yield return Toils_Misc.TakeItemFromInventoryToCarrier(this.pawn, TargetIndex.A);
            }
            else
            {
                yield return this.ReserveFoodIfWillIngestWholeStack();
                Toil gotoToPickup = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
                yield return Toils_Jump.JumpIf(gotoToPickup, () => this.pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation));
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
                yield return Toils_Jump.Jump(chewToil);
                yield return gotoToPickup;
                yield return Toils_Ingest.PickupIngestible(TargetIndex.A, this.pawn);
                Toil reserveExtraFoodToCollect = Toils_Reserve.Reserve(TargetIndex.C, 1, -1, null);
                Toil findExtraFoodToCollect = new Toil();
                findExtraFoodToCollect.initAction = delegate ()
                {
                    if (this.pawn.inventory.innerContainer.TotalStackCountOfDef(this.IngestibleSource.def) < this.job.takeExtraIngestibles)
                    {
                        Thing thing = GenClosest.ClosestThingReachable(this.pawn.Position, this.pawn.Map, ThingRequest.ForDef(this.IngestibleSource.def), PathEndMode.Touch, TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.ByPawn, false), 12f, (Thing x) => this.pawn.CanReserve(x, 1, -1, null, false) && !x.IsForbidden(this.pawn) && x.IsSociallyProper(this.pawn), null, 0, -1, false, RegionType.Set_Passable, false);
                        if (thing != null)
                        {
                            this.job.SetTarget(TargetIndex.C, thing);
                            this.JumpToToil(reserveExtraFoodToCollect);
                        }
                    }
                };
                findExtraFoodToCollect.defaultCompleteMode = ToilCompleteMode.Instant;
                yield return Toils_Jump.Jump(findExtraFoodToCollect);
                yield return reserveExtraFoodToCollect;
                yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch);
                yield return Toils_Haul.TakeToInventory(TargetIndex.C, () => this.job.takeExtraIngestibles - this.pawn.inventory.innerContainer.TotalStackCountOfDef(this.IngestibleSource.def));
                yield return findExtraFoodToCollect;
            }
            yield return Toils_Ingest.CarryIngestibleToChewSpot(this.pawn, TargetIndex.A).FailOnDestroyedOrNull(TargetIndex.A);
            yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.B, TargetIndex.A);
            yield break;
        }

        // Token: 0x060003E7 RID: 999 RVA: 0x000281E4 File Offset: 0x000265E4
        private IEnumerable<Toil> PrepareToIngestToils_NonToolUser()
        {
            yield return this.ReserveFoodIfWillIngestWholeStack();
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield break;
        }

        // Token: 0x060003E8 RID: 1000 RVA: 0x00028208 File Offset: 0x00026608
        private Toil ReserveFoodIfWillIngestWholeStack()
        {
            return new Toil
            {
                initAction = delegate ()
                {
                    if (this.pawn.Faction == null)
                    {
                        return;
                    }
                    Thing thing = this.job.GetTarget(TargetIndex.A).Thing;
                    if (this.pawn.carryTracker.CarriedThing == thing)
                    {
                        return;
                    }
                    int num = FoodUtility.WillIngestStackCountOf(this.pawn, thing.def, thing.GetStatValue(StatDefOf.Nutrition, true));
                    if (num >= thing.stackCount)
                    {
                        if (!thing.Spawned)
                        {
                            this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                            return;
                        }
                        this.pawn.Reserve(thing, this.job, 1, -1, null, true);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant,
                atomicWithPrevious = true
            };
        }

        // Token: 0x060003E9 RID: 1001 RVA: 0x0002823C File Offset: 0x0002663C
        public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool behind, ref bool flip)
        {
            IntVec3 cell = this.job.GetTarget(TargetIndex.B).Cell;
            return JobDriver_Ingest.ModifyCarriedThingDrawPosWorker(ref drawPos, ref behind, ref flip, cell, this.pawn);
        }

        // Token: 0x060003EA RID: 1002 RVA: 0x00028270 File Offset: 0x00026670
        public static bool ModifyCarriedThingDrawPosWorker(ref Vector3 drawPos, ref bool behind, ref bool flip, IntVec3 placeCell, Pawn pawn)
        {
            if (pawn.pather.Moving)
            {
                return false;
            }
            Thing carriedThing = pawn.carryTracker.CarriedThing;
            if (carriedThing == null || !carriedThing.IngestibleNow)
            {
                return false;
            }
            if (placeCell.IsValid && placeCell.AdjacentToCardinal(pawn.Position) && placeCell.HasEatSurface(pawn.Map) && carriedThing.def.ingestible.ingestHoldUsesTable)
            {
                drawPos = new Vector3((float)placeCell.x + 0.5f, drawPos.y, (float)placeCell.z + 0.5f);
                return true;
            }
            if (carriedThing.def.ingestible.ingestHoldOffsetStanding != null)
            {
                HoldOffset holdOffset = carriedThing.def.ingestible.ingestHoldOffsetStanding.Pick(pawn.Rotation);
                if (holdOffset != null)
                {
                    drawPos += holdOffset.offset;
                    behind = holdOffset.behind;
                    flip = holdOffset.flip;
                    return true;
                }
            }
            return false;
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
