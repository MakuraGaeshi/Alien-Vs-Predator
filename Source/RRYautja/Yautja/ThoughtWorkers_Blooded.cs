using RimWorld;
using RRYautja;
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
            bool selected = false;
            if (p.Map!=null)
            {
                selected = Find.Selector.SelectedObjects.Contains(p);
            }
            if (!p.health.hediffSet.hediffs.Any(x=> x.def.defName.Contains(marked.defName)))
            {
                return false;
            }
#if DEBUG
            if (Prefs.DevMode)
            {
                if (selected) Log.Message(string.Format("{0} ThoughtWorker_Marked vs {1}", p.Label, other.Label));
            }
#endif
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
            {
                return false;
            }
            if (other.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
            {
                return false;
            }
            /*
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return ThoughtState.Inactive;
            }
            */
            bool Pblooded = YautjaBloodedUtility.Marked(p, out this.Marked);
            if (Pblooded)
            {
                bool Oblooded = YautjaBloodedUtility.Marked(other, out Hediff otherMarked);
                if (Oblooded)
                {
                    if (otherMarked.def == (unblooded))
                    {
                        stageIndex = 0;
#if DEBUG
                        if (Prefs.DevMode)
                        {
                            if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unblooded).Label, other.health.hediffSet.HasHediff(unblooded), stageIndex));
                        }
#endif
                    }
                    else if (otherMarked.def == (unmarked))
                    {
                        stageIndex = 1;
#if DEBUG
                        if (Prefs.DevMode)
                        {
                            if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unmarked).Label, other.health.hediffSet.HasHediff(unmarked), stageIndex));
                        }
#endif
                    }
                    else if (otherMarked.def.defName.Contains(marked.defName))
                    {
                        stageIndex = 2;
#if DEBUG
                        if (Prefs.DevMode)
                        {
                            if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), other.Label, otherMarked.Label, other.health.hediffSet.HasHediff(othermarked)));
                        }
#endif
                    }
                }
                else
                {
                    return false;
                }
                if (Marked.def.defName.Contains(marked.defName))
                {
#if DEBUG
                    if (Prefs.DevMode)
                    {
                        if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, Marked.Label, p.health.hediffSet.HasHediff(marked)));
                    }
#endif
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
        Hediff otherMarked;
        int stageIndex;
        // Token: 0x06000A02 RID: 2562 RVA: 0x0004F2B0 File Offset: 0x0004D6B0
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            bool selected = false;
            if (p.Map != null)
            {
                selected = Find.Selector.SelectedObjects.Contains(p);
            }
            if (!p.health.hediffSet.HasHediff(unmarked))
            {
                return false;
            }
#if DEBUG
            if (Prefs.DevMode)
            {
                if (selected) Log.Message(string.Format("{0} ThoughtWorker_Unmarked vs {1}", p.Label, other.Label));
            }
#endif
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
            {
                return false;
            }
            if (other.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
            {
                return false;
            }
            /*
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return ThoughtState.Inactive;
            }
            */
            if (other.health.hediffSet.HasHediff(unblooded))
            {
                stageIndex = 0;
#if DEBUG
                if (Prefs.DevMode)
                {
                    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unblooded).Label, other.health.hediffSet.HasHediff(unblooded), stageIndex));
                }
#endif
            }
            else if (other.health.hediffSet.HasHediff(unmarked))
            {
                stageIndex = 1;
#if DEBUG
                if (Prefs.DevMode)
                {
                    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(othermarked).Label, other.health.hediffSet.HasHediff(othermarked), stageIndex));
                }
#endif
            }
            else
                foreach (var hd in other.health.hediffSet.hediffs)
                {
                    if (hd.def.defName.Contains("RRY_Hediff_BloodedM"))
                    {
                        otherMarked = hd;
                        othermarked = hd.def;
                        stageIndex = 2;

#if DEBUG
                        if (Prefs.DevMode)
                        {
                            if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), other.Label, otherMarked.Label, other.health.hediffSet.HasHediff(othermarked)));
                        }
#endif
                        break;
                    }
                }
            if (p.health.hediffSet.HasHediff(unmarked))
            {
#if DEBUG
                if (Prefs.DevMode)
                {
                    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, unblooded.label, p.health.hediffSet.HasHediff(unblooded)));
                }
#endif
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
        Hediff otherMarked;
        int stageIndex;
        // Token: 0x06000A02 RID: 2562 RVA: 0x0004F2B0 File Offset: 0x0004D6B0
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            bool selected = false;
            if (p.Map != null)
            {
                selected = Find.Selector.SelectedObjects.Contains(p);
            }
            if (!p.health.hediffSet.HasHediff(unblooded))
            {
                return false;
            }
#if DEBUG
            if (Prefs.DevMode)
            {
                if (selected) Log.Message(string.Format("{0} ThoughtWorker_Unblooded vs {1}", p.Label, other.Label));
            }
#endif
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
            {
                return false;
            }
            if (other.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
            {
                return false;
            }
            /*
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return ThoughtState.Inactive;
            }
            */
            if (other.health.hediffSet.HasHediff(unblooded))
            {
                stageIndex = 0;
#if DEBUG
                if (Prefs.DevMode)
                {
                    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unblooded).Label, other.health.hediffSet.HasHediff(unblooded), stageIndex));
                }
#endif
            }
            else if (other.health.hediffSet.HasHediff(unmarked))
            {
                stageIndex = 1;
#if DEBUG
                if (Prefs.DevMode)
                {
                    if (selected) Log.Message(string.Format("{0} CurrentStateInternal stageIndex:{4} {1} {2}: {3}", this.GetType(), other.Label, other.health.hediffSet.GetFirstHediffOfDef(unblooded).Label, other.health.hediffSet.HasHediff(othermarked), stageIndex));
                }
#endif
            }
            else
                foreach (var hd in other.health.hediffSet.hediffs)
                {
                    if (hd.def.defName.Contains("RRY_Hediff_BloodedM"))
                    {
                        List<Thought> list = new List<Thought>();
                        otherMarked = hd;
                        othermarked = hd.def;
                        stageIndex = 2;
#if DEBUG
                        if (Prefs.DevMode)
                        {
                            if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), other.Label, otherMarked.Label, other.health.hediffSet.HasHediff(othermarked)));
                        }
#endif
                        break;
                    }
                }
                if (p.health.hediffSet.HasHediff(unblooded))
                {
#if DEBUG
                    if (Prefs.DevMode)
                {
                    if (selected) Log.Message(string.Format("{0} CurrentSocialStateInternal {1} {2}: {3}", this.GetType(), p.Label, unmarked.label, p.health.hediffSet.HasHediff(marked)));
                }
#endif
                    return ThoughtState.ActiveAtStage(stageIndex);
                }
            return false;
        }
    }
}
