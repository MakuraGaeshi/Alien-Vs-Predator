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
    [HarmonyPatch(typeof(GenHostility), "HostileTo", new Type[] { typeof(Thing), typeof(Thing) })]
    public static class AvP_GenHostility_HostileTo_Xenomorph_Patch
    {
        public static bool Postfix(bool __result, Thing a, Thing b)
        {
            return __result || (a.isXenomorph() && !b.isXenomorph()) || (!a.isXenomorph() && b.isXenomorph());
        }
    }

    [HarmonyPatch(typeof(GenHostility), "HostileTo", new Type[] { typeof(Thing), typeof(Thing) })]
    public static class AvP_GenHostility_HostileTo_Neomorph_Patch
    {
        public static bool Postfix(bool __result, Thing a, Thing b)
        {
            return __result || (a.isNeomorph() && !b.isNeomorph()) || (!a.isNeomorph() && b.isNeomorph());
        }
    }
}