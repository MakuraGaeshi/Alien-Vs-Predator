using System;
using Verse;

namespace RimWorld
{
    // Token: 0x0200074A RID: 1866
    public class CompProperties_MaintainableLikea : CompProperties
    {
        // Token: 0x0600292F RID: 10543 RVA: 0x001389BE File Offset: 0x00136DBE
        public CompProperties_MaintainableLikea()
        {
            this.compClass = typeof(CompMaintainableLikea);
        }
        
    }

    // Token: 0x02000749 RID: 1865
    public class CompMaintainableLikea : ThingComp
    {
        // Token: 0x17000645 RID: 1605
        // (get) Token: 0x06002926 RID: 10534 RVA: 0x00138837 File Offset: 0x00136C37
        public CompProperties_MaintainableLikea Props
        {
            get
            {
                return (CompProperties_MaintainableLikea)this.props;
            }
        }
        
    }
}
