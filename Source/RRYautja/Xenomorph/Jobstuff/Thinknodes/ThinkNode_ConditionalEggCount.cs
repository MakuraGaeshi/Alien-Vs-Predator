using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001EC RID: 492
    public class ThinkNode_ConditionalEggCount : ThinkNode_Conditional
    {
        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalEggCount thinkNode_ConditionalXenoEggCount = (ThinkNode_ConditionalEggCount)base.DeepCopy(resolve);
            thinkNode_ConditionalXenoEggCount.eggDef = this.eggDef;
            thinkNode_ConditionalXenoEggCount.eggCount = this.eggCount;
            return thinkNode_ConditionalXenoEggCount;
        }

        // Token: 0x060009B9 RID: 2489 RVA: 0x0004E0A3 File Offset: 0x0004C4A3
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.Map.listerThings.ThingsOfDef(XenomorphDefOf.AvP_EggXenomorphFertilized).Count > this.eggCount;
        }

        // Token: 0x040003F6 RID: 1014
        public ThingDef eggDef;
        public int eggCount;
    }

}
