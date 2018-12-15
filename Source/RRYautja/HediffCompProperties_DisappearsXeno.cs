using System;
using Verse;

namespace RRYautja
{
    // Token: 0x02000D5A RID: 3418
    public class HediffCompProperties_XenoFacehugger : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_XenoFacehugger()
        {
            this.compClass = typeof(HediffComp_XenoFacehugger);
        }

        // Token: 0x040033C3 RID: 13251
        public PawnKindDef pawnKindDef;
        public bool spawnLive = false;
        public bool killHost = false;
    }
    // Token: 0x02000D5A RID: 3418
    public class HediffCompProperties_XenoSpawner : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_XenoSpawner()
        {
            this.compClass = typeof(HediffComp_XenoSpawner);
        }

        // Token: 0x040033C3 RID: 13251
        public PawnKindDef pawnKindDef;
        public bool spawnLive = false;
        public bool killHost = false;
    }
}
