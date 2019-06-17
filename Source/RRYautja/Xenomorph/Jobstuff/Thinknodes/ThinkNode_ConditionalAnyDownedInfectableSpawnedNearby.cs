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
                List<Pawn> list = pawn.Map.mapPawns.AllPawns.Where((Pawn x) => x.RaceProps.FleshType != XenomorphRacesDefOf.RRY_Xenomorph && x.RaceProps.FleshType != XenomorphRacesDefOf.RRY_Neomorph && !x.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) && x.Downed && XenomorphUtil.isInfectablePawn(x) && pawn.CanReach(x, PathEndMode.InteractionCell, Danger.Deadly, false, TraverseMode.NoPassClosedDoors)).ToList();
                result = !list.NullOrEmpty() ? list.Any<Pawn>(x => x.Spawned) : false;
            }
            else
            {
                result = false;
            }
            Log.Message(string.Format("{0} Result: {1}", this, result));
            return result;
        }

    }
}
