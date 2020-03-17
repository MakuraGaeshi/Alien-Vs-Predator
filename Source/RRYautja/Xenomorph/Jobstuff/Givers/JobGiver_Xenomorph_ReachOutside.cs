using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000CAC RID: 3244
	public class JobGiver_Xenomorph_ReachOutside : JobGiver_Xenomorph_ExitMap
	{
		// Token: 0x06003DA8 RID: 15784 RVA: 0x0016D79C File Offset: 0x0016B99C
		protected override bool TryFindGoodExitDest(Pawn pawn, bool canDig, out IntVec3 spot)
		{
			TraverseMode mode = canDig ? TraverseMode.PassAllDestroyableThings : TraverseMode.ByPawn;
			return RCellFinder.TryFindRandomSpotJustOutsideColony(pawn, out spot); //RCellFinder.TryFindBestExitSpot(pawn, out spot, mode);
		}

	}
}
