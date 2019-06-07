using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000D6 RID: 214
    public class JobGiver_PredalienImpregnate : ThinkNode_JobGiver
    {
        // Token: 0x060004D1 RID: 1233 RVA: 0x0003100C File Offset: 0x0002F40C
        protected override Job TryGiveJob(Pawn pawn)
        {
        //    Log.Message(string.Format("JobGiver_PredalienImpregnate TryGiveJob"));
            Pawn t;
            if (XenomorphKidnapUtility.TryFindGoodImpregnateVictim(pawn, 18f, out t, null) && !GenAI.InDangerousCombat(pawn))
            {
            //    Log.Message(string.Format("victim found: {0}",t.LabelShortCap));
                return new Job(XenomorphDefOf.RRY_Job_PredalienImpregnate)
                {
                    targetA = t
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
