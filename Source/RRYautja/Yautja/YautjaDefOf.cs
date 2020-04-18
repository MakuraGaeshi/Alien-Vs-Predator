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
    //    public static ThingDef AvP_Yautja_Hound;

        // Yautja ThingDefs  Equipment
        public static ThingDef AvP_Yautja_HealthShard;

        // Yautja ThingDefs  Weapons
        public static ThingDef AvP_Yautja_Gun_Hunting_Bow;
        public static ThingDef AvP_Yautja_Gun_Compound_Bow;

        // Yautja ThingDefs  Projectiles
        public static ThingDef AvP_SmartDisk_Thrown;
        public static ThingDef AvP_SmartDisk_Returning;

        // Yautja ThingDefs  Motes
        public static ThingDef AvP_SmartDisk_Mote;

        // Yautja PawnsArrivalModeDefs 
        public static PawnsArrivalModeDef EdgeWalkInGroups;

        // Yautja ThoughtDefs
        //    public static ThoughtDef AvP_Thought_UnBlooded;
        //    public static ThoughtDef AvP_Thought_BloodedUM;
    //    public static ThoughtDef AvP_Thought_BloodedM;

        // Yautja ThoughtDefs Thought_SituationalSocial
        public static ThoughtDef AvP_HonourableVsBadBlood;
        public static ThoughtDef AvP_BadBloodVsHonourable;
        /*
        public static ThoughtDef AvP_UnbloodedVs_ThoughtDef;
        public static ThoughtDef AvP_UnmarkedVs_ThoughtDef;
        public static ThoughtDef AvP_MarkedVs_ThoughtDef;

        // Yautja ThoughtDefs Memories
        public static ThoughtDef AvP_Thought_ThrillOfTheHunt;
        */
        // Yautja HairDefs 
        public static HairDef AvP_Yautja_Dreds;
        public static HairDef AvP_Yautja_Ponytail;
        public static HairDef AvP_Yautja_Bald;

        // Yautja JobDefs 
        /*
        public static JobDef AvP_Yautja_MarkSelf;
        public static JobDef AvP_Yautja_MarkOther;
        public static JobDef AvP_Yautja_TakeTrophy;
        */
        public static JobDef AvP_Yautja_TendSelf;
        public static JobDef AvP_Yautja_HealthShardUse;
        public static JobDef AvP_Yautja_HealthShardRestock;

        // Yautja FactionDefs 
        public static FactionDef AvP_Yautja_PlayerColony;
        public static FactionDef AvP_Yautja_PlayerUnblooded;
        public static FactionDef AvP_Yautja_PlayerBlooded;

        public static FactionDef AvP_Yautja_BadBloodFaction;
        public static FactionDef AvP_Yautja_JungleClan;

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
        public static ConceptDef AvP_Concept_Gauntlet;
        public static ConceptDef AvP_Concept_Wistblade;
        public static ConceptDef AvP_Concept_SelfDestruct;
        public static ConceptDef AvP_Concept_MediComp;
        public static ConceptDef AvP_Concept_ShardInjector;
        public static ConceptDef AvP_Concept_Cloak;
        public static ConceptDef AvP_Concept_Plasmacaster;
        public static ConceptDef AvP_Concept_SmartDisk;
        public static ConceptDef AvP_Concept_CombiStaff;
    }
}
