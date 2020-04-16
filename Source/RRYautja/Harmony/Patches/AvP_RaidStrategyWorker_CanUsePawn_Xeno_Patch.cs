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
    
    [HarmonyPatch(typeof(RaidStrategyWorker), "CanUsePawn")]
    public static class AvP_RaidStrategyWorker_CanUsePawn_Xeno_Patch
    {
        [HarmonyPostfix]
        public static void CanUse_XenoQueen_Postfix(RaidStrategyWorker __instance,ref Pawn p, List<Pawn> otherPawns, ref bool __result)
        {
            if (p.def == XenomorphRacesDefOf.RRY_Xenomorph_Queen)
            {
                if (otherPawns.Any(x => x.def == XenomorphRacesDefOf.RRY_Xenomorph_Queen))
                {
                    p.def = XenomorphRacesDefOf.RRY_Xenomorph_Warrior;
                    p.kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                }
            }
        }
    }
    
}