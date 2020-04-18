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
using System.Text.RegularExpressions;

namespace AvP.HarmonyInstance
{
    
    [HarmonyPatch(typeof(BackstoryDatabase), "GetIdentifierClosestMatch")]
    public static class AvP_BackstoryDatabase_GetIdentifierClosestMatch_Patch
    {
        public static void Prefix(ref string identifier)
        {
            if (identifier.Contains("RRY"))
            {
                identifier = Regex.Replace(identifier, "^RRY", "AvP");
            }
        }
    }
    
}