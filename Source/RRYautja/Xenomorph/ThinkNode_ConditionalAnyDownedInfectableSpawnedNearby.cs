using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001EA RID: 490
    public class ThinkNode_ConditionalAnyDownedInfectableSpawnedNearby : ThinkNode_Conditional
    {
        // Token: 0x060009B3 RID: 2483 RVA: 0x0004DFD8 File Offset: 0x0004C3D8
        protected override bool Satisfied(Pawn pawn)
        {
            bool result;
            if (pawn.Spawned && XenomorphUtil.IsXenomorph(pawn))
            {
                if (!XenomorphUtil.EggsPresent(pawn.Map))
                {
                    Log.Message(string.Format("pawn.Spawned && XenomorphUtil.IsXenomorph(pawn) && !XenomorphUtil.EggsPresent(pawn.Map)"));
                    List<Pawn> list = pawn.Map.mapPawns.AllPawns.Where((Pawn x) => x.Downed && XenomorphUtil.isInfectablePawn(x) && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.NoPassClosedDoors)).ToList();
                    result = list.Any<Pawn>(x => x.Spawned);
                }
                else
                {
                    Log.Message(string.Format("pawn.Spawned && XenomorphUtil.IsXenomorph(pawn) && XenomorphUtil.EggsPresent(pawn.Map)"));
                    List<Pawn> list = pawn.Map.mapPawns.AllPawns.Where((Pawn x) => x.Downed && XenomorphUtil.isInfectablePawn(x) && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.NoPassClosedDoors)).ToList();
                    result = list.Any<Pawn>(x => x.Spawned && XenomorphUtil.DistanceBetween(XenomorphUtil.ClosestReachableEgg(x).Position, x.Position) > 3f);
                }
            }
            else
            {
                Log.Message(string.Format("false"));
                result = false;
            }
            Log.Message(string.Format("result: {0}", result));
            return result;
        }

    }
}
