using System;
using System.Collections.Generic;
using RimWorld;
using System.Text;
using Verse;
using HunterMarkingSystem;

namespace RRYautja
{
    public static class YautjaBloodedUtility
    {
        public static HediffDef unbloodedDef = HMSDefOf.HMS_Hediff_Unblooded;
        public static HediffDef unmarkedDef = HMSDefOf.HMS_Hediff_BloodedUM;
        public static HediffDef markedDef = HMSDefOf.HMS_Hediff_BloodedM;
        public static AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("AvP_Yautja_YoungBlood");
        public static AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("AvP_Yautja_Blooded");
        public static AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("AvP_Yautja_BadBloodA");
        public static AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("AvP_Yautja_BadBloodB");
        
    }
}
