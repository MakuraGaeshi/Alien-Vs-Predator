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
using HunterMarkingSystem.Settings;
using HunterMarkingSystem.ExtensionMethods;

namespace HunterMarkingSystem
{
    [StaticConstructorOnStartup]
    class Harmony
    {
        static Harmony()
        {
            //    HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create("com.ogliss.rimworld.mod.HunterMarkingSystem");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}