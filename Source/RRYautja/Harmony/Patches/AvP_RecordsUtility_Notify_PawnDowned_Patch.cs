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
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    
    [HarmonyPatch(typeof(RecordsUtility), "Notify_PawnDowned")]
    public static class AvP_RecordsUtility_Notify_PawnDowned_Patch
    {
        [HarmonyPrefix]
        public static bool IncrementPostfix(Pawn downed, Pawn instigator)
        {
            if (instigator!=null)
            {
                if (instigator.isXenomorph())
                {
                    HealthUtility.DamageUntilDowned(downed,false);
                    return false;
                }
            }
            return true;
        }
    }
}