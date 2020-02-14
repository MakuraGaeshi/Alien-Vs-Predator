using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using System.Reflection;

namespace RRYautja.Harmony
{
    [HarmonyPatch(typeof(Building_Door), "PawnCanOpen")]
    public static class AvP_Building_Door_CanOpen_Cloaked_Patch
    {
        [HarmonyPostfix]
        public static void PawnCanOpenPostfix(ref bool __result, Pawn p)
        {
            __result = __result || (p != null && p.health != null && p.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked));
        }
    }
}
