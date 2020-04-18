using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000956 RID: 2390
	[DefOf]
    public static class XenomorphDefOf
    {
        // Token: 0x06003781 RID: 14209 RVA: 0x001A8393 File Offset: 0x001A6793
        static XenomorphDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(XenomorphDefOf));
        }
        // Xenomorph DamageDefs 
        public static DamageDef AvP_AcidDamage;
        public static DamageDef AvP_AcidBurn;

        // Xenomorph DamageDefs Special
        public static DamageDef AvP_EmergingChestbursterDD;

        // Xenomorph HefiffDefs 
    //    public static HediffDef AvP_Hediff_Xenomorph_Hidden;
        public static HediffDef AvP_Hediff_Cocooned;
        public static HediffDef HypothermicSlowdown;
        public static HediffDef AvP_Hediff_Anesthetic;
        public static HediffDef AvP_Hediff_Xenomorph_ProtoBodypart;
        public static HediffDef AvP_Hediff_Xenomorph_RestoredBodypart;

        // Xenomorph HefiffDefs HypothermicSlowdown
        public static HediffDef AvP_FaceHuggerInfection;
        public static HediffDef AvP_XenomorphImpregnation;
        public static HediffDef AvP_HiddenXenomorphImpregnation;

        // Neomorph HefiffDefs
        public static HediffDef AvP_NeomorphImpregnation;
        public static HediffDef AvP_HiddenNeomorphImpregnation;

        // Surgical RecipeDefs
        public static RecipeDef AvP_FaceHuggerRemoval;
        public static RecipeDef AvP_XenomorphImpregnationRemoval;
        public static RecipeDef AvP_NeomorphImpregnationRemoval;

        // Xenomorph PawnKindDefs
        public static PawnKindDef AvP_Xenomorph_FaceHugger;
        public static PawnKindDef AvP_Xenomorph_RoyaleHugger;
        public static PawnKindDef AvP_Xenomorph_Queen;
        public static PawnKindDef AvP_Xenomorph_Thrumbomorph;
        public static PawnKindDef AvP_Xenomorph_Warrior;
        public static PawnKindDef AvP_Xenomorph_Drone;
        public static PawnKindDef AvP_Xenomorph_Runner;
        public static PawnKindDef AvP_Xenomorph_Predalien;
        /*
        public static PawnKindDef AvP_Xenomorph_Preatorian;
        public static PawnKindDef AvP_Xenomorph_Boiler;
        public static PawnKindDef AvP_Xenomorph_Spitter;
        public static PawnKindDef AvP_Xenomorph_Crusher;
        public static PawnKindDef AvP_Xenomorph_King;
        */

        // Neomorph PawnKindDefs
        public static PawnKindDef AvP_Xenomorph_Neomorph;

        // Xenomorph FactionDefs
        public static FactionDef AvP_Xenomorph;

        // Xenomorph PawnsArrivalModeDefs 
        public static PawnsArrivalModeDef AvP_RandomDropThroughRoof;
        public static PawnsArrivalModeDef AvP_DropThroughRoofNearPower;
        public static PawnsArrivalModeDef AvP_RandomEnterFromTunnel;

        // Xenomorph RaidStrategyDefs 
        public static RaidStrategyDef AvP_PowerCut;

        // Xenomorph JobDefs 
        public static JobDef AvP_Job_Xenomorph_EnterHiveTunnel;
        public static JobDef AvP_Job_Xenomorph_LayEgg;
        public static JobDef AvP_Job_Xenomorph_Kidnap;
        public static JobDef AvP_Job_Xenomorph_PredalienImpregnate;
        public static JobDef AvP_Job_Xenomorph_MaintainHive;
        public static JobDef AvP_Job_Xenomorph_Construct_Hive_Slime;
        public static JobDef AvP_Job_Xenomorph_Construct_Hive_Node;
        public static JobDef AvP_Job_Xenomorph_Construct_Hive_Wall;
        public static JobDef AvP_Job_Xenomorph_Construct_Hive_Roof;
        public static JobDef AvP_Job_DestroyCocoon; 
        public static JobDef AvP_Neomorph_Ingest;
        public static JobDef AvP_Xenomorph_Ingest; 

        // Xenomorph DutyDefs    
        public static DutyDef AvP_Xenomorph_DefendHiveLoc;
        public static DutyDef AvP_Xenomorph_AssaultColony;
        public static DutyDef AvP_Xenomorph_AssaultColony_CutPower;
        public static DutyDef AvP_Xenomorph_DefendAndExpandHive;
        public static DutyDef AvP_Xenomorph_DefendHiveAggressively;
        public static DutyDef AvP_Xenomorph_Kidnap; 


        // Xenomorph ThingDefs  
        public static ThingDef AvP_Xenomorph_Cocoon_Humanoid;
        public static ThingDef AvP_Xenomorph_Cocoon_Animal;
        public static ThingDef AvP_EggXenomorphFertilized;
        public static ThingDef AvP_Xenomorph_TailSpike;
        public static ThingDef AvP_Xenomorph_HeadShell;
        public static ThingDef AvP_Leather_Xenomorph; 

        public static ThingDef AvP_Neomorph_Spores;
        public static ThingDef AvP_Neomorph_Spores_Hidden;
        
        public static ThingDef AvP_Plant_Neomorph_Fungus;
        public static ThingDef AvP_Plant_Neomorph_Fungus_Hidden;

        public static ThingDef AvP_XenomorphCryptosleepCasket;

        public static ThingDef AvP_XenomorphCrashedShipPart;
        public static ThingDef TunnelHiveLikeSpawner;
        public static ThingDef AvP_Xenomorph_Hive; 
        public static ThingDef AvP_Xenomorph_Hive_Child;
        public static ThingDef AvP_Xenomorph_Hive_Wall;
        public static ThingDef AvP_Xenomorph_Hive_Slime;

        public static ThingDef AvP_Filth_Slime;

        // Xenomorph BloodDefs 
        public static ThingDef AvP_FilthBloodXenomorph_Active;
        public static ThingDef AvP_FilthBloodXenomorph;

        // Xenomorph LifeStageDefs 
        public static LifeStageDef AvP_XFacehuggerFullyFormed;
        public static LifeStageDef AvP_XenomorphChestBursterBaby;
        public static LifeStageDef AvP_XenomorphChestBursterBig;
        public static LifeStageDef AvP_XenomorphFormedBaby;
        public static LifeStageDef AvP_XenomorphFormedMid;
        public static LifeStageDef AvP_XenomorphFullyFormed;

        // Neomorph LifeStageDefs 
        public static LifeStageDef AvP_NeomorphBloodBursterBaby;
        public static LifeStageDef AvP_NeomorphBloodBursterBig;
        public static LifeStageDef AvP_NeomorphFormedBaby;
        public static LifeStageDef AvP_NeomorphFormedMid;
        public static LifeStageDef AvP_NeomorphFullyFormed;

        // Xenomorph IncidentDefs AvP_XenomorphInfestation
        public static IncidentDef AvP_Neomorph_FungusSprout;
        public static IncidentDef AvP_Neomorph_FungusSprout_Hidden;
        public static IncidentDef AvP_XenomorphInfestation;
        public static IncidentDef AvP_XenomorphCrashedShipPartCrash;
    //    public static IncidentDef AvP_RaidEnemy_PowerCut_Xenomorph;
        public static IncidentDef AvP_PowerCut_Xenomorph;

        // Xenomorph BodyPartDefs
        public static BodyPartDef AvP_Xeno_TailSpike;
        public static BodyPartDef AvP_Xeno_Shell;

        // Xenomorph AbilityDefs
        public static AbilityDef AvP_Ability_SpitAcid;

        // Xenomorph GameCOnditionDefs
        public static GameConditionDef AvP_Xenomorph_PowerCut;

        // Xenomorph LifeStageDefs AvP_Xeno_Shell 
        //   public static LifeStageDef AvP_EggXenomorphFertilized;
/*
#if DEBUG
            if (Prefs.DevMode)
            {

            }
#endif
*/
    }

    [DefOf]
    public static class XenomorphRacesDefOf
    {
        // Token: 0x06003781 RID: 14209 RVA: 0x001A8393 File Offset: 0x001A6793
        static XenomorphRacesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(XenomorphRacesDefOf));
        }
        // Xenomorph ThingDefs Races
        public static ThingDef AvP_Xenomorph_FaceHugger;
        public static ThingDef AvP_Xenomorph_Queen;
        public static ThingDef AvP_Xenomorph_Warrior;
        public static ThingDef AvP_Xenomorph_Drone;
        public static ThingDef AvP_Xenomorph_Runner;
        public static ThingDef AvP_Xenomorph_Predalien;
        public static ThingDef AvP_Xenomorph_Thrumbomorph;

        // Neomorph ThingDefs Races
        public static ThingDef AvP_Xenomorph_Neomorph;

        // Xenomorph FleshTypeDefs
        public static FleshTypeDef AvP_Xenomorph;
        public static FleshTypeDef AvP_Neomorph;
    }
    [DefOf]
    public static class XenomorphConceptDefOf
    {
        // Token: 0x06003781 RID: 14209 RVA: 0x001A8393 File Offset: 0x001A6793
        static XenomorphConceptDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(XenomorphRacesDefOf));
        }
        // Xenomorph ConceptDefs  
        public static ConceptDef AvP_Concept_Cocoons;
        public static ConceptDef AvP_Concept_Facehuggers;
        public static ConceptDef AvP_Concept_Royalehuggers;
        public static ConceptDef AvP_Concept_Embryo;
        public static ConceptDef AvP_Concept_Chestbursters;
        public static ConceptDef AvP_Concept_Runners;
        public static ConceptDef AvP_Concept_Drones;
        public static ConceptDef AvP_Concept_Warriors;
        public static ConceptDef AvP_Concept_Predaliens;
        public static ConceptDef AvP_Concept_Queens;
        public static ConceptDef AvP_Concept_Neomorphs;

        public static ConceptDef AvP_Concept_HiveLike;
        public static ConceptDef AvP_Concept_ShipPart;
        public static ConceptDef AvP_Concept_Eggs;
        public static ConceptDef AvP_Concept_SporeSac;
    }
}
