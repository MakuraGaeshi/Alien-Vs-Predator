using RimWorld;
using Verse;
using Harmony;
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
        static Main()
        {
            //    HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create("com.ogliss.rimworld.mod.rryatuja");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
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