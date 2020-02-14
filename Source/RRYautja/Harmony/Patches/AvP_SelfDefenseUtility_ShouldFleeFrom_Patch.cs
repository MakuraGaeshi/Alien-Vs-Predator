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
    [HarmonyPatch(typeof(SelfDefenseUtility), "ShouldFleeFrom")]
    public static class AvP_SelfDefenseUtility_ShouldFleeFrom_Patch
    {
        [HarmonyPostfix]
        public static void ShouldFleeFromPostfix(Thing t, Pawn pawn, bool checkDistance, bool checkLOS, ref bool __result)
        {
            if (pawn!=null)
            {
                if (t.GetType() == typeof(Pawn))
                {
                    Pawn other = (Pawn)t;
                //    Log.Message(string.Format("{0} vs {1}", pawn.LabelShortCap, other.LabelShortCap));
                }
                if (t.TryGetComp<Comp_Xenomorph>() is Comp_Xenomorph _Xeno && pawn.CanSee(t))
                {
                //    Log.Message("t.TryGetComp<Comp_Xenomorph>() is Comp_Xenomorph _Xeno && pawn.CanSee(t)");
                    if (pawn.isXenomorph())
                    {
                    //    Log.Message("pawn.isXenomorph() __result = false");
                        __result = false;
                        return;
                    }
                    if (pawn.isNeomorph() && pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_NeomorphFullyFormed)
                    {
                    //    Log.Message("pawn.isNeomorph() && pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_NeomorphFullyFormed __result = true");
                        __result = true;
                        return;
                    }
                    if (!pawn.isNeomorph() && !pawn.isXenomorph())
                    {
                    //    Log.Message("!pawn.isNeomorph() && !pawn.isXenomorph() __result = true");
                        __result = true;
                        return;
                    }
                }
                if (t.TryGetComp<Comp_Facehugger>() is Comp_Facehugger _Hugger)
                {
                //    Log.Message("t.TryGetComp<Comp_Facehugger>() is Comp_Facehugger _Hugger");
                    if (pawn.isXenomorph())
                    {
                    //    Log.Message("pawn.isXenomorph() __result = false");
                        __result = false;
                        return;
                    }
                    if (pawn.isNeomorph())
                    {
                    //    Log.Message("pawn.isNeomorph() __result = false");
                        __result = false;
                        return;
                    }
                    if (pawn.isPotentialHost() && !pawn.isXenomorph())
                    {
                    //    Log.Message("pawn.isPotentialHost() && !pawn.isXenomorph() __result = true");
                        __result = true;
                        return;
                    }
                }
                if (t.TryGetComp<Comp_Neomorph>() is Comp_Neomorph _Neo)
                {
                //    Log.Message("t.TryGetComp<Comp_Neomorph>() is Comp_Neomorph _Neo");
                    if (pawn.isXenomorph() && pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_NeomorphFullyFormed)
                    {
                    //    Log.Message("pawn.isXenomorph() && pawn.ageTracker.CurLifeStage != XenomorphDefOf.RRY_NeomorphFullyFormed __result = true");
                        __result = true;
                        return;
                    }
                    if (!pawn.isNeomorph() && !pawn.isXenomorph())
                    {
                    //    Log.Message("!pawn.isNeomorph() && !pawn.isXenomorph() __result = true");
                        __result = true;
                        return;
                    }
                }
                if (t.GetType() == typeof(Pawn))
                {
                    Pawn other = (Pawn)t;
                //    Log.Message(string.Format("{1}.GetType() == typeof(Pawn) vs {0}", pawn.LabelShortCap, other.LabelShortCap));
                    if (other != null && pawn.CanSee(other))
                    {
                    //    Log.Message("other != null && pawn.CanSee(other)");
                        if (pawn.Position.DistanceTo(other.Position) < 20 && (!other.isXenomorph() && pawn.isXenomorph(out Comp_Xenomorph comp) && comp.hidden) || (other.isNeomorph() && !pawn.isNeomorph()))
                        {
                        //    Log.Message("pawn.Position.DistanceTo(other.Position) < 20 && (!other.isXenomorph() && pawn.isXenomorph(out Comp_Xenomorph comp) && comp.hidden) || (other.isNeomorph() && !pawn.isNeomorph()) __result = true");
                            __result = true;
                            return;
                        }
                    }
                }
            }
        }
    }

}