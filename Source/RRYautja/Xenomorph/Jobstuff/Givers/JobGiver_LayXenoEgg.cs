using RRYautja;
using System;
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
            IntVec3 c = IntVec3.Zero;
            CompXenoEggLayer compEggLayer = pawn.TryGetComp<CompXenoEggLayer>();
            if (compEggLayer == null || !compEggLayer.CanLayNow)
            {
                return null;
            }
            if (c== IntVec3.Zero)
            {
#if DEBUG
                if (Prefs.DevMode)
                {
                //    Log.Message(string.Format("JobGiver_LayXenoEgg TryGiveJob \n{0} has location: {1}", pawn.LabelShort, c));
                }
#endif
                if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendHiveLoc lordjob)
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, lordjob.lord.Graph.StartingToil.FlagLoc, 5f, null, Danger.Some);
                }
                else
                {
                    Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
                    if (thing != null)
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 5f, null, Danger.Some);
                    }
                    else
                    {
                        if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map) && pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                        {
                            c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                        }
                        else
                        {

                            c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 3f, null, Danger.Some);
                        }
                    }
                }
            }
#if DEBUG
            if (Prefs.DevMode)
            {
            //    Log.Message(string.Format("JobGiver_LayXenoEgg TryGiveJob \n{0} has location: {1}", pawn.LabelShort, c));
            }
#endif
            return new Job(XenomorphDefOf.RRY_Job_LayXenomorphEgg, c);
        }

        // Token: 0x04000275 RID: 629
        private const float LayRadius = 5f;
    }
}
