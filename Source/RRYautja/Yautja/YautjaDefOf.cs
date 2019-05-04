using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000956 RID: 2390
	[DefOf]
	public static class YautjaDefOf
    {
		// Token: 0x06003781 RID: 14209 RVA: 0x001A8393 File Offset: 0x001A6793
		static YautjaDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(YautjaDefOf));
		}

        // Yautja HefiffDefs
        public static HediffDef RRY_Hediff_Unblooded;
        public static HediffDef RRY_Hediff_BloodedUM;
        public static HediffDef RRY_Hediff_BloodedM;
        public static HediffDef RRY_Hediff_BloodedMHuman;
        public static HediffDef RRY_Hediff_BloodedMWorthyHuman;
        public static HediffDef RRY_Hediff_BloodedMHumanlike;
        public static HediffDef RRY_Hediff_BloodedMWorthyHumanlike;
        public static HediffDef RRY_Hediff_BloodedMXenomorph;
        public static HediffDef RRY_Hediff_BloodedMXenomorphQueen;
        public static HediffDef RRY_Hediff_BloodedMPredalien;
        public static HediffDef RRY_Hediff_BloodedMBadBlood;
        public static HediffDef RRY_Hediff_BloodedMHound;
        public static HediffDef RRY_Hediff_BloodedMCrusher;
        public static HediffDef RRY_Hediff_BloodedMGroTye;

        public static HediffDef RRY_Hediff_Cloaked;
        // Yautja PawnKindDefs

        // Yautja ThingDefs  Races 
        public static ThingDef Alien_Yautja;
        public static ThingDef RRY_Yautja_Hound;
        public static ThingDef RRY_Rynath;

        // Yautja ThingDefs  Equipment
        public static ThingDef RRY_Equipment_HunterGauntlet;

        // Yautja JobDefs
        public static JobDef RRY_Yautja_MarkSelf;
        public static JobDef RRY_Yautja_TakeTrophy;
        public static JobDef RRY_Yautja_RearmTrapJob;
        public static JobDef RRY_Yautja_TendSelf;
        public static JobDef RRY_Yautja_HealthShard;

        // Yautja ResearchProjectDefs 
        public static ResearchProjectDef RRY_YautjaMediComp;
        public static ResearchProjectDef RRY_YautjaHealthShard;
        public static ResearchProjectDef RRY_YautjaCloakGenerator;

        // Yautja WorkTypeDefs 
        public static WorkTypeDef Rearm;

        // Yautja DesignationDefs
        public static DesignationDef RearmTrap;

        // Yautja RecordDefs
        public static RecordDef TrapsRearmed;
    }
}
