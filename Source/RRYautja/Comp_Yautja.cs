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

        public HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public HediffDef bloodedbyDef;
        public Hediff bloodedby;
        public Corpse markCorpse;

        public bool blooded;
        public bool hasunblooded = false;
        public bool hasbloodedUM = false;
        public bool hasbloodedM = false;
        public BodyPartRecord partRecord;
        public HediffDef hediff;
        public Hediff myhediff;
        public float combatpower;
        public float bodysize;
        public bool preadator;
        String log = string.Format("");


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (base.parent != null && base.parent is Pawn pawn)
            {
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
                        break;
                    }
                }
                if (hasunblooded && !hasbloodedUM && !hasbloodedM)
                {
                    blooded = false;
                }
                else if (hasunblooded && (hasbloodedUM || hasbloodedM))
                {
                    blooded = true;
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Unblooded);
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
                    bool selected = Find.Selector.SingleSelectedThing == pawn;
                    pawnKills = pawn.records.GetAsInt(RecordDefOf.Kills);
                    foreach (var part in pawn.RaceProps.body.AllParts.Where(x => x.def.defName == "Head"))
                    {
                        partRecord = part;
                    }
                    if (selected) Log.Message("Records mismatch");
                    if (pawn.LastAttackedTarget != null && pawn.LastAttackedTarget.Thing is Pawn other && !pawn.Dead)
                    {
                        if (selected) Log.Message("found pawn");
                        Corpse otherCorpse = pawn.LastAttackedTarget.Thing as Corpse;
                        markCorpse = otherCorpse;
                        Hediff unblooded = pawn.health.hediffSet.GetFirstHediffOfDef(unbloodedDef);

						
                        Hediff blooded = HediffMaker.MakeHediff(YautjaDefOf.RRY_Hediff_BloodedUM, pawn, partRecord);
                        if (other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && other.kindDef.race.defName.Contains("Queen") && other.Dead)
                        {
                            if (selected) Log.Message("Xenomorph Queen kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMXenomorphQueen";

                        }
                        else if (other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && !other.kindDef.race.defName.Contains("Queen") && !other.kindDef.race.defName.Contains("Predalien") && !other.kindDef.race.defName.Contains("FaceHugger") && other.Dead)
                        {
                              if (selected) Log.Message("Xenomorph kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMXenomorph";

                        }
                        else if (other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && other.kindDef.race.defName.Contains("Predalien") && other.Dead)
                        {
                              if (selected) Log.Message("Predalien kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMPredalien";

                        }
                        else if (other.kindDef.race.defName == "Alien_Yautja" && other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead)
                        {
                              if (selected) Log.Message("BadBlood kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMBadBlood";

                        }
                        else if (other.kindDef.race.defName == "Human" && !other.kindDef.factionLeader && other.Dead)
                        {
                              if (selected) Log.Message("Human kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMHuman";

                        }
                        else if (other.kindDef.race.defName == "Human" && other.kindDef.factionLeader && other.Dead)
                        {
                              if (selected) Log.Message("Worthy Human kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMWorthyHuman";

                        }
                        else if (other.kindDef.race.defName != "Human" && !other.kindDef.factionLeader && other.RaceProps.Humanlike && other.Dead)
                        {
                              if (selected) Log.Message("Humanlike kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMHumanlike";

                        }
                        else if (other.kindDef.race.defName != "Human" && other.kindDef.factionLeader && other.RaceProps.Humanlike && other.Dead)
                        {
                              if (selected) Log.Message("Worthy Humanlike kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMWorthyHumanlike";

                        }
                        else if (!other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && !other.RaceProps.Humanlike && other.Dead)
                        {
                              if (selected) Log.Message("Other kill");
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedM";
							

                        }
                        if (unblooded==null)
                        {
							unblooded = HediffMaker.MakeHediff(unbloodedDef, pawn, partRecord);
                        }
                          if (selected) Log.Message(string.Format("pawn.health.hediffSet.HasHediff(unblooded.def) = {0}", pawn.health.hediffSet.HasHediff(unblooded.def)));
                          if (selected) Log.Message(string.Format("pawn.health.hediffSet.HasHediff(blooded.def) = {0}", pawn.health.hediffSet.HasHediff(blooded.def)));
                        if (pawn.health.hediffSet.HasHediff(unblooded.def) && !pawn.health.hediffSet.HasHediff(blooded.def))
                        {
                              if (selected) Log.Message("store info");

                            blooded.def.stages[1].label = other.KindLabel;
                            blooded.def.stages[0].partIgnoreMissingHP = other.kindDef.RaceProps.predator;
                            blooded.def.stages[0].vomitMtbDays = other.BodySize;
                            blooded.def.stages[0].deathMtbDays = other.kindDef.combatPower;


                              if (selected) Log.Message("removing old unblooded hediff");
                            pawn.health.hediffSet.hediffs.Remove(unblooded);
                              if (selected) Log.Message("adding hediff");
                            pawn.health.AddHediff(blooded, partRecord, null);
                        }
                        else if (!pawn.health.hediffSet.HasHediff(unblooded.def) && pawn.health.hediffSet.HasHediff(blooded.def))
                        {
                            myhediff = pawn.health.hediffSet.GetFirstHediffOfDef(blooded.def);
                              if (selected) Log.Message(string.Format("old {0} new {1}", myhediff.def.stages[0].deathMtbDays ,other.kindDef.combatPower));
                            if (myhediff.def.stages[0].deathMtbDays< other.kindDef.combatPower)
                            {
                                  if (selected) Log.Message("removing old blooded hediff");

                                pawn.health.hediffSet.hediffs.Remove(myhediff);

                                  if (selected) Log.Message("store info");

                                blooded.def.stages[1].label = other.def.LabelCap+ " " +other.KindLabel +" " +other.kindDef.combatPower;
                                blooded.def.stages[0].partIgnoreMissingHP = other.kindDef.RaceProps.predator;
                                blooded.def.stages[0].vomitMtbDays = other.BodySize;
                                blooded.def.stages[0].deathMtbDays = other.kindDef.combatPower;

                                  if (selected) Log.Message("adding new hediff");

							//	otherCorpse.comps.Add(new CompProperties_Necron())
                                pawn.health.AddHediff(blooded, partRecord, null);
                            }
                        }
						
                         if (other.kindDef.race.defName == "Alien_Yautja" && !other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead && (other.Faction.PlayerGoodwill>0 || other.Faction.IsPlayer))
                        {
                              if (selected) Log.Message("Honourable Bad blood Yautja kill");
                            blooded.def.stages[1].label = " Yautja " + other.Faction.Name + other.KindLabel;
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMYautja";

                        }
                    }
                    TotalkillsRecord = pawn.records.GetAsInt(RecordDefOf.Kills);
                }
            }
		
        }

        public override void CompTickRare()
        {
			
            base.CompTickRare();
            if (base.parent != null && base.parent is Pawn pawn)
            {
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
                        break;
                    }
                }
                if (hasunblooded && !hasbloodedUM && !hasbloodedM)
                {
                    blooded = false;
                }
                else if (hasunblooded && (hasbloodedUM || hasbloodedM))
                {
                    blooded = true;
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Unblooded);
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
                            HediffDef hediffDef = Props.bloodedDefs.RandomElement();
                            pawn.health.AddHediff(hediffDef);
                        }
                    }
                    else
                    {
                        blooded = false;
                        pawn.health.AddHediff(YautjaDefOf.RRY_Hediff_Unblooded);
                    }

                }

            }

        }

        public override void PostIngested(Pawn ingester)
        {
            base.PostIngested(ingester);
            Log.Message("noms");
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);


        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);
          //  Log.Message(signal.ToString());
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            Pawn Instigator = dinfo.Instigator as Pawn;
            Props.Instigator = Instigator;


            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }


    }
}
