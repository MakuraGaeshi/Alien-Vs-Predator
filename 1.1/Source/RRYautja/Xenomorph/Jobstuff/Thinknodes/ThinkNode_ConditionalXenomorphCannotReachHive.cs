using RRYautja.ExtensionMethods;
using System;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001DB RID: 475
    public class ThinkNode_ConditionalXenomorphCannotReachHive : ThinkNode_Conditional
    {
        // Token: 0x06003E87 RID: 16007 RVA: 0x0004D62F File Offset: 0x0004BA2F
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            if (this.Satisfied(pawn) == !this.invert)
            {
                return base.TryIssueJobPackage(pawn, jobParams);
            }
            return ThinkResult.NoJob;
        }

        // Token: 0x06000992 RID: 2450 RVA: 0x0004DBA2 File Offset: 0x0004BFA2
        protected override bool Satisfied(Pawn pawn)
        {
            IntVec3 c;
            if (!pawn.isXenomorph())
            {
                return false;
            }
            c = pawn.mindState.duty.focus.Cell;
            if (!pawn.xenomorph().HiveLoc.IsValid || pawn.xenomorph().HiveLoc == IntVec3.Zero)
            {
                pawn.xenomorph().HiveLoc = c;
            }
            HiveLike hive = (HiveLike)c.GetThingList(pawn.Map).First(x => x.def.defName.Contains("Xenomorph_Hive"));
            bool canReach = pawn.CanReach(c, PathEndMode.Touch, Danger.Deadly, true, TraverseMode.PassAllDestroyableThingsNotWater);
            bool Hive = hive != null;
            bool result = !canReach && !Hive;

            if (pawn.jobs.debugLog) pawn.jobs.DebugLogEvent(string.Format("{0}: {1} = Result: {2}", this, pawn.LabelShortCap, result));
            
            return result;
        }
    }
}
