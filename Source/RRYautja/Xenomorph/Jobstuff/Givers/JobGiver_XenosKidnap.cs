using RRYautja;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000D6 RID: 214
    public class JobGiver_XenosKidnap : ThinkNode_JobGiver
    {
        // Token: 0x060004D1 RID: 1233 RVA: 0x0003100C File Offset: 0x0002F40C
        protected override Job TryGiveJob(Pawn pawn)
        {
            IntVec3 c;
            if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn)&&(!XenomorphUtil.EggsPresent(pawn.Map)|| XenomorphUtil.ClosestReachableEgg(pawn)==null))
            {
                return null;
            }
            if (XenomorphUtil.EggsPresent(pawn.Map)&& XenomorphUtil.ClosestReachableEgg(pawn) != null)
            {
                c = XenomorphUtil.ClosestReachableEggNeedsHost(pawn).Position.RandomAdjacentCell8Way();
            }
            Pawn t;
            if (KidnapAIUtility.TryFindGoodKidnapVictim(pawn, 18f, out t, null) && !GenAI.InDangerousCombat(pawn))
            {
                return new Job(XenomorphDefOf.RRY_Job_XenomorphKidnap)
                {
                    targetA = t,
                    targetB = c,
                    count = 1
                };
            }
            return null;
        }

        // Token: 0x040002AB RID: 683
        public const float VictimSearchRadiusInitial = 8f;

        // Token: 0x040002AC RID: 684
        private const float VictimSearchRadiusOngoing = 18f;
    }
}
