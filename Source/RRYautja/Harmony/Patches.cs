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

    [HarmonyPatch(typeof(FeedPatientUtility), "ShouldBeFed")]
    public static class FeedPatientUtility_ShouldBeFedPatch
    {
        [HarmonyPostfix]
        public static void IgnoreCocooned(Pawn p, ref bool __result)
        {
            __result = __result && !(p.InBed() && p.CurrentBed() is Building_XenomorphCocoon);
        }
    }

    [HarmonyPatch(typeof(HediffGiver_Hypothermia), "OnIntervalPassed")]
    public static class HediffGiver_Hypothermia_OnIntervalPassed_Patch
    {
        [HarmonyPrefix]
        public static bool OnIntervalPassedPrefix(Pawn pawn, Hediff cause)
        {
            if (pawn.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph)
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
                    else if (pawn.RaceProps.FleshType != XenomorphRacesDefOf.RRY_Xenomorph && ambientTemperature < 0f && firstHediffOfDef.Severity > 0.37f)
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

    [HarmonyPatch(typeof(JobGiver_WanderColony), "GetWanderRoot")]
    public static class JobGiver_WanderColony_GetWanderRootPatch
    {
        [HarmonyPostfix]
        public static void GetWanderRoot(Pawn pawn, ref IntVec3 __result)
        {
            if (!__result.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon).DestroyedOrNull())
            {
                __result = pawn.Position;
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "AnythingToStrip")]
    public static class Pawn_AnythingToStripPatch
    {
        [HarmonyPostfix]
        public static void IgnoreWristblade(Pawn __instance, ref bool __result)
        {
            __result = !(__instance.apparel != null && __instance.apparel.WornApparelCount == 1 && __instance.apparel.WornApparel.Any(x => x.def == YautjaDefOf.RRY_Equipment_HunterGauntlet) && __instance.Faction != Faction.OfPlayerSilentFail);

        }
    }

    [HarmonyPatch(typeof(WorkGiver_Tend), "GoodLayingStatusForTend")]
    public static class WorkGiver_Tend_GoodLayingStatusForTend_Patch
    {
        [HarmonyPostfix]
        public static void PawnInCocoon(WorkGiver_Tend __instance, Pawn patient, Pawn doctor, ref bool __result)
        {
            __result = __result && (!patient.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned)&&!(patient.CurrentBed() is Building_XenomorphCocoon));
        //    Log.Message(string.Format("WorkGiver_Tend_GoodLayingStatusForTend_Patch patient: {0}, doctor: {1}, __Result: {2}", patient, doctor, __result));

        }
    }

    [HarmonyPatch(typeof(Pawn_NeedsTracker), "NeedsTrackerTick", null)]
    public static class Pawn_NeedsTracker_Patch
    {
        // Token: 0x06000F66 RID: 3942 RVA: 0x000CB228 File Offset: 0x000C9428
        public static bool Prefix(Pawn_NeedsTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = (Pawn)Pawn_NeedsTracker_Patch.pawn.GetValue(__instance);
            bool flag = pawn != null;
            if (flag)
            {
                bool flag2 = pawn.InBed() && pawn.CurrentBed() is Building_XenomorphCocoon;
                if (flag2)
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x04001015 RID: 4117
        public static FieldInfo pawn = typeof(Pawn_NeedsTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }


    [HarmonyPatch(typeof(Pawn), "Strip")]
    public static class Pawn_StripPatch
    {
        [HarmonyPrefix]
        public static bool IgnoreWristblade(Pawn __instance)
        {

            bool result = true;
            if (__instance.RaceProps.Humanlike)
            {
                result = !(__instance.apparel.WornApparel.Any(x => x.def == YautjaDefOf.RRY_Equipment_HunterGauntlet));
            }
        //    Log.Message(string.Format("Pawn_StripPatch IgnoreWristblade: {0}", result));
            if (!result)
            {

                Caravan caravan = __instance.GetCaravan();
                if (caravan != null)
                {
                    CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(__instance, caravan.PawnsListForReading, null);
                    if (__instance.apparel != null)
                    {
                        CaravanInventoryUtility.MoveAllApparelToSomeonesInventory(__instance, caravan.PawnsListForReading);
                    }
                    if (__instance.equipment != null)
                    {
                        CaravanInventoryUtility.MoveAllEquipmentToSomeonesInventory(__instance, caravan.PawnsListForReading);
                    }
                }
                else
                {
                    IntVec3 pos = (__instance.Corpse == null) ? __instance.PositionHeld : __instance.Corpse.PositionHeld;
                    if (__instance.equipment != null)
                    {
                        __instance.equipment.DropAllEquipment(pos, false);
                    }
                    if (__instance.apparel != null)
                    {
                        DropAll(__instance, pos, false);
                    }
                    if (__instance.inventory != null)
                    {
                        __instance.inventory.DropAllNearPawn(pos, false, false);
                    }
                }
            }
            return result;
        }
        
		// Token: 0x04000E58 RID: 3672
		private static List<Apparel> tmpApparelList = new List<Apparel>();
        
        public static void DropAll(Pawn __instance, IntVec3 pos, bool forbid = true)
        {
            tmpApparelList.Clear();
            for (int i = 0; i < __instance.apparel.WornApparel.Count; i++)
            {
                if (__instance.apparel.WornApparel[i].def != YautjaDefOf.RRY_Equipment_HunterGauntlet)
                {
                    tmpApparelList.Add(__instance.apparel.WornApparel[i]);
                }
                else
                {
                //    Log.Message(string.Format("Ignoring Wristblade"));
                }
            }
            for (int j = 0; j < tmpApparelList.Count; j++)
            {
                __instance.apparel.TryDrop(tmpApparelList[j], out Apparel apparel, pos, forbid);
            }
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
        //    Log.Message(string.Format("RestUtility_Bed_IsValidBedFor sleeper: {0} traveler: {1} result: {2} = !flag: {3} && flag2: {4}", sleeper, traveler, __result, !flag , flag2));
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
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool) })]
    static class Pawn_DrawTracker_get_DrawPos
    {
        static void Prefix(PawnRenderer __instance, ref Vector3 rootLoc, ref bool portrait)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (!portrait)
            {
                if (pawn.CurrentBed() != null && pawn.CurrentBed() is Building_XenomorphCocoon)
                {
                    //rootLoc.z += 1f;
                    //rootLoc.x += 1f;
                    if (pawn.CurrentBed().Rotation == Rot4.North)
                    {
                        //rootLoc.x += 0.5f;
                        rootLoc.z -= 0.5f;
                    }
                    else if (pawn.CurrentBed().Rotation == Rot4.South)
                    {
                        //rootLoc.x += 0.5f;
                        rootLoc.z += 0.5f;
                    }
                    else if (pawn.CurrentBed().Rotation == Rot4.East)
                    {
                        rootLoc.x -= 0.5f;
                        //rootLoc.z += 0.5f;
                    }
                    else if (pawn.CurrentBed().Rotation == Rot4.West)
                    {
                        rootLoc.x += 0.5f;
                        //rootLoc.z += 0.5f;
                    }
                    else rootLoc = pawn.CurrentBed().DrawPos;
                }
            }
        }

        static void Postfix(PawnRenderer __instance, ref Vector3 rootLoc)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.RaceProps.Humanlike || pawn.kindDef.race.GetModExtension<FacehuggerOffsetDefExtension>()!=null)
            {
                foreach (var hd in pawn.health.hediffSet.hediffs)
                {
                    HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                    if (comp != null)
                    {
                        comp.DrawImplant(rootLoc);
                    }
                }
            } // DrawWornExtras()
            else
            {
                foreach (var hd in pawn.health.hediffSet.hediffs)
                {
                    HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                    if (comp != null)
                    {
                        comp.DrawWornExtras();
                    }
                }
            }
        }

    }
    
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecute")]
    public static class IncidentWorker_RaidEnemyPatch_TryExecute
    {
        // Token: 0x06000017 RID: 23 RVA: 0x00002CD0 File Offset: 0x00000ED0
        [HarmonyPrefix]
        public static bool PreExecute(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && ((parms.faction.leader != null && parms.faction.leader.kindDef.race == YautjaDefOf.RRY_Alien_Yautja) || (parms.faction.def.basicMemberKind != null && parms.faction.def.basicMemberKind.race == YautjaDefOf.RRY_Alien_Yautja)))
                {
                    Log.Message(string.Format("PreExecute Yautja Raid"));

                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {
                        Log.Message(string.Format("PreExecute During Heatwave, originally {0} points", parms.points));
                        parms.points *= 2;
                        parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;

                        Log.Message(string.Format("PreExecute During Heatwave, modified {0} points", parms.points));
                    }
                }
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
                    Log.Message(string.Format("PreExecute Xenomorph Raid CurSkyGlow: {0}", (parms.target as Map).skyManager.CurSkyGlow));

                    if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                    {
                        Log.Message(string.Format("PreExecute During Nighttime, originally {0} points", parms.points));
                        parms.points *= 2;
                        parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;

                        Log.Message(string.Format("PreExecute During Nighttime, modified {0} points", parms.points));
                    }
                }
            }
            return true;
        }

        /*
        [HarmonyPostfix]
        public static void PostExecute(bool __result, ref IncidentParms parms)
        {
            if (__result && parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && parms.faction.leader.kindDef.race == YautjaDefOf.RRY_Alien_Yautja)
                {

                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {

                    }
                }
            }
        }
        */
    }


    /*
    [HarmonyPatch(typeof(Pawn), "Tick")]
    public static class Pawn_TickPatch
    {
        [HarmonyPostfix]
        public static void ApparelCompTick(Pawn __instance)
        {
            if (__instance.apparel.WornApparelCount>0)
            {
                List<Apparel> list = __instance.apparel.WornApparel;
                if (list.Any(x => x.TryGetComp<CompWearable>()!=null))
                {
                    foreach (var item in list.All(x => x.TryGetComp<CompWearable>() != null))
                    {

                    }
                }
            }

        }
        
    }
    */

    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "GetLetterText")]
    public static class IncidentWorker_RaidEnemyPatch_GetLetterText
    {
        [HarmonyPostfix]
        public static void PostExecute(ref string __result, ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && ((parms.faction.leader != null && parms.faction.leader.kindDef.race == YautjaDefOf.RRY_Alien_Yautja) || (parms.faction.def.basicMemberKind != null && parms.faction.def.basicMemberKind.race == YautjaDefOf.RRY_Alien_Yautja)))
                {
                    Log.Message(string.Format("PostGetLetterText Yautja Raid"));

                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {
                        string text = "El Diablo, cazador de hombre. Only in the hottest years this happens. And this year it grows hot.";
                        text += "\n\n";
                        text += __result;
                        __result = text;
                    }
                }
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
                    Log.Message(string.Format("PostGetLetterText Xenomorph Raid CurSkyGlow: {0}", (parms.target as Map).skyManager.CurSkyGlow));

                    if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                    {
                        string text = "They mostly come at night......mostly.....";
                        text += "\n\n";
                        text += __result;
                        __result = text;
                    }
                }
            }
        }
    }

    /*
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(IncidentWorker_WandererJoin), "TryExecute")]
    public static class IncidentWorker_WandererJoinPatch_TryExecute
    {
        public static string stranger = "StrangerInBlack";
        [HarmonyPrefix]
        public static bool PreExecute(IncidentWorker_WandererJoin __instance, ref IncidentParms parms ,bool __result)
        { // request parms.faction.def

            Log.Message(string.Format("Original race: {0}", __instance.def.pawnKind.race));
            Log.Message(string.Format("Original faction: {0}", parms.faction.def));






            return true;
        }
    }
    */


    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(HediffComp_Infecter), "CheckMakeInfection")]
    public static class HediffComp_Infecter_Patch_CheckMakeInfection
    {
        [HarmonyPrefix]
        public static bool preCheckMakeInfection(HediffComp_Infecter __instance)
        {
            Log.Message(string.Format("trying to add an infection to {0}'s wounded {1}", __instance.Pawn, __instance.parent.Part));
            if (__instance.Pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned)|| (__instance.Pawn.InBed() && __instance.Pawn.CurrentBed() is Building_XenomorphCocoon))
            {
                Log.Message(string.Format("{0} protected from infection", __instance.Pawn));
                return false;
            }
            return true;
        }
    }


}