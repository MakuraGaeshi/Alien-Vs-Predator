using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000072 RID: 114
    public class JobDriver_XenosKidnap : JobDriver_XenoTakeToCocoon
    {
        // Token: 0x170000A4 RID: 164
        // (get) Token: 0x06000324 RID: 804 RVA: 0x0001F257 File Offset: 0x0001D657
        protected Pawn Takee
        {
            get
            {
                return (Pawn)base.Takee;
            }
        }

        // Token: 0x06000325 RID: 805 RVA: 0x0001F264 File Offset: 0x0001D664
        public override string GetReport()
        {
            if (this.Takee == null || this.pawn.HostileTo(this.Takee))
            {
                return base.GetReport();
            }
            return JobDefOf.Rescue.reportString.Replace("TargetA", this.Takee.LabelShort);
        }

        // Token: 0x06000326 RID: 806 RVA: 0x0001F2B8 File Offset: 0x0001D6B8
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => this.Takee == null || (!this.Takee.Downed && this.Takee.Awake()));
            foreach (Toil t in base.MakeNewToils())
            {
                yield return t;
            }
            yield break;
        }
    }
}
