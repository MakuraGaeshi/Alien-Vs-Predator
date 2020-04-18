using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AvP
{
    public class CompProperties_SynthProps : CompProperties
    {
        public CompProperties_SynthProps()
        {
            this.compClass = typeof(CompSynthProps);
        }
        public List<HediffDef> AllowedDiseases = new List<HediffDef>();
    }

    // Token: 0x02000002 RID: 2
    public class CompSynthProps : ThingComp
    {
        public CompProperties_SynthProps Props => (CompProperties_SynthProps)this.props;
        public List<HediffDef> AllowedDiseases => Props.AllowedDiseases;

        public override void PostExposeData()
        {
            base.PostExposeData();
        }
    }
}
