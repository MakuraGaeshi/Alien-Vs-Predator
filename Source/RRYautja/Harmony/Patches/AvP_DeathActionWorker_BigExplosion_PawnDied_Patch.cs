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
    [HarmonyPatch(typeof(DeathActionWorker_BigExplosion), "PawnDied")]
    public static class AvP_DeathActionWorker_BigExplosion_PawnDied_Patch
    {
        public static bool Prefix(Corpse corpse, DeathActionWorker_BigExplosion __instance)
        {
            bool flag = XenomorphUtil.IsInfectedPawn(corpse.InnerPawn);
            if (flag)
            {
                return false;
            }
            return true;
        }
    }
    
}