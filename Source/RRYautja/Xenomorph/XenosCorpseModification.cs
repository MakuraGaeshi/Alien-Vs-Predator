using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RRYautja
{
    /*
    [StaticConstructorOnStartup]
    public class XenosCorpseModification
    {
        
        static XenosCorpseModification()
        {
            
            DefDatabase<ThingDef>.AllDefsListForReading.ForEach(action: td =>
            {
                if (td.IsCorpse)
                {
                    td.comps.Add(new CompProperties_UsableCorpse()
                    {
                        compClass = typeof(CompTakeTrophy),
                        useJob = YautjaDefOf.RRY_Yautja_TakeTrophy,
                        useLabel = "Take Trophy from {0}"
                    });
                    td.comps.Add(new CompProperties_UseEffect()
                    {
                        compClass = typeof(CompUseEffect_TakeTrophy)
                        
                        
                        //     chance = 0.25f
                    });
                    //    td.tickerType = TickerType.Normal;
                }
            });
            
        }

    }


    // Token: 0x02000791 RID: 1937
    public class CompUseEffect_TakeTrophy : CompUseEffect
    {
        bool logonce = false;
        BodyPartRecord partRecord;
        bool hasTail;
        bool hasShell;
        Corpse corpse;

        // Token: 0x06002ADC RID: 10972 RVA: 0x001433C0 File Offset: 0x001417C0
        public override void DoEffect(Pawn user)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(user);
        //    base.DoEffect(user);
            corpse.InnerPawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, corpse.InnerPawn, this.partRecord));
            ThingDef thingDef = null;
            if (this.partRecord.def == XenomorphDefOf.RRY_Xeno_TailSpike)
            {
                thingDef = XenomorphDefOf.RRY_Xenomorph_TailSpike;
            }
            if (this.partRecord.def == XenomorphDefOf.RRY_Xeno_Shell)
            {
                thingDef = XenomorphDefOf.RRY_Xenomorph_TailSpike;
            }
            if (thingDef!=null)
            {
                GenSpawn.Spawn(ThingMaker.MakeThing(thingDef), user.Position, user.Map);
            }
            
        }

        // Token: 0x06002ADD RID: 10973 RVA: 0x00143464 File Offset: 0x00141864
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(p);

            if (this.parent is Corpse corpse)
            {
                this.corpse = corpse;
            //    Log.Message(string.Format("hastail: {0}", hasTail));
                if (XenomorphUtil.IsXenoCorpse(corpse))
                {
                    foreach (var item in corpse.InnerPawn.RaceProps.body.AllParts)
                    {
                        if (item.def == XenomorphDefOf.RRY_Xeno_TailSpike)
                        {
                            partRecord = item;
                            failReason = null;
                            hasTail = true;
                            return true;
                        }
                        if (item.def == XenomorphDefOf.RRY_Xeno_Shell)
                        {
                            partRecord = item;
                            failReason = null;
                            hasShell = true;
                            return true;
                        }
                    }
                }
                else
                {
                    failReason = "Missing Parts";
                    return false;
                }
            }
            else
            {
                failReason = "not a corpse";
                return false;
            }
            return base.CanBeUsedBy(p, out failReason);
        }

        public override float OrderPriority => 1;
    }
    // Token: 0x0200074F RID: 1871
    public class CompTakeTrophy : Comp_UsableCorpse
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
            if (!this.CanBeUsedBy(myPawn, out failReason))
            {
                yield break;
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

        // Token: 0x06002944 RID: 10564 RVA: 0x00139525 File Offset: 0x00137925
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        }

        
        // Token: 0x06002945 RID: 10565 RVA: 0x00139539 File Offset: 0x00137939
       // public override string TransformLabel(string label)
       // {
       //     return this.skill.LabelCap + " " + label;
       // }
        

        // Token: 0x06002946 RID: 10566 RVA: 0x00139554 File Offset: 0x00137954
        public override bool AllowStackWith(Thing other)
        {
            return false;
        }

        // Token: 0x040016DD RID: 5853
        public SkillDef skill;
    }
    */
}
