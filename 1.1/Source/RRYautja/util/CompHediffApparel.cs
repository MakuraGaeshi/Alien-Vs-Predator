using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RRYautja
{
    public class CompProperties_HediffApparel : CompProperties
    {
        public HediffDef hediffDef;
        public List<BodyPartDef> partsToAffect;
        public List<BodyPartGroupDef> groupsToAffect;
        public bool severityBasedOnDurability = false;
        public bool dropOnPartLost = false;

        public CompProperties_HediffApparel()
        {
            base.compClass = typeof(CompHediffApparel);
        }
    }

    public class CompHediffApparel : ThingComp
    {
        private float lastDurability;
        private Pawn lastWearer;

        public CompProperties_HediffApparel Props => (CompProperties_HediffApparel)base.props;

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_ApparelTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }
        // Determine if this ThingComp is being worn presently. Returns True/False
        protected virtual bool IsWorn => (GetWearer != null);

        public void MyRemoveHediffs(Pawn pawn)
        {
            if (pawn != null)
            {
                List<Hediff> diffs = pawn.health.hediffSet.hediffs.Where(d => d.def.defName == Props.hediffDef.defName).ToList();
                foreach (Hediff diff in diffs)
                {
                    pawn.health.RemoveHediff(diff);
                }
            }
        }

        public bool MyAddHediffs(Pawn pawn)
        {
            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn == null) return false;

            // Special case; if we're not told to apply to anything in particular, apply to the Whole Body.
            if (Props.partsToAffect.NullOrEmpty() && Props.groupsToAffect.NullOrEmpty())
            {
                return HediffGiverUtility.TryApply(pawn, Props.hediffDef, null);
            }

            IEnumerable<BodyPartRecord> source = pawn.health.hediffSet.GetNotMissingParts();
            List<BodyPartDef> partsToAffect = new List<BodyPartDef>();
            int countToAffect;

            // Add the specified parts, if they exist, to our list of parts to affect.
            if (!Props.partsToAffect.NullOrEmpty())
            {
                partsToAffect.AddRange(from p in source where Props.partsToAffect.Contains(p.def) select p.def);
            }

            // Now do it for all the parts in the specified groups.
            if (!Props.groupsToAffect.NullOrEmpty())
            {
                foreach (var item in source)
                {
                    if (Props.groupsToAffect.Count ==1 && item.groups.Any(x => x == Props.groupsToAffect[0]))
                    {
                    //    Log.Message(string.Format("{0}", item.customLabel));
                        GetWearer.health.AddHediff(Props.hediffDef, item);
                        partsToAffect.AddRange(from p in source where Props.groupsToAffect.Intersect(p.groups).Any() select p.def);
                        return true;
                    }
                }
            }

            // We need to count of parts to affect ahead of time because we are removing duplicates for performance reasons.
            countToAffect = partsToAffect.Count();
            partsToAffect.RemoveDuplicates();

            // Apply our hediffs!
            return false;
            // return HediffGiverUtility.TryApply(pawn, Props.hediffDef, partsToAffect, false, countToAffect);
        }

        public void MyUpdateSeverity(Pawn pawn)
        {
            // Get our current durability as a percentage.
            float currentDurability = (float)parent.HitPoints / parent.MaxHitPoints;

            // Only update if our durability has changed.
            if (lastDurability != currentDurability)
            {
                List<Hediff> diffs = pawn.health.hediffSet.hediffs.Where(d => d.def.defName == Props.hediffDef.defName).ToList();

                // Set the severity for each of our hediffs.
                foreach (Hediff diff in diffs)
                {
                    diff.Severity = currentDurability;
                }

                // Update our durability so we don't run code too often.
                lastDurability = currentDurability;
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            // We've been destroyed, so remove our effects.
            MyRemoveHediffs(lastWearer);
        }

        

        public override void CompTick()
        {
            base.CompTick();

            // We know our parent is an Apparel; cast it as such so we can access its Wearer member.
            Apparel parent = base.parent as Apparel;
            if (IsWorn)
            {
                if (parent.Wearer.health.hediffSet.HasHediff(Props.hediffDef))
                {
                    (parent.Wearer.health.hediffSet.hediffs.Find(x => x.def == Props.hediffDef)).Part.def.canSuggestAmputation = true;
                    if (Props.dropOnPartLost && !parent.Wearer.health.hediffSet.GetNotMissingParts().Contains(parent.Wearer.health.hediffSet.hediffs.Find(x => x.def == Props.hediffDef).Part))
                    {
                    //    parent.Wearer.apparel.Notify_ApparelRemoved(parent);
                    }
                }
            }
            // We only need to do something if our wearer has changed.
            if (parent.Wearer != lastWearer)
            {
                // It has, so remove our effects from the last wearer and apply them to the new one.
                MyRemoveHediffs(lastWearer);
                MyAddHediffs(parent.Wearer);
                // Update our wearer so we don't run code too often.
                lastWearer = parent.Wearer;
                // Set our last recorded durability to some impossible value to force an update.
                lastDurability = -1;
            }

            // Check to see if we should update our severity.
            if (Props.severityBasedOnDurability)
            {
                MyUpdateSeverity(parent.Wearer);
            }
        }
    }
}