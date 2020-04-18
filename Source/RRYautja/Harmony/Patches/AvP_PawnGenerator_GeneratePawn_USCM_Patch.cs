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
using AvP.settings;
using AvP.ExtensionMethods;

namespace AvP.HarmonyInstance
{

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) })]
    public static class AvP_PawnGenerator_GeneratePawn_USCM_Patch
    {
        public static void Postfix(PawnGenerationRequest request, ref Pawn __result)
        {
            /*
            if (__result.Faction.def.defName.Contains("AvP_USCM"))
            {

            }
            */
        }
    }

}