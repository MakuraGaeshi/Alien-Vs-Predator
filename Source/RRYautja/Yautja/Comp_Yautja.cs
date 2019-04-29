using RimWorld;
using RRYautja;
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
        public bool blooded;
        
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

        public BodyPartRecord partRecord
        {
            get
            {
                foreach (var part in ((Pawn)parent).RaceProps.body.AllParts.Where(x => x.def.defName == "Head"))
                {
                    return part;
                }
                return null;
            }
        }

        public Pawn Pawn
        {
            get
            {
                return (Pawn)parent;
            }
        }
        
        public Pawn other;
    //    public Pawn Instigator;

        public int TotalkillsRecord = 0;
        //    public int HumanlikekillsRecord = 0;
        //    public int AnimalkillsRecord = 0;
        //    public int MechanoidkillsRecord = 0;

        public int pawnKills = 0;
        //    public int pawnKillsAnimals = 0;
        //    public int pawnKillsHumanlikes = 0;
        //    public int pawnKillsMechanoids = 0;

        public HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
        public HediffDef markedDef;
        //public Hediff bloodedby;
        //public Hediff marked;
        public Corpse markCorpse;

        public bool blooded;
        //public bool hasunblooded = false;
        //public bool hasbloodedUM = false;
        //public bool hasbloodedM = false;
        //public HediffDef hediff;
        public Hediff BloodStatus;
        public Hediff unmarked;
        //public float combatpower;
        //public float bodysize;
        //public bool preadator;
        //String log = string.Format("");
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.TotalkillsRecord, "TotalkillsRecord");
            Scribe_Values.Look<int>(ref this.pawnKills, "pawnKills");
            Scribe_Deep.Look<Hediff>(ref this.unmarked, "bloodedUnmarked");

            Scribe_Defs.Look<HediffDef>(ref this.MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef", true);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef", true);
            Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");
            Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");
            Scribe_Values.Look<bool>(ref this.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
        }

        public HediffDef MarkedhediffDef;
        public Corpse corpse;
        public Pawn pawn;
        public string MarkHedifftype;
        public string MarkHedifflabel;
        public bool predator;
        public float combatPower;
        public float BodySize;



        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            blooded = YautjaBloodedUtility.BloodStatus(Pawn, out BloodStatus);
            if (BloodStatus.def == unmarkedDef)
            {
                unmarked = BloodStatus;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (base.parent.IsHashIntervalTick(30) && base.parent != null && base.parent is Pawn pawn)
            {
                bool selected = Find.Selector.SelectedObjects.Contains(Pawn);
                blooded = YautjaBloodedUtility.BloodStatus(Pawn, out BloodStatus);
                if (selected) Log.Message(string.Format("BloodStatus: {0}", BloodStatus));
                if (BloodStatus.def == unmarkedDef)
                {
                    if (selected) Log.Message(string.Format("unmarkedDef: {0}", unmarkedDef));
                    unmarked = BloodStatus;
                    if (selected) Log.Message(string.Format("unmarked: {0}", unmarked));
                    if (this.MarkHedifflabel!=null)
                    {
                        if (selected) Log.Message(string.Format("{0}", this.MarkHedifflabel));
                    }
                }
                if (Pawn.records.GetAsInt(RecordDefOf.Kills) > TotalkillsRecord)
                {
                    pawnKills = Pawn.records.GetAsInt(RecordDefOf.Kills);
#if DEBUG
                    if (selected) Log.Message("Records mismatch");
                    if (Pawn.LastAttackedTarget.Thing is Pawn p) Log.Message(string.Format("pawn: {0}, {1} ", p.Label, Pawn.LastAttackedTarget.Thing.Label));
                    if (Pawn.LastAttackedTarget.Thing is Corpse c) Log.Message(string.Format("corpse: {0}", c.Label));
#endif
                    if (Pawn.LastAttackedTarget != null && (Pawn.LastAttackedTarget.Thing is Pawn other && !Pawn.Dead))
                    {
#if DEBUG
                        if (selected) Log.Message("found pawn");
#endif
#if DEBUG

#endif
                        Corpse otherCorpse = other.Corpse;
#if DEBUG
                        if (selected) Log.Message("found corpse");
#endif
                        Hediff unblooded = Pawn.health.hediffSet.GetFirstHediffOfDef(this.unbloodedDef);
#if DEBUG
                        if (selected) Log.Message("found unblooded");
#endif

                        if (other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Queen && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen;
                        }
                        else if (other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Drone || other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Runner || other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Warrior || other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Neomorph && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMXenomorph;
                        }
                        else if (other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Predalien && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMPredalien;
                        }
                        else if (other.kindDef.race == YautjaDefOf.Alien_Yautja && other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMBadBlood;
                        }
                        else if (other.kindDef.race == ThingDefOf.Human && !other.kindDef.factionLeader && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMHuman;
                        }
                        else if (other.kindDef.race == ThingDefOf.Human && other.kindDef.factionLeader && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman;
                        }
                        else if (other.kindDef.race != ThingDefOf.Human && !other.kindDef.factionLeader && other.RaceProps.Humanlike && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMHumanlike;
                        }
                        else if (other.kindDef.race != ThingDefOf.Human && other.kindDef.factionLeader && other.RaceProps.Humanlike && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike;
                        }
                        /*
                        else if (!other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && !other.RaceProps.Humanlike && other.Dead)
                        {
                              if (selected) Log.Message("Other kill");
                            markedDef = "RRY_Hediff_BloodedM";
                        }
                        */
                        if (unblooded==null)
                        {
							unblooded = HediffMaker.MakeHediff(unbloodedDef, Pawn, partRecord);
                        }
                        if (Pawn.health.hediffSet.HasHediff(unblooded.def))
                        {
#if DEBUG
                            if (selected) Log.Message("removing old unblooded hediff");
#endif
                            Pawn.health.hediffSet.hediffs.Remove(unblooded);

#if DEBUG
                            if (selected) Log.Message("adding hediff");
#endif
                            this.markCorpse = otherCorpse;
#if DEBUG
                            if (selected) Log.Message("mark corpse");
#endif

                            this.pawn = other;
                            this.corpse = otherCorpse;
                            this.MarkedhediffDef = markedDef;
                            this.MarkHedifflabel = other.KindLabel;
                            this.predator = other.kindDef.RaceProps.predator;
                            this.BodySize = other.BodySize;
                            this.combatPower = other.kindDef.combatPower;
                            Pawn.health.AddHediff(HediffMaker.MakeHediff(YautjaDefOf.RRY_Hediff_BloodedUM, Pawn, partRecord), partRecord, null);
                            HediffWithComps blooded = (HediffWithComps)Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                            blooded.source = Pawn.LastAttackedTarget.Thing.def;
                            blooded.comps[0].props = new HediffCompProperties_BloodedYautja
                            {
                                pawn = other,
                                corpse = otherCorpse,
                                MarkedhediffDef = markedDef,
                                predator = other.kindDef.RaceProps.predator,
                                BodySize = other.BodySize,
                                combatPower = other.kindDef.combatPower
                            };
#if DEBUG
                            if (selected) Log.Message("adding comp to hediff");
                            if (selected) Log.Message("store info");
                            if (selected) Log.Message(string.Format(
                                "{6} is storing other:{0}, corpse:{1}, MarkedhediffDef:{2}, predator:{3}, BodySize:{4}, combatPower:{5}",
                            other.Label,
                            otherCorpse.Label,
                            markedDef,
                            other.kindDef.RaceProps.predator,
                            other.BodySize,
                            other.kindDef.combatPower,
                            Pawn.Name.ToStringShort
                            ));
#endif
                            HediffComp_BloodedYautja bloodedYautja = blooded.TryGetComp<HediffComp_BloodedYautja>();
                            /*
                            HediffComp_BloodedYautja bloodedYautja = new HediffComp_BloodedYautja
                            {
                                props = new HediffCompProperties_BloodedYautja
                                {
                                    pawn = other,
                                    corpse = otherCorpse,
                                    MarkedhediffDef = markedDef,
                                    predator = other.kindDef.RaceProps.predator,
                                    BodySize = other.BodySize,
                                    combatPower = other.kindDef.combatPower
                                }
                            };
                            */
#if DEBUG
                            if (selected) Log.Message("info Stored");
#endif
                            //blooded.comps.Add(bloodedYautja);
#if DEBUG
                           // if (selected) Log.Message("added comp hediff");
                            if (selected) Log.Message(string.Format(
                                "{6} stored other:{0}, corpse:{1}, MarkedhediffDef:{2}, predator:{3}, BodySize:{4}, combatPower:{5}",
                            bloodedYautja.HediffProps.pawn,
                            bloodedYautja.HediffProps.corpse,
                            bloodedYautja.HediffProps.MarkedhediffDef,
                            bloodedYautja.HediffProps.predator,
                            bloodedYautja.HediffProps.BodySize,
                            bloodedYautja.HediffProps.combatPower,
                            Pawn.Name.ToStringShort
                            ));
#endif
                        }
                        /*
                        else if (!pawn.health.hediffSet.HasHediff(unblooded.def) && pawn.health.hediffSet.HasHediff(blooded.def))
                        {
                            myhediff = pawn.health.hediffSet.GetFirstHediffOfDef(blooded.def);
                            HediffWithComps mhdwc = (HediffWithComps)myhediff;
                            HediffComp_BloodedYautja mybloodedYautja = mhdwc.TryGetComp<HediffComp_BloodedYautja>();
                            if (mybloodedYautja.combatPower < other.kindDef.combatPower)
                            {
                                HediffWithComps hdwc = (HediffWithComps)blooded;
                                HediffComp_BloodedYautja bloodedYautja = hdwc.TryGetComp<HediffComp_BloodedYautja>();
                                if (selected) Log.Message("removing old blooded hediff");
                                pawn.health.hediffSet.hediffs.Remove(myhediff);


                                if (selected) Log.Message("store info");
                                //	otherCorpse.comps.Add(new CompProperties_Necron())
                                //Log.Message(string.Format("old bloodedYautja.Props.pawn:{0}, bloodedYautja.Props.corpse:{1}, bloodedYautja.Props.MarkedhediffDef:{2}, ", bloodedYautja.Props.pawn, bloodedYautja.Props.corpse, bloodedYautja.Props.MarkedhediffDef));
                                bloodedYautja.pawn = other;
                                bloodedYautja.corpse = other.Corpse;
                                bloodedYautja.MarkedhediffDef = marked;
                                bloodedYautja.predator = other.kindDef.RaceProps.predator;
                                bloodedYautja.BodySize = other.BodySize;
                                bloodedYautja.combatPower = other.kindDef.combatPower;
                                if (selected) Log.Message("adding new hediff");
                                pawn.health.AddHediff(blooded, partRecord, null);
                            }
                        }
                        */
						
                        /*
                         if (other.kindDef.race.defName == "Alien_Yautja" && !other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead && (other.Faction.PlayerGoodwill>0 || other.Faction.IsPlayer))
                        {
                              if (selected) Log.Message("Honourable Bad blood Yautja kill");
                            blooded.def.stages[1].label = " Yautja " + other.Faction.Name + other.KindLabel;
                            blooded.def.stages[0].label = "RRY_Hediff_BloodedMYautja";

                        }
                         */
                    }
                    TotalkillsRecord = Pawn.records.GetAsInt(RecordDefOf.Kills);
                }
            }
		
        }
        

        public override void PostIngested(Pawn ingester)
        {
            base.PostIngested(ingester);
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            
        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }


    }
}
