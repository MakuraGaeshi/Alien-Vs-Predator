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
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(PowerNet), "PowerNetTick")]
    public static class AvP_PowerNet_PowerNetTick_Patch
    {
        [HarmonyPrefix]
        public static bool prePowerNetTick(PowerNet __instance)
        {
            float num = __instance.CurrentEnergyGainRate();
            float num2 = __instance.CurrentStoredEnergy();
            bool active = !__instance.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
        //    Log.Message(string.Format("PowerNetTick CurrentEnergyGainRate: {0}, CurrentStoredEnergy: {1}", num, num2));
            
            return true;
        }
    }
    */
}