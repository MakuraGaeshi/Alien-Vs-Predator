using AvP.ExtensionMethods;
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
        // Token: 0x17000ED7 RID: 3799
        // (get) Token: 0x060052BD RID: 21181 RVA: 0x001B325C File Offset: 0x001B145C
        private List<Pawn> ColonistsNeedingRescue
        {
            get
            {
                this.colonistsNeedingRescueResult.Clear();
                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if (pawn.Cocooned())
                    {
                        this.colonistsNeedingRescueResult.Add(pawn);
                    }
                }
                return this.colonistsNeedingRescueResult;
            }
        }

        // Token: 0x060052BE RID: 21182 RVA: 0x001B32CC File Offset: 0x001B14CC
        public static bool NeedsRescue(Pawn p)
        {
            return p.Downed && p.Cocooned() && !(p.ParentHolder is Pawn_CarryTracker) && (p.jobs.jobQueue == null || p.jobs.jobQueue.Count <= 0 || !p.jobs.jobQueue.Peek().job.CanBeginNow(p, false));
        }

        // Token: 0x060052BF RID: 21183 RVA: 0x001B3339 File Offset: 0x001B1539
        public override string GetLabel()
        {
            if (this.ColonistsNeedingRescue.Count == 1)
            {
                return "ColonistNeedsRescueFromCocoon".Translate();
            }
            return "ColonistsNeedRescueFromCocoon".Translate();
        }

        // Token: 0x060052C0 RID: 21184 RVA: 0x001B3368 File Offset: 0x001B1568
        public override TaggedString GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn pawn in this.ColonistsNeedingRescue)
            {
                stringBuilder.AppendLine("  - " + pawn.NameShortColored.Resolve());
            }
            return "ColonistsNeedRescueFromCocoonDesc".Translate(stringBuilder.ToString());
        }

        // Token: 0x060052C1 RID: 21185 RVA: 0x001B33F0 File Offset: 0x001B15F0
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(this.ColonistsNeedingRescue);
        }

        // Token: 0x04002D6A RID: 11626
        private List<Pawn> colonistsNeedingRescueResult = new List<Pawn>();
    }
}
