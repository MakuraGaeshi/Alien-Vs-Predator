using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    [HarmonyPatch(typeof(ThingSelectionUtility), "SelectableByMapClick")]
    public static class AvP_ThingSelectionUtility_SelectableByMapClick_HostileStealth_Patch
    {
        [HarmonyPostfix]
        public static void ThiefException(ref bool __result, Thing t)
        { // (pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked)|| pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden))
            if (t!=null)
            {
                if (t.GetType() == typeof(Pawn))
                {
                    Pawn pawn = (Pawn)t;
                    if (pawn != null)
                    {
                        bool flag_Faction = pawn.Faction != Faction.OfPlayer;
                        bool flag_Cloaked = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false) && pawn.Faction != Faction.OfPlayer;
                        bool flag_HiddenXeno = pawn.isXenomorph(out Comp_Xenomorph comp) && comp.hidden;
                        if ((flag_HiddenXeno || flag_Cloaked) && flag_Faction)
                        {
                            __result = false;
                        }
                    }
                }
            }
        }
    }
}
