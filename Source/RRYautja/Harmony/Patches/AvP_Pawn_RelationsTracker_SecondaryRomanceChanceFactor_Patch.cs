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

namespace RRYautja.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryRomanceChanceFactor", null)]
    public class AvP_Pawn_RelationsTracker_SecondaryRomanceChanceFactor_Patch
    {
        [HarmonyPostfix]
        public static void SecondaryRomanceChanceFactor(Pawn_RelationsTracker __instance, Pawn otherPawn, ref float __result)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = (Pawn)AvP_Pawn_RelationsTracker_SecondaryRomanceChanceFactor_Patch.pawn.GetValue(__instance);
            bool flag = pawn != null && otherPawn != null;
            if (flag)
            {
                bool alien = !Equals(otherPawn.def, pawn.def);
                if (pawn.isYautja() && alien)
                {
                    float num = 0f;
                    __result *= num;
                }
                else
                {
                    /*
                    int degree = pawn.story.traits.DegreeOfTrait(DefDatabase<TraitDef>.GetNamedSilentFail("Xenophobia"));
                    if (alien)
                    {
                        if (degree == 1)
                        {
                            float num = 0.25f;
                            __result *= num;
                        //    Log.Message(string.Format("{0}({1}) is alien to {2}({3}), lowering compatability by {4} to {5} due to {2}'s Xenophobia", otherPawn.LabelShortCap, otherPawn.def.LabelCap, pawn.LabelShortCap, pawn.def.LabelCap, num, __result));
                        }
                        else if (degree == -1)
                        {
                            float num = 1.75f;
                            __result *= num;
                        //    Log.Message(string.Format("{0}({1}) is alien to {2}({3}), increasing compatability by {4} to {5} due to {2}'s Xenophelia", otherPawn.LabelShortCap, otherPawn.def.LabelCap, pawn.LabelShortCap, pawn.def.LabelCap, num, __result));
                        }
                        else
                        {
                            float num = 0.5f;
                            __result *= num;
                        //    Log.Message(string.Format("{0}({1}) is alien to {2}({3}), lowering compatability by {4} to {5}", otherPawn.LabelShortCap, otherPawn.def.LabelCap, pawn.LabelShortCap, pawn.def.LabelCap, num, __result));
                        }
                    }
                    else
                    {
                        float num = 1f;
                        __result *= num;

                    //    Log.Message(string.Format("{0} and {1} are both {2} no action taken", otherPawn.LabelShortCap, pawn.LabelShortCap, pawn.def.LabelCap));
                    }
                    */
                }
            }
        }

        public static FieldInfo pawn = typeof(Pawn_RelationsTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

    }

}