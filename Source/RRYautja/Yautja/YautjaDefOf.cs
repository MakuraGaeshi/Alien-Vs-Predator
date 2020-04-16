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
        
        public static HediffDef AvP_Hediff_Cloaked;
        public static HediffDef AvP_Hediff_BouncedProjectile; 

        // Yautja PawnKindDefs 

        // Yautja ThingDefs  Races 
        public static ThingDef AvP_Alien_Yautja;
    //    public static ThingDef RRY_Yautja_Hound;

        // Yautja ThingDefs  Equipment
        public static ThingDef AvP_Yautja_HealthShard;

        // Yautja ThingDefs  Weapons
        public static ThingDef AvP_Yautja_Gun_Hunting_Bow;
        public static ThingDef AvP_Yautja_Gun_Compound_Bow;

        // Yautja ThingDefs  Projectiles
        public static ThingDef RRY_SmartDisk_Thrown;
        public static ThingDef RRY_SmartDisk_Returning;

        // Yautja ThingDefs  Motes
        public static ThingDef RRY_SmartDisk_Mote;

        // Yautja PawnsArrivalModeDefs 
        public static PawnsArrivalModeDef EdgeWalkInGroups;

        // Yautja ThoughtDefs
        //    public static ThoughtDef RRY_Thought_UnBlooded;
        //    public static ThoughtDef RRY_Thought_BloodedUM;
    //    public static ThoughtDef RRY_Thought_BloodedM;

        // Yautja ThoughtDefs Thought_SituationalSocial
        public static ThoughtDef RRY_HonourableVsBadBlood;
        public static ThoughtDef RRY_BadBloodVsHonourable;
        /*
        public static ThoughtDef RRY_UnbloodedVs_ThoughtDef;
        public static ThoughtDef RRY_UnmarkedVs_ThoughtDef;
        public static ThoughtDef RRY_MarkedVs_ThoughtDef;

        // Yautja ThoughtDefs Memories
        public static ThoughtDef RRY_Thought_ThrillOfTheHunt;
        */
        // Yautja HairDefs 
        public static HairDef RRY_Yaujta_Dreds;
        public static HairDef RRY_Yaujta_Ponytail;
        public static HairDef RRY_Yaujta_Bald;

        // Yautja JobDefs 
        /*
        public static JobDef RRY_Yautja_MarkSelf;
        public static JobDef RRY_Yautja_MarkOther;
        public static JobDef RRY_Yautja_TakeTrophy;
        */
        public static JobDef RRY_Yautja_TendSelf;
        public static JobDef RRY_Yautja_HealthShard;

        // Yautja FactionDefs 
        public static FactionDef RRY_Yautja_PlayerColony;
        public static FactionDef RRY_Yautja_PlayerUnblooded;
        public static FactionDef RRY_Yautja_PlayerBlooded;

        public static FactionDef RRY_Yautja_BadBloodFaction;
        public static FactionDef RRY_Yautja_JungleClan;

        // Yautja ResearchProjectDefs 
        public static ResearchProjectDef AvP_Tech_Yautja_MediComp;
        public static ResearchProjectDef AvP_Tech_Yautja_HealthShard;
        public static ResearchProjectDef AvP_Tech_Yautja_CloakGenerator;

        public static ResearchProjectDef AvP_Tech_Yautja_Ranged_T1;
        public static ResearchProjectDef AvP_Tech_Yautja_Ranged_T2;
        public static ResearchProjectDef AvP_Tech_Yautja_Ranged_T3;
        

        // Yautja BodyTypeDefs
        public static BodyTypeDef RRYYautjaFemale;
        public static BodyTypeDef RRYYautjaMale;

    }
    [DefOf]
    public static class YautjaConceptDefOf
    {
        // Token: 0x06003781 RID: 14209 RVA: 0x001A8393 File Offset: 0x001A6793
        static YautjaConceptDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(XenomorphRacesDefOf));
        }
        // Xenomorph ConceptDefs
        public static ConceptDef RRY_Concept_Gauntlet;
        public static ConceptDef RRY_Concept_Wistblade;
        public static ConceptDef RRY_Concept_SelfDestruct;
        public static ConceptDef RRY_Concept_MediComp;
        public static ConceptDef RRY_Concept_ShardInjector;
        public static ConceptDef RRY_Concept_Cloak;
        public static ConceptDef RRY_Concept_Plasmacaster;
        public static ConceptDef RRY_Concept_SmartDisk;
        public static ConceptDef RRY_Concept_CombiStaff;
    }
}
