﻿using AbilityUser;
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
            harmony.Patch(method5, null, new HarmonyMethod(method6), null);

            /*
            MethodInfo method7 = typeof(HarmonyPatches).GetMethod("Patch_PawnRenderer_RenderPawnAt_XenomorphCocoon");
            harmony.Patch(method5, null, new HarmonyMethod(method7), null);

            harmony.Patch(AccessTools.Method(typeof(Pawn_DrawTracker), "DrawAt", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "DrawAt_PostFix", null), null);
            */

            //Patch_PawnRenderer_WigglerTick
            /*
            harmony.Patch(
                original: AccessTools.Method(type: typeof(PawnRenderer), name: "WigglerTick"),
                prefix: null,
                postfix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Patch_PawnRenderer_WigglerTick)));
            */

            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            }, null), new HarmonyMethod(typeof(HarmonyPatches), "PawnRenderer_Blur_Prefix", null), null, null);
            
            /*
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            }, null), new HarmonyMethod(typeof(HarmonyPatches), "PawnRenderer_XenosColour_Prefix", null), null, null);
            */
            harmony.Patch(
                original: AccessTools.Method(type: typeof(AlienPartGenerator.BodyAddon), name: "CanDrawAddon"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_CanDrawAddon_Cloak)),
                postfix: null);

            harmony.Patch(
                original: AccessTools.Method(type: typeof(PawnRenderer), name: "DrawEquipment"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_DrawEquipment_Cloak)),
                postfix: null);

            // Verse.HealthUtility
            //Patch_HealthUtility_AdjustSeverity
            /*
            harmony.Patch(
                original: AccessTools.Method(type: typeof(HealthUtility), name: "AdjustSeverity"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Patch_HealthUtility_AdjustSeverity), parameters: new Type[]
                {
                    typeof(HediffDef),
                    typeof(float)
                }),
                postfix: null);
            */

            //Patch_PawnRenderer_WigglerTick
            /*
            harmony.Patch(
                original: AccessTools.Method(type: typeof(PawnRenderer), name: "WigglerTick"),
                prefix: null,
                postfix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Patch_PawnRenderer_WigglerTick)));
            */
            //DeathActionWorker_BigExplosion
            harmony.Patch(
                original: AccessTools.Method(type: typeof(DeathActionWorker_BigExplosion), name: "PawnDied"),
                prefix: new HarmonyMethod(type: typeof(HarmonyPatches), name: nameof(Pre_PawnDied_Facehugger)),
                postfix: null);

            harmony.Patch(
                AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) }), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Post_GeneratePawn_Yautja)));
            /*
            harmony.Patch(
                AccessTools.Method(typeof(DownedRefugeeQuestUtility), "GenerateRefugee", new[] { typeof(Pawn) }), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Post_GenerateRefugee_Yautja)));
            */
            //    harmony.Patch(AccessTools.Method(typeof(Corpse), "RareTick", null, null), new HarmonyMethod(typeof(HarmonyPatches), "RareTickPostfix", null), null, null);
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

        //PawnRenderer DrawEquipment
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

        //PawnRenderer PawnDied
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


        // Token: 0x060000EB RID: 235 RVA: 0x00008E6C File Offset: 0x0000706C
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

        // Token: 0x060000EC RID: 236 RVA: 0x00008F44 File Offset: 0x00007144
        public static void PushEffect(Thing Caster, Thing target, int distance, bool damageOnCollision = false)
        {
            LongEventHandler.QueueLongEvent(delegate ()
            {
                Pawn pawn;
                if (target != null && (pawn = (target as Pawn)) != null && pawn.Spawned && !pawn.Downed && !pawn.Dead && ((pawn != null) ? pawn.MapHeld : null) != null)
                {
                    bool flag2;
                    Vector3 vector = HarmonyPatches.PushResult(Caster, target, distance, out flag2);
                    FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("JT_FlyingObject"), pawn.PositionHeld, pawn.MapHeld, 0);
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

        // Token: 0x0600000C RID: 12 RVA: 0x0000283C File Offset: 0x00000A3C
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

        public static void DrawAt_PostFix(Pawn_DrawTracker __instance, Vector3 loc)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_DrawTracker), "pawn").GetValue(__instance);
            
            bool flag = pawn.CurrentBed()!=null && pawn.Spawned;
            if (flag)
            {
                bool flag2 = pawn.CurrentBed() is Building_XenomorphCocoon;
                if (flag2)
                {
                    loc = new Vector3(loc.x+0.5f, loc.y, loc.z);
                }

            }
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000283C File Offset: 0x00000A3C
        public static void Patch_PawnRenderer_RenderPawnAt_XenomorphCocoon(PawnRenderer __instance, ref Vector3 drawLoc, ref RotDrawMode bodyDrawType, ref bool headStump)
        {
            Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn(__instance);
            bool selected = Find.Selector.SelectedObjects.Contains(pawn);
            if (pawn.GetPosture() != PawnPosture.Standing)
            {
                Rot4 rot = pawn.Rotation;
                Building_Bed building_Bed = pawn.CurrentBed();
                bool flag = building_Bed is Building_XenomorphCocoon;
                bool renderBody;
                float angle;
                Vector3 rootLoc;
                if (building_Bed != null && pawn.RaceProps.Humanlike && flag)
                {
                    renderBody = building_Bed.def.building.bed_showSleeperBody;
                    Rot4 rotation = building_Bed.Rotation;
                    rotation.AsInt += 2;
                    angle = rotation.AsAngle;
                    AltitudeLayer altLayer = (AltitudeLayer)Mathf.Max((int)building_Bed.def.altitudeLayer, 15);
                    Vector3 vector2 = pawn.Position.ToVector3ShiftedWithAltitude(altLayer);
                    Vector3 vector3 = vector2;
                    vector3.y += 0.02734375f;
                    float d = -__instance.BaseHeadOffsetAt(Rot4.South).z;
                    d = -__instance.BaseHeadOffsetAt(Rot4.South).z;
                    Vector3 a = rotation.FacingCell.ToVector3();
                    rootLoc = vector2 + a * d;
                    rootLoc.y += 0.0078125f;
                    rootLoc.x += 0.5f;
                    if (selected) Log.Message(string.Format("Patch_PawnRenderer_RenderPawnAt_XenomorphCocoon 4 Old Drawloc {0}", drawLoc));
                    if (selected) Log.Message(string.Format("Patch_PawnRenderer_RenderPawnAt_XenomorphCocoon 5 Old pawn.DrawPos {0}", pawn.DrawPos));
                    drawLoc = rootLoc;
                    if (selected) Log.Message(string.Format("Patch_PawnRenderer_RenderPawnAt_XenomorphCocoon 6 new Drawloc {0}", drawLoc));
                    if (selected) Log.Message(string.Format("Patch_PawnRenderer_RenderPawnAt_XenomorphCocoon 7 new pawn.DrawPos {0}", pawn.DrawPos));
                }
            }


        }

        // Patch_PawnRenderer_WigglerTick
        /*
        public static void Patch_PawnRenderer_WigglerTick(PawnRenderer __instance)
        {
            Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn(__instance);
            foreach (var hd in pawn.health.hediffSet.hediffs)
            {
                HediffComp_XenoSpawner comp = hd.TryGetComp<HediffComp_XenoSpawner>();
                if (comp != null)
                {
                    int num = Find.TickManager.TicksGame % 300 * 2;
                    if (num < 90)
                    {
                        this.downedAngle += 0.35f;
                    }
                    else if (num < 390 && num >= 300)
                    {
                        this.downedAngle -= 0.35f;
                    }
                }
            }

        }
        */
        /*
        
					int num = Find.TickManager.TicksGame % 300 * 2;
					if (num < 90)
					{
						this.downedAngle += 0.35f;
					}
					else if (num < 390 && num >= 300)
					{
						this.downedAngle -= 0.35f;
					}
        */

        // Patch_HealthUtility_AdjustSeverity

        public static bool Patch_HealthUtility_AdjustSeverity(Pawn __instance, ref HediffDef ___hdDef, ref float ___sevOffset)
        {
        //    Log.Message(string.Format("Patch_HealthUtility_AdjustSeverity {0},", __instance));
            return true;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000283C File Offset: 0x00000A3C
        public static void RareTickPostfix(Corpse __instance)
        {
            if (XenomorphUtil.IsInfectedPawn(__instance.InnerPawn))
            {
                HediffWithComps hediff = null;
                if (__instance.InnerPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection))
                {
                    hediff = (HediffWithComps)__instance.InnerPawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_FaceHuggerInfection);
                }
                else if (__instance.InnerPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation))
                {
                    hediff = (HediffWithComps)__instance.InnerPawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_XenomorphImpregnation);
                }
                else if (__instance.InnerPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation))
                {
                    hediff = (HediffWithComps)__instance.InnerPawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
                }
                else if (__instance.InnerPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_NeomorphImpregnation))
                {
                    hediff = (HediffWithComps)__instance.InnerPawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_NeomorphImpregnation);
                }
                else if (__instance.InnerPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenNeomorphImpregnation))
                {
                    hediff = (HediffWithComps)__instance.InnerPawn.health.hediffSet.GetFirstHediffOfDef(XenomorphDefOf.RRY_HiddenNeomorphImpregnation);
                }
                if (hediff!=null)
                {
                    for (int i = 0; i <= 250; i++)
                    {
                    //    float sev = 0f;
                    //    hediff.TryGetComp<HediffComp_XenoSpawner>().CompPostTick(ref sev);
                        hediff.PostTick();
                    }
                }
            }

        }

        // Verse.AI.Pawn_PathFollower
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
            //graphics // (pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked)|| pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden))
            bool flag = (value.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked) || value.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Xenomorph_Hidden));
            if (flag)
            {
                if (value.kindDef.race.GetType() == typeof(AlienRace.ThingDef_AlienRace))
                {
                    bool selected = Find.Selector.SingleSelectedThing == value;
                //    if (selected) Log.Message(string.Format("A Foul Xenos"));
                }
                __instance.graphics = new PawnGraphicSet_Invisible(value);
                __instance.graphics.nakedGraphic = new Graphic_Invisible();
                __instance.graphics.rottingGraphic = null;
                __instance.graphics.packGraphic = null;
                __instance.graphics.headGraphic = null;
                __instance.graphics.desiccatedHeadGraphic = null;
                __instance.graphics.skullGraphic = null;
                __instance.graphics.headStumpGraphic = null;
                __instance.graphics.desiccatedHeadStumpGraphic = null;
                __instance.graphics.hairGraphic = null;
            }
            return true;
        }

        public static bool PawnRenderer_XenosColour_Prefix(PawnRenderer __instance, ref Vector3 drawLoc, ref RotDrawMode bodyDrawType, bool headStump)
        {
            Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool selected = Find.Selector.SingleSelectedThing == value;
            PawnGraphicSet value2 = Traverse.Create(__instance).Field("graphics").GetValue<PawnGraphicSet>();
            Comp_Xenomorph _Xenomorph = value.TryGetComp<Comp_Xenomorph>();
            if (_Xenomorph!=null)
            {
                if (_Xenomorph.host != null)
                {
                //    if (selected) Log.Message(string.Format("Old value.kindDef.lifeStages[0].bodyGraphicData.color: {0}", value.kindDef.lifeStages[0].bodyGraphicData.color));
                //    if (selected) Log.Message(string.Format("_Xenomorph.host.race.race.BloodDef.graphicData.color: {0}", _Xenomorph.host.race.race.BloodDef.graphicData.color));
                    value.kindDef.lifeStages[0].bodyGraphicData.color =
                    _Xenomorph.host.race.race.BloodDef.graphicData.color;
                //    if (selected) Log.Message(string.Format("New value.kindDef.lifeStages[0].bodyGraphicData.color: {0}", value.kindDef.lifeStages[0].bodyGraphicData.color));
                }
                else if (selected) Log.Message(string.Format("_Xenomorph.host: null"));
            }
            else if (selected) Log.Message(string.Format("_Xenomorph: null"));
            return true;
        }

        public static void Post_GeneratePawn_Yautja(PawnGenerationRequest request, ref Pawn __result)
        {
            if (__result.kindDef.race == YautjaDefOf.RRY_Alien_Yautja)
            {
                Comp_Yautja _Yautja = __result.TryGetComp<Comp_Yautja>();
                if (_Yautja != null)
                {
                    Backstory pawnStoryC = __result.story.childhood;
                    Backstory pawnStoryA = __result.story.adulthood != null ? __result.story.adulthood : null;

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
            /*
            if (request.KindDef.race != ThingDefOf.Human && request.KindDef.RaceProps.Humanlike && request.KindDef.defName.Contains("StrangerInBlack") && request.Faction == Faction.OfPlayer)
            {
                PawnKindDef pawnKind = request.KindDef;
                Faction ofPlayer = Faction.OfPlayer;

                Log.Message(string.Format("{0}", ofPlayer.def.defName));
                var list = (from def in DefDatabase<PawnKindDef>.AllDefs
                            where ((def.race == ofPlayer.def.basicMemberKind.race) && (def.defName.Contains("StrangerInBlack")))
                            select def).ToList();
                if (list.Count > 0)
                {
                    pawnKind = list.RandomElement<PawnKindDef>();
                    pawnKind.defaultFactionType = ofPlayer.def;
                    __result.kindDef = pawnKind;
                }
                Log.Message(string.Format("{0}", pawnKind.defName));
                bool pawnMustBeCapableOfViolence = true;
                Gender? fixedGender = Gender.Male;
                request = new PawnGenerationRequest(pawnKind, ofPlayer, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, pawnMustBeCapableOfViolence, 20f, false, true, true, false, false, false, false, null, null, null, null, null, fixedGender, null, null);
                __result = PawnGenerator.GeneratePawn(request);
            }
            */
            if (Rand.Chance(0.005f)&&XenomorphUtil.isInfectablePawn(__result))
            {
                HediffDef def = Rand.Chance(0.75f) ? XenomorphDefOf.RRY_HiddenXenomorphImpregnation : XenomorphDefOf.RRY_HiddenNeomorphImpregnation;
                __result.health.AddHediff(def);
            }
        }

        public static void Post_GenerateRefugee_Yautja(Pawn request, ref Pawn __result)
        {
            
            if (Find.FactionManager.OfPlayer.def == YautjaDefOf.RRY_Yautja_PlayerBlooded || Find.FactionManager.OfPlayer.def == YautjaDefOf.RRY_Yautja_PlayerColony || Find.FactionManager.OfPlayer.def == YautjaDefOf.RRY_Yautja_PlayerUnblooded)
            {
            //    Log.Message(string.Format("Refugee Yautja"));
            }
        }

        private static FieldInfo pawnField_PawnRenderer;
    }
}