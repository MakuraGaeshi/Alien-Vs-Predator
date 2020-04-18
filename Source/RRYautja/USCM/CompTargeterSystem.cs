using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AvP
{
    public class CompProperties_TargeterSystem : CompProperties
    {
        public CompProperties_TargeterSystem()
        {
            this.compClass = typeof(CompTargeterSystem);
        }
    }

    // Token: 0x02000002 RID: 2
    public class CompTargeterSystem : ThingComp
    {
        public CompProperties_TargeterSystem Props => (CompProperties_TargeterSystem)this.props;
        
        public override void PostExposeData()
        {
            base.PostExposeData();
        //    Scribe_Values.Look(ref this.originalwarmupTime, "originalwarmupTime");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {

            }
        }
        

    }
}
