using System;
using UnityEngine;
using Verse;
using RRYautja;

namespace RimWorld
{
    // Token: 0x0200021E RID: 542
    public class ThoughtWorker_MarkedMood : ThoughtWorker
    {
        // Token: 0x06000A30 RID: 2608 RVA: 0x0004FE58 File Offset: 0x0004E258
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
        //    int stageIndex = 0;
            Comp_Yautja _Yautja = p.TryGetComp<Comp_Yautja>();
            Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal def.thoughtClass: {0}, def.ThoughtClass: {1}", def.thoughtClass, def.ThoughtClass));
            
            YautjaBloodedUtility.Marked(p, out Hediff firstHediffOfDef);

            // def.stages.Find(x => x.label == _Yautja.MarkHedifflabel);
            /*
            for (int i = 0; i < def.stages.Count; i++)
            {
                if (stage.label == _Yautja.MarkHedifflabel)
                {

                }
            }
            */
            /*
            def.stages.Add(new ThoughtStage
            {
                baseMoodEffect = def.stages[0].baseMoodEffect + (1 + i),
                description = string.Format("{0}{1}{2}", desc1, list[i].stages[0].label, desc2),
                label = string.Format("{0} {1}", list[i].stages[0].label, def.stages[0].label)
            });
            */



            if (firstHediffOfDef == null || firstHediffOfDef.def.stages == null)
            {
                return ThoughtState.Inactive;
            }
            
            int stageIndex = Mathf.Min(new int[]
            {
                firstHediffOfDef.CurStageIndex,
                firstHediffOfDef.def.stages.Count - 1,
                this.def.stages.Count - 1
            });
            
            return ThoughtState.ActiveAtStage(stageIndex, "high");
        }

    }
}
