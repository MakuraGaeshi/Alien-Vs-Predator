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
            CompXenoEggLayer compEggLayer = pawn.TryGetComp<CompXenoEggLayer>();
            if (compEggLayer == null || !compEggLayer.CanLayNow)
            {
                return null;
            }
            IntVec3 c;
            if (pawn.GetLord()!=null && pawn.GetLord().LordJob is LordJob_DefendBase lordjob)
            {
                c = lordjob.lord.Graph.StartingToil.FlagLoc;
                c = RCellFinder.RandomWanderDestFor(pawn, c, 5f, null, Danger.Some);
            }
            else
            {
                Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
                if (thing == null)
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 5f, null, Danger.Some);
                }
                else
                {
                    InfestationCellFinder.TryFindCell(out c, pawn.Map);
                    if (pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                    }
                    else
                    {

                        c = RCellFinder.RandomWanderDestFor(pawn, thing.Position, 3f, null, Danger.Some);
                    }
                }
            }
            return new Job(XenomorphDefOf.LayXenomorphEgg, c);
        }

        // Token: 0x04000275 RID: 629
        private const float LayRadius = 5f;
    }
}
