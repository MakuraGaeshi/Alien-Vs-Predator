using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001BE RID: 446
    public class ThinkNode_ConditionalQueenPresent : ThinkNode_Conditional
    {
        // Token: 0x06000956 RID: 2390 RVA: 0x0004D678 File Offset: 0x0004BA78
        protected override bool Satisfied(Pawn pawn)
        {
            PawnKindDef Queen = XenomorphDefOf.RRY_Xenomorph_Queen;
            return pawn.Map.mapPawns.AllPawnsSpawned.Any(x => x.kindDef == Queen);
        }

    }

    public class ThinkNode_ConditionalQueenAbsent : ThinkNode_Conditional
    {
        // Token: 0x06000956 RID: 2390 RVA: 0x0004D678 File Offset: 0x0004BA78
        protected override bool Satisfied(Pawn pawn)
        {
            PawnKindDef Queen = XenomorphDefOf.RRY_Xenomorph_Queen;
            return !pawn.Map.mapPawns.AllPawnsSpawned.Any(x => x.kindDef == Queen);
        }

    }
}
