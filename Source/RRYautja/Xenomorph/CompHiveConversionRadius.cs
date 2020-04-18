using System;
using Verse;

namespace RimWorld
{
    // Token: 0x02000751 RID: 1873
    public class CompProperties_HiveConversionRadius : CompProperties
    {
        // Token: 0x06002955 RID: 10581 RVA: 0x0013999F File Offset: 0x00137D9F
        public CompProperties_HiveConversionRadius()
        {
            this.compClass = typeof(CompHiveConversionRadius);
        }

        // Token: 0x040016E9 RID: 5865
        public SimpleCurve radiusPerDayCurve;

        // Token: 0x040016EA RID: 5866
        public float harmFrequencyPerArea = 1f;
    }
    // Token: 0x02000752 RID: 1874
    public class CompHiveConversionRadius : ThingComp
    {
        // Token: 0x17000656 RID: 1622
        // (get) Token: 0x06002957 RID: 10583 RVA: 0x001399D5 File Offset: 0x00137DD5
        protected CompProperties_HiveConversionRadius PropsPlantHarmRadius
        {
            get
            {
                return (CompProperties_HiveConversionRadius)this.props;
            }
        }

        // Token: 0x06002958 RID: 10584 RVA: 0x001399E2 File Offset: 0x00137DE2
        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.hiveconversionAge, "hiveconversionAge", 0, false);
            Scribe_Values.Look<int>(ref this.ticksToHiveConversion, "ticksToHiveConversion", 0, false);
        }

        // Token: 0x06002959 RID: 10585 RVA: 0x00139A08 File Offset: 0x00137E08
        public override void CompTick()
        {
            if (!this.parent.Spawned)
            {
                return;
            }
            this.hiveconversionAge++;
            this.ticksToHiveConversion--;
            if (this.ticksToHiveConversion <= 0)
            {
                float x = (float)this.hiveconversionAge / 60000f;
                float num = this.PropsPlantHarmRadius.radiusPerDayCurve.Evaluate(x);
                float num2 = 3.14159274f * num * num;
                float num3 = num2 * this.PropsPlantHarmRadius.harmFrequencyPerArea;
                float num4 = 60f / num3;
                int num5;
                if (num4 >= 1f)
                {
                    this.ticksToHiveConversion = GenMath.RoundRandom(num4);
                    num5 = 1;
                }
                else
                {
                    this.ticksToHiveConversion = 1;
                    num5 = GenMath.RoundRandom(1f / num4);
                }
                for (int i = 0; i < num5; i++)
                {
                    this.ConvertRandomCellInRadius(num);
                }
            }
        }

        // Token: 0x0600295A RID: 10586 RVA: 0x00139AE4 File Offset: 0x00137EE4
        private void ConvertRandomCellInRadius(float radius)
        {
            Map map = this.parent.Map;
            IntVec3 c = this.parent.Position + (Rand.InsideUnitCircleVec3 * radius).ToIntVec3();
            if (!c.InBounds(map))
            {
                return;
            }
            TerrainDef terrain = c.GetTerrain(map);
            Thing thing = c.GetEdifice(map);
            bool flith = c.GetThingList(map).Any((Thing x) => x.def == XenomorphDefOf.AvP_Filth_Slime);
            bool wall = thing!=null;
            if (flith)
            {
                terrain = TerrainDefOf.WaterShallow;
            }
        }

        // Token: 0x040016EB RID: 5867
        private int hiveconversionAge;

        // Token: 0x040016EC RID: 5868
        private int ticksToHiveConversion;

    }
}
