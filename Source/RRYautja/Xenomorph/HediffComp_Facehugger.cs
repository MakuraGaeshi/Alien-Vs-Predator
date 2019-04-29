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

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look<Pawn>(ref this.Props.Instigator, "Instigator", true);
            Scribe_References.Look<Pawn>(ref Instigator, "Instigator", true);
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
                GenSpawn.Spawn(pawn, base.parent.pawn.Position, base.parent.pawn.Map, 0);
            }
            else
            {
                GenSpawn.Spawn(pawn, base.parent.pawn.Position, base.parent.pawn.Map, 0);
                pawn.Kill(null);
            }
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
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            DeathActionWorker daw = this.Pawn.def.race.DeathActionWorker;
            this.Pawn.def.race.deathActionWorkerClass = typeof(DeathActionWorker_Simple);
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            bool selected = Find.Selector.SingleSelectedThing == parent.pawn;
            if (parent.CurStageIndex >= parent.def.stages.Count - 3 && this.Pawn.Map == null) return;
            base.CompPostTick(ref severityAdjustment);
            if (parent.CurStageIndex == parent.def.stages.Count-2)
            {
#if DEBUG
                if (selected) Log.Message(string.Format("Pre Death stage: {0}", parent.CurStage.label));
#endif
                int num = Find.TickManager.TicksGame % 300 * 2;
#if DEBUG
                if (selected) Log.Message(string.Format("num: {0}", num));
#endif
                if (num < 90)
                {
                    Pawn.Drawer.renderer.wiggler.downedAngle += 0.35f;
                }
                else if (num < 390 && num >= 300)
                {
                    Pawn.Drawer.renderer.wiggler.downedAngle -= 0.35f;
                }
                else if (num < 270 && num >= 180)
                {
                    Pawn.Drawer.renderer.wiggler.downedAngle += 0.35f;
                }
                else if (num < 570 && num >= 510)
                {
                    Pawn.Drawer.renderer.wiggler.downedAngle -= 0.35f;
                }
            }
        }
        public override void Notify_PawnDied()
        {
            bool selected = Find.Selector.SingleSelectedThing == parent.pawn;
            ///((Pawn)base.Pawn).def.race.deathActionWorkerClass.
            this.Pawn.def.race.deathActionWorkerClass = typeof(DeathActionWorker_Simple);
            List<PawnKindDef> pawnKindDefs = Props.pawnKindDefs;
            List<float> pawnKindWeights = Props.pawnKindWeights;
            PawnKindDef pawnKindDef = pawnKindDefs[pawnKindDefs.Count-1];
            int ind = 0;
            bool fullterm = this.parent.CurStageIndex > this.parent.def.stages.Count - 3;
            if (!fullterm) return;
            if (this.Pawn.Map==null) return;
            bool QueenPresent = false;
            foreach (var p in base.parent.pawn.MapHeld.mapPawns.AllPawnsSpawned)
            {
                if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                {
                    QueenPresent = true;
                    break;
                }
            }
            foreach (var PKDef in pawnKindDefs)
            {
                float hostSize = base.parent.pawn.BodySize;
                float spawnRoll = ((Rand.Range(1, 100)) * hostSize);
                
                if (PKDef == XenomorphDefOf.RRY_Xenomorph_Queen && QueenPresent)
                {
                    spawnRoll = 0;
#if DEBUG
                    if (selected) Log.Message(string.Format("{0} :{1}", PKDef.label, QueenPresent));
#endif
                }
                
                if (spawnRoll > (100-pawnKindWeights[ind]))
                {
                    pawnKindDef = PKDef;
                    break;
                }

                ind++;
            }
#if DEBUG
            Log.Message(string.Format("{0} Old pawnKindDef.lifeStages[0].bodyGraphicData.color: {1}", base.parent.pawn.Name, pawnKindDef.lifeStages[0].bodyGraphicData.color));
            Log.Message(string.Format("{0} base.parent.pawn.def.race.BloodDef.graphic.color: {1}", base.parent.pawn.Name, base.parent.pawn.def.race.BloodDef.graphic.color));
#endif
            if (Pawn.kindDef.race==YautjaDefOf.Alien_Yautja)
            {
                pawnKindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
            }
            Color color = base.parent.pawn.def.race.BloodDef.graphic.color;
            color.a = 1f;
            pawnKindDef.lifeStages[0].bodyGraphicData.color = color;
#if DEBUG
            Log.Message(string.Format("{0} new value.kindDef.lifeStages[0].bodyGraphicData.color: {1}", base.parent.pawn.Name, pawnKindDef.lifeStages[0].bodyGraphicData.color));
#endif
            PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, Find.FactionManager.FirstFactionOfDef(pawnKindDef.defaultFactionType), PawnGenerationContext.NonPlayer, -1, true, false, true, false, true, true, 20f);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            pawn.ageTracker.CurKindLifeStage.bodyGraphicData.color = color;
            //this this will error  pawn.Graphic.data.color = color;
            if (pawnKindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
            {
                pawn.gender = Gender.Female;
            }
            else
            {
                pawn.gender = Gender.Male;
            }
            pawn.ageTracker.AgeBiologicalTicks = 0;
            pawn.ageTracker.AgeChronologicalTicks = 0;
            Comp_Xenomorph _Xenomorph = pawn.TryGetComp<Comp_Xenomorph>();
            if (_Xenomorph!=null)
            {
                _Xenomorph.host = base.parent.pawn.kindDef;
            }
            Vector3 vector = base.parent.pawn.PositionHeld.ToVector3Shifted();
            GenSpawn.Spawn(pawn, base.parent.pawn.PositionHeld, base.parent.pawn.MapHeld, 0);
            for (int i = 0; i < 1001; i++)
            { // Find.TickManager.TicksGame
                if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 2f, 3.TicksToSeconds()))
                {
                    MoteMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    }, base.parent.pawn.MapHeld, 1.0f, new Color(color.r, color.g, color.b, 1f));
                }
            }
            //base.Pawn.health.RemoveHediff(this.parent);
        }

        [TweakValue("Gameplay", 0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;
    }
}
