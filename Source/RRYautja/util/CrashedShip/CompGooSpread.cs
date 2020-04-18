using AvP;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld
{
    // Token: 0x02000259 RID: 601
    public class CompProperties_GooSpread : CompProperties
    {
        // Token: 0x06000AC7 RID: 2759 RVA: 0x0005629B File Offset: 0x0005469B
        public CompProperties_GooSpread()
        {
            this.compClass = typeof(CompGooSpread);
        }

        // Token: 0x040004CB RID: 1227
        public int expandInterval = 500;

        // Token: 0x040004CC RID: 1228
        public float addAmount = 0.12f;

        // Token: 0x040016E9 RID: 5865
        public SimpleCurve radiusPerDayCurve;

        // Token: 0x040004CD RID: 1229
        public float maxRadius = 50f;
    }

    // Token: 0x02000762 RID: 1890
    public class CompGooSpread : ThingComp
    {
        // Token: 0x17000675 RID: 1653
        // (get) Token: 0x060029BE RID: 10686 RVA: 0x0013C3DA File Offset: 0x0013A7DA
        public CompProperties_GooSpread Props
        {
            get
            {
                return (CompProperties_GooSpread)this.props;
            }
        }

        // Token: 0x060029BF RID: 10687 RVA: 0x0013C3E7 File Offset: 0x0013A7E7
        public override void PostExposeData()
        {
            Scribe_Values.Look<float>(ref this.hiveRadius, "hiveRadius", 0f, false);
        }

        // Token: 0x060029C0 RID: 10688 RVA: 0x0013C3FF File Offset: 0x0013A7FF
        public override void CompTick()
        {
            if (!this.parent.Spawned)
            {
                return;
            }
            this.plantHarmAge++;
            if (this.parent.IsHashIntervalTick(this.Props.expandInterval))
            {
                this.TryExpandGoo();
            }
        }

        public float maxRadius
        {
            get
            {
                if (Props.maxRadius != 0f && Props.maxRadius <= GenRadial.MaxRadialPatternRadius)
                {
                    return Props.maxRadius;
                }
                else
                {
                    return GenRadial.MaxRadialPatternRadius;
                }
            }
            set
            {

            }
        }

        // Token: 0x060029C1 RID: 10689 RVA: 0x0013C434 File Offset: 0x0013A834
        private void TryExpandGoo()
        {
            if (this.parent.Map.mapTemperature.OutdoorTemp > 50f)
            {
                this.hiveRadius = 0f;
                return;
            }
            if (this.snowNoise == null)
            {
                this.snowNoise = new Perlin(0.074999999701976776, 2.0, 0.5, 5, Rand.Range(0, 651431), QualityMode.Medium); //new Perlin(0.054999999701976776, 2.0, 0.5, 5, Rand.Range(0, 651431), QualityMode.Medium);
            }
            float z = (float)this.plantHarmAge / 60000f;
            float numf = this.Props.radiusPerDayCurve.Evaluate(z);

            this.hiveRadius = numf<this.maxRadius ? numf :  this.maxRadius;// Mathf.Min(this.hiveRadius, this.maxRadius);
        //    Log.Message(string.Format("hiveRadius: {0}", hiveRadius));
            CellRect occupiedRect = this.parent.OccupiedRect();
            CompGooSpread.reachableCells.Clear();
            this.parent.Map.floodFiller.FloodFill(this.parent.Position, (IntVec3 x) => (float)x.DistanceToSquared(this.parent.Position) <= this.hiveRadius * this.hiveRadius && (occupiedRect.Contains(x) || !x.Filled(this.parent.Map) && x.InBounds(this.parent.Map)), delegate (IntVec3 x)
            {
                CompGooSpread.reachableCells.Add(x);
            }, int.MaxValue, false, null);
        //    Log.Message(string.Format("2")); errors here V when greater than 56.4 ish
            int num = GenRadial.NumCellsInRadius(this.hiveRadius);
        //    Log.Message(string.Format("3")); errors here ^ when greater than 56.4 ish
            for (int i = 0; i < num; i++)
            {
                if ((this.parent.Position + GenRadial.RadialPattern[i]).InBounds(this.parent.Map))
                {
                    IntVec3 intVec = this.parent.Position + GenRadial.RadialPattern[i];
                    if (intVec.InBounds(this.parent.Map))
                    {
                        if (CompGooSpread.reachableCells.Contains(intVec))
                        {
                            float num2 = this.snowNoise.GetValue(intVec);
                            num2 += 1f;
                            num2 *= 0.5f;
                            if (num2 < 0.1f)
                            {
                                num2 = 0.1f;
                            }

                            float lengthHorizontal = this.parent.Position.DistanceTo(intVec);
                            float num3 = 1f - (1 * (lengthHorizontal / this.hiveRadius));
                            this.parent.Map.GetComponent<MapComponent_GooGrid>().AddDepth(intVec, num3 * this.Props.addAmount /** num2*/);
                            /*
                            if (this.parent.Map.GetComponent<MapComponent_GooGrid>().GetDepth(intVec) <= num2)
                            {
                                float lengthHorizontal = this.parent.Position.DistanceTo(intVec);
                                float num3 = 1f * lengthHorizontal / this.hiveRadius;
                                this.parent.Map.GetComponent<MapComponent_GooGrid>().AddDepth(intVec, num3 * this.Props.addAmount * num2);
                            }
                            */
                        }
                    }
                }
            }
        }

        // Token: 0x04001726 RID: 5926
        private float hiveRadius;
        public int plantHarmAge;

        // Token: 0x04001727 RID: 5927
        private ModuleBase snowNoise;

        // Token: 0x04001729 RID: 5929
        private static HashSet<IntVec3> reachableCells = new HashSet<IntVec3>();
    }
}
