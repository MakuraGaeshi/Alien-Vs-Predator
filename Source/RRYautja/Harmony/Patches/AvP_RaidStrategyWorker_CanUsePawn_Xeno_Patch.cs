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
using AvP.settings;
using AvP.ExtensionMethods;

namespace AvP.HarmonyInstance
{
    
    [HarmonyPatch(typeof(RaidStrategyWorker), "CanUsePawn")]
    public static class AvP_RaidStrategyWorker_CanUsePawn_Xeno_Patch
    {
        [HarmonyPostfix]
        public static void CanUse_XenoQueen_Postfix(RaidStrategyWorker __instance,ref Pawn p, List<Pawn> otherPawns, ref bool __result)
        {
            if (p.def == XenomorphRacesDefOf.AvP_Xenomorph_Queen)
            {
                if (otherPawns.Any(x => x.def == XenomorphRacesDefOf.AvP_Xenomorph_Queen))
                {
                    p.def = XenomorphRacesDefOf.AvP_Xenomorph_Warrior;
                    p.kindDef = XenomorphDefOf.AvP_Xenomorph_Warrior;
                }
            }
        }
    }
    
}