using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AvP
{
    // Token: 0x02000012 RID: 18
    public class WorkGiver_TendSelf : WorkGiver
    {
        // Token: 0x06000047 RID: 71 RVA: 0x00003420 File Offset: 0x00001620
        public override Job NonScanJob(Pawn pawn)
        {
            ThinkResult thinkResult = WorkGiver_TendSelf.jgp.TryIssueJobPackage(pawn, default(JobIssueParams));
            bool isValid = thinkResult.IsValid;
            Job result;
            if (isValid)
            {
                result = thinkResult.Job;
            }
            else
            {
                result = null;
            }
            return result;
        }

        // Token: 0x04000020 RID: 32
        private static JobGiver_PatientGoToBedTendSelf jgp = new JobGiver_PatientGoToBedTendSelf();
    }
}
