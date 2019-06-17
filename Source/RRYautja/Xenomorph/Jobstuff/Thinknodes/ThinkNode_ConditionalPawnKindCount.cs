using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001EC RID: 492
    public class ThinkNode_ConditionalPawnKindCountGreater : ThinkNode_Conditional
    {
        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalPawnKindCountGreater thinkNode_ConditionalCountPawnKind = (ThinkNode_ConditionalPawnKindCountGreater)base.DeepCopy(resolve);
            thinkNode_ConditionalCountPawnKind.pawnKind = this.pawnKind;
            thinkNode_ConditionalCountPawnKind.pawnKindCount = this.pawnKindCount;
            return thinkNode_ConditionalCountPawnKind;
        }

        // Token: 0x060009B9 RID: 2489 RVA: 0x0004E0A3 File Offset: 0x0004C4A3
        protected override bool Satisfied(Pawn pawn)
        {
            Log.Message(string.Format("{0} needs < {2} {3}, Result: {1}", this, pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).Count < this.pawnKindCount || pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).NullOrEmpty(), pawnKindCount, pawnKind.LabelCap));
            return pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).Count > this.pawnKindCount && !pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).NullOrEmpty();
        }

        // Token: 0x040003F6 RID: 1014
        public PawnKindDef pawnKind;
        public int pawnKindCount;
    }

    // Token: 0x020001EC RID: 492
    public class ThinkNode_ConditionalPawnKindCountLesser : ThinkNode_Conditional
    {
        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalPawnKindCountLesser thinkNode_ConditionalCountPawnKind = (ThinkNode_ConditionalPawnKindCountLesser)base.DeepCopy(resolve);
            thinkNode_ConditionalCountPawnKind.pawnKind = this.pawnKind;
            thinkNode_ConditionalCountPawnKind.pawnKindCount = this.pawnKindCount;
            return thinkNode_ConditionalCountPawnKind;
        }

        // Token: 0x060009B9 RID: 2489 RVA: 0x0004E0A3 File Offset: 0x0004C4A3
        protected override bool Satisfied(Pawn pawn)
        {
            Log.Message(string.Format("{0} needs < {2} {3}, Result: {1}", this, pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).Count < this.pawnKindCount || pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).NullOrEmpty(), pawnKindCount, pawnKind.LabelCap));
            return pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).Count < this.pawnKindCount || pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.kindDef == pawnKind).NullOrEmpty();
        }

        // Token: 0x040003F6 RID: 1014
        public PawnKindDef pawnKind;
        public int pawnKindCount;
    }
}
