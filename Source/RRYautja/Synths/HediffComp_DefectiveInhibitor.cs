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
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            ticksTillTraitGain = Rand.Range(Props.TraitGainMinIntervalTicks, Props.TraitGainMaxIntervalTicks);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (ticksSinceTraitGain>=ticksTillTraitGain)
            {
                if (MaxTraits>Pawn.story.traits.allTraits.Count)
                {
                    TraitDef traitDef = GenCollection.RandomElement(Props.traitsToGive);
                    if (Pawn.story.traits.HasTrait(traitDef))
                    {
                        traitDef = GenCollection.RandomElement(Props.traitsToGive);
                    }
                    else
                    {
                        Trait trait = new Trait(traitDef);
                        Pawn.story.traits.GainTrait(trait);
                    }
                }
                ticksSinceTraitGain = 0;
                ticksTillTraitGain = Rand.Range(Props.TraitGainMinIntervalTicks, Props.TraitGainMaxIntervalTicks);
            }
            ticksSinceTraitGain++;
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            foreach (var item in Pawn.story.traits.allTraits)
            {
                if (Props.traitsToGive.Contains(item.def))
                {
                    Pawn.story.traits.allTraits.Remove(item);
                }
            }
        }

        // Token: 0x04000002 RID: 2
        public int ticksSinceTraitGain;
        public int ticksTillTraitGain;
    }
}
