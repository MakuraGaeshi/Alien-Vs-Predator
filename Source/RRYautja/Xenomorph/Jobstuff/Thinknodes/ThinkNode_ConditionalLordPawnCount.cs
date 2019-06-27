using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x020001EC RID: 492
    public class ThinkNode_ConditionalLordPawnCount : ThinkNode_Conditional
    {
        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalLordPawnCount thinkNode_ConditionalPawnKind = (ThinkNode_ConditionalLordPawnCount)base.DeepCopy(resolve);
            thinkNode_ConditionalPawnKind.pawnKind = this.pawnKind;
            thinkNode_ConditionalPawnKind.pawnKindCount = this.pawnKindCount;
            return thinkNode_ConditionalPawnKind;
        }

        // Token: 0x060009B9 RID: 2489 RVA: 0x0004E0A3 File Offset: 0x0004C4A3
        protected override bool Satisfied(Pawn pawn)
        {
            Lord lord = pawn.GetLord();
            int lordpawncount = lord.ownedPawns.Count;
            if (Find.Selector.SelectedObjects.Contains(pawn)) Log.Message(string.Format("{0} needs {3} > {2}, Result: {1}", this, pawn.kindDef != this.pawnKind, pawn.kindDef, pawnKind));
            return pawn.kindDef != this.pawnKind;
        }

        // Token: 0x040003F6 RID: 1014
        public PawnKindDef pawnKind;
        public int pawnKindCount;
    }
}
