using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace RRYautja.Harmony
{
    [HarmonyPatch(typeof(ThingSelectionUtility), "SelectableByMapClick")]
    public static class ThingSelectionUtilityPatch
    {
        [HarmonyPostfix]
        public static void ThiefException(ref bool __result, Thing t)
        { // (pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked)|| pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden))
            if (t is Pawn && ((Pawn)t).Faction!=Faction.OfPlayer && (((Pawn)t).health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked)|| ((Pawn)t).health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden)))
            {
                __result = false;
            }
        }
    }
}
