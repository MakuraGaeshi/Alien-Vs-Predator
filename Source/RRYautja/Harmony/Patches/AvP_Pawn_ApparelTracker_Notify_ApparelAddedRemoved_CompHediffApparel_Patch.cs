using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AvP.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public static class AvP_Pawn_ApparelTracker_Notify_ApparelRemoved_CompHediffApparel_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Notify_ApparelRemovedPostfix(Pawn_EquipmentTracker __instance, Apparel apparel)
        {
            if (apparel.Wearer == null)
            {
                apparel.BroadcastCompSignal(CompHediffApparel.RemoveHediffsFromPawnSignal);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded")]
    public static class AvP_Pawn_ApparelTracker_Notify_ApparelAdded_CompHediffApparel_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Notify_Notify_ApparelAddedPostfix(Pawn_EquipmentTracker __instance, Apparel apparel)
        {
            if (apparel.Wearer != null)
            {
                apparel.BroadcastCompSignal(CompHediffApparel.AddHediffsToPawnSignal);
            }
        }
    }

}
