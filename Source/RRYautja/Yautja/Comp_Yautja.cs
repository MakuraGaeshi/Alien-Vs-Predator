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

        public int TotalkillsRecord = 0;

        public int pawnKills = 0;

        public bool blooded;

        public HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
        public HediffDef GenericmarkedDef = YautjaDefOf.RRY_Hediff_BloodedM;
        public HediffDef markedDef;

        public Hediff marked;
        public Hediff BloodStatus;
        public Hediff unmarked;

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
        }

        public override void CompTick()
        {
            base.CompTick();
            if (base.parent.IsHashIntervalTick(30) && base.parent != null && base.parent is Pawn pawn)
            {
                bool selected = Find.Selector.SelectedObjects.Contains(Pawn);
                blooded = YautjaBloodedUtility.BloodStatus(Pawn, out BloodStatus);
#if DEBUG
                    if (base.parent.IsHashIntervalTick(300) && selected) Log.Message(string.Format("BloodStatus: {0}", BloodStatus));
#endif
                if (BloodStatus.def == unmarkedDef)
                {
#if DEBUG
                            if (base.parent.IsHashIntervalTick(300) && selected) Log.Message(string.Format("unmarkedDef: {0}", unmarkedDef));
#endif
                    unmarked = BloodStatus;
#if DEBUG
                            if (base.parent.IsHashIntervalTick(300) && selected) Log.Message(string.Format("unmarked: {0}", unmarked));
#endif
                    if (this.MarkHedifflabel != null)
                    {
#if DEBUG
                            if (base.parent.IsHashIntervalTick(300) && selected) Log.Message(string.Format("{0}", this.MarkHedifflabel));
#endif
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
                        Corpse otherCorpse = other.Corpse;
#if DEBUG
                        if (selected) Log.Message("found corpse");
#endif
                        int omelee = other.RaceProps.Humanlike ? other.skills.GetSkill(SkillDefOf.Melee).Level : 0;
#if DEBUG
                        if (selected) Log.Message(string.Format("omelee: {0}", omelee));
#endif
                        int oshoot = other.RaceProps.Humanlike ? other.skills.GetSkill(SkillDefOf.Shooting).Level : 0;
#if DEBUG
                        if (selected) Log.Message(string.Format("oshoot: {0}", oshoot));
#endif
                        float mdps = other.GetStatValue(StatDefOf.MeleeDPS);
#if DEBUG
                        if (selected) Log.Message(string.Format("mdps: {0}", mdps));
#endif
                        float mhc = other.GetStatValue(StatDefOf.MeleeHitChance);
#if DEBUG
                        if (selected) Log.Message(string.Format("mhc: {0}", mhc));
#endif
                        float mdc = other.GetStatValue(StatDefOf.MeleeDodgeChance);
#if DEBUG
                        if (selected) Log.Message(string.Format("mdc: {0}", mdc));
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
                        else if (other.kindDef.race == YautjaDefOf.Alien_Yautja && !other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead && (other.Faction.PlayerGoodwill > 0 || other.Faction.IsPlayer))
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedMBadBlood;
                        }
                        else if (!other.kindDef.race.defName.StartsWith("RRY_Xenomorph_") && !other.RaceProps.Humanlike && other.Dead && (other.kindDef.combatPower>100 || (other.kindDef.RaceProps.predator == true && other.kindDef.combatPower > 50)))
                        {
                            markedDef = YautjaDefOf.RRY_Hediff_BloodedM;
                        } else
                        {
#if DEBUG
                            if (selected) Log.Message(string.Format("Unworthy kill, ignoring"));
#endif
                            TotalkillsRecord = Pawn.records.GetAsInt(RecordDefOf.Kills);
                            return;
                        }
                        if (markedDef == null)
                        {
#if DEBUG
                            if (selected) Log.Message(string.Format("markedDef is null, break failed"));
#endif

                        }
#if DEBUG
                        if (selected) Log.Message(string.Format("markedDef: {0}", markedDef));
#endif
                        if (Pawn.health.hediffSet.HasHediff(unbloodedDef))
                        {
                            Hediff unblooded = Pawn.health.hediffSet.GetFirstHediffOfDef(this.unbloodedDef);
#if DEBUG
                            if (selected) Log.Message("found unblooded");
#endif
#if DEBUG
                            if (selected) Log.Message("removing old unblooded hediff");
#endif
                            Pawn.health.hediffSet.hediffs.Remove(unblooded);
#if DEBUG
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
                            this.pawn = other;
                            this.corpse = otherCorpse;
                            this.MarkedhediffDef = markedDef;
                            this.MarkHedifflabel = other.KindLabel;
                            this.predator = other.kindDef.RaceProps.predator;
                            this.BodySize = other.BodySize;
                            this.combatPower = other.kindDef.combatPower;
#if DEBUG
                            if (selected) Log.Message("adding hediff");
#endif
                            Pawn.health.AddHediff(HediffMaker.MakeHediff(YautjaDefOf.RRY_Hediff_BloodedUM, Pawn, partRecord), partRecord, null);
                            HediffWithComps blooded = (HediffWithComps)Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
#if DEBUG
                            if (selected) Log.Message("adding comp to hediff");
#endif
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
                            if (selected) Log.Message("info Stored");
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

                        //else if (!pawn.health.hediffSet.HasHediff(unblooded.def) && pawn.health.hediffSet.HasHediff(blooded.def))
                        else if (Pawn.health.hediffSet.HasHediff(unmarkedDef))
                        {
#if DEBUG
                            if (selected) Log.Message("old unmarked hediff found, checking combatPower");
#endif
                            if (this.combatPower < other.kindDef.combatPower)
                            {
#if DEBUG
                                if (selected) Log.Message("new combatPower is higher, removing old unmarked hediff");
#endif
                                Hediff oldunmarked = Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                                Pawn.health.hediffSet.hediffs.Remove(oldunmarked);

#if DEBUG
                                if (selected) Log.Message("store new info");
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
#if DEBUG
                                if (selected) Log.Message("adding comp to hediff");
#endif
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
                                if (selected) Log.Message("info Stored");
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
#if DEBUG
                                        if (selected) Log.Message("old marked hediff found, checking combatPower");
#endif
                                        if (this.combatPower < other.kindDef.combatPower)
                                        {
#if DEBUG
                                            if (selected) Log.Message("new combatPower is higher, removing old marked hediff");
#endif
                                            Hediff oldmarked = item;
                                            // Pawn.health.hediffSet.hediffs.Remove(oldmarked);

#if DEBUG
                                            if (selected) Log.Message("store new info");
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
#if DEBUG
                                            if (selected) Log.Message("adding comp to hediff");
#endif
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
                                            if (selected) Log.Message("info Stored");
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
