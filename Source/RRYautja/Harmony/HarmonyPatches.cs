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

            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            }, null), new HarmonyMethod(typeof(HarmonyPatches), "PawnRenderer_Blur_Prefix", null), null, null);

            harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveAllGraphics", null, null), new HarmonyMethod(typeof(HarmonyPatches), "ResolveAllGraphicsPostfix", null), null, null);
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

        // Token: 0x0600000C RID: 12 RVA: 0x0000283C File Offset: 0x00000A3C
        public static void ResolveAllGraphicsPostfix(PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            Log.Message(string.Format("tet"));
            Hediff_Cloak hd = (Hediff_Cloak)pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Cloaked);
            if (__instance is PawnGraphicSet_Invisible graphics)
            {
                Log.Message(string.Format("tet2"));
                graphics.ClearCache();
                graphics.nakedGraphic = new Graphic_Invisible();
                graphics.ResolveAllGraphics();
                graphics.rottingGraphic = null;
                graphics.packGraphic = null;
                graphics.headGraphic = null;
                graphics.desiccatedHeadGraphic = null;
                graphics.skullGraphic = null;
                graphics.headStumpGraphic = null;
                graphics.desiccatedHeadStumpGraphic = null;
                graphics.hairGraphic = null;
                ShadowData shadowData = new ShadowData();
                shadowData.volume = new Vector3(0, 0, 0);
                shadowData.offset = new Vector3(0, 0, 0);
                hd.SetShadowGraphic(pawn.Drawer.renderer, new Graphic_Shadow(shadowData));
            }
        }

        // Verse.AI.Pawn_PathFollower
        public static void PathOfNature(Pawn_PathFollower __instance, Pawn pawn, IntVec3 c, ref int __result)
        {
            if (pawn?.GetComp<Comp_Yautja>() is Comp_Yautja comp_Yautja)
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
            //bool flag = value.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            if (false)
            {
                int blurTick = HediffUtility.TryGetComp<HediffComp_Blur>(value.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Cloaked, false)).blurTick;
            //    Hediff_Cloak Cloak = (Hediff_Cloak)value.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Cloaked, false);
                bool flag2 = blurTick > Find.TickManager.TicksGame - 10;
            //    value2 = new PawnGraphicSet_Invisible(value);

                //    Log.Message(string.Format("blur blurtick: {1} > {2} == flag2:{0}", flag2, blurTick, (Find.TickManager.TicksGame - 10)));
                if (false)
                {
                //    Cloak.PostAdd(null);
                    float num = (float)(10 / (Find.TickManager.TicksGame - blurTick + 1)) + 5f;
                    Vector3 vector = drawLoc;
                    vector.x += Rand.Range(-0.03f, 0.03f) * num;
                    drawLoc = vector;
                //    Log.Message(string.Format("blue drawLoc:{0}", drawLoc));
                }
            }
            return true;
        }

        public static void Post_GeneratePawn_Astartes(PawnGenerationRequest request, ref Pawn __result)
        {
            Backstory pawnStoryA;
            Backstory pawnStoryC;
            HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
            HediffDef bloodedbyDef;
            PawnKindDef pawnKindDef = request.KindDef;
            if (pawnKindDef.race == YautjaDefOf.Alien_Yautja)
            {
                __result.relations.ClearAllRelations();
            }
            if (__result == null)
            {
                request = new PawnGenerationRequest(request.KindDef, request.Faction, request.Context, -1, true, false, false, false, false, true, 1f);
                return;
            }
            if (__result.kindDef.race == YautjaDefOf.Alien_Yautja)
            {
               
                    request = new PawnGenerationRequest(request.KindDef, request.Faction, request.Context, -1, true, false, false, false, false, true, 1f);
                    __result = PawnGenerator.GeneratePawn(request);

                
            }
            return;
        }

        private static FieldInfo pawnField_PawnRenderer;
    }
}