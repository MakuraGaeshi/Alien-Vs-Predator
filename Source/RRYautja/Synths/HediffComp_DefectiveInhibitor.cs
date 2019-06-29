using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RRYautja
{
    // Token: 0x02000010 RID: 16
    public class SynthTraitEntry
    {
        public TraitDef def
        {
            get
            {
                return DefDatabase<TraitDef>.GetNamedSilentFail(defName);
            }
        }
        // Token: 0x04000066 RID: 102
        public string defName;

        // Token: 0x04000067 RID: 103
        public int degree = 0;

        // Token: 0x04000068 RID: 104
        public float chance = 100f;

        // Token: 0x04000069 RID: 105
        public float commonalityMale = -1f;

        // Token: 0x0400006A RID: 106
        public float commonalityFemale = -1f;
    }

    public class HediffCompProperties_DefectiveInhibitor : HediffCompProperties
    {
        public HediffCompProperties_DefectiveInhibitor()
        {
            this.compClass = typeof(HediffComp_DefectiveInhibitor);
        }
        public List<SynthTraitEntry> traitsToGive = new List<SynthTraitEntry>();
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
            if (ticksSinceTraitGain >= ticksTillTraitGain)
            {
             //   Log.Message(string.Format("Trait change tick"));
                if (MaxTraits >= Pawn.story.traits.allTraits.Count)
                {
                //    Log.Message(string.Format("Added traits: {0}", Pawn.story.traits.allTraits.FindAll(x => !Props.traitsToGive.Any((y) => y.def == x.def)).Count));
                    SynthTraitEntry traitEntry = GenCollection.RandomElementByWeight(Props.traitsToGive, (x => x.chance));
                    TraitDef traitDef = traitEntry.def;
                //    Log.Message(string.Format("MaxTraits: {0} Curtraits: {1}", MaxTraits, Pawn.story.traits.allTraits.Count));
                    Trait replacedtrait = Pawn.story.traits.allTraits.FindAll(x => !Props.traitsToGive.Any((y) => y.def == x.def)).RandomElement();
                    if (replacedtrait == null) replacedtrait = Pawn.story.traits.allTraits.RandomElement();
                    if (Pawn.story.traits.HasTrait(traitDef))
                    {
                    //    Log.Message(string.Format("{0} has {1}", Pawn, traitDef));
                        Trait trait = Pawn.story.traits.GetTrait(traitDef);
                        if (trait.Degree == traitEntry.degree)
                        {
                            if (traitEntry.degree > 0)
                            {
                                replacedtrait = trait;
                                trait = new Trait(traitDef, traitEntry.degree + 1);
                                Pawn.story.traits.allTraits.Remove(replacedtrait);
                                GainTrait(Pawn, trait);
                            }
                            if (traitEntry.degree < 0)
                            {
                                replacedtrait = trait;
                                trait = new Trait(traitDef, traitEntry.degree - 1);
                                Pawn.story.traits.allTraits.Remove(replacedtrait);
                                GainTrait(Pawn, trait);
                            }
                        }
                    }
                    else
                    {
                    //    Log.Message(string.Format("{0} hasnt {1}", Pawn, traitDef));
                        foreach (var item in traitDef.conflictingTraits)
                        {
                            if (Pawn.story.traits.HasTrait(item))
                            {
                            //    Log.Message(string.Format("removing conflicting {0}", item));
                                Pawn.story.traits.allTraits.Remove(Pawn.story.traits.GetTrait(item));
                            }
                        }
                        if (MaxTraits == Pawn.story.traits.allTraits.Count)
                        {
                            Pawn.story.traits.allTraits.Remove(replacedtrait);
                        //    Log.Message(string.Format("removing {0}", replacedtrait));
                        }
                        Trait trait = new Trait(traitDef, traitEntry.degree);
                        /*
                        foreach (var item in traitDef.degreeDatas)
                        {
                            Log.Message(string.Format("item.degree {0}", item.degree));
                        }
                        Log.Message(string.Format("trait.Degree {0}", trait.Degree));
                        Log.Message(string.Format("adding {0}", trait));
                        */
                        GainTrait(Pawn, trait);
                        //    Pawn.story.traits.GainTrait(trait);
                    }
                }
                ticksSinceTraitGain = 0;
                ticksTillTraitGain = Rand.Range(Props.TraitGainMinIntervalTicks, Props.TraitGainMaxIntervalTicks);
            //    Log.Message(string.Format("Next in {0} ticks", ticksTillTraitGain));
            }
            ticksSinceTraitGain++;
        }


        public void GainTrait(Pawn pawn, Trait trait)
        {
            if (pawn.story.traits.HasTrait(trait.def))
            {
                Log.Warning(pawn + " already has trait " + trait.def, false);
                return;
            }
            pawn.story.traits.allTraits.Add(trait);
            if (pawn.workSettings != null)
            {
                pawn.workSettings.Notify_GainedTrait();
            }
            //    pawn.story.Notify_TraitChanged();
            if (pawn.skills != null)
            {
                pawn.skills.Notify_SkillDisablesChanged();
            }
            if (!pawn.Dead && pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            foreach (var item in Props.traitsToGive) // Pawn.story.traits.allTraits)
            {
                if (Pawn.story.traits.HasTrait(item.def))
                {
                    Pawn.story.traits.allTraits.Remove(Pawn.story.traits.GetTrait(item.def));
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
