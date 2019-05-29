using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    // Token: 0x020007BA RID: 1978
    public class Alert_ColonistNeedsRescuingFromCocoon : Alert_Critical
    {
        // Token: 0x170006D6 RID: 1750
        // (get) Token: 0x06002BEE RID: 11246 RVA: 0x001496FC File Offset: 0x00147AFC
        private IEnumerable<Pawn> ColonistsNeedingRescue
        {
            get
            {
                foreach (Pawn p in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if (Alert_ColonistNeedsRescuingFromCocoon.NeedsRescue(p))
                    {
                        yield return p;
                    }
                }
                yield break;
            }
        }

        // Token: 0x06002BEF RID: 11247 RVA: 0x00149718 File Offset: 0x00147B18
        public static bool NeedsRescue(Pawn p)
        {
            return p.Downed && (p.InBed() && p.CurrentBed() is Building_XenomorphCocoon) && !(p.ParentHolder is Pawn_CarryTracker) && (p.jobs.jobQueue == null || p.jobs.jobQueue.Count <= 0 || !p.jobs.jobQueue.Peek().job.CanBeginNow(p, false));
        }

        // Token: 0x06002BF0 RID: 11248 RVA: 0x00149797 File Offset: 0x00147B97
        public override string GetLabel()
        {
            if (this.ColonistsNeedingRescue.Count<Pawn>() == 1)
            {
                return "ColonistNeedsRescueFromCocoon".Translate();
            }
            return "ColonistsNeedRescueFromCocoon".Translate();
        }

        // Token: 0x06002BF1 RID: 11249 RVA: 0x001497C0 File Offset: 0x00147BC0
        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn pawn in this.ColonistsNeedingRescue)
            {
                stringBuilder.AppendLine("    " + pawn.LabelShort);
            }
            return "ColonistsNeedRescueFromCocoonDesc".Translate(stringBuilder.ToString());
        }

        // Token: 0x06002BF2 RID: 11250 RVA: 0x00149844 File Offset: 0x00147C44
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(this.ColonistsNeedingRescue);
        }
    }
}
