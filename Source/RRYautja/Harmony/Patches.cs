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
            //    HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create("com.ogliss.rimworld.mod.rryatuja");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        

    }

    [HarmonyPatch(typeof(RimWorld.PawnUtility), "GetManhunterOnDamageChance", new Type[] { typeof(Pawn), typeof(Thing) }), StaticConstructorOnStartup]
    public static class PawnUtility_GetManhunterOnDamageChance_Patch
    {
        [HarmonyPostfix]
        public static void GetManhunterOnDamageChancePostfix(Pawn pawn, Thing instigator, ref float __result)
        {
            if (instigator!=null)
            {
                __result = XenomorphUtil.IsXenomorphPawn(((Pawn)instigator)) ? 0.0f : __result;
           //     Log.Message(string.Format("__result: {0}", __result));
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "ThreatDisabled")]
    public static class Pawn_ThreatDisabledPatch
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

    [HarmonyPatch(typeof(VerbUtility), "HarmsHealth")]
    public static class VerbUtility_HarmsHealth_Patch
    {
        [HarmonyPostfix]
        public static void HarmsHealthPostfix(Verb verb, ref bool __result)
        {
            __result = __result || verb is Verb_Launch_Stuffable_Projectile || verb is Verb_Shoot_Stuffable;
        }
    }
    
    /*
    // Token: 0x0200007A RID: 122
    [HarmonyPatch(typeof(PawnUtility), "GetManhunterOnDamageChance")]
    internal class PawnUtility_GetManhunterOnDamageChance_Patch
    {
        // Token: 0x060001D9 RID: 473 RVA: 0x0000DB80 File Offset: 0x0000BD80
        private static bool Prefix(Pawn p, ref float __result)
        {
            return true;
        }

        // Token: 0x060001DA RID: 474 RVA: 0x0000DBFC File Offset: 0x0000BDFC
        private static void Postfix(Pawn p, ref float __result)
        {

        }
    }
    */

    [HarmonyPatch(typeof(FeedPatientUtility), "ShouldBeFed")]
    public static class FeedPatientUtility_ShouldBeFed_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreCocooned(Pawn p, ref bool __result)
        {
            __result = __result && !(p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));
        }
    }

    /*
    [HarmonyPatch(typeof(Building_XenoEgg), "get_DefaultGraphic")]
    public static class Building_XenoEgg_get_DefaultGraphic_Patch
    {
        
        [HarmonyPostfix]
        public static void RoyalEggSize(Thing __instance, ref Graphic __result)
        {
            Graphic value = Traverse.Create(__instance).Field("graphicInt").GetValue<Graphic>();
            bool flag = value != null;
            if (flag)
            {
                if (__instance is Building_XenoEgg)
                {
                //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nis Xeno Egg"));
                    CompXenoHatcher xenoHatcher = __instance.TryGetComp<CompXenoHatcher>();
                    if (xenoHatcher!=null && xenoHatcher.royalProgress>0f)
                    {
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nFound CompXenoHatcher"));
                        float num = (0.7f * xenoHatcher.royalProgress);
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nnum : {0}", num));
                        num += __instance.def.graphicData.drawSize.x;
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nnum : {0}", num));
                        value = __result.GetCopy(new Vector2((num), (num)));
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\value.drawSize : {0}", value.drawSize));
                        __result = value;
                        
                    }
                }
            }
        }
    }
    */

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
            __result = __result && !(__instance.apparel != null && __instance.apparel.WornApparelCount == 1 && __instance.apparel.WornApparel.Any(x => x.def == YautjaDefOf.RRY_Equipment_HunterGauntlet) && __instance.Faction != Faction.OfPlayerSilentFail) && !(__instance.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));

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
        public static bool Prefix(Pawn_NeedsTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = (Pawn)Pawn_NeedsTracker_Patch.pawn.GetValue(__instance);
            bool flag = pawn != null;
            if (flag)
            {
                bool flag2 = (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));
                bool flag3 = (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection) && Find.TickManager.TicksGame % 5 != 0);
                if (flag2 || flag3)
                {
                    return false;
                }
            }
            return true;
        }
        
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
                result = !(__instance.apparel.WornApparel.Any(x => x.def == YautjaDefOf.RRY_Equipment_HunterGauntlet) &&!__instance.Dead);
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

    [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
    internal static class RestUtility_Bed_IsValidBedFor
    {
        [HarmonyPostfix]
        public static void Postfix(Thing bedThing, Pawn sleeper, Pawn traveler, ref bool __result)
        {
            bool flag = bedThing is Building_XenomorphCocoon;
            bool flag2 = traveler != null ? traveler.kindDef.race.defName.Contains("RRY_Xenomorph") : false;
            bool flag3 = XenomorphUtil.isInfectablePawn(sleeper);
            __result = __result && !flag || (__result && flag && flag2);
            //    Log.Message(string.Format("RestUtility_Bed_IsValidBedFor sleeper: {0} traveler: {1} result: {2} = !flag: {3} && flag2: {4}", sleeper, traveler, __result, !flag , flag2));
            return;
        }
    }

    [HarmonyPatch(typeof(ShieldBelt), "AllowVerbCast")]
    internal static class ShieldBelt_AllowVerbCast_YautjaWeapons
    {
        [HarmonyPostfix]
        public static void Postfix(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb, ref bool __result)
        {
            bool flag = verb is Verb_Launch_Stuffable_Projectile;
            __result = __result && !flag;
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
        static void Prefix(PawnRenderer __instance, ref Vector3 rootLoc, ref float angle, ref bool renderBody, ref Rot4 bodyFacing, ref Rot4 headFacing, ref RotDrawMode bodyDrawType, ref bool portrait, ref bool headStump)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        //    bool selected = Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode;
            if (pawn.Spawned)
            {
                if (pawn.RaceProps.Humanlike && pawn.CurrentBed() != null && pawn.CurrentBed() is Building_XenomorphCocoon)
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
                bool pawnflag = !((pawn.kindDef.race.defName.StartsWith("Android") && pawn.kindDef.race.defName.Contains("Tier")) || pawn.kindDef.race.defName.Contains("ChjDroid") || pawn.kindDef.race.defName.Contains("ChjBattleDroid") || pawn.kindDef.race.defName.Contains("M7Mech")); 
                if ((pawn.RaceProps.Humanlike && pawnflag) || pawn.kindDef.race.GetModExtension<OffsetDefExtension>() != null && pawn.Map!=null)
                {
                    foreach (var hd in pawn.health.hediffSet.hediffs)
                    {
                        HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                        if (comp != null)
                        {
                            DrawImplant(comp, __instance, rootLoc, angle, renderBody, bodyFacing, headFacing, bodyDrawType, portrait, headStump);
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
		
        static void DrawImplant(HediffComp_DrawImplant comp, PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump)
        {// this.Pawn

            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool selected = Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode;
            string direction = "";
            float offset = 0f;
            Rot4 rot = bodyFacing;
            Vector3 vector3 = pawn.RaceProps.Humanlike ? __instance.BaseHeadOffsetAt(headFacing) : new Vector3();
            Vector3 s = new Vector3(pawn.BodySize*1.75f,pawn.BodySize*1.75f,pawn.BodySize*1.75f);
            bool OffsetDefExtension = (pawn.def.modExtensions.NullOrEmpty()||(!pawn.def.modExtensions.NullOrEmpty() && pawn.def.modExtensions.Any((x) => comp.parent.def.defName.Contains(((OffsetDefExtension)x).hediff.defName))) || ThingDefOf.Human.modExtensions.Any((x) => comp.parent.def.defName.Contains(((OffsetDefExtension)x).hediff.defName)));
            if (OffsetDefExtension)// && pawn.kindDef.race.GetModExtension<OffsetDefExtension>() is OffsetDefExtension offsetDef && comp.parent.def.defName.Contains(offsetDef.hediff.defName))
            {
                GetAltitudeOffset(pawn, comp.parent, rot, out float X, out float Y, out float Z, out float DsX, out float DsZ, out float ang);
                vector3.x += X;
                vector3.y += Y;
                vector3.z += Z;
                angle += ang;
                s.x = DsX;
                s.z = DsZ;
                
            }
            if (pawn.RaceProps.Humanlike)
            {
                vector3.x += 0.01f;
                vector3.z += -0.35f;
            }

            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 b = quaternion * vector3;
            Vector3 vector = rootLoc;
            Vector3 a = rootLoc;
            if (bodyFacing != Rot4.North)
            {
                a.y += 0.02734375f;
                vector.y += 0.0234375f;
            }
            else
            {
                a.y += 0.0234375f;
                vector.y += 0.02734375f;
            }
            /*
            Material material = __instance.graphics.HeadMatAt(headFacing, bodyDrawType, headStump);
            if (material != null)
            {
                Mesh mesh2 = MeshPool.humanlikeHeadSet.MeshAt(headFacing);
                GenDraw.DrawMeshNowOrLater(mesh2, a + b, quaternion, material, portrait);
            }
            */
            Vector3 loc2 = rootLoc + b;
            loc2.y += 0.03105f;
            bool flag = false;
            /*
            if (!portrait || !Prefs.HatsOnlyOnMap)
            {
                Mesh mesh3 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
                List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
                for (int j = 0; j < apparelGraphics.Count; j++)
                {
                    if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
                    {
                        if (!apparelGraphics[j].sourceApparel.def.apparel.hatRenderedFrontOfFace)
                        {
                            flag = true;
                            Material material2 = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
                            material2 = __instance.graphics.flasher.GetDamagedMat(material2);
                            GenDraw.DrawMeshNowOrLater(mesh3, loc2, quaternion, material2, portrait);
                        }
                        else
                        {
                            Material material3 = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
                            material3 = __instance.graphics.flasher.GetDamagedMat(material3);
                            Vector3 loc3 = rootLoc + b;
                            loc3.y += ((!(bodyFacing == Rot4.North)) ? 0.03515625f : 0.00390625f);
                            GenDraw.DrawMeshNowOrLater(mesh3, loc3, quaternion, material3, portrait);
                        }
                    }
                }
            }
            */
            if (!flag && bodyDrawType != RotDrawMode.Dessicated)
            {
#if DEBUG
                if (selected)
                {
                //    Log.Message(string.Format("{0}'s rootLoc: {1}, angle: {2}, renderBody: {3}, bodyFacing: {4}, headFacing: {5}, bodyDrawType: {6}, portrait: {7}", pawn.Label, rootLoc, angle, renderBody, bodyFacing.ToStringHuman(), headFacing.ToStringHuman(), bodyDrawType, portrait));
                }
#endif
                //    Mesh mesh4 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
                Material mat = comp.ImplantMaterial(pawn, pawn.RaceProps.Humanlike ? headFacing : bodyFacing);
            //    GenDraw.DrawMeshNowOrLater(headFacing == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, loc2, quaternion, mat, true);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(loc2, quaternion, s);
                Graphics.DrawMesh((pawn.RaceProps.Humanlike ? headFacing : bodyFacing) == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, mat, 0);
            }

            /*
            Material matSingle = comp.ImplantMaterial(pawn, rot);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(rot == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, matSingle, 0);
            */
        }
		
		static void GetAltitudeOffset(Pawn pawn, Hediff hediff, Rot4 rotation, out float OffsetX, out float OffsetY, out float OffsetZ, out float DrawSizeX, out float DrawSizeZ, out float ang)
        {
            OffsetDefExtension myDef = null;
            if (!pawn.def.modExtensions.NullOrEmpty())
            {
                myDef = (OffsetDefExtension)pawn.kindDef.race.modExtensions.Find((x) => hediff.def.defName.Contains(((OffsetDefExtension)x).hediff.defName)) ?? (OffsetDefExtension)ThingDefOf.Human.modExtensions.Find((x) => hediff.def.defName.Contains(((OffsetDefExtension)x).hediff.defName)) ?? new OffsetDefExtension();
            }
            else if (myDef==null)
            {
                myDef = (OffsetDefExtension)ThingDefOf.Human.modExtensions.Find((x) => hediff.def.defName.Contains(((OffsetDefExtension)x).hediff.defName)) ?? new OffsetDefExtension();
            }
            else
            {
                myDef =  new OffsetDefExtension() {hediff =  hediff.def};

            }
            

            if (myDef.hediff!=null)
            {
            //    Log.Message(string.Format("{0}'s drawdata for hediff {1} OffsetDefExtension.hediff {2}", pawn.LabelShortCap, hediff.LabelCap, myDef.hediff.label));
            }
            string direction;
            if (pawn.RaceProps.Humanlike)
            {
                if (rotation == Rot4.North)
                {
                    OffsetX = myDef.NorthXOffset;
                    OffsetY = myDef.NorthYOffset;
                    OffsetZ = myDef.NorthZOffset;
                    DrawSizeX = myDef.NorthXDrawSize;
                    DrawSizeZ = myDef.NorthZDrawSize;
                    ang = myDef.NorthAngle;
                    direction = "North";
                }
                else if (rotation == Rot4.West)
                {
                    OffsetX = myDef.WestXOffset;
                    OffsetY = myDef.WestYOffset;
                    OffsetZ = myDef.WestZOffset;
                    DrawSizeX = myDef.WestXDrawSize;
                    DrawSizeZ = myDef.WestZDrawSize;
                    ang = myDef.WestAngle;
                    direction = "West";
                }
                else if (rotation == Rot4.East)
                {
                    OffsetX = myDef.EastXOffset;
                    OffsetY = myDef.EastYOffset;
                    OffsetZ = myDef.EastZOffset;
                    DrawSizeX = myDef.EastXDrawSize;
                    DrawSizeZ = myDef.EastZDrawSize;
                    ang = myDef.EastAngle;
                    direction = "East";
                }
                else if (rotation == Rot4.South)
                {
                    OffsetX = myDef.SouthXOffset;
                    OffsetY = myDef.SouthYOffset;
                    OffsetZ = myDef.SouthZOffset;
                    DrawSizeX = myDef.SouthXDrawSize;
                    DrawSizeZ = myDef.SouthZDrawSize;
                    ang = myDef.SouthAngle;
                    direction = "South";
                }
                else
                {
                    OffsetX = 0f;
                    OffsetY = 0f;
                    OffsetZ = 0f;
                    DrawSizeX = 1f;
                    DrawSizeZ = 1f;
                    ang = 0f;
                    direction = "Unknown";
                }
                if (myDef.ApplyBaseHeadOffset)
                {
                    OffsetX = myDef.SouthXOffset + pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).x;
                    OffsetY = myDef.SouthYOffset + pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).y;
                    OffsetZ = myDef.SouthZOffset + pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).z;
                }
            }
            else
            {
                if (rotation == Rot4.North)
                {
                    OffsetX = myDef.NorthXOffset;
                    OffsetY = myDef.NorthYOffset;
                    OffsetZ = myDef.NorthZOffset;
                    DrawSizeX = myDef.NorthXDrawSize;
                    DrawSizeZ = myDef.NorthZDrawSize;
                    ang = myDef.NorthAngle;
                    direction = "North";
                }
                else if (rotation == Rot4.West)
                {
                    OffsetX = myDef.WestXOffset;
                    OffsetY = myDef.WestYOffset;
                    OffsetZ = myDef.WestZOffset;
                    DrawSizeX = myDef.WestXDrawSize;
                    DrawSizeZ = myDef.WestZDrawSize;
                    ang = myDef.WestAngle;
                    direction = "West";
                }
                else if (rotation == Rot4.East)
                {
                    OffsetX = myDef.EastXOffset;
                    OffsetY = myDef.EastYOffset;
                    OffsetZ = myDef.EastZOffset;
                    DrawSizeX = myDef.EastXDrawSize;
                    DrawSizeZ = myDef.EastZDrawSize;
                    ang = myDef.EastAngle;
                    direction = "East";
                }
                else if (rotation == Rot4.South)
                {
                    OffsetX = myDef.SouthXOffset;
                    OffsetY = myDef.SouthYOffset;
                    OffsetZ = myDef.SouthZOffset;
                    DrawSizeX = myDef.SouthXDrawSize;
                    DrawSizeZ = myDef.SouthZDrawSize;
                    ang = myDef.SouthAngle;
                    direction = "South";
                }
                else
                {
                    OffsetX = 0f;
                    OffsetY = 0f;
                    OffsetZ = 0f;
                    DrawSizeX = 1f;
                    DrawSizeZ = 1f;
                    ang = 0f;
                    direction = "Unknown";
                }
            }

        }
		
        /*
    static void Postfix(PawnRenderer __instance, ref Vector3 rootLoc)
    {
        Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        if (pawn.RaceProps.Humanlike || pawn.kindDef.race.GetModExtension<OffsetDefExtension>()!=null)
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
        */
    }
    
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
#if DEBUG
                //    Log.Message(string.Format("PreExecute Yautja Raid"));
#endif

                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {
#if DEBUG
                    //    Log.Message(string.Format("PreExecute During Heatwave, originally {0} points", parms.points));
#endif
                        parms.points *= 2;
                        parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;

#if DEBUG
                    //    Log.Message(string.Format("PreExecute During Heatwave, modified {0} points", parms.points));
#endif
                    }
                }
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
#if DEBUG
                //    Log.Message(string.Format("PreExecute Xenomorph Raid CurSkyGlow: {0}", (parms.target as Map).skyManager.CurSkyGlow));
#endif

                    if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                    {
#if DEBUG
                    //    Log.Message(string.Format("PreExecute During Nighttime, originally {0} points", parms.points));
#endif
                        parms.points *= 2;
                        parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;

#if DEBUG
                    //    Log.Message(string.Format("PreExecute During Nighttime, modified {0} points", parms.points));
#endif
                        if (Rand.Chance(0.05f))
                        {
                            int @int = Rand.Int;
                            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, (Map)parms.target);
                            raidParms.forced = true;
                            raidParms.faction = parms.faction;
                            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                            raidParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                            raidParms.spawnCenter = parms.spawnCenter;
                            raidParms.points = Mathf.Max(raidParms.points * new FloatRange(1f, 1.6f).RandomInRange, parms.faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat));
                            raidParms.pawnGroupMakerSeed = new int?(@int);
                            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, raidParms, false);
                            defaultPawnGroupMakerParms.points = IncidentWorker_Raid.AdjustedRaidPoints(defaultPawnGroupMakerParms.points, raidParms.raidArrivalMode, raidParms.raidStrategy, defaultPawnGroupMakerParms.faction, PawnGroupKindDefOf.Combat);
                            IEnumerable<PawnKindDef> pawnKinds = PawnGroupMakerUtility.GeneratePawnKindsExample(defaultPawnGroupMakerParms);
                            QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms), Find.TickManager.TicksGame + new IntRange(1000, 4000).RandomInRange, 0);
                            Find.Storyteller.incidentQueue.Add(qi);
                        }
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
    
    [HarmonyPatch(typeof(PawnWoundDrawer), "RenderOverBody")]
    public static class PawnWoundDrawerPatch_TryExecute
    {
        // Token: 0x06000017 RID: 23 RVA: 0x00002CD0 File Offset: 0x00000ED0
        [HarmonyPrefix]
        public static bool PreExecute(PawnWoundDrawer __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool flag_Cloaked = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            bool flag_HiddenXeno = pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden, false);
            if (flag_Cloaked || flag_HiddenXeno)
            {
                return false;
            }
            return true;
        }
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
#if DEBUG
                //    Log.Message(string.Format("PostGetLetterText Yautja Raid"));
#endif

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
#if DEBUG
                //    Log.Message(string.Format("PostGetLetterText Xenomorph Raid CurSkyGlow: {0}", (parms.target as Map).skyManager.CurSkyGlow));
#endif

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

        //    Log.Message(string.Format("Original race: {0}", __instance.def.pawnKind.race));
        //    Log.Message(string.Format("Original faction: {0}", parms.faction.def));






            return true;
        }
    }
    */
    
    [HarmonyPatch(typeof(HediffComp_Infecter), "CheckMakeInfection")]
    public static class HediffComp_Infecter_Patch_CheckMakeInfection
    {
        [HarmonyPrefix]
        public static bool preCheckMakeInfection(HediffComp_Infecter __instance)
        {
#if DEBUG
        //    Log.Message(string.Format("trying to add an infection to {0}'s wounded {1}", __instance.Pawn, __instance.parent.Part));
#endif
            if (__instance.Pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned)|| (__instance.Pawn.InBed() && __instance.Pawn.CurrentBed() is Building_XenomorphCocoon))
            {
#if DEBUG
            //    Log.Message(string.Format("{0} protected from infection", __instance.Pawn));
#endif
                return false;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(Pawn), "PreApplyDamage")]
    public static class Pawn_PreApplyDamage_Patch
    {
        [HarmonyPrefix]
        public static bool Ignore_Acid_Damage(Pawn __instance, ref DamageInfo dinfo, out bool absorbed)
        {
            if (__instance.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) || XenomorphUtil.IsXenomorph(__instance))
            {
                absorbed = dinfo.Def == XenomorphDefOf.RRY_AcidBurn || dinfo.Def == XenomorphDefOf.RRY_AcidDamage;
            }
            else
            {
                absorbed = false;
            }
            if (absorbed)
            {
#if DEBUG
            //    Log.Message(string.Format("absorbed"));
#endif
            }
            return !absorbed;
        }
        
    }

    /*
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(PowerNet), "PowerNetTick")]
    public static class PowerNet_PowerNetTick_Patch
    {
        [HarmonyPrefix]
        public static bool prePowerNetTick(PowerNet __instance)
        {
            float num = __instance.CurrentEnergyGainRate();
            float num2 = __instance.CurrentStoredEnergy();
            bool active = !__instance.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
        //    Log.Message(string.Format("PowerNetTick CurrentEnergyGainRate: {0}, CurrentStoredEnergy: {1}", num, num2));
            
            return true;
        }
    }
    */
    
    [HarmonyPatch(typeof(GameConditionManager), "ConditionIsActive")]
    internal static class GameConditionManager_ConditionIsActive_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(GameConditionManager __instance, ref GameConditionDef def, ref bool __result)
        {
            if (def == GameConditionDefOf.SolarFlare)
            {
            //    Log.Message(string.Format("GameConditionManager_ConditionIsActive_Patch SolarFlare: {0}", __result));
                __result = __result || __instance.ConditionIsActive(XenomorphDefOf.RRY_Xenomorph_PowerCut);
#if DEBUG
            //    Log.Message(string.Format("GameConditionManager_ConditionIsActive_Patch Xenomorph_PowerCut: {0}", __result));
#endif
            }
        }
    }
    
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
    public static class YautjaSpecificHatPatch
    {
        // Token: 0x0600004B RID: 75 RVA: 0x0000349C File Offset: 0x0000169C
        [HarmonyPostfix]
        public static void Yautja_SpecificHatPatch(ref Apparel apparel, ref BodyTypeDef bodyType, ref ApparelGraphicRecord rec)
        {
            bool flag = bodyType == YautjaDefOf.RRYYautjaFemale || bodyType == YautjaDefOf.RRYYautjaMale;
            if (flag)
            {
                bool flag2 = apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead;
                if (flag2)
                {
                    string text = apparel.def.apparel.wornGraphicPath + "_" + bodyType.defName;
                    bool flag3 = ContentFinder<Texture2D>.Get(text + "_north", false) == null || ContentFinder<Texture2D>.Get(text + "_east", false) == null || ContentFinder<Texture2D>.Get(text + "_south", false) == null;
                    if (!flag3)
                    {
                        Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(text, ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, apparel.DrawColor);
                        rec = new ApparelGraphicRecord(graphic, apparel);
                    }
                }
                else
                {
                    bool flag4 = !GenText.NullOrEmpty(apparel.def.apparel.wornGraphicPath);
                    if (flag4)
                    {
                        string text2 = apparel.def.apparel.wornGraphicPath + "_" + bodyType.defName;
                        bool flag5 = ContentFinder<Texture2D>.Get(text2 + "_north", false) == null || ContentFinder<Texture2D>.Get(text2 + "_east", false) == null || ContentFinder<Texture2D>.Get(text2 + "_south", false) == null;
                        if (flag5)
                        {
                            text2 = apparel.def.apparel.wornGraphicPath + "_Female";
                            Graphic graphic2 = GraphicDatabase.Get<Graphic_Multi>(text2, ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, apparel.DrawColor);
                            rec = new ApparelGraphicRecord(graphic2, apparel);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
    public static class YautjaGenerateBodyTypePatch
    {
        [HarmonyPostfix]
        public static void Yautja_GenerateBodyTypePatch(ref Pawn pawn)
        {
            bool flag = pawn.def==YautjaDefOf.RRY_Alien_Yautja;
            if (flag)
            {
                pawn.story.bodyType = (pawn.gender != Gender.Female) ? YautjaDefOf.RRYYautjaMale : YautjaDefOf.RRYYautjaFemale;
            }
        }
    }
    
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecute")]
    public static class IncidentWorker_RaidEnemy_Patch_TryExecute
    {
        [HarmonyPrefix]
        public static bool PreExecute(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
                    if ((parms.target is Map))
                    {
                        if (Rand.Chance(0.05f))
                        {
                            int @int = Rand.Int;
                            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, (Map)parms.target);
                            raidParms.forced = true;
                            raidParms.faction = parms.faction;
                            raidParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                            raidParms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;
                            raidParms.spawnCenter = parms.spawnCenter;
                            raidParms.points = Mathf.Max(raidParms.points * new FloatRange(1f, 1.6f).RandomInRange, parms.faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat));
                            raidParms.pawnGroupMakerSeed = new int?(@int);
                            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, raidParms, false);
                            defaultPawnGroupMakerParms.points = IncidentWorker_Raid.AdjustedRaidPoints(defaultPawnGroupMakerParms.points, raidParms.raidArrivalMode, raidParms.raidStrategy, defaultPawnGroupMakerParms.faction, PawnGroupKindDefOf.Combat);
                            IEnumerable<PawnKindDef> pawnKinds = PawnGroupMakerUtility.GeneratePawnKindsExample(defaultPawnGroupMakerParms);
                            QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDefOf.RaidEnemy, null, raidParms), Find.TickManager.TicksGame + new IntRange(1000, 4000).RandomInRange, 0);
                            Find.Storyteller.incidentQueue.Add(qi);
                        }
                    }
                }
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "GetLetterText")]
    public static class IncidentWorker_RaidEnemy_Patch_GetLetterText
    {
        [HarmonyPostfix]
        public static void PostExecute(ref string __result, ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
#if DEBUG
                //    Log.Message(string.Format("PostGetLetterText Xenomorph Raid CurSkyGlow: {0}", (parms.target as Map).skyManager.CurSkyGlow));
#endif
                    /*
                    if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                    {
                        string text = "They mostly come at night......mostly.....";
                        text += "\n\n";
                        text += __result;
                        __result = text;

                    }
                    */
                }
            }
        }
    }

}