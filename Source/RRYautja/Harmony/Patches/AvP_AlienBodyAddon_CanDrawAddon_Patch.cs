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
using AlienRace;

namespace RRYautja.HarmonyInstance
{
    [HarmonyPatch(typeof(AlienPartGenerator.BodyAddon), "CanDrawAddon")]
    public static class AvP_AlienBodyAddon_CanDrawAddon_Patch
    {
        public static bool Prefix(Pawn pawn)
        {
            bool flag = pawn.health.hediffSet.HasHediff(YautjaDefOf.AvP_Hediff_Cloaked, false);
            if (flag)
            {
                //Log.Message(string.Format("tetet"));
                return false;
            }
            return true;
        }
    }
    
}