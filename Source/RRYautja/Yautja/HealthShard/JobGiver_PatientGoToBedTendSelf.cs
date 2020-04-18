using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AvP
{
    // Token: 0x0200000D RID: 13
    public class JobGiver_PatientGoToBedTendSelf : ThinkNode
    {
        // Token: 0x0600002D RID: 45 RVA: 0x00002758 File Offset: 0x00000958
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParm)
        {
            bool flag = !HealthAIUtility.ShouldSeekMedicalRest(pawn);
            ThinkResult result;
            if (flag)
            {
                result = ThinkResult.NoJob;
            }
            else
            {
                bool flag2 = !HealthAIUtility.ShouldBeTendedNowByPlayer(pawn);
                if (flag2)
                {
                    result = ThinkResult.NoJob;
                }
                else
                {
                    bool flag3 = !GenCollection.Any<Apparel>(pawn.apparel.WornApparel, (Apparel x) => x.def.defName.Contains("AvP_Equipment_HunterGauntlet"));
                    if (flag3)
                    {
                        result = ThinkResult.NoJob;
                    }
                    else
                    {
                        Thing thing = RestUtility.FindPatientBedFor(pawn);
                        bool flag4 = thing == null;
                        if (flag4)
                        {
                            result = ThinkResult.NoJob;
                        }
                        else
                        {
                            Thing thing2 = null;
                            bool flag5 = Medicine.GetMedicineCountToFullyHeal(pawn) > 0;
                            if (flag5)
                            {
                                thing2 = HealthAIUtility.FindBestMedicine(pawn, pawn);
                            }
                            bool flag6 = thing2 != null;
                            Job job;
                            if (flag6)
                            {
                                job = new Job(YautjaDefOf.AvP_Yautja_TendSelf, thing, thing2);
                            }
                            else
                            {
                                job = new Job(YautjaDefOf.AvP_Yautja_TendSelf, thing);
                            }
                            result = new ThinkResult(job, this, null, false);
                        }
                    }
                }
            }
            return result;
        }
    }
}
