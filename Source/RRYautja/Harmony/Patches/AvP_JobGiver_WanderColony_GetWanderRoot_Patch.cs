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
    // stop Pawns trying to wander near Cocooned colonists
    [HarmonyPatch(typeof(JobGiver_WanderColony), "GetWanderRoot")]
    public static class AvP_JobGiver_WanderColony_GetWanderRoot_Patch
    {
        [HarmonyPostfix]
        public static void GetWanderRoot(Pawn pawn, ref IntVec3 __result)
        {
            if (!__result.GetFirstThing(pawn.Map, XenomorphDefOf.AvP_Xenomorph_Cocoon_Humanoid).DestroyedOrNull())
            {
                __result = pawn.Position;
            }
        }
    }
}