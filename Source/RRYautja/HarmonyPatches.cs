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

            var type = typeof(HarmonyPatches);
            harmony.Patch(
                AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) }), null,
                new HarmonyMethod(type, nameof(Post_GeneratePawn_Astartes)));

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
            bool flag = value.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_Cloaked, false);
            if (flag)
            {
                int blurTick = HediffUtility.TryGetComp<HediffComp_Blur>(value.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_Cloaked, false)).blurTick;
                bool flag2 = blurTick > Find.TickManager.TicksGame - 10;
                if (flag2)
                {
                    float num = (float)(10 / (Find.TickManager.TicksGame - blurTick + 1)) + 5f;
                    Vector3 vector = drawLoc;
                    vector.x += Rand.Range(-0.03f, 0.03f) * num;
                    drawLoc = vector;
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