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
    // Stuffable Projectile StatWorker patch
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public static class AvP_StatWorker_GetValueUnfinalized_Patch
    {
        [HarmonyPostfix]
        public static void GetValueUnfinalized(StatWorker __instance, StatRequest req, ref float __result)
        {
            if (__instance != null)
            {

                StatDef value = Traverse.Create(__instance).Field("stat").GetValue<StatDef>();
                if (req != null && req.Thing != null && req.Def != null && (req.Def == YautjaDefOf.RRY_Gun_Hunting_Bow || req.Def == YautjaDefOf.RRY_Gun_Compound_Bow) && value == StatDefOf.RangedWeapon_DamageMultiplier)
                {
                    //    Log.Message(string.Format("GetValueUnfinalized value: {0}, Def: {1}, Empty: {2}, HasThing: {3}, QualityCategory: {4}, StuffDef: {5}, Thing: {6}", value, req.Def, req.Empty, req.HasThing, req.QualityCategory, req.StuffDef, req.Thing));
                    //    Log.Message(string.Format("GetValueUnfinalized Original __result: {0}", __result));

                    DamageArmorCategoryDef CategoryOfDamage = ((ThingDef)req.Def).Verbs[0].defaultProjectile.projectile.damageDef.armorCategory;

                    float num = __result;
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
                            num *= req.StuffDef.GetStatValueAbstract(statDef, null);
                        }
                        __result = num;
                    }

                    //    Log.Message(string.Format("GetValueUnfinalized Modified __result: {0}", __result));
                }
            }
            return;
        }
    }

}