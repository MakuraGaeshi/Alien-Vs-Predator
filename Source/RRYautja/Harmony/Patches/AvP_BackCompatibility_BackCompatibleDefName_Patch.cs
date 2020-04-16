using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;

namespace RRYautja.HarmonyInstance
{
    [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleDefName")]
    public static class AvP_BackCompatibility_BackCompatibleDefName_Patch
    {
        [HarmonyPostfix]
        public static void BackCompatibleDefName_Postfix(Type defType, string defName, bool forDefInjections, ref string __result)
        {
            if (GenDefDatabase.GetDefSilentFail(defType, defName, false) == null)
            {
                //    Log.Message(string.Format("Checking for replacement for {0} Type: {1}", defName, defType));
                if (defType == typeof(ThingDef))
                {
                    if (defName.Contains("RRY_Melee_Combistaff"))
                    {
                        __result = "AvP_Yautja_Melee_Combistaff";
                    }
                    if (defName.Contains("RRY_Melee_BladedMaul"))
                    {
                        __result = "AvP_Yautja_Melee_BladedMaul";
                    }
                    if (defName.Contains("RRY_Gun_SmartDisk"))
                    {
                        __result = "AvP_Yautja_Gun_SmartDisk";
                    }
                    if (defName.Contains("RRY_Gun_Hunting_Bow"))
                    {
                        __result = "AvP_Yautja_Gun_Hunting_Bow";
                    }
                    if (defName.Contains("RRY_Gun_Compound_Bow"))
                    {
                        __result = "AvP_Yautja_Gun_Compound_Bow";
                    }
                    if (defName.Contains("RRY_Gun_NeedlerRifle"))
                    {
                        __result = "AvP_Yautja_Gun_Needler";
                    }
                    if (defName.Contains("RRY_Gun_SpearRifle"))
                    {
                        __result = "AvP_Yautja_Gun_SpearRifle";
                    }
                    if (defName.Contains("RRY_Gun_YautjaBlaster"))
                    {
                        __result = "AvP_Yautja_Gun_Blaster";
                    }


                    if (defName.Contains("RRY_Apparel_HunterVest"))
                    {
                        __result = "AvP_Yautja_Apparel_HunterVest";
                    }

                    if (defName.Contains("RRY_Apparel_TribalCloak"))
                    {
                        __result = "AvP_Yautja_Apparel_TribalCloak";
                    }

                    if (defName.Contains("RRY_Apparel_Mesh"))
                    {
                        __result = "AvP_Yautja_Apparel_ThermalMesh";
                    }

                    if (defName.Contains("RRY_Equipment_HunterGauntlet"))
                    {
                        __result = "AvP_Yautja_Equipment_Gauntlet";
                        if (defName.Contains("_TOGGLEDEF_RH"))
                        {
                            __result += "_TOGGLEDEF_RH";
                        }
                        if (defName.Contains("_TOGGLEDEF_LH"))
                        {
                            __result = "_TOGGLEDEF_LH";
                        }
                    }

                    if (defName.Contains("RRY_Equipment_HunterShoulderCannon"))
                    {
                        __result = "AvP_Yautja_Equipment_PlasmaCaster";
                    }

                    if (defName.Contains("RRY_Apparel_HunterLightArmourChestplate"))
                    {
                        __result = "AvP_Yautja_Armour_ChestplateLight";
                    }
                    if (defName.Contains("RRY_Apparel_LightBioMask"))
                    {
                        __result = "AvP_Yautja_Armour_BioMaskLight";
                    }

                    if (defName.Contains("RRY_Apparel_HunterArmourChestplate"))
                    {
                        __result = "AvP_Yautja_Armour_Chestplate";
                    }
                    if (defName.Contains("RRY_Apparel_HunterBioMask"))
                    {
                        __result = "AvP_Yautja_Armour_BioMask";
                    }

                    if (defName.Contains("RRY_Apparel_HunterHeavyArmourChestplate"))
                    {
                        __result = "AvP_Yautja_Armour_ChestplateHeavy";
                    }

                    if (defName.Contains("RRY_Apparel_HunterEliteArmourChestplate"))
                    {
                        __result = "AvP_Yautja_Armour_ChestplateElite";
                    }
                    if (defName.Contains("RRY_Apparel_HunterEliteBioMask"))
                    {
                        __result = "AvP_Yautja_Armour_BioMaskElite";
                    }
                    if (defName.Contains("RRY_Apparel_HunterFalconerBioMask"))
                    {
                        __result = "AvP_Yautja_Armour_BioMaskFalconer";
                    }

                    if (defName.Contains("RRY_Apparel_HunterLeaderArmourChestplate"))
                    {
                        __result = "AvP_Yautja_Armour_ChestplateOrnate";
                    }
                    if (defName.Contains("RRY_Apparel_HunterLeaderBioMask"))
                    {
                        __result = "AvP_Yautja_Armour_BioMaskOrnate";
                    }

                    if (defName.Contains("RRY_Apparel_HunterArmourGreaves"))
                    {
                        __result = "AvP_Yautja_Armour_Greaves";
                    }

                    if (defName.Contains("RRY_HealthShard"))
                    {
                        __result = "AvP_Yautja_HealthShard";
                    }

                    if (defName.Contains("RRY_YautjaMediComp"))
                    {
                        __result = "AvP_Yautja_MediComp";
                    }
                    


                    if (defName.Contains("RRY_Rynath"))
                    {
                        __result = "AvP_Rhynth";
                    }
                    if (defName.Contains("RRY_Leather_Rynath"))
                    {
                        __result = "AvP_Leather_Rhynth";
                    }
                    

                    if (defName.Contains("RRY_Alien_Yautja"))
                    {
                        __result = "AvP_Alien_Yautja";
                    }


                }
                if (defType == typeof(FactionDef))
                {

                }
                if (defType == typeof(PawnKindDef))
                {
                    if (defName.Contains("RRY_Rynath"))
                    {
                        __result = "AvP_Rynath";
                    }
                }
                if (defType == typeof(ResearchProjectDef))
                {
                    if (defName.Contains("RRY_XenomorphCrafting"))
                    {
                        __result = "AvP_Tech_Common_XenomorphCrafting";
                    }

                    if (defName.Contains("RRY_YautjaTechBase"))
                    {
                        __result = "AvP_Tech_Yautja_T1";
                    }
                    if (defName.Contains("RRY_YautjaTechMid"))
                    {
                        __result = "AvP_Tech_Yautja_T2";
                    }
                    if (defName.Contains("RRY_YautjaTechHigh"))
                    {
                        __result = "AvP_Tech_Yautja_T3";
                    }

                    if (defName.Contains("RRY_YautjaMelee_Basic"))
                    {
                        __result = "AvP_Tech_Yautja_Melee_T1";
                    }
                    if (defName.Contains("RRY_YautjaMelee_Special"))
                    {
                        __result = "AvP_Tech_Yautja_Melee_T2";
                    }

                    if (defName.Contains("RRY_YautjaRanged_Basic"))
                    {
                        __result = "AvP_Tech_Yautja_Ranged_T1";
                    }
                    if (defName.Contains("RRY_YautjaRanged_Med"))
                    {
                        __result = "AvP_Tech_Yautja_Ranged_T2";
                    }
                    if (defName.Contains("RRY_YautjaRanged_Adv"))
                    {
                        __result = "AvP_Tech_Yautja_Ranged_T3";
                    }

                    if (defName.Contains("RRY_YautjaMediComp"))
                    {
                        __result = "AvP_Tech_Yautja_MediComp";
                    }
                    if (defName.Contains("RRY_YautjaHealthShard"))
                    {
                        __result = "AvP_Tech_Yautja_HealthShard";
                    }
                    if (defName.Contains("RRY_YautjaCloakGenerator"))
                    {
                        __result = "AvP_Tech_Yautja_CloakGenerator";
                    }

                    if (defName.Contains("RRY_Yautja_Traps"))
                    {
                        __result = "AvP_Yautja_Traps";
                    }

                    if (defName.Contains("RRY_USCM_CarapaceArmor"))
                    {
                        __result = "AvP_USCM_CarapaceArmor";
                    }
                }
                if (defType == typeof(HediffDef))
                {
                    if (defName.Contains("RRY_Hediff_Cloaked"))
                    {
                        __result = "AvP_Hediff_Cloaked";
                    }
                    if (defName.Contains("RRY_Hediff_BouncedProjectile"))
                    {
                        __result = "AvP_Hediff_BouncedProjectile";
                    }
                }
            }
        }
    }

}
