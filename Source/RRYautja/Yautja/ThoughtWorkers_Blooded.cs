using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x02000207 RID: 519
    public class ThoughtWorker_Marked : ThoughtWorker
    {
        HediffDef unblooded = YautjaDefOf.RRY_Hediff_Unblooded;
        HediffDef unmarked = YautjaDefOf.RRY_Hediff_BloodedUM;
        HediffDef marked = YautjaDefOf.RRY_Hediff_BloodedM;
        HediffDef othermarked = YautjaDefOf.RRY_Hediff_BloodedM;
        Hediff Marked;
        Hediff otherMarked;
        int stageIndex;
        // Token: 0x06000A02 RID: 2562 RVA: 0x0004F2B0 File Offset: 0x0004D6B0
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            bool selected = Find.Selector.SingleSelectedThing == p;
        //    if (selected) Log.Message(string.Format("{0} vs {1}", p.Label, other.Label));
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.Alien_Yautja)
            {
                return false;
            }
            /*
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            */
            if (other.health.hediffSet.HasHediff(unblooded))
            {
                stageIndex = 0;
            //    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unblooded).Label, other.health.hediffSet.HasHediff(unblooded), stageIndex));
            }
            else if (other.health.hediffSet.HasHediff(unmarked))
            {
                stageIndex = 1;
            //    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(othermarked).Label, other.health.hediffSet.HasHediff(othermarked), stageIndex));
            }
            else
                foreach (var hd in other.health.hediffSet.hediffs)
                {
                    if (hd.def.defName.Contains("RRY_Hediff_BloodedM"))
                    {
                        otherMarked = hd;
                        othermarked = hd.def;
                        stageIndex = 2;

                    //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), other.Label, otherMarked.Label, other.health.hediffSet.HasHediff(othermarked)));
                        break;
                    }
                 }
            foreach (var hd in p.health.hediffSet.hediffs)
            {
                if (hd.def.defName.Contains("RRY_Hediff_BloodedM"))
                {
                    Marked = hd;
                    marked = hd.def;
                //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, Marked.Label, p.health.hediffSet.HasHediff(marked)));
                    //Log.Message(string.Format("{0} CurrentSocialStateInternal p marked: {1}", this.GetType(), p.health.hediffSet.HasHediff(marked)));
                //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, Marked.Label, p.health.hediffSet.HasHediff(marked)));
                    return ThoughtState.ActiveAtStage(stageIndex);
                }
            }
            return false;
        }
    }

    // Token: 0x02000207 RID: 519
    public class ThoughtWorker_Unmarked : ThoughtWorker
    {
        HediffDef unblooded = YautjaDefOf.RRY_Hediff_Unblooded;
        HediffDef unmarked = YautjaDefOf.RRY_Hediff_BloodedUM;
        HediffDef marked = YautjaDefOf.RRY_Hediff_BloodedM;
        HediffDef othermarked = YautjaDefOf.RRY_Hediff_BloodedM;
        Hediff Marked;
        Hediff otherMarked;
        int stageIndex;
        // Token: 0x06000A02 RID: 2562 RVA: 0x0004F2B0 File Offset: 0x0004D6B0
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            bool selected = Find.Selector.SingleSelectedThing == p;
        //    if (selected) Log.Message(string.Format("{0} vs {1}", p.Label, other.Label));
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.Alien_Yautja)
            {
                return false;
            }
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (other.health.hediffSet.HasHediff(unblooded))
            {
                stageIndex = 0;
            //    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unblooded).Label, other.health.hediffSet.HasHediff(unblooded), stageIndex));
            }
            else if (other.health.hediffSet.HasHediff(unmarked))
            {
                stageIndex = 1;
            //    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(othermarked).Label, other.health.hediffSet.HasHediff(othermarked), stageIndex));
            }
            else
                foreach (var hd in other.health.hediffSet.hediffs)
                {
                    if (hd.def.defName.Contains("RRY_Hediff_BloodedM"))
                    {
                        otherMarked = hd;
                        othermarked = hd.def;
                        stageIndex = 2;

                    //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), other.Label, otherMarked.Label, other.health.hediffSet.HasHediff(othermarked)));
                        break;
                    }
                }
            if (p.health.hediffSet.HasHediff(unblooded))
            {
            //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, Marked.Label, p.health.hediffSet.HasHediff(marked)));
                //Log.Message(string.Format("{0} CurrentSocialStateInternal p marked: {1}", this.GetType(), p.health.hediffSet.HasHediff(marked)));
            //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, Marked.Label, p.health.hediffSet.HasHediff(marked)));
                return ThoughtState.ActiveAtStage(stageIndex);
            }
            return false;
        }
    }

    // Token: 0x02000207 RID: 519
    public class ThoughtWorker_Unblooded : ThoughtWorker
    {
        HediffDef unblooded = YautjaDefOf.RRY_Hediff_Unblooded;
        HediffDef unmarked = YautjaDefOf.RRY_Hediff_BloodedUM;
        HediffDef marked = YautjaDefOf.RRY_Hediff_BloodedM;
        HediffDef othermarked = YautjaDefOf.RRY_Hediff_BloodedM;
        Hediff Marked;
        Hediff otherMarked;
        int stageIndex;
        // Token: 0x06000A02 RID: 2562 RVA: 0x0004F2B0 File Offset: 0x0004D6B0
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            bool selected = Find.Selector.SingleSelectedThing == p;
        //    if (selected) Log.Message(string.Format("{0} vs {1}", p.Label, other.Label));
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.Alien_Yautja)
            {
                return false;
            }
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (other.health.hediffSet.HasHediff(unblooded))
            {
                stageIndex = 0;
            //    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unblooded).Label, other.health.hediffSet.HasHediff(unblooded), stageIndex));
            }
            else if (other.health.hediffSet.HasHediff(unmarked))
            {
                stageIndex = 1;
            //    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(othermarked).Label, other.health.hediffSet.HasHediff(othermarked), stageIndex));
            }
            else
                foreach (var hd in other.health.hediffSet.hediffs)
                {
                    if (hd.def.defName.Contains("RRY_Hediff_BloodedM"))
                    {
                        otherMarked = hd;
                        othermarked = hd.def;
                        stageIndex = 2;

                    //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), other.Label, otherMarked.Label, other.health.hediffSet.HasHediff(othermarked)));
                        break;
                    }
                }
                if (p.health.hediffSet.HasHediff(unmarked))
                {
                //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, Marked.Label, p.health.hediffSet.HasHediff(marked)));
                    //Log.Message(string.Format("{0} CurrentSocialStateInternal p marked: {1}", this.GetType(), p.health.hediffSet.HasHediff(marked)));
                //    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, Marked.Label, p.health.hediffSet.HasHediff(marked)));
                    return ThoughtState.ActiveAtStage(stageIndex);
                }
            return false;
        }
    }
}
