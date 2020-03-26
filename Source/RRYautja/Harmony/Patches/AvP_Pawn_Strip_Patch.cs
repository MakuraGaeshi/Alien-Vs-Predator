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
    // Disallows stripping of the Wristblade
    [HarmonyPatch(typeof(Pawn), "Strip")]
    public static class AvP_Pawn_Strip_Patch
    {
        [HarmonyPrefix]
        public static bool IgnoreWristblade(Pawn __instance)
        {

            bool result = true;
            if (__instance.RaceProps.Humanlike)
            {
                result = !(__instance.apparel.WornApparel.Any(x => x.def.defName.Contains("RRY_Equipment_HunterGauntlet")) && !__instance.Dead);
            }
            //    Log.Message(string.Format("Pawn_StripPatch IgnoreWristblade: {0}", result));
            if (!result)
            {

                Caravan caravan = __instance.GetCaravan();
                if (caravan != null)
                {
                    CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(__instance, caravan.PawnsListForReading, null);
                    if (__instance.apparel != null)
                    {
                        CaravanInventoryUtility.MoveAllApparelToSomeonesInventory(__instance, caravan.PawnsListForReading);
                    }
                    if (__instance.equipment != null)
                    {
                        CaravanInventoryUtility.MoveAllEquipmentToSomeonesInventory(__instance, caravan.PawnsListForReading);
                    }
                }
                else
                {
                    IntVec3 pos = (__instance.Corpse == null) ? __instance.PositionHeld : __instance.Corpse.PositionHeld;
                    if (__instance.equipment != null)
                    {
                        __instance.equipment.DropAllEquipment(pos, false);
                    }
                    if (__instance.apparel != null)
                    {
                        DropAll(__instance, pos, false);
                    }
                    if (__instance.inventory != null)
                    {
                        __instance.inventory.DropAllNearPawn(pos, false, false);
                    }
                }
            }
            return result;
        }

        private static List<Apparel> tmpApparelList = new List<Apparel>();

        public static void DropAll(Pawn __instance, IntVec3 pos, bool forbid = true)
        {
            tmpApparelList.Clear();
            for (int i = 0; i < __instance.apparel.WornApparel.Count; i++)
            {
                if (!__instance.apparel.WornApparel[i].def.defName.Contains("RRY_Equipment_HunterGauntlet"))
                {
                    tmpApparelList.Add(__instance.apparel.WornApparel[i]);
                }
                else
                {
                    //    Log.Message(string.Format("Ignoring Wristblade"));
                }
            }
            for (int j = 0; j < tmpApparelList.Count; j++)
            {
                __instance.apparel.TryDrop(tmpApparelList[j], out Apparel apparel, pos, forbid);
            }
        }
    }
}