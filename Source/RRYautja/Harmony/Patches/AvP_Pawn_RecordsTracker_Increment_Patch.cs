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
    // Marking system tick replacement
    [HarmonyPatch(typeof(Pawn_RecordsTracker), "Increment")]
    public static class AvP_Pawn_RecordsTracker_Increment_Patch
    {
        [HarmonyPostfix]
        public static void IncrementPostfix(Pawn_RecordsTracker __instance, RecordDef def)
        {
            if (def == RecordDefOf.Kills)
            {
                Pawn p = __instance.pawn;
                if (p != null && p.isBloodable() && p.BloodStatus() is Comp_Yautja _Yautja)
                {
                    if (p.CurBloodStatus() == AvPExtensions.BloodStatusMode.None)
                    {
                        p.health.AddHediff(YautjaDefOf.RRY_Hediff_Unblooded);
                    }
                    _Yautja.CalcTick();
                }
                if (p.isYautja())
                {
                    List<Thought_Memory> _Memories = p.needs.mood.thoughts.memories.Memories.FindAll(x => x.def == YautjaDefOf.RRY_Thought_ThrillOfTheHunt);
                    if (_Memories.Count < YautjaDefOf.RRY_Thought_ThrillOfTheHunt.stackLimit)
                    {
                        p.needs.mood.thoughts.memories.Memories.Add(new Thought_Memory()
                        {
                            def = YautjaDefOf.RRY_Thought_ThrillOfTheHunt
                        });
                    }
                }
            }
        }
    }
}