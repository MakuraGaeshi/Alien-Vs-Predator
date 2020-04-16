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
    // StatWorker patch
    [HarmonyPatch(typeof(StatWorker), "StatOffsetFromGear")]
    public static class AvP_StatPart_StatWorker_TransformValue_Smartgun_Patch
    {
        [HarmonyPostfix]
        public static void TransformValue_postfix(StatWorker __instance, Thing gear, StatDef stat, ref float __result)
        {
            if (stat == StatDefOf.MoveSpeed)
            {
                if (gear != null)
                {
                    CompSmartgunSystem smartgunSystem = gear.TryGetComp<CompSmartgunSystem>();
                    if (smartgunSystem!=null && smartgunSystem.hasHarness)
                    {
                        __result = 0f;
                    }
                }
            }
            return;
        }
    }

}