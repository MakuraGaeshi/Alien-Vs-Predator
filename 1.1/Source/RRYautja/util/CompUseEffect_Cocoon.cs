using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace RimWorld
{ 
    // Token: 0x02000002 RID: 2
    public class CompUseEffect_Cocoon : CompUseEffect
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002067 File Offset: 0x00000267
        public override void DoEffect(Pawn usedBy)
        {
            //   base.DoEffect(usedBy);
            this.parent.Destroy();
        }
        // Token: 0x06002ADD RID: 10973 RVA: 0x00143464 File Offset: 0x00141864
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            return base.CanBeUsedBy(p, out failReason);
        }

    }
}
