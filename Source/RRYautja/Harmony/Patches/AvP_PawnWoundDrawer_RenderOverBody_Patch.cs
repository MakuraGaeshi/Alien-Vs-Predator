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

namespace RRYautja.HarmonyInstance
{
    // Hides wounds on Stealthed Pawns
    [HarmonyPatch(typeof(PawnWoundDrawer), "RenderOverBody")]
    public static class AvP_PawnWoundDrawer_RenderOverBody_Patch
    {
        // Token: 0x06000017 RID: 23 RVA: 0x00002CD0 File Offset: 0x00000ED0
        [HarmonyPrefix]
        public static bool RenderOverBodyPrefix(PawnWoundDrawer __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool flag_Cloaked = pawn.health.hediffSet.HasHediff(YautjaDefOf.AvP_Hediff_Cloaked, false);
            Comp_Xenomorph comp = null;
            bool flag_HiddenXeno = (pawn.isXenomorph(out comp) || !pawn.isXenomorph() && pawn.CarriedBy!= null && pawn.CarriedBy.isXenomorph(out comp)) && (comp!=null && (comp.Hidden || comp.hidden));
            if (flag_Cloaked || flag_HiddenXeno)
            {
                return false;
            }
            return true;
        }
    }
}