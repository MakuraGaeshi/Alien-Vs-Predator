using System;
using UnityEngine;
using Verse;
using RRYautja;

namespace RimWorld
{
    // Token: 0x0200021E RID: 542
    public class ThoughtWorker_MarkedMood : ThoughtWorker
    {
        public static HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public static HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
        public static HediffDef markedDef = YautjaDefOf.RRY_Hediff_BloodedM;
        
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            bool selected = Find.Selector.SelectedObjects.Contains(p) && Prefs.DevMode;
            Comp_Yautja _Yautja = p.TryGetComp<Comp_Yautja>();
            
            if ((_Yautja != null))
            {
                if ((_Yautja.inducted)||p.kindDef.race==YautjaDefOf.RRY_Alien_Yautja)
                {
                    if (selected) Log.Message(string.Format("{0} {1} == active", this, p.LabelShortCap));

                    if (YautjaBloodedUtility.Marked(p, out Hediff BloodHD))
                    {
                        if (selected) Log.Message(string.Format("{0} {1} == 2", this, p.LabelShortCap));
                        return ThoughtState.ActiveAtStage(2);
                    }
                    if (p.health.hediffSet.HasHediff(unmarkedDef))
                    {
                        if (selected) Log.Message(string.Format("{0} {1} == 1", this, p.LabelShortCap));
                        return ThoughtState.ActiveAtStage(1);
                    }
                    if (p.health.hediffSet.HasHediff(unbloodedDef))
                    {
                        if (selected) Log.Message(string.Format("{0} {1} == 0", this, p.LabelShortCap));
                        return ThoughtState.ActiveAtStage(0);
                    }
                }
            }
            if (selected) Log.Message(string.Format("{0} {1} == Inactive", this, p.LabelShortCap));
            return ThoughtState.Inactive;
        }

    }
}
