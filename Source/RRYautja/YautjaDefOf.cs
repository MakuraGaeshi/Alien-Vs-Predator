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
        public static HediffDef RRYUnblooded;
        public static HediffDef RRYBloodedUM; 
        public static HediffDef RRYBloodedM;
        

        // Xenomorph PawnKindDefs
        // Yautja ThingDefs = Races
        public static ThingDef Alien_Yautja;
        
    }
}
