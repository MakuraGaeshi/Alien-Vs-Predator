using System;
using System.Collections.Generic;
using RimWorld;
using System.Text;
using Verse;

namespace RRYautja
{
    public static class YautjaBloodedUtility
    {
        public static HediffDef unbloodedDef = YautjaDefOf.HMS_Hediff_Unblooded;
        public static HediffDef unmarkedDef = YautjaDefOf.HMS_Hediff_BloodedUM;
        public static HediffDef markedDef = YautjaDefOf.HMS_Hediff_BloodedM;
        public static AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_YoungBlood");
        public static AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_Blooded");
        public static AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodA");
        public static AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodB");
        
    }
}
