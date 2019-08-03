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
    
    // Marking system tick replacement
    [HarmonyPatch(typeof(Pawn_RecordsTracker), "Increment")]
    public static class AvP_Pawn_RecordsTracker_Increment_Patch
    {
        [HarmonyPostfix]
        public static void IncrementPostfix(Pawn_RecordsTracker __instance, RecordDef def)
        {
            if (def == RecordDefOf.Kills)
            {
                Pawn p = __instance.pawn;
                if (p!=null && p.isBloodable() && p.BloodStatus() is Comp_Yautja _Yautja)
                {
                    if (p.CurBloodStatus()==AvPExtensions.BloodStatusMode.None)
                    {
                        p.health.AddHediff(YautjaDefOf.RRY_Hediff_Unblooded);
                    }
                    _Yautja.CalcTick();
                }
                if (p.isYautja())
                {
                    List<Thought_Memory> _Memories = p.needs.mood.thoughts.memories.Memories.FindAll(x=> x.def == YautjaDefOf.RRY_Thought_ThrillOfTheHunt);
                    if (_Memories.Count < YautjaDefOf.RRY_Thought_ThrillOfTheHunt.stackLimit)
                    {
                        p.needs.mood.thoughts.memories.Memories.Add(new Thought_Memory()
                        {
                            def = YautjaDefOf.RRY_Thought_ThrillOfTheHunt
                        });
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryRomanceChanceFactor", null)]
    public class XenophobeAttractionMultiplier
    {
        [HarmonyPostfix]
        public static void SecondaryRomanceChanceFactor(Pawn_RelationsTracker __instance, Pawn otherPawn, ref float __result)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = (Pawn)XenophobeAttractionMultiplier.pawn.GetValue(__instance);
            bool flag = pawn != null && otherPawn != null;
            if (flag)
            {
                bool alien = !Equals(otherPawn.def, pawn.def);
                if (pawn.isYautja() && alien)
                {
                    float num = 0f;
                    __result *= num;
                }
                else
                {
                    /*
                    int degree = pawn.story.traits.DegreeOfTrait(DefDatabase<TraitDef>.GetNamedSilentFail("Xenophobia"));
                    if (alien)
                    {
                        if (degree == 1)
                        {
                            float num = 0.25f;
                            __result *= num;
                            Log.Message(string.Format("{0}({1}) is alien to {2}({3}), lowering compatability by {4} to {5} due to {2}'s Xenophobia", otherPawn.LabelShortCap, otherPawn.def.LabelCap, pawn.LabelShortCap, pawn.def.LabelCap, num, __result));
                        }
                        else if (degree == -1)
                        {
                            float num = 1.75f;
                            __result *= num;
                            Log.Message(string.Format("{0}({1}) is alien to {2}({3}), increasing compatability by {4} to {5} due to {2}'s Xenophelia", otherPawn.LabelShortCap, otherPawn.def.LabelCap, pawn.LabelShortCap, pawn.def.LabelCap, num, __result));
                        }
                        else
                        {
                            float num = 0.5f;
                            __result *= num;
                            Log.Message(string.Format("{0}({1}) is alien to {2}({3}), lowering compatability by {4} to {5}", otherPawn.LabelShortCap, otherPawn.def.LabelCap, pawn.LabelShortCap, pawn.def.LabelCap, num, __result));
                        }
                    }
                    else
                    {
                        float num = 1f;
                        __result *= num;

                        Log.Message(string.Format("{0} and {1} are both {2} no action taken", otherPawn.LabelShortCap, pawn.LabelShortCap, pawn.def.LabelCap));
                    }
                    */
                }
            }
        }

        public static FieldInfo pawn = typeof(Pawn_RelationsTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

    }

    // Stops wild animals attacked by xeno/neomorphs triggering manhunter 
    [HarmonyPatch(typeof(RimWorld.PawnUtility), "GetManhunterOnDamageChance", new Type[] { typeof(Pawn), typeof(Thing) }), StaticConstructorOnStartup]
    public static class AvP_PawnUtility_GetManhunterOnDamageChance_Patch
    {
        [HarmonyPostfix]
        public static void GetManhunterOnDamageChancePostfix(Pawn pawn, Thing instigator, ref float __result)
        {
            if (instigator != null)
            {
                __result = XenomorphUtil.IsXenomorphPawn(((Pawn)instigator)) ? 0.0f : __result;
                //     Log.Message(string.Format("__result: {0}", __result));
            }
        }
    }

    // Pawns avoid acid Xenomorph acid
    [HarmonyPatch(typeof(Verse.AI.PathGrid), "CalculatedCostAt", new Type[] { typeof(IntVec3), typeof(bool), typeof(IntVec3) })]
    public static class AvP_PathGrid_CalculatedCostAt_Patch
    {
        [HarmonyPostfix]
        public static void CalculatedCostAtPostfix(IntVec3 c, bool perceivedStatic, IntVec3 prevCell, ref int __result)
        {
            Map map = Find.CurrentMap;

            if (map != null)
            {
                List<Thing> list = map.thingGrid.ThingsListAt(c);
                for (int j = 0; j < 9; j++)
                {
                    IntVec3 b = GenAdj.AdjacentCellsAndInside[j];
                    IntVec3 c2 = c + b;
                    if (c2.InBounds(map) && perceivedStatic)
                    {
                        Filth_AddAcidDamage acid = null;
                        list = map.thingGrid.ThingsListAtFast(c2);
                        if (list.Any(x=> x.def == XenomorphDefOf.RRY_FilthBloodXenomorph))
                        {
                            list = list.FindAll(x => x.def == XenomorphDefOf.RRY_FilthBloodXenomorph);
                            for (int k = 0; k < list.Count; k++)
                            {
                                acid = (list[k] as Filth_AddAcidDamage);
                                if (acid != null)
                                {
                                    if (acid.active)
                                    {
                                        if (__result<9000)
                                        {
                                            //    Log.Message(string.Format("acid is active: {0} = {1}", acid.active, __result));
                                            if (b.x == 0 && b.z == 0)
                                            {
                                                __result += 1000;
                                            //    Log.Message(string.Format("acid @: {0}, active: {1}, PathCost: {2}", c, acid.active, __result));
                                            }
                                            else
                                            {
                                                __result += 500;
                                            //    Log.Message(string.Format("acid adjacent to: {0} @: {1}, active: {2}, PathCost: {3}", c, c2, acid.active, __result));
                                            }
                                        }
                                    }
                                    else
                                    {
                                    //    Log.Message(string.Format("acid @: {0}, active: {1}, PathCost: {2}", c, acid.active, __result));
                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }

        }
    }


    // Xenomorphs should prefer blunt attacks when attempting to gather host pawns
    [HarmonyPatch(typeof(Pawn_MeleeVerbs), "ChooseMeleeVerb")]
    public static class AvP_Pawn_MeleeVerbs_ChooseMeleeVerb_Patch
    {
        [HarmonyPostfix]
        public static void HarmsHealthPostfix(Pawn_MeleeVerbs __instance, Thing target, ref Verb ___curMeleeVerb)
        {
            if (__instance.Pawn.isXenomorph() && __instance.Pawn.def != XenomorphRacesDefOf.RRY_Xenomorph_FaceHugger && target is Pawn pawn)
            {
                if (XenomorphUtil.isInfectablePawn(pawn))
                {
                    bool flag = Rand.Chance(0.04f);
                    List<VerbEntry> updatedAvailableVerbsList = __instance.GetUpdatedAvailableVerbsList(flag);
                //    Log.Message(string.Format("All AvailableVerbs for {0}: {1}", __instance.Pawn.LabelShortCap, updatedAvailableVerbsList.Count));
                    updatedAvailableVerbsList = updatedAvailableVerbsList.FindAll(x=> x.verb.maneuver == DefDatabase<ManeuverDef>.GetNamedSilentFail("Smash"));
                //    Log.Message(string.Format("AvailableVerbs Smash for {0}: {1}", __instance.Pawn.LabelShortCap, updatedAvailableVerbsList.Count));
                    bool flag2 = false;
                    VerbEntry verbEntry;
                    if (updatedAvailableVerbsList.TryRandomElementByWeight((VerbEntry ve) => ve.GetSelectionWeight(target), out verbEntry))
                    {
                        flag2 = true;
                    //    Log.Message(string.Format("{0}'s using {1} against {2}", __instance.Pawn.LabelShortCap, verbEntry.verb.maneuver, pawn.LabelShortCap));
                    }
                    else if (flag)
                    {
                        updatedAvailableVerbsList = __instance.GetUpdatedAvailableVerbsList(false);
                        updatedAvailableVerbsList = updatedAvailableVerbsList.FindAll(x => x.verb.maneuver == DefDatabase<ManeuverDef>.GetNamedSilentFail("Smash"));
                        flag2 = updatedAvailableVerbsList.TryRandomElementByWeight((VerbEntry ve) => ve.GetSelectionWeight(target), out verbEntry);
                    //    Log.Message(string.Format("{0}'s using {1} against {2}", __instance.Pawn.LabelShortCap, verbEntry.verb.maneuver, pawn.LabelShortCap));
                    }
                    if (flag2)
                    {
                    //    verbEntry.verb.tool.capacities.Contains();
                        ___curMeleeVerb = verbEntry.verb;
                    }
                    else
                    {
                        Log.ErrorOnce(string.Concat(new object[]
                        {
                    __instance.Pawn.ToStringSafe<Pawn>(),
                    " has no available melee attack, spawned=",
                    __instance.Pawn.Spawned,
                    " dead=",
                    __instance.Pawn.Dead,
                    " downed=",
                    __instance.Pawn.Downed,
                    " curJob=",
                    __instance.Pawn.CurJob.ToStringSafe<Job>(),
                    " verbList=",
                    updatedAvailableVerbsList.ToStringSafeEnumerable(),
                    " bodyVerbs=",
                    __instance.Pawn.verbTracker.AllVerbs.ToStringSafeEnumerable()
                        }), __instance.Pawn.thingIDNumber ^ 195867354, false);
                        ___curMeleeVerb =null;
                    }
                }
            }
        }
    }

    // FoodUtility.BestPawnToHuntForPredator(getter, forceScanWholeMap)  BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
    // Xeno/Neomorph Hunting patch
    [HarmonyPatch(typeof(FoodUtility), "BestPawnToHuntForPredator")]
    public static class AvP_FoodUtility_BestPawnToHuntForPredator_Patch
    {
        [HarmonyPostfix]
        public static void BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap, ref Pawn __result)
        {
            if (predator.isNeomorph())
            {
                Comp_Neomorph _Neomorph = predator.TryGetComp<Comp_Neomorph>();
                __result = _Neomorph.BestPawnToHuntForPredator(predator, forceScanWholeMap);
            }
            if (predator.isXenomorph())
            {

            }
        }
    }
    /*
    // FoodUtility.BestPawnToHuntForPredator(getter, forceScanWholeMap)  BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
    // Xeno/Neomorph Hunting patch
    [HarmonyPatch(typeof(FoodUtility), "TryFindBestFoodSourceFor")]
    public static class AvP_FoodUtility_TryFindBestFoodSourceFor_Patch
    {
        [HarmonyPostfix]
        public static void TryFindBestFoodSourceFor(Pawn getter, Pawn eater, bool desperate, ref Thing foodSource, ref ThingDef foodDef, ref bool __result, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false)
        {
            if (eater.isNeomorph())
            {
                Comp_Neomorph _Neomorph = eater.TryGetComp<Comp_Neomorph>();
                __result = _Neomorph.TryFindBestFoodSourceFor(getter, eater, desperate, out foodSource, out foodDef, canRefillDispenser, canUseInventory, allowForbidden, allowCorpse, allowSociallyImproper, allowHarvest,forceScanWholeMap);
            }
            if (eater.isXenomorph())
            {

            }
        }
    }
    */
    // Stuffable Projectile hunting weapon fix
    [HarmonyPatch(typeof(VerbUtility), "HarmsHealth")]
    public static class AvP_VerbUtility_HarmsHealth_Patch
    {
        [HarmonyPostfix]
        public static void HarmsHealthPostfix(Verb verb, ref bool __result)
        {
            __result = __result || verb is Verb_Launch_Stuffable_Projectile || verb is Verb_Shoot_Stuffable;
        }
    }

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

    // Disables Stuffable Projectiles Firing while wearing vanillia Shield Belts
    [HarmonyPatch(typeof(ShieldBelt), "AllowVerbCast")]
    internal static class AvP_ShieldBelt_AllowVerbCast_YautjaWeapons_Patch
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

    // Xeno/Neomorph Hypothermic slowdown
    [HarmonyPatch(typeof(HediffGiver_Hypothermia), "OnIntervalPassed")]
    public static class AvP_HediffGiver_Hypothermia_OnIntervalPassed_Patch
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
    
    // Protects Cocooned Pawns from wound infections
    [HarmonyPatch(typeof(HediffComp_Infecter), "CheckMakeInfection")]
    public static class AvP_HediffComp_Infecter_CheckMakeInfection_Patch
    {
        [HarmonyPrefix]
        public static bool preCheckMakeInfection(HediffComp_Infecter __instance)
        {
#if DEBUG
        //    Log.Message(string.Format("trying to add an infection to {0}'s wounded {1}", __instance.Pawn, __instance.parent.Part));
#endif
            if (__instance.Pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) || (__instance.Pawn.InBed() && __instance.Pawn.CurrentBed() is Building_XenomorphCocoon) || __instance.Pawn.RaceProps.FleshType.defName.Contains("RRY_SynthFlesh"))
            {
#if DEBUG
            //    Log.Message(string.Format("{0} protected from infection", __instance.Pawn));
#endif
                return false;
            }
            return true;
        }
    }

    // Stops Cocooned Pawns taking damage from Xeno blood
    [HarmonyPatch(typeof(Pawn), "PreApplyDamage")]
    public static class AvP_Pawn_PreApplyDamage_Patch
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

    // stop Pawns trying to wander near Cocooned colonists
    [HarmonyPatch(typeof(JobGiver_WanderColony), "GetWanderRoot")]
    public static class AvP_JobGiver_WanderColony_GetWanderRoot_Patch
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

    // Stop Doctors trying to feed cocooned Pawns
    [HarmonyPatch(typeof(FeedPatientUtility), "ShouldBeFed")]
    public static class AvP_FeedPatientUtility_ShouldBeFed_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreCocooned(Pawn p, ref bool __result)
        {
            __result = __result && !(p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));
        }
    }

    // Doctors Ignore Cocooned Pawns that need tending
    [HarmonyPatch(typeof(WorkGiver_Tend), "GoodLayingStatusForTend")]
    public static class AvP_WorkGiver_Tend_GoodLayingStatusForTend_Patch
    {
        [HarmonyPostfix]
        public static void PawnInCocoon(WorkGiver_Tend __instance, Pawn patient, Pawn doctor, ref bool __result)
        {
            __result = __result && (!patient.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned)&&!(patient.CurrentBed() is Building_XenomorphCocoon));
        //    Log.Message(string.Format("WorkGiver_Tend_GoodLayingStatusForTend_Patch patient: {0}, doctor: {1}, __Result: {2}", patient, doctor, __result));

        }
    }

    // Pauses NeedsTracker on Cocooned Pawns
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "NeedsTrackerTick", null)]
    public static class AvP_Pawn_NeedsTracker_Patch
    {
        public static bool Prefix(Pawn_NeedsTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = (Pawn)AvP_Pawn_NeedsTracker_Patch.pawn.GetValue(__instance);
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

    // Ignore Cocoons as Beds
    [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
    internal static class AvP_RestUtility_Bed_IsValidBedFor_Patch
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

    // Disallows stripping of the Wristblade
    [HarmonyPatch(typeof(Pawn), "AnythingToStrip")]
    public static class AvP_Pawn_AnythingToStrip_Patch
    {
        [HarmonyPostfix]
        public static void IgnoreWristblade(Pawn __instance, ref bool __result)
        {
            __result = __result && !(__instance.apparel != null && __instance.apparel.WornApparelCount == 1 && __instance.apparel.WornApparel.Any(x => x.def == YautjaDefOf.RRY_Equipment_HunterGauntlet) && __instance.Faction != Faction.OfPlayerSilentFail) && !(__instance.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));

        }
    }

    // Disallows stripping of the Wristblade
    [HarmonyPatch(typeof(Pawn), "Strip")]
    public static class AvP_Pawn_Strip_Patch
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

    // Hides wounds on Stealthed Pawns
    [HarmonyPatch(typeof(PawnWoundDrawer), "RenderOverBody")]
    public static class AvP_PawnWoundDrawer_TryExecute_Patch
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

    // Plasmacasters ignore Cloaked things
    [HarmonyPatch(typeof(Building_Turret_Shoulder), "ThreatDisabled")]
    public static class AvP_Building_Turret_Shoulder_ThreatDisabled_Patch
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

    // Gets Gizmos from Apparel's Comps
    [HarmonyPatch(typeof(Apparel), "GetWornGizmos")]
    public static class AvP_RimWorld_Apparel_GetWornGizmos_Patch
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

    // Gets Gizmos from Cloakgen's Comps
    [HarmonyPatch(typeof(Cloakgen), "GetWornGizmos")]
    public static class AvP_RimWorld_Cloakgen_GetWornGizmos_Patch
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
   
    // Hediff_Implant Drawer
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool) })]
    static class AvP_Pawn_DrawTracker_get_DrawPos_Patch
    {
        static void Prefix(PawnRenderer __instance, ref Vector3 rootLoc, ref float angle, ref bool renderBody, ref Rot4 bodyFacing, ref Rot4 headFacing, ref RotDrawMode bodyDrawType, ref bool portrait, ref bool headStump)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            //    bool selected = Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode;
            if (!portrait)
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
                if ((pawn.RaceProps.Humanlike && pawnflag) || pawn.kindDef.race.GetModExtension<OffsetDefExtension>() != null)
                {
                    foreach (var hd in pawn.health.hediffSet.hediffs)
                    {
                        HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                        if (comp != null)
                        {
                            DrawImplant(comp, __instance, rootLoc, angle, renderBody, bodyFacing, headFacing, bodyDrawType, portrait, headStump);
                        }
                    }
                    /*
                    */
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
    internal static class AvP_GameConditionManager_ConditionIsActive_Patch
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
    public static class AvP_YautjaSpecificHat_Patch
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
    public static class AvP_PawnGenerator_GenerateBodyType_Yautja_Patch
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
    public static class AvP_IncidentWorker_RaidEnemyPatch_TryExecute_Patch
    {
        // Token: 0x06000017 RID: 23 RVA: 0x00002CD0 File Offset: 0x00000ED0
        [HarmonyPrefix]
        public static bool PreExecute(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && ((parms.faction.leader != null && parms.faction.leader.kindDef.race == YautjaDefOf.RRY_Alien_Yautja) || (parms.faction.def.basicMemberKind != null && parms.faction.def.basicMemberKind.race == YautjaDefOf.RRY_Alien_Yautja)))
                {
                    parms.generateFightersOnly = true;
                    if ((parms.target as Map).GameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
                    {
                        parms.points *= 2;
                        parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;
                    }
                }
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
                    parms.generateFightersOnly = true;

                    if ((parms.target as Map).skyManager.CurSkyGlow <= 0.5f)
                    {
                        parms.points *= 2;
                        parms.raidArrivalMode = YautjaDefOf.EdgeWalkInGroups;
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

    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecute")]
    public static class AvP_IncidentWorker_RaidEnemy_Patch_TryExecute_Patch
    {
        [HarmonyPrefix]
        public static bool PreExecute(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def == XenomorphDefOf.RRY_Xenomorph))
                {
                    parms.generateFightersOnly = true;
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
    public static class AvP_IncidentWorker_RaidEnemyPatch_GetLetterText_Patch
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
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "GetLetterText")]
    public static class AvP_IncidentWorker_RaidEnemy_Patch_GetLetterText_Patch
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
    */

    // Prevents disabled factions from generating in a new world
    [HarmonyPatch(typeof(FactionGenerator), "GenerateFactionsIntoWorld", null)]
    public static class AvP_FactionGenerator_GenerateFactionsIntoWorld_Patch
    {
        // Token: 0x06000012 RID: 18 RVA: 0x000027D0 File Offset: 0x000017D0
        public static bool Prefix()
        {
            int i = 0;
            int num = 0;
            foreach (FactionDef factionDef in DefDatabase<FactionDef>.AllDefs)
            {
                if (!factionDef.isPlayer)
                {
                    string defName = factionDef.defName;
                    if (factionDef==XenomorphDefOf.RRY_Xenomorph&& !SettingsHelper.latest.AllowXenomorphFaction)
                    {
                        AvP_FactionGenerator_GenerateFactionsIntoWorld_Patch.UpdateDef(factionDef, 0);
                        //    return false;
                    }
                    if (defName.Contains("RRY_Yautja_") && !SettingsHelper.latest.AllowYautjaFaction)
                    {
                        AvP_FactionGenerator_GenerateFactionsIntoWorld_Patch.UpdateDef(factionDef, 0);
                        //    return false;
                    }
                }
            }
            return true;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002C2C File Offset: 0x00001C2C
        private static void UpdateDef(FactionDef def, int requiredCount)
        {
            def.requiredCountAtGameStart = requiredCount;
            if (def.requiredCountAtGameStart < 1)
            {
                def.maxCountAtGameStart = 0;
                return;
            }
            def.maxCountAtGameStart = 100;
        }
    }

    // ScenarioLister.ScenariosInCategory
    // Hides Yautja Scenarios when Faction is disabled
    [HarmonyPatch(typeof(ScenarioLister), "ScenariosInCategory")]
    public static class AvP_Page_ScenarioLister_ScenariosInCategory_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<Scenario> ScenariosInCategoryPrefix(IEnumerable<Scenario> scenario ,ScenarioCategory cat)
        {
            if (cat == ScenarioCategory.FromDef)
            {
                IEnumerable<ScenarioDef> scenarios = null;
                scenarios = DefDatabase<ScenarioDef>.AllDefs;
                foreach (ScenarioDef scenDef in DefDatabase<ScenarioDef>.AllDefs)
                {
                //    Log.Message(string.Format("Found {0}", scenarios.Count()));
                    if ((!scenDef.defName.Contains("Yautja") && !SettingsHelper.latest.AllowYautjaFaction) || SettingsHelper.latest.AllowYautjaFaction)
                    {
                        yield return scenDef.scenario;
                    }
                }
            }
            else if (cat == ScenarioCategory.CustomLocal)
            {
                IEnumerable<Scenario> scenarios = ScenarioFiles.AllScenariosLocal;
                foreach (Scenario scen in scenarios)
                {
                //    Log.Message(string.Format("Found {0}", scenarios.Count()));
                    if ((!scen.name.Contains("Yautja") && !SettingsHelper.latest.AllowYautjaFaction) || SettingsHelper.latest.AllowYautjaFaction)
                    {
                        yield return scen;
                    }
                }
            }
            else if (cat == ScenarioCategory.SteamWorkshop)
            {
                IEnumerable<Scenario> scenarios = ScenarioFiles.AllScenariosWorkshop;
                foreach (Scenario scen2 in scenarios)
                {
                //    Log.Message(string.Format("Found {0}", scenarios.Count()));
                    if ((!scen2.name.Contains("Yautja") && !SettingsHelper.latest.AllowYautjaFaction) || SettingsHelper.latest.AllowYautjaFaction)
                    {
                        yield return scen2;
                    }
                }
            }
            yield break;

        }
    }
    // DoComplexCalcs
    /*
    [HarmonyPatch(typeof(Fire), "DoComplexCalcs")]
    public static class AvP_Fire_DoComplexCalcs_Patch
    {
        [HarmonyPostfix]
        public static void DoComplexCalcsPostfix(Fire __instance)
        {
            Map map = __instance.Map != null ? __instance.Map : __instance.MapHeld;
            IntVec3 center = __instance.Position != null ? __instance.Position : __instance.PositionHeld;
            float radius = __instance.fireSize * 3f;
            MapComponent_HiveGrid _HiveGrid = map.GetComponent<MapComponent_HiveGrid>();
            if (_HiveGrid != null)
            {
                HiveUtility.AddHiveRadial(center, map, radius, -(__instance.fireSize * 0.1f));
            }
        }
    }
    */
    [HarmonyPatch(typeof(SnowUtility), "AddSnowRadial")]
    public static class AvP_SnowUtility_AddSnowRadial_Patch
    {
        [HarmonyPostfix]
        public static void AddSnowRadialPostfix(IntVec3 center, Map map, float radius, float depth)
        {
        //    Log.Message(string.Format("AddSnowRadial center: {0}, radius: {1}, depth: {2}", center, radius, depth));
            MapComponent_HiveGrid _HiveGrid = map.GetComponent<MapComponent_HiveGrid>();
            if (_HiveGrid != null)
            {
        //        Log.Message(string.Format("AddSnowRadial _HiveGrid != null center: {0}, radius: {1}, depth: {2}", center, radius, depth));
                HiveUtility.AddHiveRadial(center, map, radius, depth);
            }
        }
    }

}