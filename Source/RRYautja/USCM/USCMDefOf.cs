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
        public static HediffDef AvP_Damaged_Inhibitor;
        public static HediffDef AvP_Defective_Inhibitor;

        // USCM PawnKindDefs 

        // USCM ThingDefs  Races 
        public static ThingDef AvP_Synth;

        // USCM ThingDefs  Equipment
        public static ThingDef AvP_USCM_Equipment_HeadMountedSight;
        public static ThingDef AvP_USCM_Armour_M56CombatHarness;

        // USCM ThingDefs  Weapons
        //    public static ThingDef AvP_Gun_Hunting_Bow;
        //    public static ThingDef AvP_Gun_Compound_Bow;

        // USCM ThingDefs  Projectiles
        //    public static ThingDef AvP_SmartDisk_Thrown;

        // USCM ThingDefs  Equipment AvP_USCM_ActiveDropshipUD4L
        public static ThingDef AvP_USCM_DropshipUD4L;
        public static ThingDef AvP_USCM_ActiveDropshipUD4L;
    //    public static ThingDef AvP_USCM_TravelingDropshipUD4L;
        public static ThingDef AvP_USCM_DropshipUD4LIncoming;
        public static ThingDef AvP_USCM_DropshipUD4LLeaving;

        // USCM ThingDefs  Motes
        //    public static ThingDef AvP_SmartDisk_Mote;

        // USCM PawnsArrivalModeDefs 
        //    public static PawnsArrivalModeDef EdgeWalkInGroups;

        // USCM ThoughtDefs
        //    public static ThoughtDef AvP_HonourableVsBadBlood;

        // USCM ThoughtDefs Memories
        //    public static ThoughtDef AvP_Thought_ThrillOfTheHunt;

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
        public static ConceptDef AvP_Concept_Gauntlet;
    }
}
