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

    // ObserveSurroundingThings
    [HarmonyPatch(typeof(PawnObserver), "ObserveSurroundingThings")]
    public static class AvP_PawnObserver_ObserveSurroundingThings_Patch
    {
        [HarmonyPostfix]
        public static void ObserveSurroundingThingsPostfix(PawnObserver __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = (Pawn)AvP_PawnObserver_ObserveSurroundingThings_Patch.pawn.GetValue(__instance);
            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
            {
                return;
            }
            Map map = pawn.Map;
            int num = 0;
            while ((float)num < 100f)
            {
                IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[num];
                if (intVec.InBounds(map))
                {
                    if (GenSight.LineOfSight(intVec, pawn.Position, map, true, null, 0, 0))
                    {
                        List<Thing> thingList = intVec.GetThingList(map).FindAll(x => x.TryGetComp<Comp_Xenomorph>() != null);
                        for (int i = 0; i < thingList.Count; i++)
                        {
                            IThoughtGiver thoughtGiver = thingList[i].TryGetComp<Comp_Xenomorph>() as IThoughtGiver;
                            if (thoughtGiver != null)
                            {
                                Thought_Memory thought_Memory = thoughtGiver.GiveObservedThought();
                                if (thought_Memory != null)
                                {
                                    pawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, null);
                                }
                            }
                        }
                    }
                }
                num++;
            }
        }
        public static FieldInfo pawn = typeof(PawnObserver).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }

}