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
    [HarmonyPatch(typeof(Pawn_NativeVerbs), "CheckCreateVerbProperties")]
    public static class AvP_Pawn_NativeVerbs_CheckCreateVerbProperties_Patch
    {
        public static bool Prefix(ref Pawn_NativeVerbs __instance)
        {
            bool flag = Main._cachedVerbProperties.GetValue(__instance) != null;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                bool flag2 = XenomorphUtil.IsXenomorph(Main.pawnPawnNativeVerbs(__instance));
                if (flag2)
                {
                    Main._cachedVerbProperties.SetValue(__instance, new List<VerbProperties>());
                    Main.cachedVerbProperties(__instance).Add(NativeVerbPropertiesDatabase.VerbWithCategory((VerbCategory)1));
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }
    }
    
}