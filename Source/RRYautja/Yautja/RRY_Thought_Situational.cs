using RRYautja;
using System;
using Verse;

namespace RimWorld
{
    // Token: 0x0200054E RID: 1358
    public class RRY_Thought_Situational : Thought_Situational
    {

        // Token: 0x170003A1 RID: 929
        // (get) Token: 0x0600197E RID: 6526 RVA: 0x0004E9ED File Offset: 0x0004CDED
        public override int CurStageIndex
        {
            get
            {
                if (YautjaBloodedUtility.Marked(pawn, out Hediff BloodHD))
                {
                    return 2;
                }
                if (pawn.health.hediffSet.HasHediff(unmarkedDef) && pawn.health.hediffSet.GetFirstHediffOfDef(unmarkedDef) is Hediff UnmarkedHD)
                {
                    return 1;
                }
                if (pawn.health.hediffSet.HasHediff(unbloodedDef) && pawn.health.hediffSet.GetFirstHediffOfDef(unbloodedDef) is Hediff UnbloodedHD)
                {
                    return 0;
                }
                return 0;
            }
        }

        public Comp_Yautja _Yautja
        {
            get
            {
                return pawn.TryGetComp<Comp_Yautja>();
            }
        }

        private bool selected => Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode;
        public static HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public static HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
        public static HediffDef markedDef = YautjaDefOf.RRY_Hediff_BloodedM;

        // Token: 0x170003A2 RID: 930
        // (get) Token: 0x0600197F RID: 6527 RVA: 0x0004E9F5 File Offset: 0x0004CDF5
        
        public override string LabelCap
        {
            get
            {
                string labelstring = !this.reason.NullOrEmpty() ? base.CurStage.label: base.LabelCap;
                if (pawn!=null)
                { 
                    if (_Yautja != null)
                    {
                        if (!_Yautja.MarkHedifflabel.NullOrEmpty())
                        {
                            labelstring = labelstring.CapitalizeFirst()+" ("+ _Yautja.MarkHedifflabel.CapitalizeFirst()+")";
                            
                        }
                    }
                }
                if (!this.reason.NullOrEmpty())
                {
                    return string.Format(labelstring, this.reason).CapitalizeFirst();
                }
                return labelstring;
            }
        }

        public override float MoodOffset()
        {
            return base.MoodOffset();
        }
        
        /*
        public new string Description
        {
            get
            {
                string description = this.CurStage.description;
                if (pawn != null)
                { // if (selected) Log.Message("found corpse");
                  //    Log.Message("Description found pawn");
                    if (_Yautja != null)
                    {
                        //    Log.Message("Description found Comp_Yautja");
                        if (!_Yautja.MarkHedifflabel.NullOrEmpty())
                        {
                            description = desc1.CapitalizeFirst() + " (" + _Yautja.MarkHedifflabel.CapitalizeFirst() + ") "+ desc2;

                        }
                    }
                }
                if (description != null)
                {
                    return description;
                }
                return this.def.description;
            }
        }
        */
        protected override float BaseMoodOffset
        {
            get
            {
                if (YautjaBloodedUtility.Marked(pawn, out Hediff BloodHD))
                {
                    if (selected) Log.Message(string.Format("{0} BloodHD is {1}", pawn.LabelShortCap, BloodHD.Label));
                    return ((_Yautja.combatPower / 10) / 4);
                }
                if (pawn.health.hediffSet.HasHediff(unmarkedDef) && pawn.health.hediffSet.GetFirstHediffOfDef(unmarkedDef) is Hediff UnmarkedHD)
                {
                    if (selected) Log.Message(string.Format("{0} UnmarkedHD is {1}", pawn.LabelShortCap, UnmarkedHD.Label));
                    return 0;
                }
                if (pawn.health.hediffSet.HasHediff(unbloodedDef) && pawn.health.hediffSet.GetFirstHediffOfDef(unbloodedDef) is Hediff UnbloodedHD)
                {
                    if (selected) Log.Message(string.Format("{0} UnbloodedHD is {1}", pawn.LabelShortCap, UnbloodedHD.Label));
                    return base.BaseMoodOffset;
                }
                return 0;

            }
        }
        // Token: 0x04000F33 RID: 3891
        private int curStageIndex = 0;

        String desc = string.Format("I've proven myself by killing a Worthy foe and marked myself with its blood. I feel amazing.");

        String desc1 = string.Format("I've proven myself by killing a Worthy foe");

        String desc2 = string.Format(" and marked myself with its blood. I feel amazing.");
    }
}
