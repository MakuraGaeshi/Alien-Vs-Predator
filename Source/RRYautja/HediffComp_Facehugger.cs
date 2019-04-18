using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
    // Token: 0x02000D5B RID: 3419
    public class HediffComp_XenoFacehugger : HediffComp
    {
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

        /*
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            dInfo = (DamageInfo)dinfo;
            // Log.Message("Adding Facehugger");
            Instigator = (Pawn)dInfo.Instigator;
            // Log.Message(Instigator.LabelCap);
            Instigator.DeSpawn();
        }
        */

        public override void CompPostPostRemoved()
        {
            Thing hostThing = base.Pawn;
            Pawn hostPawn = base.Pawn;
            bool spawnLive = Props.spawnLive;
            if (hostPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation)&&!hasImpregnated)
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
                pawn.mindState.StartFleeingBecauseOfPawnAction(hostThing);
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
            }
            else { pawn.Kill(null); }
            GenSpawn.Spawn(pawn, base.parent.pawn.Position, base.parent.pawn.Map, 0);
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

    // Token: 0x02000D5A RID: 3418
    public class HediffCompProperties_XenoSpawner : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_XenoSpawner()
        {
            this.compClass = typeof(HediffComp_XenoSpawner);
        }

        // Token: 0x040033C3 RID: 13251
        public PawnKindDef pawnKindDef;
        public List<PawnKindDef> pawnKindDefs;
        public List<float> pawnKindWeights;

    }
    public class HediffComp_XenoSpawner : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_XenoSpawner Props
        {
            get
            {
                return (HediffCompProperties_XenoSpawner)this.props;
            }
        }


        public override void Notify_PawnDied()
        {
            List<PawnKindDef> pawnKindDefs = Props.pawnKindDefs;
            List<float> pawnKindWeights = Props.pawnKindWeights;
            PawnKindDef pawnKindDef = pawnKindDefs[pawnKindDefs.Count-1];
            int ind = 0;
            
            foreach (var PKDef in pawnKindDefs)
            {
                float hostSize = base.parent.pawn.BodySize;
                float spawnRoll = ((Rand.Range(1, 100)) * hostSize);
                if (PKDef == XenomorphDefOf.RRY_Xenomorph_Queen && SpawnedQueenCount(base.parent.pawn.MapHeld) != 0)
                {
                    spawnRoll = 0;
                }
                if (spawnRoll > (100-pawnKindWeights[ind]))
                {
                    pawnKindDef = PKDef;
                    break;
                }

                ind++;
            }
            PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, null, PawnGenerationContext.NonPlayer, -1, true, false, true, false, true, true, 20f);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            pawn.ageTracker.AgeBiologicalTicks = 0;
            GenSpawn.Spawn(pawn, base.parent.pawn.PositionHeld, base.parent.pawn.MapHeld, 0);
        }

        public static int SpawnedQueenCount(Map map)
        {
            return map.listerThings.ThingsOfDef(XenomorphRacesDefOf.RRY_Xenomorph_Queen).Count;
        }
    }
}
