using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
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
        /*
        Scribe_Defs.Look<HediffDef>(ref MarkedhediffDef, "MarkedhediffDef");
        Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef");//, Props.corpse);//
        Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef");//, Props.pawn);
        Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");//, Props.Marklabel);
        Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");//, Props.Marklabel);
        Scribe_Values.Look<bool>(ref this.predator, "thisPred");
        Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
        Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
        */
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.lastCoughTick, "thislastCoughTick");
            Scribe_Values.Look<int>(ref this.lastCoughStage, "thislastCoughStage");
            Scribe_Values.Look<int>(ref this.timesCoughed, "thistimesCoughed");
            Scribe_Values.Look<int>(ref this.timesCoughedBlood, "thistimesCoughedBlood");
            Scribe_Values.Look<float>(ref this.lastCoughSeverity, "thislastCoughSeverity");
        }

        bool logonce = false;
        int lastCoughTick = 0;
        int nextCoughTick = 0; 
        int lastCoughStage=0;
        int timesCoughed = 0;
        int timesCoughedBlood = 0;
        float lastCoughSeverity=0;

        List<string> Coughlist = new List<string>
        {
            "quietly",
            "abruptly",
            "loudly",
            "painfully",
            "violently"
        };

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
            Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 1"));
            base.CompPostPostAdd(dinfo);
            Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 2"));
            DeathActionWorker daw = this.Pawn.def.race.DeathActionWorker;
            Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 3"));
            this.Pawn.def.race.deathActionWorkerClass = typeof(DeathActionWorker_Simple);
            Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 4"));
            if (parent.def == XenomorphDefOf.RRY_HiddenNeomorphImpregnation|| parent.def == XenomorphDefOf.RRY_NeomorphImpregnation&& this.parent.pawn.Faction == Faction.OfPlayer && base.parent.pawn.Map!=null)
            {
                Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 5"));
                string text = TranslatorFormattedStringExtensions.Translate("Xeno_Neospores_Added", base.parent.pawn.LabelShortCap, parent.Part.LabelShort);
                //    Log.Message(text);
                Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 6"));
                MoteMaker.ThrowText(base.parent.pawn.Position.ToVector3(), base.parent.pawn.Map, text, 3f);
                Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 7"));
            }
            Log.Message(string.Format("HediffComp_XenoSpawner CompPostPostAdd 8"));
        }

        public void DoNeoCough()
        {
            lastCoughSeverity = parent.Severity;
            lastCoughStage = parent.CurStageIndex;
            lastCoughTick = parent.ageTicks; // Find.TickManager.TicksGame; // parent.ageTicks; //
            nextCoughTick = lastCoughTick + Rand.RangeInclusive(3000, 24000);
            float chance = ((0.5f * lastCoughSeverity) * lastCoughStage) + (((timesCoughedBlood*2) + timesCoughed)/10);
            if (Rand.Chance(chance))
            {
                string text = TranslatorFormattedStringExtensions.Translate("Xeno_Neospores_Cough", base.parent.pawn.LabelShortCap, Coughlist[timesCoughed+timesCoughedBlood]);
                if (this.parent.pawn.Faction == Faction.OfPlayer)
                {
                //    Log.Message(text);
                    MoteMaker.ThrowText(base.parent.pawn.Position.ToVector3(), base.parent.pawn.Map, text, 3f);
                }
                if (Rand.Chance(chance))
                {
                    text = TranslatorFormattedStringExtensions.Translate("Xeno_Neospores_Cough_Blood");
                    if (this.parent.pawn.Faction == Faction.OfPlayer)
                    {
                    //    Log.Message(text);
                        MoteMaker.ThrowText(base.parent.pawn.Position.ToVector3(), base.parent.pawn.Map, text, 3f);
                    }
                    parent.pawn.health.DropBloodFilth();
                    timesCoughedBlood++;
                }
                else
                {
                    timesCoughed++;
                }
                
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            bool selected = Find.Selector.SingleSelectedThing == parent.pawn;
            if (parent.CurStageIndex >= parent.def.stages.Count - 3 && this.Pawn.Map == null) return;
            base.CompPostTick(ref severityAdjustment);
            if (parent.ageTicks> nextCoughTick && (this.Def == XenomorphDefOf.RRY_HiddenNeomorphImpregnation || this.Def == XenomorphDefOf.RRY_NeomorphImpregnation))
            {
                DoNeoCough();

            }

            if (parent.CurStageIndex == parent.def.stages.Count - 2)
            {
                if (parent.Part != parent.pawn.RaceProps.body.corePart) parent.Part = parent.pawn.RaceProps.body.corePart;
                if (!this.logonce && this.parent.pawn.Downed)
                {
                    string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_PreEmerge", this.Pawn.LabelShort);
                //    Log.Message(text);
                    MoteMaker.ThrowText(base.parent.pawn.Position.ToVector3(), base.parent.pawn.Map, text, 5f);
                    this.logonce = true;

                }
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
            IntVec3 spawnLoc = !base.Pawn.Dead ? base.parent.pawn.Position : base.parent.pawn.PositionHeld;
            Map spawnMap = !base.Pawn.Dead ? base.parent.pawn.Map : base.parent.pawn.MapHeld;
            bool selected = Find.Selector.SingleSelectedThing == parent.pawn;
            ///((Pawn)base.Pawn).def.race.deathActionWorkerClass.
            this.Pawn.def.race.deathActionWorkerClass = typeof(DeathActionWorker_Simple);
            List<PawnKindDef> pawnKindDefs = Props.pawnKindDefs;
            List<float> pawnKindWeights = Props.pawnKindWeights;
            PawnKindDef pawnKindDef = pawnKindDefs[pawnKindDefs.Count - 1];
            int ind = 0;
            bool fullterm = this.parent.CurStageIndex > this.parent.def.stages.Count - 3;
            if (!fullterm) return;
            if (this.Pawn.MapHeld == null) return;
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

                if (spawnRoll > (100 - pawnKindWeights[ind]))
                {
                    pawnKindDef = PKDef;
                    break;
                }

                ind++;
            }
#if DEBUG
        //    Log.Message(string.Format("{0} Old pawnKindDef.lifeStages[0].bodyGraphicData.color: {1}", base.parent.pawn.Name, pawnKindDef.lifeStages[0].bodyGraphicData.color));
        //    Log.Message(string.Format("{0} base.parent.pawn.def.race.BloodDef.graphic.color: {1}", base.parent.pawn.Name, base.parent.pawn.def.race.BloodDef.graphic.color));
#endif
            if (Pawn.kindDef.race == YautjaDefOf.RRY_Alien_Yautja)
            {
                pawnKindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
            }
            Color color = base.parent.pawn.def.race.BloodDef.graphic.color;
            color.a = 1f;
            pawnKindDef.lifeStages[0].bodyGraphicData.color = color;
#if DEBUG
        //    Log.Message(string.Format("{0} new value.kindDef.lifeStages[0].bodyGraphicData.color: {1}", base.parent.pawn.Name, pawnKindDef.lifeStages[0].bodyGraphicData.color));
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
            if (_Xenomorph != null)
            {
                _Xenomorph.host = base.parent.pawn.kindDef;
            }
            Vector3 vector = spawnLoc.ToVector3Shifted();
            GenSpawn.Spawn(pawn, spawnLoc, spawnMap, 0);
            for (int i = 0; i < 101; i++)
            { // Find.TickManager.TicksGame
                if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 2f, 3.TicksToSeconds()))
                {
                    MoteMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    }, spawnMap, 1.0f, new Color(color.r, color.g, color.b, 1f));
                }
            }
            string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_Emerge", base.parent.pawn.LabelShort, this.parent.Part.LabelShort);
            MoteMaker.ThrowText(spawnLoc.ToVector3(), spawnMap, text, 5f);
            //base.Pawn.health.RemoveHediff(this.parent);
        }

        [TweakValue("Gameplay", 0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;
    }
}
