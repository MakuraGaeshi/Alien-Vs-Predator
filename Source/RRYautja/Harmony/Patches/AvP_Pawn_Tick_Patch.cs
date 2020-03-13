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
    /*
    [HarmonyPatch(typeof(Pawn), "Tick")]
    public static class AvP_Pawn_Tick_Patch
    {
        [HarmonyPostfix]
        public static void ApparelCompTick(Pawn __instance)
        {
            if (__instance.apparel.WornApparelCount>0)
            {
                List<Apparel> list = __instance.apparel.WornApparel;
                if (list.Any(x => x.TryGetComp<CompWearable>()!=null))
                {
                    foreach (var item in list.All(x => x.TryGetComp<CompWearable>() != null))
                    {

                    }
                }
            }

        }
        
    }
    */
}