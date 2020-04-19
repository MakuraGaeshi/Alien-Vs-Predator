﻿using AvP;
using AvP.ExtensionMethods;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x020000A7 RID: 167
    public class JobGiver_LayXenoEgg : ThinkNode_JobGiver
    {
        ThingDef namedA = XenomorphDefOf.AvP_Xenomorph_Cocoon_Humanoid;
        ThingDef namedB = XenomorphDefOf.AvP_Xenomorph_Cocoon_Animal;
        // Token: 0x06000424 RID: 1060 RVA: 0x0002CE84 File Offset: 0x0002B284
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Map == null || pawn.Map != Find.CurrentMap)
            {
                return null;
            }
            IntVec3 c = IntVec3.Invalid;
            CompXenoEggLayer compEggLayer = pawn.TryGetComp<CompXenoEggLayer>();
            if (compEggLayer == null || !compEggLayer.CanLayNow)
            {
                return null;
            }
            if (!pawn.isXenomorph(out Comp_Xenomorph xenomorph))
            {
                return null;
            }
            else
            {
                c = xenomorph.HiveLoc;
                bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) && (Prefs.DevMode) : false;
                if (pawn.CanReach(c, PathEndMode.ClosestTouch, Danger.Deadly, true, TraverseMode.PassAllDestroyableThingsNotWater))
                {
                    using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, c, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassAllDestroyableThings, false), PathEndMode.OnCell))
                    {
                        IntVec3 cellBeforeBlocker;
                        Thing thing = pawnPath.FirstBlockingBuilding(out cellBeforeBlocker, pawn);
                        if (thing != null)
                        {
                            Job job = DigUtility.PassBlockerJob(pawn, thing, cellBeforeBlocker, true, true);
                            if (job != null)
                            {
                                return job;
                            }
                        }
                    }
                    Log.Message("queen can reach eggsite");
                    Predicate<IntVec3> validator = delegate (IntVec3 y)
                    {
                        bool adjacent = c.AdjacentTo8WayOrInside(y);
                        bool filled = y.Filled(pawn.Map);
                        bool edifice = y.GetEdifice(pawn.Map).DestroyedOrNull();
                        bool building = y.GetFirstBuilding(pawn.Map).DestroyedOrNull();
                        bool thingA = y.GetFirstThing(pawn.Map, namedA).DestroyedOrNull();
                        bool thingB = y.GetFirstThing(pawn.Map, namedB).DestroyedOrNull();
                        return !adjacent && !filled && edifice && building && thingA && thingB;
                    };

                    bool b = RCellFinder.TryFindRandomCellNearWith(c, validator, pawn.Map, out IntVec3 lc, 6, 12);

                    List<Thing> egglist = pawn.Map.listerThings.ThingsOfDef(XenomorphDefOf.AvP_EggXenomorphFertilized).FindAll(x => lc.InHorDistOf(x.Position, 9));
                    bool eggflag = egglist.CountAllowNull() < 40;
                    if (pawn.GetLord() == null)
                    {
                        //    Log.Message(string.Format("XenoLord TryFindGoodHiveLoc for {0} Cell Found: {1}, Allow: Fogged, Digging", pawn.LabelShortCap, c));
                        LordJob newJob = new LordJob_DefendAndExpandHiveLike(false, pawn.Faction, c, 40f);
                        pawn.CreateNewLord(c, newJob);
                    }
                    return new Job(XenomorphDefOf.AvP_Job_Xenomorph_LayEgg, lc);
                }
                else
                {
                    Log.Warning("queen couldnt reach eggsite");
                }
                if (XenomorphKidnapUtility.TryFindGoodHiveLoc(pawn, out c))
                {
                    if (c != IntVec3.Invalid && pawn.CanReach(c, PathEndMode.ClosestTouch, Danger.Deadly, true, TraverseMode.PassAllDestroyableThings))
                    {
                        Predicate<IntVec3> validator = delegate (IntVec3 y)
                        {
                            bool adjacent = c.AdjacentTo8WayOrInside(y);
                            bool filled = y.Filled(pawn.Map);
                            bool edifice = y.GetEdifice(pawn.Map).DestroyedOrNull();
                            bool building = y.GetFirstBuilding(pawn.Map).DestroyedOrNull();
                            bool thingA = y.GetFirstThing(pawn.Map, namedA).DestroyedOrNull();
                            bool thingB = y.GetFirstThing(pawn.Map, namedB).DestroyedOrNull();
                            return !adjacent && !filled && edifice && building && thingA && thingB;
                        };

                        bool b = RCellFinder.TryFindRandomCellNearWith(c, validator, pawn.Map, out IntVec3 lc, 6, 12);

                        List<Thing> egglist = pawn.Map.listerThings.ThingsOfDef(XenomorphDefOf.AvP_EggXenomorphFertilized).FindAll(x => lc.InHorDistOf(x.Position, 9));
                        bool eggflag = egglist.CountAllowNull() < 40;
                        return new Job(XenomorphDefOf.AvP_Job_Xenomorph_LayEgg, lc);
                    }
                    else
                    {
                        if (pawn.jobs.debugLog) pawn.jobs.DebugLogEvent(string.Format("{0} something went wrong", this));
                    }
                }
                else
                {
                    if (pawn.jobs.debugLog) pawn.jobs.DebugLogEvent(string.Format("{0} No Egglaying spot Found", this));
                    c = pawn.Position;
                    Predicate<IntVec3> validator = delegate (IntVec3 y)
                    {
                        bool adjacent = c.AdjacentTo8WayOrInside(y);
                        bool filled = y.Filled(pawn.Map);
                        bool edifice = y.GetEdifice(pawn.Map).DestroyedOrNull();
                        bool building = y.GetFirstBuilding(pawn.Map).DestroyedOrNull();
                        bool thingA = y.GetFirstThing(pawn.Map, namedA).DestroyedOrNull();
                        bool thingB = y.GetFirstThing(pawn.Map, namedB).DestroyedOrNull();
                        return !adjacent && !filled && edifice && building && thingA && thingB;
                    };

                    bool b = RCellFinder.TryFindRandomCellNearWith(c, validator, pawn.Map, out IntVec3 lc, 6, 12);

                    List<Thing> egglist = pawn.Map.listerThings.ThingsOfDef(XenomorphDefOf.AvP_EggXenomorphFertilized).FindAll(x => lc.InHorDistOf(x.Position, 9));
                    bool eggflag = egglist.CountAllowNull() < 40;
                    return new Job(XenomorphDefOf.AvP_Job_Xenomorph_LayEgg, lc);
                }
            }
            return null;
        }

        // Token: 0x04000275 RID: 629
        private const float LayRadius = 3f;
    }
}
