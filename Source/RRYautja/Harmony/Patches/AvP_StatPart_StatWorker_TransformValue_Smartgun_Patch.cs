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

namespace RRYautja
{
    /*
    // StatWorker patch GearAffectsStat RelevantGear
    [HarmonyPatch(typeof(StatWorker), "RelevantGear")]
    public static class AvP_StatPart_StatWorker_RelevantGear_Smartgun_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<Thing> RelevantGear_Postfix(IEnumerable<Thing> __result, Pawn pawn, StatDef stat)
        {
            foreach (Thing gear in __result)
            {
                if (stat == StatDefOf.MoveSpeed)
                {
                    Log.Message("speed stat mod");
                    if (gear != null)
                    {
                        Log.Message(string.Format("on {0}", gear.LabelCap));
                        CompSmartgunSystem smartgunSystem = gear.TryGetComp<CompSmartgunSystem>();
                        if (smartgunSystem != null && smartgunSystem.hasHarness)
                        {
                            Log.Message(string.Format("{0} hasHarness", gear.LabelCap));
                            continue;
                        }
                    }
                }
                yield return gear;
            }
            yield break;
        }
    }
    */
    
    // StatWorker patch StatOffsetFromGear
    [HarmonyPatch(typeof(StatWorker), "StatOffsetFromGear"), HarmonyAfter("Infusion.Harmonize.StatWorker")]
    public static class AvP_StatPart_StatWorker_StatOffsetFromGear_Smartgun_Patch
    {
        [HarmonyPostfix]
        public static void StatOffsetFromGear_Postfix(StatWorker __instance, Thing gear, StatDef stat, ref float __result)
        {
            if (stat == StatDefOf.MoveSpeed)
            {
                Log.Message("speed stat mod");
                if (gear != null)
                {
                    Log.Message(string.Format("on {0}", gear.LabelCap));
                    CompSmartgunSystem smartgunSystem = gear.TryGetComp<CompSmartgunSystem>();
                    if (smartgunSystem != null && smartgunSystem.hasHarness)
                    {
                        Log.Message(string.Format("{0} hasHarness", gear.LabelCap));
                        __result = 0f;
                    }
                }
            }
            return;
        }
    }

}