using RRYautja;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x0200021E RID: 542
    public class ThoughtWorker_MutliHediff : ThoughtWorker
    {
        Comp_Yautja _Yautja;
        int stageIndex = 0;
        // Token: 0x06000A30 RID: 2608 RVA: 0x0004FE58 File Offset: 0x0004E258
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            HediffSet hediffSet = p.health.hediffSet;
            if (p.kindDef.race!=YautjaDefOf.Alien_Yautja)
            {
                return ThoughtState.Inactive;
            }
            YautjaBloodedUtility.Marked(p, out Hediff hediff);
            if (p.health.hediffSet.hediffs!=null)
            {
                Comp_Yautja _Yautja = p.TryGetComp<Comp_Yautja>();
                if (_Yautja!=null)
                {
                    this._Yautja = _Yautja;
                }
                List<HediffDef> list = _Yautja.Props.bloodedDefs;
                for (int i = 0; i < list.Count; i++)
                {

                    def.stages.Add(new ThoughtStage
                    {
                        baseMoodEffect = def.stages[0].baseMoodEffect + (1 + i),
                        description = string.Format("{0}{1}{2}", desc1, list[i].stages[0].label, desc2),
                        label = string.Format("{0} {1}", list[i].stages[0].label, def.stages[0].label)
                    });
                }
                stageIndex = list.IndexOf(hediffDef);
            }
            if (hediff == null || hediff.def.stages == null)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(stageIndex);
        }

        public override string ToString()
        {
            if (_Yautja!=null)
            {
                return "(" + _Yautja.MarkHedifflabel + ")";
            }
            return base.ToString();
        }

        public string Description
        {
            get
            {
                string description = this.def.description;
                if (description != null)
                {
                    return description;
                }
                return this.def.description;
            }
        }

        public ThoughtState thoughtState;
        // Token: 0x040003FA RID: 1018

        HediffDef hediffDef;

        String desc1 = string.Format("I've proven myself by killing a ");

        String desc2 = string.Format(" and marked myself with its blood. I feel amazing.");
       
    }
}
