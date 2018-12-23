using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x0200021E RID: 542
    public class ThoughtWorker_MutliHediff : ThoughtWorker
    {
        // Token: 0x06000A30 RID: 2608 RVA: 0x0004FE58 File Offset: 0x0004E258
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            int ind = 0;
            if (p.health.hediffSet.hediffs!=null)
            {
                List<HediffDef> list = p.GetComp<RRYautja.Comp_Yautja>().Props.bloodedDefs;
                foreach (var hediffDef in list)
                {
                    ind++;
                    if (hediffDef != null && p.health.hediffSet.HasHediff(hediffDef))
                    {
                        this.hediffDef = hediffDef;
                        def.stages[0].baseMoodEffect = def.stages[0].baseMoodEffect+(5*ind);
                        def.stages[0].description = string.Format("{0}{1}{2}", desc1, hediffDef.stages[0].label, desc2);
                        break;
                    }
                }
            }
            Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);
            if (firstHediffOfDef == null || firstHediffOfDef.def.stages == null)
            {
                return ThoughtState.Inactive;
            }
            int stageIndex = 0;
            stageIndex = Mathf.Min(new int[]
            {
                    firstHediffOfDef.CurStageIndex,
                    firstHediffOfDef.def.stages.Count - 1,
                    this.def.stages.Count - 1
            });
            return ThoughtState.ActiveAtStage(stageIndex);
        }


        public ThoughtState thoughtState;
        // Token: 0x040003FA RID: 1018

        HediffDef hediffDef;

        String desc1 = string.Format("I've proven myself by killing a ");

        String desc2 = string.Format(" and marked myself with its blood. I feel amazing.");
       
    }
}
