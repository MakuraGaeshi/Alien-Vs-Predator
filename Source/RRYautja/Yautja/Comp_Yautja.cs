using AlienRace;
using HunterMarkingSystem;
using RimWorld;
using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RRYautja
{

    public class CompProperties_Yautja : CompProperties
    {
        public CompProperties_Yautja()
        {
            this.compClass = typeof(Comp_Yautja);
        }
        
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

        public static AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.AllDefsListForReading.Find(x => x.defName.Contains("Yautja_YoungBlood"));
        public static AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.AllDefsListForReading.Find(x => x.defName.Contains("Yautja_Blooded"));
        public static AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.AllDefsListForReading.Find(x => x.defName.Contains("Yautja_BadBloodA"));
        public static AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.AllDefsListForReading.Find(x => x.defName.Contains("Yautja_BadBloodB"));

        public Comp_Markable markable => this.parent.TryGetComp<Comp_Markable>();

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

        public bool alienRace
        {
            get
            {
                return Pawn.def is ThingDef_AlienRace;
            }
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (Pawn.kindDef.race == YautjaDefOf.AvP_Alien_Yautja&&(Pawn.story.hairDef != YautjaDefOf.AvP_Yautja_Dreds && Pawn.story.hairDef != YautjaDefOf.AvP_Yautja_Ponytail && Pawn.story.hairDef != YautjaDefOf.AvP_Yautja_Bald))
            {
                Pawn.story.hairDef = Rand.Chance(0.5f) ? YautjaDefOf.AvP_Yautja_Dreds : YautjaDefOf.AvP_Yautja_Ponytail;
            }
            if (Pawn.kindDef.race != YautjaDefOf.AvP_Alien_Yautja)
            {

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
