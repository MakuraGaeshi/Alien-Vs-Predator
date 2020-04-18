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
    // Stuffable Projectile hunting weapon fix
    [HarmonyPatch(typeof(VerbUtility), "HarmsHealth")]
    public static class AvP_VerbUtility_HarmsHealth_Patch
    {
        [HarmonyPostfix]
        public static void HarmsHealthPostfix(Verb verb, ref bool __result)
        {
            __result = __result || verb is Verb_Launch_Stuffable_Projectile || verb is Verb_Shoot_Stuffable;
        }
    }

}