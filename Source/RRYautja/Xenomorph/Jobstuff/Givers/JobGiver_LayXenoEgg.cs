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
            IntVec3 c = IntVec3.Invalid;
            CompXenoEggLayer compEggLayer = pawn.TryGetComp<CompXenoEggLayer>();
            if (compEggLayer == null || !compEggLayer.CanLayNow)
            {
                return null;
            }
            if (c == IntVec3.Invalid)
            {
                if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendAndExpandHiveLike LordJob_DefendAndExpandHiveLike)
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, LordJob_DefendAndExpandHiveLike.lord.Graph.StartingToil.FlagLoc, 3f, null, Danger.Some);
                    if (pawn.GetLord().CurLordToil is LordToil_DefendAndExpandHiveLike defender)
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, defender.myFocus.Cell, 3f, null, Danger.Some);
                    //    Log.Message(string.Format("pawn.GetLord().CurLordToil is LordToil_DefendAndExpandHiveLike @: {0}",c));
                    }
                    if (c == IntVec3.Invalid)
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, LordJob_DefendAndExpandHiveLike.lord.CurLordToil.FlagLoc, 3f, null, Danger.Some);
                    }
                }
                else if(pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendHiveLoc LordJob_DefendHiveLoc)
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, LordJob_DefendHiveLoc.lord.CurLordToil.FlagLoc, 3f, null, Danger.Some);
                    if (c == IntVec3.Invalid)
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, LordJob_DefendHiveLoc.lord.Graph.StartingToil.FlagLoc, 3f, null, Danger.Some);
                    }
                }
                else if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendPoint LordJob_DefendPoint)
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, LordJob_DefendPoint.lord.CurLordToil.FlagLoc, 5f, null, Danger.Some);
                    if (c == IntVec3.Invalid)
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, LordJob_DefendPoint.lord.Graph.StartingToil.FlagLoc, 5f, null, Danger.Some);
                    }
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
                        if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map, false) && pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
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
            if (c == IntVec3.Invalid)
            {
                if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob lordjob)
                {
                    c = RCellFinder.RandomWanderDestFor(pawn, lordjob.lord.Graph.StartingToil.FlagLoc, 5f, null, Danger.Some);
                    if (c == IntVec3.Invalid)
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, pawn.mindState.duty.focus.Cell, 5f, null, Danger.Some);
                    }
                }
                if (c == IntVec3.Invalid)
                {
                    if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map) && pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, c, 3f, null, Danger.Some);
                    }
                    else
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 3f, null, Danger.Some);
                    }
                }
            }
            return new Job(XenomorphDefOf.RRY_Job_LayXenomorphEgg, c);
        }

        // Token: 0x04000275 RID: 629
        private const float LayRadius = 3f;
    }
}
