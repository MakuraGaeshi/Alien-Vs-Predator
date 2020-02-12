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

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_OverHealth : ThinkNode_Conditional
    {
        // Token: 0x040003F6 RID: 1014
        public float pawnHealth;

        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_Conditional_OverHealth thinkNode_Conditional_OverHealth = (ThinkNode_Conditional_OverHealth)base.DeepCopy(resolve);
            thinkNode_Conditional_OverHealth.pawnHealth = this.pawnHealth;
            return thinkNode_Conditional_OverHealth;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.jobs.debugLog) pawn.jobs.DebugLogEvent(string.Format("{0} needs {3} > {2}, Result: {1}", this, pawn.health.summaryHealth.SummaryHealthPercent >= pawnHealth, pawnHealth, pawn.health.summaryHealth.SummaryHealthPercent));
            
            return pawn.health.summaryHealth.SummaryHealthPercent >= pawnHealth && pawn.health.hediffSet.PainTotal < pawnHealth;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_UnderHealth : ThinkNode_Conditional
    {
        // Token: 0x040003F6 RID: 1014
        public float pawnHealth;

        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_Conditional_UnderHealth thinkNode_Conditional_UnderHealth = (ThinkNode_Conditional_UnderHealth)base.DeepCopy(resolve);
            thinkNode_Conditional_UnderHealth.pawnHealth = this.pawnHealth;
            return thinkNode_Conditional_UnderHealth;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.jobs.debugLog)  pawn.jobs.DebugLogEvent(string.Format("{0} needs {3} < {2}, Result: {1}", this, pawn.health.summaryHealth.SummaryHealthPercent <= pawnHealth, pawnHealth, pawn.health.summaryHealth.SummaryHealthPercent));
            
            return pawn.health.summaryHealth.SummaryHealthPercent <= pawnHealth;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_OverBleed : ThinkNode_Conditional
    {
        // Token: 0x040003F6 RID: 1014
        public float pawnBleedRate;

        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_Conditional_OverBleed thinkNode_Conditional_BleedRate = (ThinkNode_Conditional_OverBleed)base.DeepCopy(resolve);
            thinkNode_Conditional_BleedRate.pawnBleedRate = this.pawnBleedRate;
            return thinkNode_Conditional_BleedRate;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.jobs.debugLog) pawn.jobs.DebugLogEvent(string.Format("{0} needs {3} > {2}, Result: {1}", this, pawn.health.summaryHealth.SummaryHealthPercent >= pawnBleedRate, pawnBleedRate, pawn.health.summaryHealth.SummaryHealthPercent));
            return pawn.health.hediffSet.BleedRateTotal >= pawnBleedRate;
        }
    }

    // Token: 0x020001D6 RID: 470
    public class ThinkNode_Conditional_UnderBleed : ThinkNode_Conditional
    {
        // Token: 0x040003F6 RID: 1014
        public float pawnBleedRate;

        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_Conditional_UnderBleed thinkNode_Conditional_BleedRate = (ThinkNode_Conditional_UnderBleed)base.DeepCopy(resolve);
            thinkNode_Conditional_BleedRate.pawnBleedRate = this.pawnBleedRate;
            return thinkNode_Conditional_BleedRate;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.jobs.debugLog) pawn.jobs.DebugLogEvent(string.Format("{0} needs {3} < {2}, Result: {1}", this, pawn.health.summaryHealth.SummaryHealthPercent <= pawnBleedRate, pawnBleedRate, pawn.health.summaryHealth.SummaryHealthPercent));
            return pawn.health.hediffSet.BleedRateTotal <= pawnBleedRate;
        }
    }

}
