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
        public float severityPerDay;
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
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_XenoSpawner Props
        {
            get
            {
                return (HediffCompProperties_XenoSpawner)this.props;
            }
        }
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.lastCoughTick, "thislastCoughTick");
            Scribe_Values.Look<int>(ref this.lastCoughStage, "thislastCoughStage");
            Scribe_Values.Look<int>(ref this.timesCoughed, "thistimesCoughed");
            Scribe_Values.Look<int>(ref this.timesCoughedBlood, "thistimesCoughedBlood");
            Scribe_Values.Look<float>(ref this.lastCoughSeverity, "thislastCoughSeverity");
            Scribe_Values.Look<int>(ref this.Impregnations, "Impregnations", 0);
            Scribe_Values.Look<int>(ref this.countToSpawn, "countToSpawn", 1);
            Scribe_Values.Look<bool>(ref this.royaleHugger, "royaleHugger");
            Scribe_Values.Look<bool>(ref this.predalienImpregnation, "predalienImpregnation",false);
        }

        bool logonce = false;
        public int countToSpawn = 1;
        int lastCoughTick = 0;
        int nextCoughTick = 0; 
        int lastCoughStage=0;
        int timesCoughed = 0;
        int timesCoughedBlood = 0;
        float lastCoughSeverity=0;
        public bool royaleHugger;
        public bool predalienImpregnation;
        public int Impregnations;

        public bool RoyaleHugger
        {
            get
            {
                return royaleHugger;
            }
        }

        public bool RoyaleEmbryo
        {
            get
            {
                return RoyaleHugger && Impregnations < maxImpregnations;
            }
        }

        public int maxImpregnations
        {
            get
            {
                if (RoyaleHugger)
                {
                    return 2;
                }
                return 1;
            }
        }

        List<string> Coughlist = new List<string>
        {
            "quietly",
            "abruptly",
            "loudly",
            "painfully",
            "violently"
        };

        public IntVec3 MyPos
        {
            get
            {
                return Pawn.Position != null ? Pawn.Position : Pawn.Position;
            }
        }

        public Map MyMap
        {
            get
            {
                return Pawn.Map ?? Pawn.MapHeld;
            }
        }

        public bool RoyalPresent
        {
            get
            {
                bool selected = Find.Selector.SelectedObjects.Contains(Pawn) && Prefs.DevMode;
                Predicate<Pawn> validator = delegate (Pawn t)
                {
                    bool SelfFlag = t != Pawn;
                    bool RoyalHuggerInfection = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_FaceHuggerInfection).TryGetComp<HediffComp_XenoFacehugger>().RoyaleHugger);
                    bool RoyalImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_XenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
                    bool RoyalHiddenImpregnation = (t.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation) && t.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_HiddenXenomorphImpregnation).TryGetComp<HediffComp_XenoSpawner>().RoyaleHugger);
                    return SelfFlag || RoyalHuggerInfection || RoyalImpregnation || RoyalHiddenImpregnation;
                };
                return MyMap.mapPawns.AllPawnsSpawned.Any(validator);
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            DeathActionWorker daw = this.Pawn.def.race.DeathActionWorker;
            this.Pawn.def.race.deathActionWorkerClass = typeof(DeathActionWorker_Simple);
            if (parent.pawn.Spawned)
            {
                if (parent.pawn.Map != null)
                {
                    if (parent.def == XenomorphDefOf.RRY_HiddenNeomorphImpregnation || parent.def == XenomorphDefOf.RRY_NeomorphImpregnation && this.parent.pawn.Faction == Faction.OfPlayer)
                    {
                        string text = TranslatorFormattedStringExtensions.Translate("Xeno_Neospores_Added", base.parent.pawn.LabelShortCap, parent.Part.LabelShort);
                        MoteMaker.ThrowText(base.parent.pawn.Position.ToVector3(), base.parent.pawn.Map, text, 3f);
                    }
                }
            }
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
            if (base.Pawn.IsHashIntervalTick(200))
            {
                float num = this.SeverityChangePerDay();
                num *= 0.00333333341f;
                severityAdjustment += num;
            }
            if (parent.ageTicks> nextCoughTick && (this.Def == XenomorphDefOf.RRY_HiddenNeomorphImpregnation || this.Def == XenomorphDefOf.RRY_NeomorphImpregnation) && Pawn.Map != null && Pawn.Spawned)
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
            //    if (selected) Log.Message(string.Format("Pre Death stage: {0}", parent.CurStage.label));
#endif
                int num = Find.TickManager.TicksGame % 300 * 2;
#if DEBUG
            //    if (selected) Log.Message(string.Format("num: {0}", num));
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

        public Pawn XenomorphSpawnRequest()
        {
            Gender gender;
            bool QueenPresent = false;
            bool selected = Find.Selector.SingleSelectedThing == parent.pawn && Prefs.DevMode;
            List<PawnKindDef> pawnKindDefs = Props.pawnKindDefs;
            List<float> pawnKindWeights = Props.pawnKindWeights;
            PawnKindDef pawnKindDef = pawnKindDefs[pawnKindDefs.Count - 1];
            int ind = 0;
            foreach (var p in MyMap.mapPawns.AllPawnsSpawned)
            {
                if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                {

                    QueenPresent = true;
                    break;
                }
            }
            if (RoyaleEmbryo)
            {
                pawnKindDef = XenomorphDefOf.RRY_Xenomorph_Queen;
            }
            else
            {
                foreach (var PKDef in pawnKindDefs)
                {
                    float hostSize = base.parent.pawn.BodySize;
                    float spawnRoll = ((Rand.Range(1, 100)) * hostSize);

                    if (PKDef == XenomorphDefOf.RRY_Xenomorph_Queen && (QueenPresent || predalienImpregnation))
                    {
                        spawnRoll = 0;
#if DEBUG
                    //    Log.Message(string.Format("{0} :{1}", PKDef.label, QueenPresent));
#endif
                    }
                    else if (PKDef == XenomorphDefOf.RRY_Xenomorph_Queen && !QueenPresent && !predalienImpregnation && !RoyalPresent)
                    {
                        if (PKDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                        {
                            spawnRoll *= 2;
                        }
                    }
                    else if (predalienImpregnation)
                    {
                        if (PKDef == XenomorphDefOf.RRY_Xenomorph_Runner)
                        {
                            spawnRoll *= 2;
                        }
                        else if (PKDef == XenomorphDefOf.RRY_Xenomorph_Drone)
                        {
                            spawnRoll *= 2;
                        }
                    }
                    if (spawnRoll > (100 - pawnKindWeights[ind]))
                    {
                        pawnKindDef = PKDef;
                        break;
                    }
                    ind++;
                }
                if (Pawn.kindDef.race == YautjaDefOf.RRY_Alien_Yautja && !predalienImpregnation)
                {
                    pawnKindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
                }
            }
            

            if (pawnKindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
            {
                gender = Gender.Female;
            }
            else
            {
                gender = Gender.None;
            }
#if DEBUG
            if (Prefs.DevMode)
            {
             //    Log.Message(string.Format("spawning: {0}", pawnKindDef.label));
            }
#endif
            return PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKindDef, Find.FactionManager.FirstFactionOfDef(pawnKindDef.defaultFactionType), PawnGenerationContext.NonPlayer, -1, true, true, false, false, true, true, 20f, fixedGender: gender));
        }

        public Color HostBloodColour
        {
            get
            {
                Color color = base.parent.pawn.def.race.BloodDef.graphic.color;
                color.a = 1f;
                return color;
            }
        } 

        public override void Notify_PawnDied()
        {
            IntVec3 spawnLoc = !base.Pawn.Dead ? base.parent.pawn.Position : base.parent.pawn.PositionHeld;
            Map spawnMap = !base.Pawn.Dead ? base.parent.pawn.Map : base.parent.pawn.MapHeld;
            bool selected = true;
            this.Pawn.def.race.deathActionWorkerClass = typeof(DeathActionWorker_Simple);
            bool fullterm = this.parent.CurStageIndex > this.parent.def.stages.Count - 3;
            if (!fullterm)
            {
#if DEBUG
                if (Prefs.DevMode)
                {
                //    Log.Message(string.Format("fullterm on death: {0}", fullterm));
                }
#endif
                return;
            }
            else
            {
#if DEBUG
                if (Prefs.DevMode)
                {
                //    Log.Message(string.Format("fullterm on death: {0}", fullterm));
                }
#endif
            }
            if (spawnMap == null || spawnLoc == null)
            {
#if DEBUG
                if (Prefs.DevMode)
                {
                //    Log.Message(string.Format("spawnMap == null {0} || spawnLoc == null {1}", spawnMap == null, spawnLoc == null));
                }
#endif
                return;
            }
            else
            {
#if DEBUG
                if (Prefs.DevMode)
                {
                //    Log.Message(string.Format("spawnMap == {0} spawnLoc == {1}", spawnMap, spawnLoc));
                }
#endif
            }
#if DEBUG
            if (Prefs.DevMode)
            {
            //    Log.Message(string.Format("countToSpawn == {0}", countToSpawn));
            }
#endif
            if (countToSpawn == 0) countToSpawn++;
            for (int i = 0; i < countToSpawn; i++)
            {
                Pawn pawn = XenomorphSpawnRequest();
                pawn.ageTracker.CurKindLifeStage.bodyGraphicData.color = HostBloodColour;
                pawn.Notify_ColorChanged();
                pawn.ageTracker.AgeBiologicalTicks = 0;
                pawn.ageTracker.AgeChronologicalTicks = 0;
                Comp_Xenomorph _Xenomorph = pawn.TryGetComp<Comp_Xenomorph>();
                if (_Xenomorph != null)
                {
                    _Xenomorph.host = base.parent.pawn.kindDef;
                }
#if DEBUG
                if (Prefs.DevMode)
                {
                //    Log.Message(string.Format("spawning: {0}, spawnLoc: {1}, spawnMap: {2}", pawn.LabelCap, spawnLoc, spawnMap));
                }
#endif
                GenSpawn.Spawn(pawn, spawnLoc, spawnMap, 0);
            }
            Vector3 vector = spawnLoc.ToVector3Shifted();
            for (int i = 0; i < 101; i++)
            {
                if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 2f, 3.TicksToSeconds()))
                {
                    MoteMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    }, spawnMap, 1.0f, new Color(HostBloodColour.r, HostBloodColour.g, HostBloodColour.b, HostBloodColour.a));
                }
            }
            string text = TranslatorFormattedStringExtensions.Translate("Xeno_Chestburster_Emerge", base.parent.pawn.LabelShort, this.parent.Part.LabelShort);
            MoteMaker.ThrowText(spawnLoc.ToVector3(), spawnMap, text, 5f);

        }

        // Token: 0x06004C89 RID: 19593 RVA: 0x002379D6 File Offset: 0x00235DD6
        protected virtual float SeverityChangePerDay()
        {
            if (RoyaleHugger)
            {
                return this.Props.severityPerDay/5;
            }
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

        [TweakValue("Gameplay", 0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;
    }
}
