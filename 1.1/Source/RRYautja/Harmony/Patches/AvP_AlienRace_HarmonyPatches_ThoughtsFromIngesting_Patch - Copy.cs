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
using AlienRace;

namespace RRYautja
{
    [HarmonyPatch(typeof(PawnUtility), "IsInvisible")]
    public static class AvP_PawnUtility_IsInvisible_Patch
    {
        [HarmonyPostfix]
        public static void ThoughtsFromIngestingPostPrefix(Pawn __instance, bool __result)
        {
            if (__instance==null)
            {
                return;
            }
            if (__instance.isXenomorph(out Comp_Xenomorph xenomorph))
            {
                if (xenomorph!=null)
                {
                    if (xenomorph.Hidden)
                    {
                        __result = true;
                    }
                }
            }
        }
    }
    
}