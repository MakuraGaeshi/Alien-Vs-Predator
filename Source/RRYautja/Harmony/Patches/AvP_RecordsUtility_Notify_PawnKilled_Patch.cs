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
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    [HarmonyPatch(typeof(RecordsUtility), "Notify_PawnKilled")]
    public static class AvP_RecordsUtility_Notify_PawnKilled_Patch
    {
        [HarmonyPostfix]
        public static void IncrementPostfix(Pawn killed, Pawn killer)
        {
            if (killer!=null)
            {
                if (killed.isXenomorph())
                {
                    if (killer.needs.mood.thoughts.memories.AnyMemoryConcerns(killed))
                    {
                        killer.needs.mood.thoughts.memories.RemoveMemoriesWhereOtherPawnIs(killed);
                    }
                }
            }
        }
    }
}