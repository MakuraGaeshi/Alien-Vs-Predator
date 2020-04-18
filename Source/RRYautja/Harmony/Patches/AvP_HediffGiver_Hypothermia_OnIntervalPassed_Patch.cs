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

namespace RRYautja.HarmonyInstance
{
    // Xeno/Neomorph Hypothermic slowdown
    [HarmonyPatch(typeof(HediffGiver_Hypothermia), "OnIntervalPassed")]
    public static class AvP_HediffGiver_Hypothermia_OnIntervalPassed_Patch
    {
        [HarmonyPrefix]
        public static bool OnIntervalPassedPrefix(Pawn pawn, Hediff cause)
        {
            if (pawn.RaceProps.FleshType == XenomorphRacesDefOf.AvP_Xenomorph)
            {
                float ambientTemperature = pawn.AmbientTemperature;
                FloatRange floatRange = pawn.ComfortableTemperatureRange();
                FloatRange floatRange2 = pawn.SafeTemperatureRange();
                HediffSet hediffSet = pawn.health.hediffSet;
                HediffDef hediffDef = XenomorphDefOf.HypothermicSlowdown;
                Hediff firstHediffOfDef = hediffSet.GetFirstHediffOfDef(hediffDef, false);
                if (ambientTemperature < floatRange2.min)
                {
                    float num = Mathf.Abs(ambientTemperature - floatRange2.min);
                    float num2 = num * 6.45E-05f;
                    num2 = Mathf.Max(num2, 0.00075f);
                    HealthUtility.AdjustSeverity(pawn, hediffDef, num2);
                    if (pawn.Dead)
                    {
                        return true;
                    }
                }
                if (firstHediffOfDef != null)
                {
                    if (ambientTemperature > floatRange.min)
                    {
                        float num3 = firstHediffOfDef.Severity * 0.027f;
                        num3 = Mathf.Clamp(num3, 0.0015f, 0.015f);
                        firstHediffOfDef.Severity -= num3;
                    }
                    else if (pawn.RaceProps.FleshType != XenomorphRacesDefOf.AvP_Xenomorph && ambientTemperature < 0f && firstHediffOfDef.Severity > 0.37f)
                    {
                        float num4 = 0.025f * firstHediffOfDef.Severity;
                        if (Rand.Value < num4)
                        {
                            BodyPartRecord bodyPartRecord;
                            if ((from x in pawn.RaceProps.body.AllPartsVulnerableToFrostbite
                                 where !hediffSet.PartIsMissing(x)
                                 select x).TryRandomElementByWeight((BodyPartRecord x) => x.def.frostbiteVulnerability, out bodyPartRecord))
                            {
                                int num5 = Mathf.CeilToInt((float)bodyPartRecord.def.hitPoints * 0.5f);
                                DamageDef frostbite = DamageDefOf.Frostbite;
                                float amount = (float)num5;
                                BodyPartRecord hitPart = bodyPartRecord;
                                DamageInfo dinfo = new DamageInfo(frostbite, amount, 0f, -1f, null, hitPart, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                pawn.TakeDamage(dinfo);
                            }
                        }
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}