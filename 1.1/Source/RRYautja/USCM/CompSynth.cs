using System;
using UnityEngine;
using Verse;

namespace RRYautja
{
    // Token: 0x0200024D RID: 589
    public class CompProperties_Synth : CompProperties
    {
        // Token: 0x06000AAF RID: 2735 RVA: 0x00055935 File Offset: 0x00053D35
        public CompProperties_Synth()
        {
            this.compClass = typeof(CompSynth);
        }

        
    }

    // Token: 0x0200073A RID: 1850
    [StaticConstructorOnStartup]
    public class CompSynth : ThingComp
    {
        // Token: 0x17000623 RID: 1571
        // (get) Token: 0x060028B9 RID: 10425 RVA: 0x00135B3C File Offset: 0x00133F3C
        public CompProperties_Synth Props => (CompProperties_Synth)this.props;
        
        // Token: 0x060028BB RID: 10427 RVA: 0x00135BB5 File Offset: 0x00133FB5
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}
