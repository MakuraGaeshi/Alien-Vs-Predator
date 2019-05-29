using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_XenoFacehugger : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_XenoFacehugger()
        {
            this.compClass = typeof(HediffComp_XenoFacehugger);
        }

        // Token: 0x040033C3 RID: 13251
        public PawnKindDef pawnKindDef;
        public bool spawnLive = false;
        public bool killHost = false;
        public float severityPerDay;
        public Pawn Instigator;
        public bool royaleHugger = false;
    }
    // Token: 0x02000D5B RID: 3419
    public class HediffComp_XenoFacehugger : HediffComp
    {
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look<Pawn>(ref this.Instigator, "pawnRef", true);


            Scribe_Defs.Look<PawnKindDef>(ref this.pawnKindDef, "pawnKindDef");
            Scribe_Defs.Look<HediffDef>(ref this.heDiffDeff, "heDiffDeff");
            Scribe_References.Look<Pawn>(ref this.Instigator, "pawnRef", true);//, Props.pawn);
            Scribe_Deep.Look<Pawn>(ref this.Instigator, "pawnRefDeep");//, Props.pawn);

        }

        // public PawnKindDef pawnKindDef = YautjaDefOf.RRY_Xenomorph_FaceHugger;
        public PawnKindDef pawnKindDef = XenomorphDefOf.RRY_Xenomorph_FaceHugger;
        public HediffDef heDiffDeff = XenomorphDefOf.RRY_XenomorphImpregnation;
        public int timer = 0;
        public int timer2 = 0;
        public bool isImpregnated = false;
        public bool hasImpregnated = false;
        public Pawn Instigator;
        public DamageInfo dInfo;

        public Pawn intigator
        {
            get
            {
                if (Instigator!=null)
                {
                    return this.Instigator;
                }
                if (Props.Instigator!=null)
                {
                    return Props.Instigator;
                }
                return null;
            }
            set
            {
                intigator = value;
            }
        }
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_XenoFacehugger Props
		{
			get
			{
				return (HediffCompProperties_XenoFacehugger)this.props;
			}
		}



        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (base.Pawn.IsHashIntervalTick(200))
            {
                float num = this.SeverityChangePerDay();
                num *= 0.00333333341f;
                severityAdjustment += num;
            }
            if ((this.parent.CurStageIndex == 1 || this.parent.CurStage.label == "impregnation")&&!this.parent.pawn.health.hediffSet.HasHediff(heDiffDeff))
            {
                timer++;
                if (timer>=600&&!isImpregnated)
                {
                    // Log.Message("checking Severity");
                    if (Rand.Value>this.parent.Severity)
                    {
                        // Log.Message("adding embryo");
                        parent.pawn.health.AddHediff(heDiffDeff, parent.pawn.RaceProps.body.corePart);
                        Hediff hediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(heDiffDeff);
                        hediff.TryGetComp<HediffComp_XenoSpawner>();
                        hasImpregnated = true;
                        if (base.Pawn.health.hediffSet.HasHediff(heDiffDeff) && !isImpregnated)
                        {
                            isImpregnated = true;
                            // Log.Message("is Impregnated", isImpregnated);
                        }
                    }
                    timer = 0;
                }
            }
            else if ((this.parent.CurStageIndex == 2 || this.parent.CurStage.label == "post impregnation"))
            {
                timer2++;
                if (timer2 >= 600)
                {
                    // Log.Message("checking Severity");
                    if (Rand.Value<this.parent.Severity)
                    {
                        // Log.Message("removing Facehugger");
                        Hediff heDiff = base.Pawn.health.hediffSet.GetFirstHediffOfDef(base.parent.def);
                        base.Pawn.health.hediffSet.hediffs.Remove(heDiff);
                        this.CompPostPostRemoved();
                    }
                    timer2 = 0;
                }
            }
        }
        
        public override void Notify_PawnDied()
        {
            this.CompPostPostRemoved();
            base.Notify_PawnDied();
        }

        public override void CompPostPostRemoved()
        {
            Thing hostThing = base.Pawn;
            Pawn hostPawn = base.Pawn;
            IntVec3 spawnLoc = !base.Pawn.Dead ? base.parent.pawn.Position : base.parent.pawn.PositionHeld;
            Map spawnMap = !base.Pawn.Dead ? base.parent.pawn.Map : base.parent.pawn.MapHeld;
            bool spawnLive = Props.spawnLive;
            if ((hostPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation) && !hasImpregnated)||hostPawn.Dead)
            {
                spawnLive = true;
            }
            PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, null, PawnGenerationContext.NonPlayer, -1, true, false, true, false, true, true, 20f);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            if (Instigator!=null)
            {
                pawn = intigator;
            }
            if (spawnLive == true)
            {
                GenSpawn.Spawn(pawn, spawnLoc, spawnMap, 0);
            }
            else
            {
                GenSpawn.Spawn(pawn, spawnLoc, spawnMap, 0);
                pawn.Kill(null);
            }
            string text = TranslatorFormattedStringExtensions.Translate("Xeno_Facehugger_Detach", base.parent.pawn.LabelShort);
            if (!base.Pawn.Dead) MoteMaker.ThrowText(spawnLoc.ToVector3(), spawnMap, text, 5f);
        }

        // Token: 0x06004C89 RID: 19593 RVA: 0x002379D6 File Offset: 0x00235DD6
        protected virtual float SeverityChangePerDay()
        {
            return this.Props.severityPerDay;
        }

        // Token: 0x06004C8A RID: 19594 RVA: 0x002379E4 File Offset: 0x00235DE4
        public override string CompDebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.CompDebugString());
            if (!base.Pawn.Dead)
            {
                stringBuilder.AppendLine("severity/day: " + this.SeverityChangePerDay().ToString("F3"));
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }

        // Token: 0x04003401 RID: 13313
        protected const int SeverityUpdateInterval = 200;
    }
    
}
