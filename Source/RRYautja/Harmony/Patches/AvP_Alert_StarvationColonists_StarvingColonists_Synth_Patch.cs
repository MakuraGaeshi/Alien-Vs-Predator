﻿using RimWorld;
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
    
    [HarmonyPatch(typeof(Alert_StarvationColonists), "get_StarvingColonists")]
    public static class AvP_Alert_StarvationColonists_StarvingColonists_Synth_Patch
    {
        public static void Postfix(ref List<Pawn> __result)
        {
            List<Pawn> list = __result.Where(x=> x.def != USCMDefOf.RRY_Synth).ToList();
            __result = list;
        }
    }
    
}