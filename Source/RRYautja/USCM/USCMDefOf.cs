using System;
using Verse;

namespace RimWorld
{
	[DefOf]
	public static class USCMDefOf
    {
		static USCMDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(USCMDefOf));
		}

        // USCM HefiffDefs
        public static HediffDef RRY_Damaged_Inhibitor;
        public static HediffDef RRY_Defective_Inhibitor;

        // USCM PawnKindDefs 

        // USCM ThingDefs  Races 
        public static ThingDef RRY_Synth;

        // USCM ThingDefs  Equipment
        //    public static ThingDef RRY_Equipment_HunterGauntlet;

        // USCM ThingDefs  Weapons
    //    public static ThingDef RRY_Gun_Hunting_Bow;
    //    public static ThingDef RRY_Gun_Compound_Bow;

        // USCM ThingDefs  Projectiles
    //    public static ThingDef RRY_SmartDisk_Thrown;

        // USCM ThingDefs  Motes
    //    public static ThingDef RRY_SmartDisk_Mote;

        // USCM PawnsArrivalModeDefs 
    //    public static PawnsArrivalModeDef EdgeWalkInGroups;

        // USCM ThoughtDefs
    //    public static ThoughtDef RRY_HonourableVsBadBlood;

        // USCM ThoughtDefs Memories
    //    public static ThoughtDef RRY_Thought_ThrillOfTheHunt;
        
    }
    [DefOf]
    public static class USCMConceptDefOf
    {
        // Token: 0x06003781 RID: 14209 RVA: 0x001A8393 File Offset: 0x001A6793
        static USCMConceptDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(USCMConceptDefOf));
        }
        // USCM ConceptDefs
        public static ConceptDef RRY_Concept_Gauntlet;
    }
}
