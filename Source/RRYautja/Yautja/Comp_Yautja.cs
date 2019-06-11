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

        public Pawn pawn;
        public Pawn other;
        public HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
        public HediffDef GenericmarkedDef = YautjaDefOf.RRY_Hediff_BloodedM;
        public HediffDef markedDef;
        public HediffDef MarkedhediffDef;
        public Hediff marked;
        public Hediff BloodStatus;
        public Hediff unmarked;
        public Corpse corpse;
        public string MarkHedifftype;
        public string MarkHedifflabel;
        public float combatPower;
        public float BodySize;
        public int TotalkillsRecord = 0;
        public int pawnKills = 0;
        public bool TurretIsOn;
        public bool predator;
        public bool blooded;

        AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_YoungBlood");
        AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_Blooded");
        AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodA");
        AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodB");
        
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
            Scribe_Values.Look<bool>(ref this.TurretIsOn, "thisTurretIsOn");
            Scribe_Values.Look<bool>(ref this.blooded, "thisblooded");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if ((Pawn.story.hairDef != YautjaDefOf.RRY_Yaujta_Dreds && Pawn.story.hairDef != YautjaDefOf.RRY_Yaujta_Ponytail && Pawn.story.hairDef != YautjaDefOf.RRY_Yaujta_Bald))
            {
                Pawn.story.hairDef = Rand.Chance(0.5f) ? YautjaDefOf.RRY_Yaujta_Dreds : YautjaDefOf.RRY_Yaujta_Ponytail;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (base.parent.IsHashIntervalTick(30) && base.parent != null && base.parent is Pawn pawn && pawn.Map !=null)
            {
                bool selected = Find.Selector.SelectedObjects.Contains(Pawn);
                blooded = YautjaBloodedUtility.BloodStatus(Pawn, out BloodStatus);
                
                if (BloodStatus.def == unmarkedDef)
                {
                    unmarked = BloodStatus;
                    if (this.MarkHedifflabel != null)
                    {

                    }
                }
                if (Pawn.records.GetAsInt(RecordDefOf.Kills) > TotalkillsRecord)
                {
                    pawnKills = Pawn.records.GetAsInt(RecordDefOf.Kills);
                    if (Pawn.LastAttackedTarget != null && (Pawn.LastAttackedTarget.Thing is Pawn other && !Pawn.Dead))
                    {
                        Corpse otherCorpse = other.Corpse;
                        int omelee = other.RaceProps.Humanlike ? other.skills.GetSkill(SkillDefOf.Melee).Level : 0;
                        int oshoot = other.RaceProps.Humanlike ? other.skills.GetSkill(SkillDefOf.Shooting).Level : 0;
                        float mdps = other.GetStatValue(StatDefOf.MeleeDPS);
                        float mhc = other.GetStatValue(StatDefOf.MeleeHitChance);
                        float mdc = other.GetStatValue(StatDefOf.MeleeDodgeChance);
                        if (other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Queen && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen;
                        }
                        else if (other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Drone || other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Runner || other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Warrior || other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Neomorph && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMXenomorph;
                        }
                        else if (other.kindDef.race == ThingDefOf.Thrumbo && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMXenomorph;
                        }
                        else if (other.kindDef.race == XenomorphRacesDefOf.RRY_Xenomorph_Predalien && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMPredalien;
                        }
                        else if (other.kindDef.race == YautjaDefOf.RRY_Alien_Yautja && other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMBadBlood;
                        }
                        else if (other.kindDef.race == ThingDefOf.Human && !other.kindDef.factionLeader && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMHuman;
                        }
                        else if (other.kindDef.race == ThingDefOf.Human && (other.kindDef.factionLeader || (other.kindDef.isFighter && other.kindDef.combatPower > (100-(omelee + oshoot)))) && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman;
                        }
                        else if (other.kindDef.race != ThingDefOf.Human && !other.kindDef.factionLeader && other.RaceProps.Humanlike && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMHumanlike;
                        }
                        else if (other.kindDef.race != ThingDefOf.Human && (other.kindDef.factionLeader || (other.kindDef.isFighter && other.kindDef.combatPower > (100 - (omelee + oshoot)))) && other.RaceProps.Humanlike && other.Dead)
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike;
                        }
                        else if (other.kindDef.race == YautjaDefOf.RRY_Alien_Yautja && !other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead && (other.Faction.PlayerGoodwill > 0 || other.Faction.IsPlayer))
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMBadBlood;
                        }
                        else if (!other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && !other.RaceProps.Humanlike && other.Dead && (other.kindDef.combatPower>100 || (other.kindDef.RaceProps.predator == true && other.kindDef.combatPower > 50)))
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedM;
                        }
                        else
                        {
                            TotalkillsRecord = Pawn.records.GetAsInt(RecordDefOf.Kills);
                            return;
                        }
                        if (markedDef == null)
                        {

                        }
                        if (Pawn.health.hediffSet.HasHediff(unbloodedDef))
                        {
                            Hediff unblooded = Pawn.health.hediffSet.GetFirstHediffOfDef(this.unbloodedDef);
                            Pawn.health.hediffSet.hediffs.Remove(unblooded);
#if DEBUG
                        //    if (selected) Log.Message("store info");
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
                            HediffComp_BloodedYautja bloodedYautja = blooded.TryGetComp<HediffComp_BloodedYautja>();
#if DEBUG
                        //    if (selected) Log.Message("info Stored");
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
                        else if (Pawn.health.hediffSet.HasHediff(unmarkedDef))
                        {
                            if (this.combatPower < other.kindDef.combatPower)
                            {
                                Hediff oldunmarked = Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                                Pawn.health.hediffSet.hediffs.Remove(oldunmarked);
#if DEBUG
                            //    if (selected) Log.Message("store new info");
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
                                HediffComp_BloodedYautja bloodedYautja = blooded.TryGetComp<HediffComp_BloodedYautja>();
#if DEBUG
                            //    if (selected) Log.Message("info Stored");
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
                        }
                        else
                        {
                            bool hasbloodedM = Pawn.health.hediffSet.hediffs.Any<Hediff>(x => x.def.defName.StartsWith(GenericmarkedDef.defName));
                            if (hasbloodedM)
                            {
                                foreach (var item in Pawn.health.hediffSet.hediffs)
                                {
                                    if (item.def.defName.StartsWith(GenericmarkedDef.defName))
                                    {
                                        if (this.combatPower < other.kindDef.combatPower)
                                        {
                                            Hediff oldmarked = item;
#if DEBUG
                                        //    if (selected) Log.Message("store new info");
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
                                            HediffComp_BloodedYautja bloodedYautja = blooded.TryGetComp<HediffComp_BloodedYautja>();
#if DEBUG
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
                                        break;
                                    }
                                }
                            }

                        }
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
