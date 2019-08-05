using RRYautja.ExtensionMethods;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000F3 RID: 243
    public class JobGiver_XenomorphSelfTend : ThinkNode_JobGiver
    {
        // Token: 0x0600052D RID: 1325 RVA: 0x00033CB8 File Offset: 0x000320B8
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.isXenomorph() && !pawn.isNeomorph() || !pawn.health.HasHediffsNeedingTend(false) || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || pawn.InAggroMentalState)
            {
                return null;
            }
            return new Job(JobDefOf.TendPatient, pawn)
            {
                endAfterTendedOnce = true
            };
        }
    }
}
