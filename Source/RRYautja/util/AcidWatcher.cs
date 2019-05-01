using RRYautja;
using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x02000399 RID: 921
    public class AcidWatcher
    {
        // Token: 0x06001011 RID: 4113 RVA: 0x000773AF File Offset: 0x000757AF
        public AcidWatcher(Map map)
        {
            this.map = map;
        }

        // Token: 0x17000254 RID: 596
        // (get) Token: 0x06001012 RID: 4114 RVA: 0x000773C9 File Offset: 0x000757C9
        public float AcidDanger
        {
            get
            {
                return this.acidDanger;
            }
        }

        // Token: 0x17000255 RID: 597
        // (get) Token: 0x06001013 RID: 4115 RVA: 0x000773D1 File Offset: 0x000757D1
        public bool LargeAcidDangerPresent
        {
            get
            {
                if (this.acidDanger < 0f)
                {
                    this.UpdateObservations();
                }
                return this.acidDanger > 90f;
            }
        }

        // Token: 0x06001014 RID: 4116 RVA: 0x000773F6 File Offset: 0x000757F6
        public void AcidWatcherTick()
        {
            if (Find.TickManager.TicksGame % 426 == 0)
            {
                this.UpdateObservations();
            }
        }

        // Token: 0x06001015 RID: 4117 RVA: 0x00077414 File Offset: 0x00075814
        private void UpdateObservations()
        {
            this.acidDanger = 0f;
            List<Thing> list = this.map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_FilthBloodXenomorph);
            for (int i = 0; i < list.Count; i++)
            {
                Filth_AddAcidDamage acid = list[i] as Filth_AddAcidDamage;
                this.acidDanger += 0.5f + acid.thickness;
            }
        }

        // Token: 0x040009F0 RID: 2544
        private Map map;

        // Token: 0x040009F1 RID: 2545
        private float acidDanger = -1f;

        // Token: 0x040009F2 RID: 2546
        private const int UpdateObservationsInterval = 426;

        // Token: 0x040009F3 RID: 2547
        private const float BaseDangerPerFire = 0.5f;
    }
}
