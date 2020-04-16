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

    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public static class AvP_PawnRenderer_DrawEquipment_Cloak_Patch
    {
        public static bool Prefix(PawnRenderer __instance)
        {
            Pawn pawn = Main.PawnRenderer_GetPawn(__instance);
            bool flag = pawn.health.hediffSet.HasHediff(YautjaDefOf.AvP_Hediff_Cloaked, false);
            if (flag)
            {
                return false;
            }
            return true;
        }
    }

}