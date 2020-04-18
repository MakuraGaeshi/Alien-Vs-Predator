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
    // Stops Cocooned Pawns taking damage from Xeno blood
    [HarmonyPatch(typeof(Pawn), "PreApplyDamage")]
    public static class AvP_Pawn_PreApplyDamage_Patch
    {
        [HarmonyPrefix]
        public static bool Ignore_Acid_Damage(Pawn __instance, ref DamageInfo dinfo, out bool absorbed)
        {
            if (__instance.health.hediffSet.HasHediff(XenomorphDefOf.AvP_Hediff_Cocooned) || __instance.isXenomorph())
            {
                absorbed = dinfo.Def == XenomorphDefOf.AvP_AcidBurn || dinfo.Def == XenomorphDefOf.AvP_AcidDamage;
            }
            else
            {
                absorbed = false;
            }
            /*
            if (dinfo.Instigator!=null)
            {
                if (dinfo.Instigator.isXenomorph())
                {
                //    Log.Message("cause by Xenomorph");
                }
            }
            */
            return !absorbed;
        }

    }

}