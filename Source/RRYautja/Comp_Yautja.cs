using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RRYautja
{
    public class CompProperties_Yautja : CompProperties
    {
        public CompProperties_Yautja()
        {
            this.compClass = typeof(Comp_Yautja);
        }

        public Pawn pawn;
        public Pawn other;
        public Pawn Instigator;
        public bool blooded = false;

        public int TotalkillsRecord = 0;
        public int HumanlikekillsRecord = 0;
        public int AnimalkillsRecord = 0;
        public int MechanoidkillsRecord = 0;

        public List<HediffDef> bloodedDefs;
    }

    public class Comp_Yautja : ThingComp
    {
        public CompProperties_Yautja Props
        {
            get
            {
                return (CompProperties_Yautja)this.props;
            }
        }

        public Backstory pawnStoryA;
        public Backstory pawnStoryC;

        public Pawn pawn;
        public Pawn other;
        public Pawn Instigator;

        public int TotalkillsRecord = 0;
        public int HumanlikekillsRecord = 0;
        public int AnimalkillsRecord = 0;
        public int MechanoidkillsRecord = 0;

        public int pawnKills = 0;
        public int pawnKillsAnimals = 0;
        public int pawnKillsHumanlikes = 0;
        public int pawnKillsMechanoids = 0;

        public HediffDef bloodedbyDef;
        public Hediff bloodedby;

        public bool blooded;
        public bool hasunblooded = false;
        public bool hasbloodedUM = false;
        public bool hasbloodedM = false;
        private BodyPartRecord partRecord;
        private HediffDef hediff;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            String log = string.Format("Loading.....");
            //// Log.Message(log);
            if (base.parent != null && base.parent is Pawn pawn)
            {
                log = string.Format("{0} is a Pawn", pawn.Label);
                // Log.Message(log);
                pawnStoryC = pawn.story.childhood;
                if (pawn.story.adulthood != null)
                {
                    pawnStoryA = pawn.story.adulthood;
                }
                hasunblooded = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Unblooded);
                hasbloodedUM = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_BloodedUM);
                foreach (var Def in pawn.health.hediffSet.hediffs)
                {
                    if (Def.def.defName.StartsWith("RRY_Hediff_BloodedM"))
                    {

                        hasbloodedM = true;
						bloodedby = Def;
                        bloodedbyDef = Def.def;
                        log = string.Format("pawn is blooded {0} and marked with hediff {1} of def {2}", hasbloodedM, bloodedby, bloodedbyDef);
                        // Log.Message(log);
                        break;
                    }
                }
                    log = string.Format("hasunblooded {0}, hasbloodedUM {1}, hasbloodedM {2}", hasunblooded, hasbloodedUM, hasbloodedM);
                // Log.Message(log);
                if (hasunblooded && !hasbloodedUM && !hasbloodedM)
                {
                    blooded = false;
                    log = string.Format("Blooded: {0}", blooded);
                    // Log.Message(log);
                }
                else if (hasunblooded && (hasbloodedUM || hasbloodedM))
                {
                    blooded = true;
                    log = string.Format("Blooded: {0}", blooded);
                    // Log.Message(log);
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Unblooded);
                    log = string.Format("removing: {0}", hediff.Label);
                    // Log.Message(log);
                    pawn.health.RemoveHediff(hediff);
                }
                else
                {
                    blooded = true;
                }
                if (hasbloodedUM && hasbloodedM)
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                    pawn.health.RemoveHediff(hediff);
                }
                if (!hasunblooded && !hasbloodedUM && !hasbloodedM)
                {
                    log = string.Format("{0} Blood Status: {1}", pawn.LabelCap, blooded);
                    // Log.Message(log);
                    if (pawn.story.adulthood != null)
                    {
                        if (pawnStoryA.identifier.StartsWith("Yautja_YoungBlood")) // == "Yautja_YoungBlood")
                        {
                            blooded = false;
                            pawn.health.AddHediff(YautjaDefOf.RRY_Hediff_Unblooded);
                        }
                        else if (pawnStoryA.identifier.StartsWith("Yautja_BadBlood")) // == "Yautja_BadBloodA" || pawnStoryA.identifier == "Yautja_BadBloodB")
                        {
                            blooded = false;
                            pawn.health.AddHediff(YautjaDefOf.RRY_Hediff_Unblooded);
                        }
                        else
                        {
                            blooded = true;
                            HediffDef hediffDef =Props.bloodedDefs.RandomElement();
                            pawn.health.AddHediff(hediffDef);
                        }
                    }
                    else
                    {
                            blooded = false;
                            pawn.health.AddHediff(YautjaDefOf.RRY_Hediff_Unblooded);
                    }
                    log = string.Format("Blooded: {0}", blooded);
                    // Log.Message(log);
                }

            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (base.parent.IsHashIntervalTick(30)&& base.parent != null && base.parent is Pawn pawn)
            {
                if (pawn.records.GetAsInt(RecordDefOf.Kills) > TotalkillsRecord)
                {
                    pawnKills = pawn.records.GetAsInt(RecordDefOf.Kills);
                    // Log.Message("Records mismatch");
                    String log = string.Format("");
                    if (pawn.LastAttackedTarget != null && pawn.LastAttackedTarget.Thing is Pawn other && !pawn.Dead)
                    {
                        Corpse otherCorpse = pawn.LastAttackedTarget.Thing as Corpse;
                        Hediff unblooded = pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Unblooded);
                        foreach (var part in pawn.RaceProps.body.AllParts.Where(x => x.def.defName == "Head"))
                        {
                            partRecord = part;
                        }
                        foreach (var hd in pawn.health.hediffSet.hediffs.Where(x => x.def.defName.StartsWith("RRY_Hediff_BloodedM")))
                        {
                            hediff = hd.def;
                            Log.Message(hediff.defName);
                        }

                        if (pawn.health.hediffSet.HasHediff(unblooded.def))
                        {

                            pawn.health.RemoveHediff(unblooded);
                        }
                        Hediff blooded = HediffMaker.MakeHediff(YautjaDefOf.RRY_Hediff_BloodedUM, pawn, partRecord);
                        if (other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && other.kindDef.race.defName.Contains("Queen") && other.Dead)
                        {
                            // Log.Message("Xenomorph Queen kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Xenomorph " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMXenomorphQueen";

                          //  blooded.def.stages[1].partIgnoreMissingHP = other.kindDef.RaceProps.predator;
                          //  blooded.def.stages[1].minSeverity = other.BodySize;
                          //  blooded.def.stages[1].vomitMtbDays = other.kindDef.combatPower;

                        }
                        else if (other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && !other.kindDef.race.defName.Contains("Queen") && !other.kindDef.race.defName.Contains("Predalien") && !other.kindDef.race.defName.Contains("FaceHugger") && other.Dead)
                        {
                            // Log.Message("Xenomorph kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Xenomorph " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMXenomorph";

                        }
                        else if (other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && other.kindDef.race.defName.Contains("Predalien") && other.Dead)
                        {
                            // Log.Message("Predalien kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Predalien " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMPredalien";

                        }
                        else if (other.kindDef.race.defName == "Alien_Yautja" && other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead)
                        {
                            // Log.Message("BadBlood kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Bad Blood " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMBadBlood";

                        }
                        else if (other.kindDef.race.defName == "Human" && !other.kindDef.factionLeader && other.Dead)
                        {
                            // Log.Message("Human kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Human " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMHuman";

                        }
                        else if (other.kindDef.race.defName == "Human" && other.kindDef.factionLeader && other.Dead)
                        {
                            // Log.Message("Worthy Human kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Worthy Human " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMWorthyHuman";

                        }
                        else if (other.kindDef.race.defName != "Human" && !other.kindDef.factionLeader && other.RaceProps.Humanlike && other.Dead)
                        {
                            // Log.Message("Humanlike kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Humanlike " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMHumanlike";

                        }
                        else if (other.kindDef.race.defName != "Human" && other.kindDef.factionLeader && other.RaceProps.Humanlike && other.Dead)
                        {
                            // Log.Message("Worthy Humanlike kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Worthy Humanlike " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMWorthyHumanlike";

                        }
                        else if (!other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && !other.RaceProps.Humanlike && other.Dead)
                        {
                            // Log.Message("Other kill");
                            blooded.CurStage.label = blooded.CurStage.label + " other " + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedM";

                        }
                        if (!pawn.health.hediffSet.HasHediff(hediff) && !pawn.health.hediffSet.HasHediff(blooded.def))
                        {


                            blooded.def.stages[1].partIgnoreMissingHP = other.kindDef.RaceProps.predator;
                            blooded.def.stages[1].minSeverity = other.BodySize;
                            blooded.def.stages[1].vomitMtbDays = other.kindDef.combatPower;

                            pawn.health.AddHediff(blooded, partRecord, null);
                        }

						
                         if (other.kindDef.race.defName == "Alien_Yautja" && !other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead && (other.Faction.PlayerGoodwill>0 || other.Faction.IsPlayer))
                        {
                            // Log.Message("Yautja kill");
                            blooded.CurStage.label = blooded.CurStage.label + " Yautja " + other.Faction.Name + other.KindLabel;
                            blooded.def.labelNoun = "RRY_Hediff_BloodedMYautja";

                        }
                    }
                    TotalkillsRecord = pawn.records.GetAsInt(RecordDefOf.Kills);
                }
            }
		
        }

        public override void CompTickRare()
        {
			
            base.CompTickRare();

        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);

        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);
            // Log.Message(signal.ToString());
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            Pawn Instigator = dinfo.Instigator as Pawn;
            Props.Instigator = Instigator;


            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }


    }
}
