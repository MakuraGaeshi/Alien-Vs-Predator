using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    [HarmonyPatch(typeof(PawnUIOverlay), "DrawPawnGUIOverlay")]
    public static class AvP_PawnUIOverlay_DrawPawnGUIOverlay_Stealth_Patch
    {
        [HarmonyPrefix]
        public static bool DrawPawnGUIOverlay_Prefix(PawnUIOverlay __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null)
            {
                bool flag_Cloaked = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
                bool flag_HiddenXeno = pawn.isXenomorph(out Comp_Xenomorph comp) && comp.hidden;
                if (flag_HiddenXeno || flag_Cloaked)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
