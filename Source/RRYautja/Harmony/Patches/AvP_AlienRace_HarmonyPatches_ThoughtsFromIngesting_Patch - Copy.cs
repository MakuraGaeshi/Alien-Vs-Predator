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
using AlienRace;
using System.Reflection.Emit;

namespace RRYautja
{
    [HarmonyPatch(typeof(GenStep_ScatterRuinsSimple), "ScatterAt")]
    static class AvP_GenStep_ScatterRuinsSimple_ScatterAt_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (var instruction in instructionsList)
            {
                yield return instruction;
                if (instruction.operand as MethodInfo == typeof(CellRect).GetMethod("get_CenterCell"))
                {
                    Log.Message("found get_CenterCell");
                    yield return new CodeInstruction(OpCodes.Call, typeof(AvP_GenStep_ScatterRuinsSimple_ScatterAt_Patch).GetMethod("CenterCellValue"));
                }

            }
        }
        public static IntVec3 CenterCellValue(IntVec3 pos)
        {
            Log.Message("x: " + pos.x);
            Log.Message("y: " + pos.y);
            Log.Message("center cell value: " + pos);

            return pos;
        }
    }
    
}