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
            Comp_Xenomorph _Xenomorph = pawn.TryGetComp<Comp_Xenomorph>();
            if (_Xenomorph!=null)
            {
                if (_Xenomorph.HiveLoc!=IntVec3.Invalid && _Xenomorph.HiveLoc != IntVec3.Zero)
                {
                    c = _Xenomorph.HiveLoc;
               //     Log.Message(string.Format("Laying Egg @ Comp_Xenomorph _Xenomorph.HiveLoc: {0}", c));
                }
            }
            if (c == IntVec3.Invalid || c == IntVec3.Zero)
            {
                if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendAndExpandHiveLike LordJob_DefendAndExpandHiveLike)
                {
                    c = LordJob_DefendAndExpandHiveLike.lord.Graph.StartingToil.FlagLoc;
                    foreach (var item in LordJob_DefendAndExpandHiveLike.lord.Graph.lordToils)
                    {
                   //     Log.Message(string.Format("lordToils: {0}", item));
                    }
                    if (pawn.GetLord().CurLordToil is LordToil_DefendAndExpandHiveLike defender)
                    {
                        if (pawn.DutyLocation() != IntVec3.Invalid || pawn.DutyLocation() != IntVec3.Zero && (c == IntVec3.Invalid || c == IntVec3.Zero))
                        {
                            c = pawn.DutyLocation();
                       //     Log.Message(string.Format("Laying Egg @ LordToil_DefendAndExpandHiveLike pawn.DutyLocation(): {0}", c));
                        }
                        else if(defender.FlagLoc != IntVec3.Invalid || defender.FlagLoc != IntVec3.Zero && (c == IntVec3.Invalid || c == IntVec3.Zero))
                        {
                            c = defender.FlagLoc;
                       //     Log.Message(string.Format("Laying Egg @ LordToil_DefendAndExpandHiveLike defender.FlagLoc: {0}", c));
                        }
                        else if (defender.myFocus.Cell != IntVec3.Invalid || defender.myFocus.Cell != IntVec3.Zero && (c == IntVec3.Invalid || c == IntVec3.Zero))
                        {
                            c = defender.myFocus.Cell;
                       //     Log.Message(string.Format("Laying Egg @ LordToil_DefendAndExpandHiveLike defender.myFocus.Cell : {0}", c));
                        }
                    }
                    if (c == IntVec3.Invalid || c == IntVec3.Zero)
                    {
                        c = LordJob_DefendAndExpandHiveLike.lord.CurLordToil.FlagLoc;
                   //     Log.Message(string.Format("Laying Egg @ LordToil_DefendAndExpandHiveLike CurLordToil.FlagLoc @: {0}", c));
                    }
                }
                else if(pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendHiveLoc LordJob_DefendHiveLoc)
                {
                    c = LordJob_DefendHiveLoc.lord.CurLordToil.FlagLoc;
                    if (c == IntVec3.Invalid)
                    {
                        c = LordJob_DefendHiveLoc.lord.Graph.StartingToil.FlagLoc;
                    }
                }
                else if (pawn.GetLord() != null && pawn.GetLord().LordJob is LordJob_DefendPoint LordJob_DefendPoint)
                {
                    c = LordJob_DefendPoint.lord.CurLordToil.FlagLoc;
                    if (c == IntVec3.Invalid)
                    {
                        c = LordJob_DefendPoint.lord.Graph.StartingToil.FlagLoc;
                    }
                }
                else
                {
                    Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
                    if (thing != null)
                    {
                        c = thing.Position;
                    }
                    else
                    {
                        if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map, false) && pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                        {
                        //    c = c;
                        }
                        else
                        {
                            c = thing.Position;
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
                    c = lordjob.lord.Graph.StartingToil.FlagLoc;
                    if (c == IntVec3.Invalid)
                    {
                        c = pawn.mindState.duty.focus.Cell;
                    }
                }
                if (c == IntVec3.Invalid)
                {
                    if (InfestationLikeCellFinder.TryFindCell(out c, pawn.Map) && pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                    {
                     //   c = c;
                    }
                    else
                    {
                        c = pawn.Position;
                    }
                }
            }
       //     Log.Message(string.Format("finding valid location near: {0}", c));
            Predicate<IntVec3> validator = delegate (IntVec3 t)
            {
                return t.GetFirstBuilding(pawn.Map) == null;
            };
            IntVec3 fc = IntVec3.Invalid;
            RCellFinder.TryFindRandomCellNearWith(c, validator, pawn.Map, out fc,3,16);
            if (fc != IntVec3.Invalid)
            {
           //     Log.Message(string.Format("valid location found: {0}", fc));
                return new Job(XenomorphDefOf.RRY_Job_LayXenomorphEgg, fc);
            }
            return null;
        }

        // Token: 0x04000275 RID: 629
        private const float LayRadius = 3f;
    }
}
