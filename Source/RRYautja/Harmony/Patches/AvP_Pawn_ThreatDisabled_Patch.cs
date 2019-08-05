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
    // Pawns ignore Cloaked things
    [HarmonyPatch(typeof(Pawn), "ThreatDisabled")]
    public static class AvP_Pawn_ThreatDisabled_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreCloak(Pawn __instance, ref bool __result, IAttackTargetSearcher disabledFor)
        {
            bool selected__instance = Find.Selector.SelectedObjects.Contains(__instance);
            Comp_Facehugger _Xenomorph = null;
            if (disabledFor != null)
            {
                if (disabledFor.Thing != null)
                {
                    _Xenomorph = disabledFor.Thing.TryGetComp<Comp_Facehugger>();
                    if (_Xenomorph != null)
                    {
                        __result = __result || !XenomorphUtil.isInfectablePawn(__instance);
                        //    Log.Message(string.Format("__instance: {0}, __result: {1}, _Xenomorph: {2}, Infectable?: {3}", __instance, __result, _Xenomorph, XenomorphUtil.isInfectablePawn(__instance)));
                    }
                }
            }
            if (__instance != null)
            {
                if (__instance != null)
                {

                }
            } // XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden
            __result = __result || ((__instance.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked) || __instance.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden)) && _Xenomorph == null);

        }
    }
}