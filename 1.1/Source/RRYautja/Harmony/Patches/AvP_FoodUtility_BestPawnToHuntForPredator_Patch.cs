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

namespace RRYautja
{
    // FoodUtility.BestPawnToHuntForPredator(getter, forceScanWholeMap)  BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
    // Xeno/Neomorph Hunting patch
    [HarmonyPatch(typeof(FoodUtility), "BestPawnToHuntForPredator")]
    public static class AvP_FoodUtility_BestPawnToHuntForPredator_Patch
    {
        [HarmonyPostfix]
        public static void BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap, ref Pawn __result)
        {
            if (predator.isNeomorph())
            {
                Comp_Neomorph _Neomorph = predator.TryGetComp<Comp_Neomorph>();
                __result = _Neomorph.BestPawnToHuntForPredator(predator, forceScanWholeMap);
            }
            if (predator.isXenomorph())
            {
                if (predator.ageTracker.CurLifeStage == XenomorphDefOf.RRY_XenomorphFullyFormed)
                {
                    Comp_Xenomorph _Xenomorph = predator.TryGetComp<Comp_Xenomorph>();
                    __result = _Xenomorph.BestPawnToHuntForPredator(predator, forceScanWholeMap);
                }
            }
        }
    }

}