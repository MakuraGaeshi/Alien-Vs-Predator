using RRYautja;
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
            ThingDef namedA = XenomorphDefOf.RRY_Xenomorph_Cocoon_Humanoid;
            ThingDef namedB = XenomorphDefOf.RRY_Xenomorph_Cocoon_Animal;
            if (XenomorphKidnapUtility.TryFindGoodHiveLoc(pawn, out c))
            {
                bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) && (Prefs.DevMode) : false;
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

                    List<Thing> egglist = pawn.Map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).FindAll(x => lc.InHorDistOf(x.Position, 9));
                    bool eggflag = egglist.CountAllowNull() < 40;
                    return new Job(XenomorphDefOf.RRY_Job_Xenomorph_LayEgg, lc);
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

                List<Thing> egglist = pawn.Map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).FindAll(x => lc.InHorDistOf(x.Position, 9));
                bool eggflag = egglist.CountAllowNull() < 40;
                return new Job(XenomorphDefOf.RRY_Job_Xenomorph_LayEgg, lc);
            }
            return null;
        }

        // Token: 0x04000275 RID: 629
        private const float LayRadius = 3f;
    }
}
