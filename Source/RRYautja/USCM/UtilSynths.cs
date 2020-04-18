﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RRYautja
{
    static class UtilSynths
    {
        public static bool isAvPSynth(PawnKindDef pawn)
        {
            bool Result = pawn.RaceProps.FleshType.defName == "AvP_SynthFlesh";

            return Result;
        }
        public static bool isAvPSynth(Pawn pawn)
        {
            bool Result = pawn.def.race.FleshType.defName == "AvP_SynthFlesh";

            return Result;
        }
        public static bool isAvPSynth(ThingDef td)
        {
            bool Result = td.race.FleshType.defName == "AvP_SynthFlesh" || td==USCMDefOf.AvP_Synth;

            return Result;
        }
    }
}
