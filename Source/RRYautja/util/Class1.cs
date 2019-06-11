using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001D6 RID: 470
    public class ThinkNode_ConditionalBleeding : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_ThreeQuatHealthBleeding : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f && pawn.health.summaryHealth.SummaryHealthPercent <= 0.75f && pawn.health.summaryHealth.SummaryHealthPercent >= 0.51f;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_HalfHealthBleeding : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f && pawn.health.summaryHealth.SummaryHealthPercent <= 0.5f && pawn.health.summaryHealth.SummaryHealthPercent >= 0.251f;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_QuatHealthBleeding : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.health.hediffSet.BleedRateTotal > 0.001f && pawn.health.summaryHealth.SummaryHealthPercent <= 0.25f;
        }
    }
}
