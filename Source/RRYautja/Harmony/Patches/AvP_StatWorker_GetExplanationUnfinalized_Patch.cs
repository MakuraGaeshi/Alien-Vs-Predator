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
    // Stuffable Projectile Explanation patch
    [HarmonyPatch(typeof(StatWorker), "GetExplanationUnfinalized")]
    public static class AvP_StatWorker_GetExplanationUnfinalized_Patch
    {
        [HarmonyPostfix]
        public static void GetExplanationUnfinalized(StatWorker __instance, StatRequest req, ToStringNumberSense numberSense, ref string __result)
        {
            if (__instance != null)
            {
                StatDef value = Traverse.Create(__instance).Field("stat").GetValue<StatDef>();
                if (req != null && req.Thing != null && req.Def != null && (req.Def == YautjaDefOf.RRY_Gun_Hunting_Bow || req.Def == YautjaDefOf.RRY_Gun_Compound_Bow) && value == StatDefOf.RangedWeapon_DamageMultiplier)
                {
                    DamageArmorCategoryDef CategoryOfDamage = ((ThingDef)req.Def).Verbs[0].defaultProjectile.projectile.damageDef.armorCategory;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(__instance.GetExplanationUnfinalized(req, numberSense));
                    stringBuilder.AppendLine();
                    ThingDef def = (ThingDef)req.Def;
                    if (req.StuffDef != null)
                    {
                        StatDef statDef = null;
                        if (CategoryOfDamage != null)
                        {
                            statDef = CategoryOfDamage.multStat;
                        }
                        if (statDef != null)
                        {
                            stringBuilder.AppendLine(req.StuffDef.LabelCap + ": x" + req.StuffDef.GetStatValueAbstract(statDef, null).ToStringPercent());
                        }
                    }
                    __result = stringBuilder.ToString();
                }
            }
            return;
        }

    }

}