using AbilityUser;
using AlienRace;
using Harmony;
using RimWorld;
using RRYautja.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RRYautja
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.ogliss.yautja");
            harmony.Patch(
                AccessTools.Method(typeof(Pawn_PathFollower), "CostToMoveIntoCell",
                    new[] { typeof(Pawn), typeof(IntVec3) }), null, new HarmonyMethod(
                    typeof(HarmonyPatches),
                    nameof(PathOfNature)), null);

            Type typeFromHandle1 = typeof(PawnRenderer);
            HarmonyPatches.pawnField_PawnRenderer = typeFromHandle1.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
            /*
            MethodInfo method5 = typeFromHandle1.GetMethod("RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            });
            MethodInfo method6 = typeof(HarmonyPatches).GetMethod("Patch_PawnRenderer_RenderPawnAt");
            harmony.Patch(method5, new HarmonyMethod(method6), null, null);
            */
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            }, null), new HarmonyMethod(typeof(HarmonyPatches), "PawnRenderer_Blur_Prefix", null), null, null);

            harmony.Patch(
                original: AccessTools.Method(type: typeof(PawnRenderer), name: "DrawEquipment"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_DrawEquipment_Cloak)),
                postfix: null);

            harmony.Patch(
                original: AccessTools.Method(type: typeof(AlienPartGenerator.BodyAddon), name: "CanDrawAddon"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_CanDrawAddon_Cloak)),
                postfix: null);

            //DeathActionWorker_BigExplosion
            harmony.Patch(
                original: AccessTools.Method(type: typeof(DeathActionWorker_BigExplosion), name: "PawnDied"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_PawnDied_Facehugger)),
                postfix: null);

            harmony.Patch(
                AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) }), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Post_GeneratePawn_Yautja)));

            Type typeFromHandle22 = typeof(Pawn_HealthTracker);
            HarmonyPatches.int_Pawn_HealthTracker_GetPawn = typeFromHandle22.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            //   harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "BaseHeadOffsetAt", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "BaseHeadOffsetAtPostfix", null), null);

            Type typeFromHandle6 = typeof(Pawn_HealthTracker);
            harmony.Patch(typeFromHandle6.GetMethod("DropBloodFilth"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_Pawn_HealthTracker_DropBloodFilth")), null, null);

            Type typeFromHandle4 = typeof(HealthUtility);
            harmony.Patch(typeFromHandle4.GetMethod("AdjustSeverity"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_HealthUtility_AdjustSeverity")), null, null);

            Type typeFromHandle2 = typeof(Pawn_NativeVerbs);
            HarmonyPatches._cachedVerbProperties = typeFromHandle2.GetField("cachedVerbProperties", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty);
            HarmonyPatches._pawnPawnNativeVerbs = typeFromHandle2.GetField("pawn", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty);
            harmony.Patch(typeFromHandle2.GetMethod("CheckCreateVerbProperties", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_CheckCreateVerbProperties")), null, null);


            harmony.Patch(
                original: AccessTools.Method(type: typeof(FoodUtility), name: "AddFoodPoisoningHediff"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_AddFoodPoisoningHediff_CompCheck)),
                postfix: null);

        }

        public static bool Pre_AddFoodPoisoningHediff_CompCheck(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
            //    Log.Message(string.Format("checkin if {0} can get food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
            CompFoodPoisonProtection compFood = pawn.TryGetComp<CompFoodPoisonProtection>();
            if (compFood != null)
            {
                if (!compFood.Props.Poisonable)
                {
                    //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because compFood.Props.Poisonable {2}", pawn.Name, ingestible, compFood.Props.Poisonable));
                    return false;
                }
                if (!compFood.Props.FoodTypeFlags.NullOrEmpty<FoodTypeFlags>())
                {
                    foreach (var ftf in compFood.Props.FoodTypeFlags)
                    {
                        if (ftf == ingestible.def.ingestible.foodType)
                        {
                            //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, ingestible.def.ingestible.foodType));
                            return false;
                        }
                    }
                }
                if (!compFood.Props.FoodPoisonCause.NullOrEmpty<FoodPoisonCause>())
                {
                    foreach (var fpc in compFood.Props.FoodPoisonCause)
                    {
                        if (fpc == cause)
                        {
                            //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static Pawn pawnPawnNativeVerbs(Pawn_NativeVerbs instance)
        {
            return (Pawn)HarmonyPatches._pawnPawnNativeVerbs.GetValue(instance);
        }

        public static List<VerbProperties> cachedVerbProperties(Pawn_NativeVerbs instance)
        {
            return (List<VerbProperties>)HarmonyPatches._cachedVerbProperties.GetValue(instance);
        }

        public static Pawn Pawn_HealthTracker_GetPawn(Pawn_HealthTracker instance)
        {
            return (Pawn)HarmonyPatches.int_Pawn_HealthTracker_GetPawn.GetValue(instance);
        }

        public static Pawn PawnRenderer_GetPawn(object instance)
        {
            return (Pawn)HarmonyPatches.pawnField_PawnRenderer.GetValue(instance);
        }

        public static bool Patch_CheckCreateVerbProperties(ref Pawn_NativeVerbs __instance)
        {
            bool flag = HarmonyPatches._cachedVerbProperties.GetValue(__instance) != null;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                bool flag2 = XenomorphUtil.IsXenomorph(HarmonyPatches.pawnPawnNativeVerbs(__instance));
                if (flag2)
                {
                    HarmonyPatches._cachedVerbProperties.SetValue(__instance, new List<VerbProperties>());
                    HarmonyPatches.cachedVerbProperties(__instance).Add(NativeVerbPropertiesDatabase.VerbWithCategory((VerbCategory)1));
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        public static bool Patch_HealthUtility_AdjustSeverity(Pawn pawn, HediffDef hdDef, float sevOffset)
        {
            bool preflag = hdDef != XenomorphDefOf.RRY_FaceHuggerInfection || hdDef != XenomorphDefOf.RRY_XenomorphImpregnation || hdDef != XenomorphDefOf.RRY_HiddenXenomorphImpregnation || hdDef != XenomorphDefOf.RRY_NeomorphImpregnation || hdDef != XenomorphDefOf.RRY_HiddenNeomorphImpregnation;
            bool flag = (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned)) && preflag;
            return !flag;
        }

        public static bool Patch_Pawn_HealthTracker_DropBloodFilth(Pawn_HealthTracker __instance)
        {
            Pawn pawn = HarmonyPatches.Pawn_HealthTracker_GetPawn(__instance);
            bool flag = (pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned));
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        public static void BaseHeadOffsetAtPostfix(PawnRenderer __instance, ref Vector3 __result)
        {
            Pawn pawn = Traverse.Create(root: __instance).Field(name: "pawn").GetValue<Pawn>();
            Vector2 offset = Vector2.zero;
            if (pawn.InBed() && pawn.CurrentBed() is Building_XenomorphCocoon cocoonthing)
            {
                //   Log.Message(string.Format("true"));
                if (cocoonthing.Rotation == Rot4.North)
                {
                    __result.z -= 0.5f;
                }
            }
            else
            {
                //   Log.Message(string.Format("false"));
            }
            __result.x += offset.x;
            __result.z += offset.y;
        }

        public static bool Pre_PawnDied_Facehugger(Corpse corpse, DeathActionWorker_BigExplosion __instance)
        {
            //    Log.Message(string.Format("{0}", corpse.Label));
            bool flag = XenomorphUtil.IsInfectedPawn(corpse.InnerPawn);
            if (flag)
            {
                return false;
            }
            return true;
        }

        public static Vector3 PushResult(Thing Caster, Thing thingToPush, int pushDist, out bool collision)
        {
            Vector3 vector = GenThing.TrueCenter(thingToPush);
            Vector3 result = vector;
            bool flag = false;
            for (int i = 1; i <= pushDist; i++)
            {
                int num = i;
                int num2 = i;
                bool flag2 = vector.x < GenThing.TrueCenter(Caster).x;
                if (flag2)
                {
                    num = -num;
                }
                bool flag3 = vector.z < GenThing.TrueCenter(Caster).z;
                if (flag3)
                {
                    num2 = -num2;
                }
                Vector3 vector2 = new Vector3(vector.x + (float)num, 0f, vector.z + (float)num2);
                bool flag4 = GenGrid.Standable(IntVec3Utility.ToIntVec3(vector2), Caster.Map);
                if (flag4)
                {
                    result = vector2;
                }
                else
                {
                    bool flag5 = thingToPush is Pawn;
                    if (flag5)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            collision = flag;
            return result;
        }

        public static void PushEffect(Thing Caster, Thing target, int distance, bool damageOnCollision = false)
        {
            if (target is Building)
            {
                return;
            }
            LongEventHandler.QueueLongEvent(delegate ()
            {
                Pawn pawn;
                if (target != null && (pawn = (target as Pawn)) != null && pawn.Spawned && !pawn.Downed && !pawn.Dead && (pawn?.MapHeld) != null)
                {
                    bool drafted = pawn.Drafted;
                    Vector3 vector = HarmonyPatches.PushResult(Caster, target, distance, out bool flag2);
                    RRY_FlyingObject flyingObject = (RRY_FlyingObject)GenSpawn.Spawn(ThingDef.Named("JT_FlyingObject"), pawn.PositionHeld, pawn.MapHeld, 0);
                    bool flag3 = flag2 & damageOnCollision;
                    if (flag3)
                    {
                        flyingObject.Launch(Caster, new LocalTargetInfo(IntVec3Utility.ToIntVec3(vector)), target, new DamageInfo?(new DamageInfo(DamageDefOf.Blunt, (float)Rand.Range(8, 10), 0f, -1f, null, null, null, 0, null)));
                    }
                    else
                    {
                        flyingObject.Launch(Caster, new LocalTargetInfo(IntVec3Utility.ToIntVec3(vector)), target);
                    }

                }
            }, "PushingCharacter", false, null);
        }
        /*    
    public static void Patch_PawnRenderer_RenderPawnAt(PawnRenderer __instance, ref Vector3 drawLoc, ref RotDrawMode bodyDrawType, ref bool headStump)
    {
        Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn(__instance);
        if (pawn.RaceProps.Humanlike)
        {
            foreach (var hd in pawn.health.hediffSet.hediffs)
            {
                HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                if (comp != null)
                {
                    comp.DrawImplant(drawLoc);
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
        public static void PathOfNature(Pawn_PathFollower __instance, Pawn pawn, IntVec3 c, ref int __result)
        {
            if (pawn?.GetComp<Comp_Yautja>() is Comp_Yautja comp_Yautja || pawn?.GetComp<Comp_Xenomorph>() is Comp_Xenomorph comp_Xenomorph)
            {
                int num;
                if (c.x == pawn.Position.x || c.z == pawn.Position.z)
                {
                    num = pawn.TicksPerMoveCardinal;
                }
                else
                {
                    num = pawn.TicksPerMoveDiagonal;
                }

                //num += pawn.Map.pathGrid.CalculatedCostAt(c, false, pawn.Position);
                Building edifice = c.GetEdifice(pawn.Map);
                if (edifice != null)
                {
                    num += (int)edifice.PathWalkCostFor(pawn);
                }

                if (num > 450)
                {
                    num = 450;
                }

                if (pawn.jobs.curJob != null)
                {
                    switch (pawn.jobs.curJob.locomotionUrgency)
                    {
                        case LocomotionUrgency.Amble:
                            num *= 3;
                            if (num < 60)
                            {
                                num = 60;
                            }

                            break;
                        case LocomotionUrgency.Walk:
                            num *= 2;
                            if (num < 50)
                            {
                                num = 50;
                            }

                            break;
                        case LocomotionUrgency.Jog:
                            num *= 1;
                            break;
                        case LocomotionUrgency.Sprint:
                            num = Mathf.RoundToInt((float)num * 0.75f);
                            break;
                    }
                }

                __result = Mathf.Max(num, 1);
            }
        }

        private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/CloakActive", ShaderDatabase.Transparent);

        public static bool Pre_CanDrawAddon_Cloak(Pawn pawn)
        {
            bool flag = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            if (flag)
            {
                //Log.Message(string.Format("tetet"));
                return false;
            }
            return true;
        }

        public static bool Pre_DrawWound_Cloak(PawnWoundDrawer __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool flag = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            if (flag)
            {
                //Log.Message(string.Format("tetet"));
                return false;
            }
            return true;
        }

        public static bool Pre_DrawEquipment_Cloak(PawnRenderer __instance)
        {
            Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn(__instance);
            bool flag = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            if (flag)
            {
                return false;
            }
            return true;
        }

        public static bool PawnRenderer_Cloak_Prefix(PawnRenderer __instance, ref Vector3 drawLoc, ref RotDrawMode bodyDrawType, bool headStump)
        {
            Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            PawnGraphicSet value2 = Traverse.Create(__instance).Field("graphics").GetValue<PawnGraphicSet>();
            bool flag = (value.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked) || value.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden));
            if (flag)
            {
                if (value.kindDef.race.GetType() == typeof(AlienRace.ThingDef_AlienRace))
                {
                    bool selected = Find.Selector.SingleSelectedThing == value;
                }

                __instance.graphics = new PawnGraphicSet_Invisible(value)
                {
                    nakedGraphic = new Graphic_Invisible(),
                    rottingGraphic = null,
                    packGraphic = null,
                    headGraphic = null,
                    desiccatedHeadGraphic = null,
                    skullGraphic = null,
                    headStumpGraphic = null,
                    desiccatedHeadStumpGraphic = null,
                    hairGraphic = null
                };
#if DEBUG
                if (Prefs.DevMode)
                {
                    float num = Mathf.Lerp(1.2f, 1.55f, value.BodySize);
                    Vector3 vector = value.Drawer.DrawPos;
                    vector.y = Altitudes.AltitudeFor(AltitudeLayer.VisEffects);
                    float angle = 0f;// (float)Rand.Range(0, 360);
                    Vector3 s = new Vector3(num, 1f, num);
                    Matrix4x4 matrix = default(Matrix4x4);
                    matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
                }
#endif
            }
            return true;
        }

        private static FieldInfo pawnField_PawnRenderer;

        /*
        public static void Pre_GeneratePawn_Xenomorph(ref PawnGenerationRequest request, ref Pawn __result)
        {
            if (request.KindDef.race.race.FleshType == XenomorphRacesDefOf.RRY_Xenomorph || request.KindDef.race.race.FleshType == XenomorphRacesDefOf.RRY_Neomorph)
            {
                //    Log.Message(string.Format("Pre_GeneratePawn_Xenomorph request is {0}, {1}, {2}", request.KindDef.LabelCap, request.FixedGender, request.MustBeCapableOfViolence));
                PawnKindDef pawnKind = request.KindDef;
                Gender gender;

                if (pawnKind == XenomorphDefOf.RRY_Xenomorph_Queen)
                {
                    
                }

                if (pawnKind == XenomorphDefOf.RRY_Xenomorph_Queen)
                {
                    gender = Gender.Female;
                }
                else
                {
                    gender = Gender.None;
                }
                request = new PawnGenerationRequest(pawnKind, request.Faction, request.Context, -1, true, false, false, false, false, true, 0f, fixedGender: gender, allowGay: false);
                //    Log.Message(string.Format("Pre_GeneratePawn_Xenomorph End request is {0}, {1}, {2}", request.KindDef.LabelCap, request.FixedGender, request.MustBeCapableOfViolence));
            }
        }
        */

        public static void Post_GeneratePawn_Yautja(PawnGenerationRequest request, ref Pawn __result)
        {
            Comp_Yautja _Yautja = __result.TryGetComp<Comp_Yautja>();

            if (_Yautja != null)
            {
                Backstory pawnStoryC = __result.story.childhood;
                Backstory pawnStoryA = __result.story.adulthood ?? null;

                AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_YoungBlood");
                AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_Blooded");
                AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodA");
                AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodB");

                HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
                HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
                HediffDef markedDef = YautjaDefOf.RRY_Hediff_BloodedM;

                bool hasunblooded = __result.health.hediffSet.HasHediff(unbloodedDef);
                bool hasbloodedUM = __result.health.hediffSet.HasHediff(unmarkedDef);
                bool hasbloodedM = __result.health.hediffSet.hediffs.Any<Hediff>(x => x.def.defName.StartsWith(markedDef.defName));

                if (!hasunblooded && !hasbloodedUM && !hasbloodedM)
                {
                    HediffDef hediffDef;
                    if (pawnStoryA != null)
                    {
                        if (pawnStoryA != bsDefUnblooded.backstory && __result.kindDef.race == YautjaDefOf.RRY_Alien_Yautja)
                        {
                            hediffDef = _Yautja.Props.bloodedDefs.RandomElement();

                            if (hediffDef != null)
                            {
                                PawnKindDef pawnKindDef = YautjaBloodedUtility.RandomMarked(hediffDef);
                                if (_Yautja != null)
                                {
                                    _Yautja.MarkHedifflabel = pawnKindDef.LabelCap;
                                    _Yautja.MarkedhediffDef = hediffDef;
                                    _Yautja.predator = pawnKindDef.RaceProps.predator;
                                    _Yautja.BodySize = pawnKindDef.RaceProps.baseBodySize;
                                    _Yautja.combatPower = pawnKindDef.combatPower;
                                    //    Log.Message(string.Format("{0}: {1}", hediffDef.stages[0].label, pawnKindDef.LabelCap));
                                }
                            }

                        }
                        else
                        {
                            hediffDef = unbloodedDef;
                        }
                    }
                    else
                    {
                        hediffDef = unbloodedDef;
                    }
                    foreach (var item in __result.RaceProps.body.AllParts)
                    {
                        if (item.def == BodyPartDefOf.Head)
                        {

                            __result.health.AddHediff(hediffDef, item);
                        }
                    }
                }
                else
                {
                    //    Log.Message(string.Format("new pawn has hasunblooded:{0}, hasbloodedUM:{1}, hasbloodedM:{2}", hasunblooded, hasbloodedUM, hasbloodedM));
                }
            }
            if (__result.kindDef.race == YautjaDefOf.RRY_Alien_Yautja)
            {
                if (request.Faction.leader == null && request.Faction != Faction.OfPlayerSilentFail && request.KindDef.race == YautjaDefOf.RRY_Alien_Yautja)
                {
                    bool upgradeWeapon = Rand.Chance(0.5f);
                    if (__result.equipment.Primary != null && upgradeWeapon)
                    {
                        __result.equipment.Primary.TryGetQuality(out QualityCategory weaponQuality);
                        if (weaponQuality != QualityCategory.Legendary)
                        {
                            Thing Weapon = __result.equipment.Primary;
                            CompQuality Weapon_Quality = Weapon.TryGetComp<CompQuality>();
                            if (Weapon_Quality != null)
                            {
                                Weapon_Quality.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                            }
                        }

                    }
                    else if (__result.apparel.WornApparelCount > 0 && !upgradeWeapon)
                    {
                        foreach (var item in __result.apparel.WornApparel)
                        {
                            item.TryGetQuality(out QualityCategory gearQuality);
                            float upgradeChance = 0.5f;
                            bool upgradeGear = Rand.Chance(0.5f);
                            if (gearQuality != QualityCategory.Legendary)
                            {
                                CompQuality Gear_Quality = item.TryGetComp<CompQuality>();
                                if (Gear_Quality != null)
                                {
                                    if (upgradeGear)
                                    {
                                        Gear_Quality.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }

            }
            /*
            if (__result.Faction.def.defName.Contains("RRY_USCM"))
            {
                __result.relations.ClearAllRelations();

                if (__result.gender != Gender.Male)
                {
                    PawnKindDef pawnKindDef = request.KindDef;
                    if (pawnKindDef == request.KindDef) pawnKindDef = Rand.Chance(0.15f) ? AstartesOGDefOf.OG_Astartes_Neophyte : AstartesOGDefOf.OG_Astartes_Brother;
                    if (pawnKindDef == request.KindDef) pawnKindDef = Rand.Chance(0.15f) ? AstartesOGDefOf.OG_Astartes_Sargent : AstartesOGDefOf.OG_Astartes_Brother;
                    if (pawnKindDef == request.KindDef) pawnKindDef = Rand.Chance(0.15f) ? AstartesOGDefOf.OG_Astartes_Captain : AstartesOGDefOf.OG_Astartes_Brother;
                    request = new PawnGenerationRequest(request.KindDef, request.Faction, request.Context, -1, true, false, false, false, false, true, 0f, fixedGender: Gender.Male, allowGay: false);
                    __result = PawnGenerator.GeneratePawn(request);
                }

                __result.story.bodyType = BodyTypeDefOf.Hulk;
                __result.gender = Gender.Male;
                //Log.Message(string.Format(__result.Drawer.renderer.graphics.headGraphic.data.texPath));
                //   __result.Drawer.renderer.graphics.headGraphic.data.texPath.ToString();
                HairDef hairdef = Rand.Chance(0.5f) ? AstartesOGDefOf.Shaved : AstartesOGDefOf.Topdog;
                __result.story.hairDef = hairdef;
            }
            */
            if (__result.kindDef.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph)
            {
                if (request.KindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                {
                    __result.gender = Gender.Female;
                    
                    bool QueenPresent = false;
                    
                    foreach (var p in Find.AnyPlayerHomeMap.mapPawns.AllPawns)
                    {
                        if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                        {
                        //    Log.Message(string.Format("Queen Found"));
                            QueenPresent = true;
                            break;
                        }
                    }
                    if (QueenPresent)
                    {
                        request = new PawnGenerationRequest(XenomorphDefOf.RRY_Xenomorph_Warrior, request.Faction, request.Context, -1, true, false, false, false, false, true, 0f, fixedGender: Gender.None, allowGay: false);
                        __result = PawnGenerator.GeneratePawn(request);
                        __result.gender = Gender.None;
                        return;
                    }
                    else
                    {
                        __result.gender = Gender.Female;
                    }
                    
                }
                else
                {
                    __result.gender = Gender.None;
                }
            }
            
            if (__result.kindDef.race != YautjaDefOf.RRY_Alien_Yautja && __result.RaceProps.Humanlike && (__result.story.hairDef == YautjaDefOf.RRY_Yaujta_Dreds || __result.story.hairDef == YautjaDefOf.RRY_Yaujta_Ponytail || __result.story.hairDef == YautjaDefOf.RRY_Yaujta_Bald))
            {
                Log.Message(string.Format("Non Yautja with Yautja Hair"));
                __result.story.hairDef = DefDatabase<HairDef>.AllDefsListForReading.FindAll(x => !x.hairTags.Contains("Yautja")).RandomElement();
            }
            if (Rand.Chance(0.005f) && XenomorphUtil.isInfectablePawn(__result) && SettingsHelper.latest.AllowHiddenInfections)
            {
                HediffDef def = Rand.Chance(0.75f) ? XenomorphDefOf.RRY_HiddenXenomorphImpregnation : XenomorphDefOf.RRY_HiddenNeomorphImpregnation;
                __result.health.AddHediff(def, __result.RaceProps.body.corePart, null);
            }
            var hediffGiverSet = __result?.def?.race?.hediffGiverSets;
            if (hediffGiverSet == null) return;
            foreach (var item in hediffGiverSet)
            {
                var hediffGivers = item.hediffGivers;
                if (hediffGivers == null) return;
                if (hediffGivers.Any(y => y is HediffGiver_StartWithHediff))
                {
                    foreach (var hdg in hediffGivers.Where(x => x is HediffGiver_StartWithHediff))
                    {
                        HediffGiver_StartWithHediff hediffGiver_StartWith = (HediffGiver_StartWithHediff)hdg;
                        hediffGiver_StartWith.GiveHediff(__result);
                    }
                }
            }
        }


        public static FieldInfo int_Pawn_HealthTracker_GetPawn;

        private const BindingFlags allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

        public static FieldInfo _jobsGivenRecentTicksTextual;

        public static FieldInfo _cachedVerbProperties;

        public static FieldInfo _pawnPawnNativeVerbs;
    }
}