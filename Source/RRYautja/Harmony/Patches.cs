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
    [StaticConstructorOnStartup]
    class Main
    {
        private static readonly Type patchType = typeof(Main);
        static Main()
        {
            //    HarmonyInstance.DEBUG = true;
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(x=> x.apparel!=null))
            {
                if (def.apparel.wornGraphicPath.NullOrEmpty())
                {
                    if (!AlienRace.RaceRestrictionSettings.apparelWhiteDict.ContainsKey(key: def))
                        AlienRace.RaceRestrictionSettings.apparelWhiteDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                    AlienRace.RaceRestrictionSettings.apparelWhiteDict[key: def].Add(item: ((AlienRace.ThingDef_AlienRace)YautjaDefOf.RRY_Alien_Yautja));
                }
            }
            IEnumerable<ThingDef> pystrainers = DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains(NeurotrainerDefGenerator.PsytrainerDefPrefix));
            foreach (AbilityDef item in DefDatabase<RRYautja.EquipmentAbilityDef>.AllDefs)
            {
                if (pystrainers.Any(x => x.defName.Contains(item.defName)))
                {
                    ThingDef trainer = pystrainers.First(x => x.defName.Contains(item.defName));
                    DefDatabase<ThingDef>.AllDefsListForReading.Remove(trainer);
                }
            }
            Log.Message("clothes loaded");
            /*
            if (enabled_AI)
            {
                Log.Message("Thumper detected");
                harmony.Patch(AccessTools.Method(typeof(IncidentWorker_Hivelike), "IncidentWorker_UniversalTryExecuteWorker_Prefix", null, null), new HarmonyMethod(Main.patchType, "AlienVsPredator_Compatibility_Prefix", null));
            }
            */
        }
        public static bool AlienVsPredator_Compatibility_Prefix(IncidentWorker_Infestation __instance, IncidentParms parms, ref bool __result)
        {
            if (parms.faction==null)
            {
                Log.Message("parms.faction == null");
            }
            return __result;
        }

    }
    
    public abstract class CompWearable : ThingComp
    {
        public virtual IEnumerable<Gizmo> CompGetGizmosWorn() {
            // return no Gizmos
            return new List<Gizmo>();
        }
    }
   
}