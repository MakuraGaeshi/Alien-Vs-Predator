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

namespace RRYautja
{
    [HarmonyPatch(typeof(PawnUtility), "IsInvisible")]
    public static class AvP_PawnUtility_IsInvisible_Patch
    {
        [HarmonyPostfix]
        public static void ThoughtsFromIngestingPostPrefix(Pawn pawn, ref bool __result)
        {
            if (pawn == null)
            {
                Log.Message("Pawn is null");
                return;
            }
            if (pawn.isXenomorph(out Comp_Xenomorph xenomorph))
            {
                Log.Message("Pawn is Xenomorph");
                if (xenomorph!=null)
                {
                    if (xenomorph.Hidden)
                    {
                        __result = true;
                    }
                }
            }
            else
            {
                if (pawn.RaceProps.Humanlike)
                {
                    Log.Message("Pawn is Humanlike");
                    if (pawn.apparel.WornApparel.Any(x=> x.GetType() == typeof(Cloakgen)))
                    {
                        Log.Message("Pawn has Cloakgen");
                        Cloakgen cloak = (Cloakgen)pawn.apparel.WornApparel.First(x => x.GetType() == typeof(Cloakgen));
                        if (cloak!=null)
                        {
                            __result = __result || cloak.cloakMode == Cloakgen.CloakMode.On;
                        }
                    }
                }
            }
        }
    }
    
}