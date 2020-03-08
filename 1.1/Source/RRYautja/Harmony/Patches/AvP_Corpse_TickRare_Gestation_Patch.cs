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

    [HarmonyPatch(typeof(Corpse), "TickRare")]
    public static class AvP_Corpse_TickRare_Gestation_Patch
    {
        [HarmonyPostfix]
        public static void Post_TickRare(ref Corpse __instance)
        {
            if (__instance!=null)
            {
                if (__instance.InnerPawn!=null)
                {
                    Hediff parasite;
                    bool host = __instance.InnerPawn.isHost(out parasite);
                    if (host)
                    {
                        if (parasite!=null)
                        {
                            for (int i = 0; i < 250; i++)
                            {
                                parasite.Tick();
                            }
                        }
                    }
                }
            }
        }
    }
    
}