using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_DefectiveInhibitor : HediffCompProperties
    {
        public HediffCompProperties_DefectiveInhibitor()
        {
            this.compClass = typeof(HediffComp_DefectiveInhibitor);
        }
        public List<TraitDef> traitsToGive = new List<TraitDef>();
        public int TraitGainMinIntervalTicks = 15000;
        public int TraitGainMaxIntervalTicks = 60000;
    }

    public class HediffComp_DefectiveInhibitor : HediffComp
    {
        public HediffCompProperties_DefectiveInhibitor Props
        {
            get
            {
                return (HediffCompProperties_DefectiveInhibitor)this.props;
            }
        }
        
        public int MaxTraits
        {
            get
            {
                int traitsmax = 4;
                if (!MoreTraitSlotsUtil.TryGetMaxTraitSlots(out traitsmax))
                {
                    traitsmax = 4;
                }
                return traitsmax;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticksSinceTraitGain, "ticksSinceTraitGain", 0, false);
            Scribe_Values.Look<int>(ref this.ticksTillTraitGain, "ticksTillTraitGain", 0, false);
            Scribe_Collections.Look(ref this.OriginalTraits, "OriginalTraits");
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            ticksTillTraitGain = Rand.Range(Props.TraitGainMinIntervalTicks, Props.TraitGainMaxIntervalTicks);
            OriginalTraits = Pawn.story.traits.allTraits;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (ticksSinceTraitGain>=ticksTillTraitGain)
            {
                Log.Message(string.Format("ticksSinceTraitGain>=ticksTillTraitGain"));
                if (MaxTraits >= Pawn.story.traits.allTraits.Count)
                {
                    Log.Message(string.Format("MaxTraits>Pawn.story.traits.allTraits.Count"));
                    TraitDef traitDef = GenCollection.RandomElement(Props.traitsToGive);
                    Trait replacedtrait = Pawn.story.traits.allTraits.FindAll(x => !Props.traitsToGive.Contains(x.def)).RandomElement();
                    if (replacedtrait == null) replacedtrait = Pawn.story.traits.allTraits.RandomElement();
                    if (Pawn.story.traits.HasTrait(traitDef))
                    {
                        Log.Message(string.Format("Pawn.story.traits.HasTrait(traitDef)"));
                        Trait trait = Pawn.story.traits.GetTrait(traitDef);
                        foreach (var item in traitDef.degreeDatas)
                        {
                            Log.Message(string.Format("degree {0}", item.degree));
                        }
                        /*
                        if (trait.Degree)
                        {

                        }
                        */
                        traitDef = GenCollection.RandomElement(Props.traitsToGive);
                    }
                    else
                    {
                        Log.Message(string.Format("!Pawn.story.traits.HasTrait(traitDef)"));
                        foreach (var item in traitDef.conflictingTraits)
                        {
                            if (Pawn.story.traits.HasTrait(item))
                            {
                                Log.Message(string.Format("removing {0}", item));
                                Pawn.story.traits.allTraits.Remove(Pawn.story.traits.GetTrait(item));
                            }
                        }
                        if (MaxTraits == Pawn.story.traits.allTraits.Count)
                        {
                            Pawn.story.traits.allTraits.Remove(replacedtrait);
                            Log.Message(string.Format("removing {0}", replacedtrait));
                        }
                        Trait trait = new Trait(traitDef);
                        Log.Message(string.Format("adding {0}", trait));
                        Pawn.story.traits.GainTrait(trait);
                    }
                }
                ticksSinceTraitGain = 0;
                Log.Message(string.Format("ticksSinceTraitGain to {0}", ticksSinceTraitGain));
                ticksTillTraitGain = Rand.Range(Props.TraitGainMinIntervalTicks, Props.TraitGainMaxIntervalTicks);
                Log.Message(string.Format("ticksTillTraitGain to {0}", ticksTillTraitGain));
            }
            ticksSinceTraitGain++;
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            foreach (var item in Props.traitsToGive) // Pawn.story.traits.allTraits)
            {
                if (Pawn.story.traits.HasTrait(item))
                {
                    Pawn.story.traits.allTraits.Remove(Pawn.story.traits.GetTrait(item));
                }
            }
            foreach (var item in OriginalTraits)
            {
                if (!Pawn.story.traits.HasTrait(item.def))
                {
                    Pawn.story.traits.GainTrait(item);
                }
            }
        }

        // Token: 0x04000002 RID: 2
        public int ticksSinceTraitGain;
        public int ticksTillTraitGain;
        public List<Trait> OriginalTraits;
    }
}
