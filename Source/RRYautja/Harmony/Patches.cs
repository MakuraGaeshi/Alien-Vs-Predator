using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;

namespace RRYautja
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.ogliss.rimworld.mod.rryatuja");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        

    }


    [HarmonyPatch(typeof(Pawn), "ThreatDisabled")]
    public static class Pawn_ThreatDisabledPatch
    {
        [HarmonyPostfix]
        public static void IgnoreCloak(Pawn __instance, ref bool __result, IAttackTargetSearcher disabledFor)
        {
            bool selected__instance = Find.Selector.SelectedObjects.Contains(__instance);
            Comp_Xenomorph _Xenomorph = null;
            if (disabledFor != null)
            {
                if (disabledFor.Thing != null)
                {
                    _Xenomorph = disabledFor.Thing.TryGetComp<Comp_Xenomorph>();
                    if (_Xenomorph != null)
                    {
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

    [HarmonyPatch(typeof(Building_Turret_Shoulder), "ThreatDisabled")]
    public static class Building_Turret_Shoulder_ThreatDisabledPatch
    {
        [HarmonyPostfix]
        public static void IgnoreShoulderTurret(Building_Turret_Shoulder __instance, ref bool __result, IAttackTargetSearcher disabledFor)
        {
            bool selected__instance = Find.Selector.SelectedObjects.Contains(__instance);
            bool shouldturret = false;
            if (__instance != null)
            {
                if (__instance is Building_Turret_Shoulder)
                {
                    shouldturret = true;
                }
            }
            __result = (__result || shouldturret);

        }
    }

    [HarmonyPatch(typeof(Apparel), "GetWornGizmos")]
    public static class Ogliss_RimWorld_Apparel_GetWornGizmos
    {
        [HarmonyPostfix]
        public static void ApparelGizmosFromComps(Apparel __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance == null)
            {
            //    Log.Warning("ApparelGizmosFromComps cannot access Apparel.");
                return;
            }
            if (__result == null)
            {
            //    Log.Warning("ApparelGizmosFromComps creating new list.");
                return;
            }

            // Find all comps on the apparel. If any have gizmos, add them to the result returned from apparel already (typically empty set).
            List<Gizmo> l = new List<Gizmo>(__result);
            foreach (CompWearable comp in __instance.GetComps<CompWearable>())
            {
                foreach (Gizmo gizmo in comp.CompGetGizmosWorn())
                {
                    l.Add(gizmo);
                }
            }
            __result = l;
        }
    }



    [HarmonyPatch(typeof(Cloakgen), "GetWornGizmos")]
    public static class Ogliss_RimWorld_Cloakgen_GetWornGizmos
    {
        [HarmonyPostfix]
        public static void ApparelGizmosFromComps(Cloakgen __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance == null)
            {
            //    Log.Warning("ApparelGizmosFromComps cannot access Apparel.");
                return;
            }
            if (__result == null)
            {
            //    Log.Warning("ApparelGizmosFromComps creating new list.");
                return;
            }

            // Find all comps on the apparel. If any have gizmos, add them to the result returned from apparel already (typically empty set).
            List<Gizmo> l2 = new List<Gizmo>(__result);
            foreach (CompWearable comp in __instance.GetComps<CompWearable>())
            {
                foreach (Gizmo gizmo in comp.CompGetGizmosWorn())
                {
                    l2.Add(gizmo);
                }
            }
            __result = l2;
        }
    }

    public abstract class CompWearable : ThingComp
    {
        public virtual IEnumerable<Gizmo> CompGetGizmosWorn() {
            // return no Gizmos
            return new List<Gizmo>();
        }
    }
    /*
   [HarmonyPatch(typeof(Pawn), "CheckAcceptArrest")]
   public static class Pawn_AcceptArrestPatch
   {
       [HarmonyPrefix]
       public static bool RevealSaboteur(Pawn __instance, Pawn arrester)
       {
           if (__instance.health.hediffSet.HasHediff(HediffDefOfIncidents.Saboteur))
           {
               __instance.health.hediffSet.hediffs.RemoveAll(h => h.def == HediffDefOfIncidents.Saboteur);
               Faction faction = Find.FactionManager.RandomEnemyFaction();
               __instance.SetFaction(faction);
               List<Pawn> thisPawn = new List<Pawn>();
               thisPawn.Add(__instance);
               IncidentParms parms = new IncidentParms();
               parms.faction = faction;
               parms.spawnCenter = __instance.Position;
               parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
               parms.raidStrategy.Worker.MakeLords(parms, thisPawn);
               __instance.Map.avoidGrid.Regenerate();
               LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
               if (faction != null)
               {
                   Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(), "SaboteurRevealedFaction".Translate(__instance.LabelShort, faction.Name, __instance.Named("PAWN")), LetterDefOf.ThreatBig, __instance, null);
               }
               else
               {
                   Find.LetterStack.ReceiveLetter("LetterLabelSabotage".Translate(), "SaboteurRevealed".Translate(__instance.LabelShort, __instance.Named("PAWN")), LetterDefOf.ThreatBig, __instance, null);
               }
           }
           return true;
       }
   }
   */

    // Token: 0x020000A1 RID: 161
    [HarmonyPatch(typeof(StatWorker), "GetExplanationUnfinalized")]
    public static class StatWorker_GetExplanationUnfinalized
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

    // Token: 0x020000A2 RID: 162
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public static class StatWorker_GetValueUnfinalized
    {
        [HarmonyPostfix]
        public static void GetValueUnfinalized(StatWorker __instance, StatRequest req, ref float __result)
        {
            if (__instance != null)
            {
                StatDef value = Traverse.Create(__instance).Field("stat").GetValue<StatDef>();
                if (req != null && req.Thing != null && req.Def != null && (req.Def == YautjaDefOf.RRY_Gun_Hunting_Bow || req.Def == YautjaDefOf.RRY_Gun_Compound_Bow) && value == StatDefOf.RangedWeapon_DamageMultiplier)
                {
                    Log.Message(string.Format("GetValueUnfinalized value: {0}, Def: {1}, Empty: {2}, HasThing: {3}, QualityCategory: {4}, StuffDef: {5}, Thing: {6}", value, req.Def, req.Empty, req.HasThing, req.QualityCategory, req.StuffDef, req.Thing));
                    Log.Message(string.Format("GetValueUnfinalized Original __result: {0}", __result));

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

                    Log.Message(string.Format("GetValueUnfinalized Modified __result: {0}", __result));
                }
            }
            return;
        }
    }

    // Token: 0x02000088 RID: 136
    [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
    internal static class RestUtility_Bed_IsValidBedFor
    {
        [HarmonyPostfix]
        public static void Postfix(Thing bedThing, Pawn sleeper, Pawn traveler, ref bool __result)
        {
            bool flag = bedThing is Building_XenomorphCocoon;
            bool flag2 = traveler != null ? traveler.kindDef.race.defName.Contains("RRY_Xenomorph") : false ;
            bool flag3 = XenomorphUtil.isInfectablePawn(sleeper);
            __result = __result&&!flag || (__result && flag && flag2);
            Log.Message(string.Format("RestUtility_Bed_IsValidBedFor sleeper: {0} traveler: {1} result: {2} = !flag: {3} && flag2: {4}", sleeper, traveler, __result, !flag , flag2));
            return;
        }
    }

    /*
    // Token: 0x02000086 RID: 134
    [HarmonyPatch(typeof(Building_Bed), "GetSleepingSlotPos")]
    internal static class Building_Bed_GetSleepingSlotPos
    {
        // Token: 0x060001EF RID: 495 RVA: 0x0000E0A8 File Offset: 0x0000C2A8
        private static void Postfix(Building_Bed __instance, ref IntVec3 __result)
        {
            bool flag = __instance is Building_XenomorphCocoon;
            bool selected = Find.Selector.SelectedObjects.Contains(__instance);
            if (selected) Log.Message(string.Format("Building_Bed_GetSleepingSlotPos 1 Old Drawloc {0}", __result));
            if (flag)
            {


                if (selected) Log.Message(string.Format("Building_Bed_GetSleepingSlotPos 2 Old Drawloc {0}", __result));
                IntVec3 bedCenter = __instance.Position;
                Rot4 bedRot = __instance.Rotation;
                IntVec2 bedSize = __instance.def.size;
                CellRect cellRect = GenAdj.OccupiedRect(bedCenter, bedRot, bedSize);
                if (bedRot == Rot4.North)
                {
                    __result = new IntVec3(cellRect.minX, bedCenter.y, cellRect.minZ);
                }
                else if (bedRot == Rot4.East)
                {
                    __result = new IntVec3(cellRect.minX, bedCenter.y, cellRect.maxZ);
                }
                else if (bedRot == Rot4.South)
                {
                    __result = new IntVec3(cellRect.minX, bedCenter.y, cellRect.maxZ);
                }
                else __result = new IntVec3(cellRect.maxX, bedCenter.y, cellRect.maxZ);
                if (selected) Log.Message(string.Format("Building_Bed_GetSleepingSlotPos 3 new Drawloc {0}", __result));

                
                

                if (__instance.Rotation == Rot4.North)
                {
                    __result = __instance.Position;
                }
                else if (__instance.Rotation == Rot4.North)
                {
                    __result = __instance.Position;
                }
                else if (__instance.Rotation == Rot4.North)
                {
                    __result = __instance.Position;
                }
                else if (__instance.Rotation == Rot4.North)
                {
                    __result = __instance.Position;
                }
                else __result = __instance.Position;

                
            }
        }
    }
    */
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecute")]
    public static class IncidentWorker_RaidEnemyPatch
    {
        // Token: 0x06000017 RID: 23 RVA: 0x00002CD0 File Offset: 0x00000ED0
        [HarmonyPostfix]
        public static void PopSaboteurs(bool __result, IncidentParms parms)
        {
            if (__result && parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction.def.basicMemberKind.race == YautjaDefOf.RRY_Alien_Yautja)
                {
                    Log.Message(string.Format("Yautja raid spawning"));
                }
            }
        }
    }
}