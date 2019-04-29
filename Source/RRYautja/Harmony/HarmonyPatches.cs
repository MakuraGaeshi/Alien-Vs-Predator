using AlienRace;
using Harmony;
using RimWorld;
using System;
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
            //graphics
            bool flag = value.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked);
            if (flag)
            {
                if (value.kindDef.race.GetType() == typeof(AlienRace.ThingDef_AlienRace))
                {
                    bool selected = Find.Selector.SingleSelectedThing == value;
                    if (selected) Log.Message(string.Format("A Foul Xenos"));
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
                    if (selected) Log.Message(string.Format("Old value.kindDef.lifeStages[0].bodyGraphicData.color: {0}", value.kindDef.lifeStages[0].bodyGraphicData.color));
                    if (selected) Log.Message(string.Format("_Xenomorph.host.race.race.BloodDef.graphicData.color: {0}", _Xenomorph.host.race.race.BloodDef.graphicData.color));
                    value.kindDef.lifeStages[0].bodyGraphicData.color =
                    _Xenomorph.host.race.race.BloodDef.graphicData.color;
                    if (selected) Log.Message(string.Format("New value.kindDef.lifeStages[0].bodyGraphicData.color: {0}", value.kindDef.lifeStages[0].bodyGraphicData.color));
                }
                else if (selected) Log.Message(string.Format("_Xenomorph.host: null"));
            }
            else if (selected) Log.Message(string.Format("_Xenomorph: null"));
            return true;
        }

        public static void Post_GeneratePawn_Yautja(PawnGenerationRequest request, ref Pawn __result)
        {
            if (__result.kindDef.race == YautjaDefOf.Alien_Yautja)
            {
                Comp_Yautja _Yautja = __result.TryGetComp<Comp_Yautja>();
                if (_Yautja!=null)
                {
                    Backstory pawnStoryC = __result.story.childhood;
                    Backstory pawnStoryA = __result.story.adulthood != null ? __result.story.adulthood : null;

                    AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("Yautja_YoungBlood");
                    AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("Yautja_Blooded");
                    AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("Yautja_BadBloodA");
                    AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("Yautja_BadBloodB");

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
                        __result.health.AddHediff(hediffDef);
                    }
                    else
                    {
                        Log.Message(string.Format("new pawn has hasunblooded:{0}, hasbloodedUM:{1}, hasbloodedM:{2}", hasunblooded, hasbloodedUM, hasbloodedM));
                    }
                }
            }
        }

        private static FieldInfo pawnField_PawnRenderer;
    }
}