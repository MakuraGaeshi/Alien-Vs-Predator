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
    [HarmonyPatch(typeof(Verb_Shoot), "TryStartCastOn")]
    public static class AvP_Verb_Shoot_TryStartCastOn_SmartGun_Patch
    {
        [HarmonyPrefix]
        public static void TryStartCastOn_SmartGun_Prefix(ref Verb __instance, LocalTargetInfo castTarg, ref bool __result)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompSmartgunSystem>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompSmartgunSystem>() is CompSmartgunSystem GunExt)
                        {
                            Log.Message("smartgun checking for targeter");
                            Thing equipment = __instance.EquipmentSource;
                            CompEquippable eq = __instance.EquipmentSource.TryGetComp<CompEquippable>();
                            Pawn pawn = __instance.CasterPawn;
                            if (GunExt.hasTargheter)
                            {
                                __instance.verbProps.warmupTime = GunExt.AdjustedWarmup;
                                Log.Message(string.Format("smartgun adjusted warmup: {0}", GunExt.AdjustedWarmup));
                            }
                            else
                            {
                                __instance.verbProps.warmupTime = GunExt.originalwarmupTime;
                                Log.Message(string.Format("smartgun original warmup: {0}", GunExt.originalwarmupTime));
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        public static void TryStartCastOn_SmartGun_Postfix(ref Verb __instance, LocalTargetInfo castTarg)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.TryGetComp<CompSmartgunSystem>() != null)
                    {
                        if (__instance.EquipmentSource.TryGetComp<CompSmartgunSystem>() is CompSmartgunSystem GunExt)
                        {
                            if (GunExt != null)
                            {
                                __instance.verbProps.warmupTime = GunExt.originalwarmupTime;
                            }
                        }
                    }
                }
            }
        }
    }

}