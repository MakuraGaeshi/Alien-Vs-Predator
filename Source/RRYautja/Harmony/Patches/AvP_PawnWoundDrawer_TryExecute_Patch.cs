using RimWorld;
using Verse;
using Harmony;
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
    // Hides wounds on Stealthed Pawns
    [HarmonyPatch(typeof(PawnWoundDrawer), "RenderOverBody")]
    public static class AvP_PawnWoundDrawer_TryExecute_Patch
    {
        // Token: 0x06000017 RID: 23 RVA: 0x00002CD0 File Offset: 0x00000ED0
        [HarmonyPrefix]
        public static bool PreExecute(PawnWoundDrawer __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool flag_Cloaked = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            bool flag_HiddenXeno = pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden, false);
            if (flag_Cloaked || flag_HiddenXeno)
            {
                return false;
            }
            return true;
        }
    }
}