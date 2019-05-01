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
        // Xenomorph PawnKindDefs
        // Yautja ThingDefs = Races 
        public static ThingDef Alien_Yautja;
        public static ThingDef RRY_Yautja_Hound;
        public static ThingDef RRY_Rynath;

        // Yautja ThingDefs = Equipment
        public static ThingDef RRY_Equipment_HunterGauntlet;

        public static JobDef RRY_Yautja_MarkSelf;
        public static JobDef RRY_Yautja_TakeTrophy;

        // Token: 0x04000002 RID: 2
        public static WorkTypeDef Rearm;

        // Token: 0x04000003 RID: 3
        public static DesignationDef RearmTrap;

        // Token: 0x04000004 RID: 4
        public static JobDef RearmTrapJob;

        // Token: 0x04000005 RID: 5
        public static RecordDef TrapsRearmed;
    }
}
