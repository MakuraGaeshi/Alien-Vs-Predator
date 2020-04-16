using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace RRYautja.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public static class AvP_Pawn_ApparelTracker_Notify_ApparelRemoved_CompAbilityItem_Patch
    {
        [HarmonyPostfix] // Apparel apparel
        public static void Notify_ApparelRemovedPostfix(Pawn_EquipmentTracker __instance, Apparel apparel)
        {
            bool abilityitem = apparel.TryGetComp<CompAbilityItem>() != null;
            if (abilityitem)
            {
                foreach (CompAbilityItem compAbilityItem in apparel.GetComps<CompAbilityItem>())
                {
                    if (__instance.pawn.abilities.abilities.Any(x => compAbilityItem.Props.Abilities.Contains(x.def)))
                    {
                        foreach (AbilityDef abilityDef in compAbilityItem.Props.Abilities)
                        {
                            if (__instance.pawn.abilities.abilities.Any(x => compAbilityItem.Props.Abilities.Contains(abilityDef)))
                            {
                                Ability ability = __instance.pawn.abilities.abilities.Find(x => x.def == abilityDef);
                                __instance.pawn.abilities.abilities.Remove(ability);
                            }
                        } 
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded")]
    public static class AvP_Pawn_ApparelTracker_Notify_ApparelAdded_CompAbilityItem_Patch
    {
        [HarmonyPostfix] // Apparel apparel
        public static void Notify_Notify_ApparelAddedPostfix(Pawn_EquipmentTracker __instance, Apparel apparel)
        {
            if (apparel.TryGetComp<CompAbilityItem>() != null && apparel.TryGetComp<CompAbilityItem>() is CompAbilityItem abilityItem)
            {
                if (!abilityItem.Props.Abilities.NullOrEmpty())
                {
                    foreach (AbilityDef def in abilityItem.Props.Abilities)
                    {
                        if (!__instance.pawn.abilities.abilities.Any(x => x.def == def))
                        {
                            __instance.pawn.abilities.GainAbility(def);
                            Ability ability = __instance.pawn.abilities.abilities.Find(x => x.def == def);
                        }
                    }
                }
            }
        }
    }

}
