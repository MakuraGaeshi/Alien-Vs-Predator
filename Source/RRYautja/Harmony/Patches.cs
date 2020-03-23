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
    [StaticConstructorOnStartup]
    class Main
    {
        public static readonly Type patchType = typeof(Main);
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

            ThingDef thing = DefDatabase<ThingDef>.GetNamedSilentFail("O21_AntiInfestationThumper");
            if (thing != null)
            {
                ThumperPatch();
            }
        }
        public static void ThumperPatch()
        {
            AvPMod.harmony.Patch(AccessTools.Method(typeof(Thumper.HarmonyPatches), "AlienVsPredator_Compatibility", null, null), new HarmonyMethod(Main.patchType, "AlienVsPredator_Compatibility_Prefix", null));
        }
        public static bool AlienVsPredator_Compatibility_Prefix()
        {
        //    Log.Message("FYT");
            return false;
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