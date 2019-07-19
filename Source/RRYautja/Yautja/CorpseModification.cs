using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;
using RRYautja.settings;

namespace RRYautja
{
    [StaticConstructorOnStartup]
    public class CorpseModification
    {
        static CorpseModification()
        {

            DefDatabase<ThingDef>.AllDefsListForReading.ForEach(action: td =>
            {
                if (td.IsCorpse)
                {
                    td.comps.Add(new CompProperties_UsableCorpse()
                    {
                        compClass = typeof(CompKillMarker),
                        useJob = YautjaDefOf.RRY_Yautja_MarkSelf,
                        useLabel = "Use {0} to mark self as Blooded"
                    });
                    td.comps.Add(new CompProperties_UseEffect()
                    {
                        compClass = typeof(CompUseEffect_MarkSelf)
                        //     chance = 0.25f
                    });
                    //    td.tickerType = TickerType.Normal;
                }
            });
            DefDatabase<ThingDef>.AllDefsListForReading.ForEach(action: td =>
            {
                if (td.race!=null)
                {
                    if (td.race.Humanlike)
                    {
                        bool pawnflag = !((td.defName.StartsWith("Android") && td.defName.Contains("Tier")) || td.defName.Contains("ChjDroid") || td.defName.Contains("ChjBattleDroid") || td.defName.Contains("M7Mech"));
                        //    Log.Message(string.Format("Checking: {0}", td.label));
                        if (!td.HasComp(typeof(Comp_Yautja))&& pawnflag)
                        {
                            td.comps.Add(new CompProperties_Yautja()
                            {
                                bloodedDefs = new List<HediffDef>()
                                {
                                    YautjaDefOf.RRY_Hediff_BloodedM,
                                    YautjaDefOf.RRY_Hediff_BloodedMHuman,
                                    YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman,
                                    YautjaDefOf.RRY_Hediff_BloodedMHumanlike,
                                    YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike,
                                    YautjaDefOf.RRY_Hediff_BloodedMMechanoid,
                                    YautjaDefOf.RRY_Hediff_BloodedMXenomorph,
                                    YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen,
                                    YautjaDefOf.RRY_Hediff_BloodedMPredalien,
                                    YautjaDefOf.RRY_Hediff_BloodedMBadBlood,
                                    YautjaDefOf.RRY_Hediff_BloodedMHound,
                                    YautjaDefOf.RRY_Hediff_BloodedMThrumbo,
                                    YautjaDefOf.RRY_Hediff_BloodedMCrusher,
                                    YautjaDefOf.RRY_Hediff_BloodedMGroTye
                                }
                            });
                            if (td.HasComp(typeof(Comp_Yautja)))
                            {
                            //    Log.Message(string.Format("Added Comp_Yautja to: {0}", td.label));
                            }
                        }
                        /*
                        td.comps.Add(new CompProperties_UseEffect()
                        {
                            compClass = typeof(CompUseEffect_MarkSelf)
                            //     chance = 0.25f
                        });
                        //    td.tickerType = TickerType.Normal;
                        */
                    }
                }
            });
            DefDatabase<PawnKindDef>.AllDefsListForReading.ForEach(action: td => 
            {

            });
        }
    }

    // Token: 0x02000791 RID: 1937
    public class CompUseEffect_MarkSelf : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(user);
            //    base.DoEffect(user);
            Comp_Yautja _Yautja = user.TryGetComp<Comp_Yautja>();
            Hediff blooded = user.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
            BodyPartRecord part = user.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM).Part;
            HediffDef markedDef = _Yautja.MarkedhediffDef;
            if (YautjaBloodedUtility.Marked(user, out Hediff hediff))
            {
                user.health.RemoveHediff(hediff);
            }
            user.health.RemoveHediff(blooded);
            Hediff marked = HediffMaker.MakeHediff(markedDef, user, part);// user.health.hediffSet.GetFirstHediffOfDef(markedDef);
            HediffComp_MarkedYautja marked_Yautja = marked.TryGetComp<HediffComp_MarkedYautja>();
            corpse = (Corpse)this.parent;
            marked_Yautja.BodySize = corpse.InnerPawn.BodySize;
            marked_Yautja.combatPower = corpse.InnerPawn.kindDef.combatPower;
            marked_Yautja.corpse = corpse;
            marked_Yautja.MarkHedifflabel = corpse.InnerPawn.KindLabel;
            marked_Yautja.pawn = corpse.InnerPawn;
            marked_Yautja.pawnKindDef = corpse.InnerPawn.kindDef;
            marked_Yautja.predator = corpse.InnerPawn.kindDef.RaceProps.predator;
            marked_Yautja.props = new HediffCompProperties_MarkedYautja
            {
                pawn = corpse.InnerPawn,
                corpse = corpse,
                MarkedhediffDef = markedDef,
                MarkHedifflabel = corpse.InnerPawn.KindLabel,
                predator = corpse.InnerPawn.kindDef.RaceProps.predator,
                BodySize = corpse.InnerPawn.BodySize,
                combatPower = corpse.InnerPawn.kindDef.combatPower
            };
            Log.Message(string.Format(" 11 "));
            user.health.AddHediff(markedDef, part);
            Log.Message(string.Format(" 12 "));
            ThingDef thingDef = null;
            Log.Message(string.Format(" 13 "));
            foreach (var item in corpse.InnerPawn.health.hediffSet.GetNotMissingParts())
            {
                if (Rand.Chance(corpse.InnerPawn.health.hediffSet.GetPartHealth(item)/3) &&item.def == XenomorphDefOf.RRY_Xeno_TailSpike && !corpse.InnerPawn.health.hediffSet.PartIsMissing(item))
                {
                    partRecord = item;
                    thingDef = XenomorphDefOf.RRY_Xenomorph_TailSpike;
                    corpse.InnerPawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, corpse.InnerPawn, this.partRecord));
                    GenSpawn.Spawn(ThingMaker.MakeThing(thingDef), user.Position, user.Map);

                }
                if (Rand.Chance(corpse.InnerPawn.health.hediffSet.GetPartHealth(item)/3) && item.def == XenomorphDefOf.RRY_Xeno_Shell && !corpse.InnerPawn.health.hediffSet.PartIsMissing(item))
                {
                    partRecord = item;
                    thingDef = XenomorphDefOf.RRY_Xenomorph_HeadShell;
                    corpse.InnerPawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, corpse.InnerPawn, this.partRecord));
                    GenSpawn.Spawn(ThingMaker.MakeThing(thingDef), user.Position, user.Map);
                }
            }
            if (user.story.adulthood.identifier == null || user.story.adulthood.identifier == "RRY_Yautja_YoungBlood")
            {
                if (marked.def==YautjaDefOf.RRY_Hediff_BloodedMXenomorph)
                {
                    AlienRace.BackstoryDef backstoryDef = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_Blooded");
#if DEBUG
            //    if (selected) Log.Message(string.Format("changing {0}", user.story.adulthood.identifier));
#endif

                    user.story.adulthood = backstoryDef.backstory;
#if DEBUG
            //    if (selected) Log.Message(string.Format("to {0}", user.story.adulthood.identifier));
#endif
                }
            }
        }

        BodyPartRecord partRecord;
        public Corpse corpse;
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(p);
            if (p.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_BloodedUM))
            {
                Hediff blooded = p.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                Comp_Yautja _Yautja = p.TryGetComp<Comp_Yautja>();
                HediffWithComps hediff = (HediffWithComps)blooded;
                HediffComp_BloodedYautja comp = blooded.TryGetComp<HediffComp_BloodedYautja>();
                /*
                if (selected) Log.Message(string.Format("{0}", _Yautja.corpse));
                if (selected) Log.Message(string.Format("{0}", _Yautja.pawn.Corpse));
                if (selected) Log.Message(string.Format("{0}", _Yautja.pawn));
                if (selected) Log.Message(string.Format("{0}", _Yautja.MarkedhediffDef));
                */
                ThingDef def = _Yautja.pawn.Corpse.InnerPawn.kindDef.race;
                if (this.parent is Corpse corpse)
                {
                    this.corpse = corpse;
                    /*
                    Log.Message(string.Format("this.parent is Corpse corpse"));
                    Log.Message(string.Format("corpse.InnerPawn.kindDef.race: {0}, def: {1}", corpse.InnerPawn.kindDef.race, def));
                    */
                    if (corpse.InnerPawn.kindDef.race == def)
                    {
                        failReason = null;
                        return true;
                    } 
                    else
                    {
                        failReason = "Wrong race";
                        return false;
                    }
                }
                else
                {
                    failReason = "not a corpse";
                    return false;
                }
                /*
                if (YautjaBloodedUtility.bloodmatch(marked, (Corpse)this.parent))
                {
                    failReason = null;
                    return true;
                }
                */
            }
            else
            {
                failReason = "Doesnt need marking";
                return false;
            }
        }
        private const float XPGainAmount = 50000f;
    }

    public class CompKillMarker : Comp_UsableCorpse
    {

        protected override string FloatMenuOptionLabel
        {
            get
            {
                return string.Format(base.Props.useLabel, this.parent.LabelCap);
            }
        }
        
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            HediffSet hediffSet = myPawn.health.hediffSet;
            if (!hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_BloodedUM, false))
            {
                yield break;
            }
            if (!this.CanBeUsedBy(myPawn, out string failReason))
            {
                yield break;
            //    yield return new FloatMenuOption(this.FloatMenuOptionLabel + ((failReason == null) ? string.Empty : (" (" + failReason + ")")), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.CanReserve(this.parent, 1, -1, null, false))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else
            {
                FloatMenuOption useopt = new FloatMenuOption(this.FloatMenuOptionLabel, delegate ()
                {
                    if (myPawn.CanReserveAndReach(this.parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
                    {
                        foreach (CompUseEffect compUseEffect in this.parent.GetComps<CompUseEffect>())
                        {
                            if (compUseEffect.SelectedUseOption(myPawn))
                            {
                                return;
                            }
                        }
                        this.TryStartUseJob(myPawn);
                    }
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield return useopt;
            }
            yield break;
        }
        
        private new bool CanBeUsedBy(Pawn p, out string failReason)
        {
            List<ThingComp> allComps = this.parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                if (allComps[i] is CompUseEffect compUseEffect && !compUseEffect.CanBeUsedBy(p, out failReason))
                {
                    return false;
                }
            }
            failReason = null;
            return true;
        }
        
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.skill = DefDatabase<SkillDef>.GetRandom();
        }
        /*
        public override string TransformLabel(string label)
        {
            return this.skill.LabelCap + " " + label;
        }
        */

        public override bool AllowStackWith(Thing other)
        {
            return false;
        }
        
        public SkillDef skill;
    }

    // Token: 0x0200025E RID: 606
    public class CompProperties_UsableCorpse : CompProperties
    {
        // Token: 0x06000ACC RID: 2764 RVA: 0x00056399 File Offset: 0x00054799
        public CompProperties_UsableCorpse()
        {
            this.compClass = typeof(Comp_UsableCorpse);
        }

        // Token: 0x040004D8 RID: 1240
        public JobDef useJob;

        // Token: 0x040004D9 RID: 1241
        [MustTranslate]
        public string useLabel;

        // Token: 0x040004DA RID: 1242
        public int useDuration = 100;
    }

    // Token: 0x02000774 RID: 1908
    public class Comp_UsableCorpse : ThingComp
    {
        // Token: 0x1700068E RID: 1678
        // (get) Token: 0x06002A48 RID: 10824 RVA: 0x00138F32 File Offset: 0x00137332
        public CompProperties_UsableCorpse Props
        {
            get
            {
                return (CompProperties_UsableCorpse)this.props;
            }
        }

        // Token: 0x1700068F RID: 1679
        // (get) Token: 0x06002A49 RID: 10825 RVA: 0x00138F3F File Offset: 0x0013733F
        protected virtual string FloatMenuOptionLabel
        {
            get
            {
                return this.Props.useLabel;
            }
        }

        // Token: 0x06002A4A RID: 10826 RVA: 0x00138F4C File Offset: 0x0013734C
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            if (!this.CanBeUsedBy(myPawn, out string failReason))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel + ((failReason == null) ? string.Empty : (" (" + failReason + ")")), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.CanReserve(this.parent, 1, -1, null, false))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else
            {
                FloatMenuOption useopt = new FloatMenuOption(this.FloatMenuOptionLabel, delegate ()
                {
                    if (myPawn.CanReserveAndReach(this.parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
                    {
                        foreach (CompUseEffect compUseEffect in this.parent.GetComps<CompUseEffect>())
                        {
                            if (compUseEffect.SelectedUseOption(myPawn))
                            {
                                return;
                            }
                        }
                        this.TryStartUseJob(myPawn);
                    }
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield return useopt;
            }
            yield break;
        }

        // Token: 0x06002A4D RID: 10829 RVA: 0x00139094 File Offset: 0x00137494
        public bool CanBeUsedBy(Pawn p, out string failReason)
        {
            List<ThingComp> allComps = this.parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                if (allComps[i] is CompUseEffect compUseEffect && !compUseEffect.CanBeUsedBy(p, out failReason))
                {
                    return false;
                }
            }
            failReason = null;
            return true;
        }

        // Token: 0x06002A4B RID: 10827 RVA: 0x00138F78 File Offset: 0x00137378
        public void TryStartUseJob(Pawn user)
        {
            if (!user.CanReserveAndReach(this.parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
            {
                return;
            }
            if (!this.CanBeUsedBy(user, out string text))
            {
                return;
            }
            Job job = new Job(this.Props.useJob, this.parent);
            user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

        // Token: 0x06002A4C RID: 10828 RVA: 0x00138FDC File Offset: 0x001373DC
        public void UsedBy(Pawn p)
        {
            if (!this.CanBeUsedBy(p, out string text))
            {
                return;
            }
            foreach (CompUseEffect compUseEffect in from x in this.parent.GetComps<CompUseEffect>()
                                                    orderby x.OrderPriority descending
                                                    select x)
            {
                try
                {
                    compUseEffect.DoEffect(p);
                }
                catch
                {
                //    Log.Error("Error in CompUseEffect: " + arg, false);
                }
            }
        }
    }
}