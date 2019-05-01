using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;

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
        }
    }

    // Token: 0x02000791 RID: 1937
    public class CompUseEffect_MarkSelf : CompUseEffect
    {
        bool logonce = false;
        // Token: 0x06002ADC RID: 10972 RVA: 0x001433C0 File Offset: 0x001417C0
        public override void DoEffect(Pawn user)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(user);
        //    base.DoEffect(user);
            Hediff hediff;
            Comp_Yautja _Yautja = user.TryGetComp<Comp_Yautja>();
            Hediff blooded = user.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
            BodyPartRecord part = user.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM).Part;
            HediffDef marked = _Yautja.MarkedhediffDef;
            if (YautjaBloodedUtility.Marked(user, out hediff))
            {
                user.health.RemoveHediff(hediff);
            }
            user.health.RemoveHediff(blooded);
            user.health.AddHediff(marked, part);
            ThingDef thingDef = null;
            foreach (var item in corpse.InnerPawn.health.hediffSet.GetNotMissingParts())
            {
                if (Rand.Chance(corpse.InnerPawn.health.hediffSet.GetPartHealth(item)) &&item.def == XenomorphDefOf.RRY_Xeno_TailSpike && !corpse.InnerPawn.health.hediffSet.PartIsMissing(item))
                {
                    partRecord = item;
                    thingDef = XenomorphDefOf.RRY_Xenomorph_TailSpike;
                    corpse.InnerPawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, corpse.InnerPawn, this.partRecord));
                    GenSpawn.Spawn(ThingMaker.MakeThing(thingDef), user.Position, user.Map);

                }
                if (Rand.Chance(corpse.InnerPawn.health.hediffSet.GetPartHealth(item)) && item.def == XenomorphDefOf.RRY_Xeno_Shell && !corpse.InnerPawn.health.hediffSet.PartIsMissing(item))
                {
                    partRecord = item;
                    thingDef = XenomorphDefOf.RRY_Xenomorph_HeadShell;
                    corpse.InnerPawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, corpse.InnerPawn, this.partRecord));
                    GenSpawn.Spawn(ThingMaker.MakeThing(thingDef), user.Position, user.Map);
                }
            }
            if (user.story.adulthood.identifier == null || user.story.adulthood.identifier == "Yautja_YoungBlood")
            {
                AlienRace.BackstoryDef backstoryDef = DefDatabase<AlienRace.BackstoryDef>.GetNamed("Yautja_Blooded");
#if DEBUG
                if (selected) Log.Message(string.Format("changing {0}", user.story.adulthood.identifier));
#endif

                user.story.adulthood = backstoryDef.backstory;
#if DEBUG
                if (selected) Log.Message(string.Format("to {0}", user.story.adulthood.identifier));
#endif
            }
        }

        BodyPartRecord partRecord;
        bool hasTail;
        bool hasShell;
        Corpse corpse;
        // Token: 0x06002ADD RID: 10973 RVA: 0x00143464 File Offset: 0x00141864
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(p);
            if (p.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_BloodedUM))
            {
            //    Log.Message(string.Format("has RRY_Hediff_BloodedUM"));
                Hediff blooded = p.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                Comp_Yautja _Yautja = p.TryGetComp<Comp_Yautja>();
                HediffWithComps hediff = (HediffWithComps)blooded;
#if DEBUG
                if (selected) Log.Message(string.Format("has {0} comp", hediff.comps.Count));
#endif
                HediffComp_BloodedYautja comp = blooded.TryGetComp<HediffComp_BloodedYautja>();
#if DEBUG
                if (selected) Log.Message(string.Format("{0}", _Yautja.corpse));
                if (selected) Log.Message(string.Format("{0}", _Yautja.pawn));
                if (selected) Log.Message(string.Format("{0}", _Yautja.MarkedhediffDef));
#endif
                logonce = true;
                ThingDef def = _Yautja.corpse.InnerPawn.kindDef.race;
                if (this.parent is Corpse corpse)
                {
                    this.corpse = corpse;
                    //    Log.Message(string.Format("this.parent is Corpse corpse"));
                    //    Log.Message(string.Format("corpse.InnerPawn.kindDef.race: {0}, def: {1}", corpse.InnerPawn.kindDef.race, def));
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
            return base.CanBeUsedBy(p, out failReason);
        }

        // Token: 0x04001786 RID: 6022
        private const float XPGainAmount = 50000f;
    }
    // Token: 0x0200074F RID: 1871
    public class CompKillMarker : Comp_UsableCorpse
    {
        // Token: 0x17000651 RID: 1617
        // (get) Token: 0x06002942 RID: 10562 RVA: 0x001394F0 File Offset: 0x001378F0
        protected override string FloatMenuOptionLabel
        {
            get
            {
                return string.Format(base.Props.useLabel, this.parent.LabelCap);
            }
        }

        // Token: 0x06002A4A RID: 10826 RVA: 0x00138F4C File Offset: 0x0013734C
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            string failReason;
            HediffSet hediffSet = myPawn.health.hediffSet;
            if (!hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_BloodedUM, false))
            {
                yield break;
            }
            if (!this.CanBeUsedBy(myPawn, out failReason))
            {
                yield break;
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
        private bool CanBeUsedBy(Pawn p, out string failReason)
        {
            List<ThingComp> allComps = this.parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                CompUseEffect compUseEffect = allComps[i] as CompUseEffect;
                if (compUseEffect != null && !compUseEffect.CanBeUsedBy(p, out failReason))
                {
                    return false;
                }
            }
            failReason = null;
            return true;
        }

        // Token: 0x06002944 RID: 10564 RVA: 0x00139525 File Offset: 0x00137925
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.skill = DefDatabase<SkillDef>.GetRandom();
        }
        /*
        // Token: 0x06002945 RID: 10565 RVA: 0x00139539 File Offset: 0x00137939
        public override string TransformLabel(string label)
        {
            return this.skill.LabelCap + " " + label;
        }
        */
        // Token: 0x06002946 RID: 10566 RVA: 0x00139554 File Offset: 0x00137954
        public override bool AllowStackWith(Thing other)
        {
            return false;
        }

        // Token: 0x040016DD RID: 5853
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
            string failReason;
            if (!this.CanBeUsedBy(myPawn, out failReason))
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
                CompUseEffect compUseEffect = allComps[i] as CompUseEffect;
                if (compUseEffect != null && !compUseEffect.CanBeUsedBy(p, out failReason))
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
            string text;
            if (!this.CanBeUsedBy(user, out text))
            {
                return;
            }
            Job job = new Job(this.Props.useJob, this.parent);
            user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

        // Token: 0x06002A4C RID: 10828 RVA: 0x00138FDC File Offset: 0x001373DC
        public void UsedBy(Pawn p)
        {
            string text;
            if (!this.CanBeUsedBy(p, out text))
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
                catch (Exception arg)
                {
                    Log.Error("Error in CompUseEffect: " + arg, false);
                }
            }
        }
    }
}