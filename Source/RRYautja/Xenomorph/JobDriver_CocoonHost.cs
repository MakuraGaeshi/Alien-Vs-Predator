using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RRYautja
{
    // Token: 0x0200000C RID: 12
    public class JobDriver_CocoonHost : JobDriver
    {
        // Token: 0x1700000D RID: 13
        // (get) Token: 0x0600006A RID: 106 RVA: 0x000059B4 File Offset: 0x00003BB4
        public ThingDef CocoonDef
        {
            get
            {
                ThingDef result = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;
                Pawn prey = this.Prey;
                bool humanlike;
                if (prey == null)
                {
                    humanlike = false;
                }
                else
                {
                    RaceProperties raceProps = prey.RaceProps;
                    humanlike = raceProps.Humanlike;
                }
                if (!humanlike)
                {
                    result = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;
                }
                return result;
            }
        }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x0600006B RID: 107 RVA: 0x00005A30 File Offset: 0x00003C30
        public Pawn Prey
        {
            get
            {
                Corpse corpse = this.Corpse;
                bool flag = corpse != null;
                Pawn result;
                if (flag)
                {
                    result = corpse.InnerPawn;
                }
                else
                {
                    result = (Pawn)this.job.GetTarget((TargetIndex)1).Thing;
                }
                return result;
            }
        }


        // Token: 0x1700000F RID: 15
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00005A74 File Offset: 0x00003C74
        private Corpse Corpse
        {
            get
            {
                return this.job.GetTarget((TargetIndex)1).Thing as Corpse;
            }
        }

        // Token: 0x0600006D RID: 109 RVA: 0x00005A9F File Offset: 0x00003C9F
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.firstHit, "firstHit", false, false);
        }

        // Token: 0x0600006E RID: 110 RVA: 0x00005ABC File Offset: 0x00003CBC
        public override string GetReport()
        {
            bool flag = this.currentActivity == "";
            if (flag)
            {
                this.currentActivity = base.ReportStringProcessed(JobDefOf.Hunt.reportString);
            }
            return this.currentActivity;
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00005B00 File Offset: 0x00003D00
        public IntVec3 CocoonPlace(Building_XenomorphCocoon exception = null)
        {
            IntVec3 result = base.TargetB.Cell;
            HashSet<Thing> hashSet = XenomorphUtil.XenomorphCocoonsFor(this.pawn.Map, this.pawn);
            bool flag = exception != null;
            if (flag)
            {
                hashSet.Remove(exception);
            }
            bool flag2 = hashSet != null && hashSet.Count > 0;
            if (flag2)
            {
                HashSet<Thing> hashSet2 = new HashSet<Thing>(GenCollection.InRandomOrder<Thing>(hashSet, null));
                foreach (Thing thing in hashSet2)
                {
                    Building_XenomorphCocoon building_Cocoon = (Building_XenomorphCocoon)thing;
                    IntVec3 nextValidPlacementSpot = building_Cocoon.NextValidPlacementSpot;
                    bool flag3 = nextValidPlacementSpot != default(IntVec3);
                    if (flag3)
                    {
                        result = nextValidPlacementSpot;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00005BDC File Offset: 0x00003DDC
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil prepareToSpin = new Toil();
            prepareToSpin.initAction = delegate ()
            {
                bool flag = this.Prey == null && this.Corpse == null;
                if (flag)
                {
                    this.pawn.jobs.EndCurrentJob((JobCondition)3, true);
                }
                else
                {
                    bool dead = this.Prey.Dead;
                    if (dead)
                    {
                        this.pawn.CurJob.SetTarget((TargetIndex)1, this.Prey.Corpse);
                    }
                }
            };
            Toil gotoBody = Toils_Goto.GotoThing((TargetIndex)1, (PathEndMode)2);
            gotoBody.AddPreInitAction(delegate ()
            {
                this.pawn.ClearAllReservations(true);
                ReservationUtility.Reserve(this.pawn, base.TargetA, this.job, 1, -1, null, true);
                string text = "ROM_SpinPreyJob1";
                Thing thing = base.TargetA.Thing;
                this.currentActivity = TranslatorFormattedStringExtensions.Translate(text, ((thing != null) ? thing.LabelShort : null) ?? "");
            });
            Toil spinDelay = new Toil
            {
                defaultCompleteMode = (ToilCompleteMode)3,
                defaultDuration = 500,
                initAction = delegate ()
                {
                    this.currentActivity = Translator.Translate("ROM_SpinPreyJob2");
                }
            };
            ToilEffects.WithProgressBarToilDelay(spinDelay, (TargetIndex)2, false, -0.5f);
            Toil spinBody = new Toil
            {
                initAction = delegate ()
                {
                    Pawn pawnWebSpinner = base.GetActor() as Pawn;
                    bool flag = pawnWebSpinner != null;
                    if (flag)
                    {
                        bool dead = this.Prey.Dead;
                        Thing thing;
                        IntVec3 intVec;
                        if (dead)
                        {
                            thing = this.Prey.Corpse;
                            intVec = this.Prey.Corpse.Position;
                        }
                        else
                        {
                            thing = this.Prey;
                            intVec = this.Prey.Position;
                            bool flag2 = !this.Prey.health.HasHediffsNeedingTend(false);
                            if (flag2)
                            {
                                return;
                            }
                            ThingDef medicineHerbal = ThingDefOf.MedicineHerbal;
                            float num = 0.5f;
                            List<Hediff> list = new List<Hediff>();
                            List<Hediff> hediffs = this.Prey.health.hediffSet.hediffs;
                            for (int i = 0; i < hediffs.Count; i++)
                            {
                                bool flag3 = hediffs[i].TendableNow(false);
                                if (flag3)
                                {
                                    list.Add(hediffs[i]);
                                }
                            }
                            for (int j = 0; j < list.Count; j++)
                            {
                                list[j].Tended(num, j);
                            }
                        }
                        bool flag4 = !thing.Spawned;
                        if (flag4)
                        {
                            base.EndJobWith((JobCondition)3);
                        }
                        else
                        {
                            thing.DeSpawn(0);
                            thing.holdingOwner = null;
                            bool flag5 = !GenConstruct.CanPlaceBlueprintAt(this.CocoonDef, intVec, Rot4.North, this.pawn.Map, false, null).Accepted;
                            if (flag5)
                            {
                                IEnumerable<IntVec3> enumerable = GenAdj.CellsAdjacent8Way(new TargetInfo(intVec, this.pawn.Map, false));
                                foreach (IntVec3 intVec2 in enumerable)
                                {
                                    bool accepted = GenConstruct.CanPlaceBlueprintAt(this.CocoonDef, intVec2, Rot4.North, base.Map, false, null).Accepted;
                                    if (accepted)
                                    {
                                        intVec = intVec2;
                                        break;
                                    }
                                }
                            }
                            Building_XenomorphCocoon building_Cocoon = (Building_XenomorphCocoon)GenSpawn.Spawn(this.CocoonDef, intVec, pawnWebSpinner.Map, 0);
                            building_Cocoon.Spinner = pawnWebSpinner;
                            ThingOwnerUtility.TryGetInnerInteractableThingOwner(building_Cocoon).TryAdd(thing, true);
                            Pawn pawn = this.pawn;
                            if (pawn != null)
                            {
                                Job curJob = pawn.CurJob;
                                if (curJob != null)
                                {
                                    curJob.SetTarget((TargetIndex)2, building_Cocoon);
                                }
                            }
                        }
                    }
                },
                defaultCompleteMode = (ToilCompleteMode)1
            };
            Toil pickupCocoon = Toils_Haul.StartCarryThing((TargetIndex)2, false, false, false);
            pickupCocoon.AddPreInitAction(delegate ()
            {
                this.pawn.CurJob.SetTarget((TargetIndex)3, this.CocoonPlace((Building_XenomorphCocoon)base.TargetB.Thing));
                ReservationUtility.Reserve(this.pawn, base.TargetC, this.job, 1, -1, null, true);
            });
            Toil relocateCocoon = Toils_Haul.CarryHauledThingToCell((TargetIndex)3);
            Toil dropCocoon = ToilFailConditions.FailOn<Toil>(Toils_Haul.PlaceHauledThingInCell((TargetIndex)3, relocateCocoon, false), () => !GenConstruct.CanPlaceBlueprintAt(this.CocoonDef, base.TargetC.Cell, Rot4.North, base.Map, false, null).Accepted);
            base.AddFinishAction(delegate ()
            {
                this.pawn.Map.physicalInteractionReservationManager.ReleaseAllClaimedBy(this.pawn);
            });
            yield return new Toil
            {
                initAction = delegate ()
                {
                    base.Map.attackTargetsCache.UpdateTarget(this.pawn);
                },
                atomicWithPrevious = true,
                defaultCompleteMode = (ToilCompleteMode)1
            };
            Action onHitAction = delegate ()
            {
                Pawn prey = this.Prey;
                bool flag = this.firstHit && !prey.IsColonist;
                bool flag2 = this.pawn.meleeVerbs.TryMeleeAttack(prey, this.job.verbToUse, flag);
                if (flag2)
                {
                    bool flag3 = !this.notifiedPlayer && PawnUtility.ShouldSendNotificationAbout(prey);
                    if (flag3)
                    {
                        this.notifiedPlayer = true;
#pragma warning disable CS0618 // Type or member is obsolete
                        Messages.Message(GenText.CapitalizeFirst(Translator.Translate("MessageAttackedByPredator", new object[]
                        {
                            prey.LabelShort,
                            GenText.LabelIndefinite(pawn)
                        })), prey, MessageTypeDefOf.ThreatBig, true);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                    base.Map.attackTargetsCache.UpdateTarget(this.pawn);
                }
                this.firstHit = false;
            };
            yield return ToilFailConditions.FailOn<Toil>(ToilJumpConditions.JumpIf(Toils_Combat.FollowAndMeleeAttack((TargetIndex)1, onHitAction), () => this.Prey.Downed || this.Prey.Dead, prepareToSpin), () => Find.TickManager.TicksGame > this.startTick + 5000 && (float)(this.job.GetTarget((TargetIndex)1).Cell - this.pawn.Position).LengthHorizontalSquared > 4f);
            yield return ToilFailConditions.FailOn<Toil>(prepareToSpin, () => this.Prey == null);
            yield return ToilFailConditions.FailOn<Toil>(gotoBody, () => this.Prey == null);
            yield return ToilFailConditions.FailOn<Toil>(spinDelay, () => this.Prey == null);
            yield return ToilFailConditions.FailOn<Toil>(spinBody, () => this.Prey == null);
            yield return pickupCocoon;
            yield return relocateCocoon;
            yield return dropCocoon;
            yield break;
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00005BEC File Offset: 0x00003DEC
        public override bool TryMakePreToilReservations(bool showResult)
        {
            return true;
        }

        // Token: 0x04000023 RID: 35
        private bool notifiedPlayer;

        // Token: 0x04000024 RID: 36
        private bool firstHit = true;

        // Token: 0x04000025 RID: 37
        private string currentActivity = "";
    }
}
