using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld
{

    public class CompProperties_HiveExpand : CompProperties
    {
        // Token: 0x06000AC7 RID: 2759 RVA: 0x0005629B File Offset: 0x0005469B
        public CompProperties_HiveExpand()
        {
            this.compClass = typeof(CompHiveExpand);
        }

        // Token: 0x040004CB RID: 1227
        public int expandInterval = 500;

        // Token: 0x040004CC RID: 1228
        public float addAmount = 0.12f;

        // Token: 0x040004CD RID: 1229
        public float maxRadius = 55f;
    }

    public class CompHiveExpand : ThingComp
    {
        // Token: 0x17000675 RID: 1653
        // (get) Token: 0x060029BE RID: 10686 RVA: 0x0013C40E File Offset: 0x0013A80E
        public CompProperties_HiveExpand Props
        {
            get
            {
                return (CompProperties_HiveExpand)this.props;
            }
        }

        // Token: 0x060029BF RID: 10687 RVA: 0x0013C41B File Offset: 0x0013A81B
        public override void PostExposeData()
        {
            Scribe_Values.Look<float>(ref this.hiveRadius, "hiveRadius", 0f, false);
        }

        // Token: 0x060029C0 RID: 10688 RVA: 0x0013C433 File Offset: 0x0013A833
        public override void CompTick()
        {
            if (!this.parent.Spawned)
            {
                return;
            }
            if (this.parent.IsHashIntervalTick(this.Props.expandInterval))
            {
                this.TryExpandSnow();
            }
        }

        // Token: 0x060029C1 RID: 10689 RVA: 0x0013C468 File Offset: 0x0013A868
        private void TryExpandSnow()
        {
            if (this.parent.Map.mapTemperature.OutdoorTemp > 10f)
            {
                this.hiveRadius = 0f;
                return;
            }
            if (this.hiveNoise == null)
            {
                this.hiveNoise = new Perlin(0.054999999701976776, 2.0, 0.5, 5, Rand.Range(0, 651431), QualityMode.Medium);
            }
            if (this.hiveRadius < 8f)
            {
                this.hiveRadius += 1.3f;
            }
            else if (this.hiveRadius < 17f)
            {
                this.hiveRadius += 0.7f;
            }
            else if (this.hiveRadius < 30f)
            {
                this.hiveRadius += 0.4f;
            }
            else
            {
                this.hiveRadius += 0.1f;
            }
            this.hiveRadius = Mathf.Min(this.hiveRadius, this.Props.maxRadius);
            CellRect occupiedRect = this.parent.OccupiedRect();
            CompHiveExpand.reachableCells.Clear();
            this.parent.Map.floodFiller.FloodFill(this.parent.Position, (IntVec3 x) => (float)x.DistanceToSquared(this.parent.Position) <= this.hiveRadius * this.hiveRadius && (occupiedRect.Contains(x) || !x.Filled(this.parent.Map)), delegate (IntVec3 x)
            {
                CompHiveExpand.reachableCells.Add(x);
            }, int.MaxValue, false, null);
            int num = GenRadial.NumCellsInRadius(this.hiveRadius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = this.parent.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.parent.Map))
                {
                    if (CompHiveExpand.reachableCells.Contains(intVec))
                    {
                        float num2 = this.hiveNoise.GetValue(intVec);
                        num2 += 1f;
                        num2 *= 0.5f;
                        if (num2 < 0.1f)
                        {
                            num2 = 0.1f;
                        }
                        if (this.parent.Map.snowGrid.GetDepth(intVec) <= num2)
                        {
                            float lengthHorizontal = (intVec - this.parent.Position).LengthHorizontal;
                            float num3 = 1f - lengthHorizontal / this.hiveRadius;
                            this.parent.Map.snowGrid.AddDepth(intVec, num3 * this.Props.addAmount * num2);
                        }
                    }
                }
            }
        }

        // Token: 0x04001726 RID: 5926
        private float hiveRadius;

        // Token: 0x04001727 RID: 5927
        private ModuleBase hiveNoise;

        // Token: 0x04001728 RID: 5928
        private const float MaxOutdoorTemp = 70f;

        // Token: 0x04001729 RID: 5929
        private static HashSet<IntVec3> reachableCells = new HashSet<IntVec3>();
    }
}
