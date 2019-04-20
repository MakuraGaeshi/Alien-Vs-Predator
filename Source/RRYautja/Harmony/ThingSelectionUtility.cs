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
        {
            if (t is Pawn && ((Pawn)t).Faction!=Faction.OfPlayer && ((Pawn)t).health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked))
            {
                __result = false;
            }
        }
    }
}
