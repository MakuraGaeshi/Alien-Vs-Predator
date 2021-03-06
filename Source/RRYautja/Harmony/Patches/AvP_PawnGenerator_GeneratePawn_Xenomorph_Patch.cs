﻿using RimWorld;
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

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) })]
    public static class AvP_PawnGenerator_GeneratePawn_Xenomorph_Patch
    {
        public static void Postfix(PawnGenerationRequest request, ref Pawn __result)
        {
            if (__result.kindDef.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph)
            {
                if (request.KindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                {
                    __result.gender = Gender.Female;
                    /*
                    bool QueenPresent = false;
                    
                    foreach (var p in Find.AnyPlayerHomeMap.mapPawns.AllPawns)
                    {
                        if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                        {
                        //    Log.Message(string.Format("Queen Found"));
                            QueenPresent = true;
                            break;
                        }
                    }
                    if (QueenPresent)
                    {
                        request = new PawnGenerationRequest(XenomorphDefOf.RRY_Xenomorph_Warrior, request.Faction, request.Context, -1, true, false, false, false, false, true, 0f, fixedGender: Gender.None, allowGay: false);
                        __result = PawnGenerator.GeneratePawn(request);
                        __result.gender = Gender.None;
                        return;
                    }
                    else
                    {
                        __result.gender = Gender.Female;
                    }
                    */
                }
                else
                {
                    __result.gender = Gender.None;
                }
                if (request.KindDef == XenomorphDefOf.RRY_Xenomorph_Thrumbomorph && !SettingsHelper.latest.AllowThrumbomorphs)
                {
                    __result.kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    __result.def = XenomorphRacesDefOf.RRY_Xenomorph_Warrior;
                }
                if (request.KindDef == XenomorphDefOf.RRY_Xenomorph_Predalien && !SettingsHelper.latest.AllowPredaliens)
                {
                    __result.kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    __result.def = XenomorphRacesDefOf.RRY_Xenomorph_Warrior;
                }
            }
        }
    }

}