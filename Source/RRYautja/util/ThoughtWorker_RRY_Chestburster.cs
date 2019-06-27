using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x0200021E RID: 542
    public class ThoughtWorker_RRY_Chestburster : ThoughtWorker
    {
        // Token: 0x06000A30 RID: 2608 RVA: 0x0004FE58 File Offset: 0x0004E258
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(this.def.hediff, false);
            if (firstHediffOfDef == null || firstHediffOfDef.def.stages == null)
            {
                return ThoughtState.Inactive;
            }
            if (!PlayerKnowledgeDatabase.IsComplete(XenomorphConceptDefOf.RRY_Concept_Chestbursters) && p.Spawned && p.IsColonist)
            {
                return ThoughtState.Inactive;
            }
            int stageIndex = Mathf.Min(new int[]
            {
                firstHediffOfDef.CurStageIndex,
                firstHediffOfDef.def.stages.Count - 1,
                this.def.stages.Count - 1
            });
            return ThoughtState.ActiveAtStage(stageIndex);
        }
    }
}
