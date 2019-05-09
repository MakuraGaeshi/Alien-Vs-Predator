using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace RRYautja.Harmony
{
    [HarmonyPatch(typeof(PawnUIOverlay), "DrawPawnGUIOverlay")]
    public static class PawnUIOverlayPatch
    {
        [HarmonyPrefix]
        public static bool ThiefException(PawnUIOverlay __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null && (pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked)|| pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden)))
            {
                return false;
            }
            return true;
        }
    }
}
