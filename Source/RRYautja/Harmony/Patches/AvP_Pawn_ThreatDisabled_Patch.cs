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
using AvP.settings;
using AvP.ExtensionMethods;

namespace AvP.HarmonyInstance
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
            Pawn pawn = null;
            if (disabledFor != null)
            {
                if (disabledFor.Thing != null)
                {
                    if (disabledFor.Thing.GetType() == typeof(Pawn))
                    {
                        pawn = (Pawn)disabledFor.Thing;
                        if (pawn != null)
                        {
                            if (pawn.equipment != null)
                            {
                                if (pawn.equipment.Primary != null)
                                {
                                    CompSmartgunSystem smartgunSystem = pawn.equipment.Primary.TryGetComp<CompSmartgunSystem>();
                                    if (smartgunSystem != null)
                                    {
                                        if (smartgunSystem.hasTargheter && smartgunSystem.hasHarness)
                                        {
                                        //    Log.Message(string.Format("{0} IgnoreCloak {1}: {2}", pawn.LabelShortCap, __instance, __result));
                                            return;
                                        }
                                    }
                                }
                            }
                            _Xenomorph = pawn.TryGetComp<Comp_Facehugger>();
                            if (_Xenomorph != null)
                            {
                                __result = __result || !__instance.isPotentialHost();
                                //    Log.Message(string.Format("__instance: {0}, __result: {1}, _Xenomorph: {2}, Infectable?: {3}", __instance, __result, _Xenomorph, __instance.isPotentialHost()));
                            }
                        }

                    }
                }
            }

            if (__instance != null)
            {
                if (__instance != null)
                {

                    bool cloaked = __instance.health.hediffSet.HasHediff(YautjaDefOf.AvP_Hediff_Cloaked);
                    bool hidden = __instance.isXenomorph(out Comp_Xenomorph comp) && comp.hidden;
                    bool Stealth = cloaked || hidden;
                    __result = __result || Stealth;

                }
            } // XenomorphDefOf.AvP_Hediff_Xenomorph_Hidden

        }
    }
}