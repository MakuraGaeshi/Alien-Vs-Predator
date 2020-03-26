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
    [HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
    public static class AvP_PawnGenerator_GenerateBodyType_Yautja_Patch
    {
        [HarmonyPostfix]
        public static void Yautja_GenerateBodyTypePatch(ref Pawn pawn)
        {
            bool flag = pawn.def == YautjaDefOf.RRY_Alien_Yautja;
            if (flag)
            {
                pawn.story.bodyType = (pawn.gender != Gender.Female) ? YautjaDefOf.RRYYautjaMale : YautjaDefOf.RRYYautjaFemale;
            }
        }
    }
}