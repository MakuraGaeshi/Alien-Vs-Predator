using AbilityUser;
using AlienRace;
using Harmony;
using RimWorld;
using System;
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

            Type typeFromHandle3 = typeof(PawnRenderer);
            HarmonyPatches.pawnField_PawnRenderer = typeFromHandle3.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method5 = typeFromHandle3.GetMethod("RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            });
            MethodInfo method6 = typeof(HarmonyPatches).GetMethod("Patch_PawnRenderer_RenderPawnAt");
            harmony.Patch(method5, new HarmonyMethod(method6), null, null);
            
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            }, null), new HarmonyMethod(typeof(HarmonyPatches), "PawnRenderer_Blur_Prefix", null), null, null);
            
            harmony.Patch(
                original: AccessTools.Method(type: typeof(AlienPartGenerator.BodyAddon), name: "CanDrawAddon"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_CanDrawAddon_Cloak)),
                postfix: null);

            harmony.Patch(
                original: AccessTools.Method(type: typeof(PawnRenderer), name: "DrawEquipment"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_DrawEquipment_Cloak)),
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

            Type typeFromHandle6 = typeof(Pawn_HealthTracker);
            harmony.Patch(typeFromHandle6.GetMethod("DropBloodFilth"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_Pawn_HealthTracker_DropBloodFilth")), null, null);

            Type typeFromHandle4 = typeof(HealthUtility);
            harmony.Patch(typeFromHandle4.GetMethod("AdjustSeverity"), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Patch_HealthUtility_AdjustSeverity")), null, null);
        }

        public static FieldInfo int_Pawn_HealthTracker_GetPawn;

        public static Pawn Pawn_HealthTracker_GetPawn(Pawn_HealthTracker instance)
        {
            return (Pawn)HarmonyPatches.int_Pawn_HealthTracker_GetPawn.GetValue(instance);
        }
        
        public static bool Patch_HealthUtility_AdjustSeverity(Pawn pawn, HediffDef hdDef, float sevOffset)
        {
            bool preflag = hdDef != XenomorphDefOf.RRY_FaceHuggerInfection || hdDef != XenomorphDefOf.RRY_XenomorphImpregnation || hdDef != XenomorphDefOf.RRY_HiddenXenomorphImpregnation || hdDef != XenomorphDefOf.RRY_NeomorphImpregnation || hdDef != XenomorphDefOf.RRY_HiddenNeomorphImpregnation;
            bool flag = (pawn.InBed() && pawn.CurrentBed() is Building_XenomorphCocoon) && preflag;
            return !flag;
        }

        public static bool Patch_Pawn_HealthTracker_DropBloodFilth(Pawn_HealthTracker __instance)
        {
            Pawn pawn = HarmonyPatches.Pawn_HealthTracker_GetPawn(__instance);
            bool flag = (pawn.InBed() && pawn.CurrentBed() is Building_XenomorphCocoon);
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
                if (cocoonthing.Rotation == Rot4.North)
                {
                    __result.z -= 0.5f;
                }
            }
            __result.x += offset.x;
            __result.z += offset.y;
        }
        
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
        
        public static bool Pre_DrawEquipment_Cloak(Vector3 rootLoc, PawnRenderer __instance)
        {
            Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn(__instance);
            bool flag = pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            if (flag)
            {
                return false;
            }
            return true;
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
        
        public static Pawn PawnRenderer_GetPawn(object instance)
        {
            return (Pawn)HarmonyPatches.pawnField_PawnRenderer.GetValue(instance);
        }
        
        public static void Patch_PawnRenderer_RenderPawnAt(PawnRenderer __instance, ref Vector3 drawLoc, ref RotDrawMode bodyDrawType, ref bool headStump)
        {
            Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn(__instance);
            foreach (var hd in pawn.health.hediffSet.hediffs)
            {
                HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                if (comp != null)
                {
                    comp.DrawImplant();
                }
            }

        }
        
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
        
        public static bool PawnRenderer_Blur_Prefix(PawnRenderer __instance, ref Vector3 drawLoc, ref RotDrawMode bodyDrawType, bool headStump)
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
            }
            return true;
        }
        
        public static void Post_GeneratePawn_Yautja(PawnGenerationRequest request, ref Pawn __result)
        {
            if (__result.kindDef.race == YautjaDefOf.RRY_Alien_Yautja)
            {
                if (__result.gender==Gender.Female && __result.story.bodyType != BodyTypeDefOf.Female)
                {
                    __result.story.bodyType = BodyTypeDefOf.Female;
                }
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
                            if (pawnStoryA != bsDefUnblooded.backstory)
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
                            if (item.def==BodyPartDefOf.Head)
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
            }
            else if (request.Faction == Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph))
            {
            //    Log.Message(string.Format("Xenomorph spawning"));
                if (request.KindDef==XenomorphDefOf.RRY_Xenomorph_Queen)
                {
                    if (__result.Map!=null)
                    {
                        bool QueenPresent = false;
                        foreach (var p in __result.Map.mapPawns.AllPawnsSpawned)
                        {
                            if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                            {
                                Log.Message(string.Format("Queen Found"));
                                QueenPresent = true;
                                break;
                            }
                        }
                        if (QueenPresent)
                        {
                            Log.Message(string.Format("Queen Present: {0}", QueenPresent));
                            request = new PawnGenerationRequest(XenomorphDefOf.RRY_Xenomorph_Warrior, request.Faction, request.Context, -1, true, false, false, false, false, true, 0f, fixedGender: Gender.None, allowGay: false);
                            __result = PawnGenerator.GeneratePawn(request);
                            __result.gender = Gender.None;
                            return;
                        }
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
            else if (__result.kindDef.race != YautjaDefOf.RRY_Alien_Yautja && __result.RaceProps.Humanlike && (__result.story.hairDef == YautjaDefOf.RRY_Yaujta_Dreds || __result.story.hairDef == YautjaDefOf.RRY_Yaujta_Ponytail || __result.story.hairDef == YautjaDefOf.RRY_Yaujta_Bald))
            {
            //    Log.Message(string.Format("Non Yautja with Yautja Hair"));
                __result.story.hairDef = DefDatabase<HairDef>.GetRandom();
            }
            if (Rand.Chance(0.005f)&&XenomorphUtil.isInfectablePawn(__result))
            {
                HediffDef def = Rand.Chance(0.75f) ? XenomorphDefOf.RRY_HiddenXenomorphImpregnation : XenomorphDefOf.RRY_HiddenNeomorphImpregnation;
                __result.health.AddHediff(def, __result.RaceProps.body.corePart, null);
            }
        }
        
        private static FieldInfo pawnField_PawnRenderer;
    }
}