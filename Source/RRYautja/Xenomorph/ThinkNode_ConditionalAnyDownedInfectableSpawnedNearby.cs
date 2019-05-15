using RRYautja;
using System;
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
                result = pawn.Map.mapPawns.AllPawns.Any((Pawn x) => x.Downed && XenomorphUtil.isInfectablePawn(x));
            }
            else if(pawn.Spawned && XenomorphUtil.IsXenomorph(pawn) && XenomorphUtil.EggsPresent(pawn.Map))
            {
                result = pawn.Map.mapPawns.AllPawns.Any((Pawn x) => x.Downed && XenomorphUtil.isInfectablePawn(x) && XenomorphUtil.DistanceBetween(XenomorphUtil.ClosestReachableEgg(x).Position, x.Position) > 3f);
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
