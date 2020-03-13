﻿using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class AvP_Pawn_EquipmentTracker_Notify_EquipmentAdded_CompAbilityItem_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq.TryGetComp<CompAbilityItem>() != null && eq.TryGetComp<CompAbilityItem>() is CompAbilityItem abilityItem)
            {
                if (!abilityItem.Props.Abilities.NullOrEmpty())
                {
                    foreach (AbilityDef def in abilityItem.Props.Abilities)
                    {
                        if (!__instance.pawn.abilities.abilities.Any(x => x.def == def))
                        {
                            __instance.pawn.abilities.GainEquipmentAbility((EquipmentAbilityDef)def,eq);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved")]
    public static class AvP_Pawn_EquipmentTracker_Notify_EquipmentRemoved_CompAbilityItem_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentRemovedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq.TryGetComp<CompAbilityItem>() != null && eq.TryGetComp<CompAbilityItem>() is CompAbilityItem abilityItem)
            {
                if (!abilityItem.Props.Abilities.NullOrEmpty())
                {
                    foreach (AbilityDef def in abilityItem.Props.Abilities)
                    {
                        if (__instance.pawn.abilities.abilities.Any(x => x.def == def))
                        {
                            Ability ability = __instance.pawn.abilities.abilities.Find(x => x.def == def);
                            __instance.pawn.abilities.abilities.Remove(ability);
                        }
                    }
                }
            }
        }
    }

}
