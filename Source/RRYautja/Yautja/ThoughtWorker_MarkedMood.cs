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
            bool selected = Find.Selector.SelectedObjects.Contains(p);
            int stageIndex = 0;
            bool hasstage = false;
            Comp_Yautja _Yautja = p.TryGetComp<Comp_Yautja>();
#if DEBUG
        //    if (selected) Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal def.thoughtClass: {0}, def.ThoughtClass: {1}", def.thoughtClass, def.ThoughtClass));
#endif

           bool blooded = YautjaBloodedUtility.Marked(p, out Hediff firstHediffOfDef);

#if DEBUG
        //    if (selected) Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal blooded: {0}, firstHediffOfDef: {1}", blooded, firstHediffOfDef));
#endif
            if (firstHediffOfDef == null || firstHediffOfDef.def.stages == null)
            {
#if DEBUG
            //    if (selected) Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal firstHediffOfDef is Null"));
#endif
                return ThoughtState.Inactive;
            }
            /*
            // def.stages.Find(x => x.label == _Yautja.MarkHedifflabel);
            for (int i = 0; i < def.stages.Count; i++)
            {
                if (def.stages[i].label.Contains(_Yautja.MarkHedifflabel))
                {
                    stageIndex = i;
                    hasstage = true;
#if DEBUG
                    if (selected) Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal hasstage: {0} @ stageIndex: {1}, stage:{2}", hasstage, stageIndex, def.stages[i].label));
#endif
                    break;
                }
            }
            if (!hasstage)
            {
#if DEBUG
                if (selected) Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal hasstage: {0}, stageIndex: {1}", hasstage, stageIndex));
#endif
                stageIndex = def.stages.Count;
#if DEBUG
                if (selected) Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal Adding new @ stageIndex: {0}", stageIndex));
#endif
                def.stages.Add(new ThoughtStage
                {
                    baseMoodEffect = def.stages[0].baseMoodEffect,
                    description = string.Format("I've proven myself by killing a Worthy foe ({0}) and marked myself with its blood. I feel amazing.", _Yautja.MarkHedifflabel.CapitalizeFirst()),
                    label = string.Format("{0}", _Yautja.MarkHedifflabel)
                });
#if DEBUG
                if (selected) Log.Message(string.Format("ThoughtWorker_MarkedMood CurrentStateInternal New stage @ stageIndex: {0}, stage: {1}", stageIndex, def.stages[stageIndex].label));
#endif
            }
            */
            // String desc = string.Format("I've proven myself by killing a Worthy foe ({0}) and marked myself with its blood. I feel amazing.", _Yautja.MarkHedifflabel.CapitalizeFirst());
            /*
            def.stages.Add(new ThoughtStage
            {
                baseMoodEffect = def.stages[0].baseMoodEffect + (1 + i),
                description = string.Format("I've proven myself by killing a Worthy foe ({0}) and marked myself with its blood. I feel amazing.", _Yautja.MarkHedifflabel.CapitalizeFirst()),
                label = string.Format("{0} {1}", list[i].stages[0].label, def.stages[0].label)
            });
            */



            /*
            int stageIndex = Mathf.Min(new int[]
            {
                firstHediffOfDef.CurStageIndex,
                firstHediffOfDef.def.stages.Count - 1,
                this.def.stages.Count - 1
            });
            */
            /*
#if DEBUG
            if (selected) Log.Message(string.Format("ActiveAtStage @ stageIndex: {0}, stage: {1}", stageIndex, def.stages[stageIndex].label));
#endif
            */
            return ThoughtState.ActiveAtStage(stageIndex);
        }

    }
}
