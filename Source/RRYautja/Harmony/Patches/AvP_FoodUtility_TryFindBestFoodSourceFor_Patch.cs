using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using RRYautja.settings;
using RRYautja.ExtensionMethods;

namespace RRYautja.HarmonyInstance
{
    /*
    // FoodUtility.BestPawnToHuntForPredator(getter, forceScanWholeMap)  BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
    // Xeno/Neomorph Hunting patch
    [HarmonyPatch(typeof(FoodUtility), "TryFindBestFoodSourceFor")]
    public static class AvP_FoodUtility_TryFindBestFoodSourceFor_Patch
    {
        [HarmonyPostfix]
        public static void TryFindBestFoodSourceFor(Pawn getter, Pawn eater, bool desperate, ref Thing foodSource, ref ThingDef foodDef, ref bool __result, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false)
        {
            if (eater.isNeomorph())
            {
                Comp_Neomorph _Neomorph = eater.TryGetComp<Comp_Neomorph>();
                __result = _Neomorph.TryFindBestFoodSourceFor(getter, eater, desperate, out foodSource, out foodDef, canRefillDispenser, canUseInventory, allowForbidden, allowCorpse, allowSociallyImproper, allowHarvest,forceScanWholeMap);
            }
            if (eater.isXenomorph())
            {

            }
        }
    }
    */
}