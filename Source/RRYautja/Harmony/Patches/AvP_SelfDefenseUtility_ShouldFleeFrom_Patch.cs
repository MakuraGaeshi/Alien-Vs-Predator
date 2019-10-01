﻿using RimWorld;
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
    [HarmonyPatch(typeof(SelfDefenseUtility), "ShouldFleeFrom")]
    public static class AvP_SelfDefenseUtility_ShouldFleeFrom_Patch
    {
        [HarmonyPostfix]
        public static void ShouldFleeFromPostfix(Thing t, Pawn pawn, bool checkDistance, bool checkLOS, ref bool __result)
        {
            if (t.TryGetComp<Comp_Xenomorph>() is Comp_Xenomorph _Xeno && pawn.CanSee(t))
            {
                if (pawn.isXenomorph())
                {
                    __result = false;
                }
                if (pawn.isNeomorph() && pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_NeomorphFullyFormed)
                {
                    __result = true;
                }
                if (!pawn.isNeomorph() && !pawn.isXenomorph())
                {
                    __result = true;
                }
            }
            if (t.TryGetComp<Comp_Facehugger>() is Comp_Facehugger _Hugger)
            {
                if (pawn.isXenomorph())
                {
                    __result = false;
                }
                if (pawn.isNeomorph())
                {
                    __result = false;
                }
                if (pawn.isPotentialHost() && !pawn.isXenomorph())
                {
                    __result = true;
                }
            }
            if (t.TryGetComp<Comp_Neomorph>() is Comp_Neomorph _Neo)
            {
                if (pawn.isXenomorph() && pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_NeomorphFullyFormed)
                {
                    __result = true;
                }
                if (!pawn.isNeomorph() && !pawn.isXenomorph())
                {
                    __result = true;
                }
            }
        }
    }

}