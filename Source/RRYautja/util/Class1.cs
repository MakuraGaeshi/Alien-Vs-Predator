using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001D6 RID: 470
    public class ThinkNode_ConditionalBleeding : ThinkNode_Conditional
    {
        // Token: 0x06000988 RID: 2440 RVA: 0x0004DAF5 File Offset: 0x0004BEF5
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_ThreeQuatHealthBleeding : ThinkNode_Conditional
    {
        // Token: 0x06000988 RID: 2440 RVA: 0x0004DAF5 File Offset: 0x0004BEF5
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f && pawn.health.summaryHealth.SummaryHealthPercent <= 0.75f && pawn.health.summaryHealth.SummaryHealthPercent >= 0.51f;
        }
    }
    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_HalfHealthBleeding : ThinkNode_Conditional
    {
        // Token: 0x06000988 RID: 2440 RVA: 0x0004DAF5 File Offset: 0x0004BEF5
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f && pawn.health.summaryHealth.SummaryHealthPercent <= 0.5f && pawn.health.summaryHealth.SummaryHealthPercent >= 0.251f;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_QuatHealthBleeding : ThinkNode_Conditional
    {
        // Token: 0x06000988 RID: 2440 RVA: 0x0004DAF5 File Offset: 0x0004BEF5
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f && pawn.health.summaryHealth.SummaryHealthPercent <= 0.25f;
        }
    }
}
