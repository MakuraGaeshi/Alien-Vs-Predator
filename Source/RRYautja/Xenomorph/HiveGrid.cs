using System;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C7C RID: 3196
    public sealed class HiveGrid : IExposable
    {
        // Token: 0x06004696 RID: 18070 RVA: 0x002108CA File Offset: 0x0020ECCA
        public HiveGrid(Map map)
        {
            this.map = map;
            this.depthGrid = new float[map.cellIndices.NumGridCells];
        }

        // Token: 0x17000AF3 RID: 2803
        // (get) Token: 0x06004697 RID: 18071 RVA: 0x002108EF File Offset: 0x0020ECEF
        internal float[] DepthGridDirect_Unsafe
        {
            get
            {
                return this.depthGrid;
            }
        }

        // Token: 0x17000AF4 RID: 2804
        // (get) Token: 0x06004698 RID: 18072 RVA: 0x002108F7 File Offset: 0x0020ECF7
        public float TotalDepth
        {
            get
            {
                return (float)this.totalDepth;
            }
        }

        // Token: 0x06004699 RID: 18073 RVA: 0x00210900 File Offset: 0x0020ED00
        public void ExposeData()
        {
            MapExposeUtility.ExposeUshort(this.map, (IntVec3 c) => HiveGrid.HiveFloatToShort(this.GetDepth(c)), delegate (IntVec3 c, ushort val)
            {
                this.depthGrid[this.map.cellIndices.CellToIndex(c)] = HiveGrid.HiveShortToFloat(val);
            }, "depthGrid");
        }

        // Token: 0x0600469A RID: 18074 RVA: 0x0021092A File Offset: 0x0020ED2A
        private static ushort HiveFloatToShort(float depth)
        {
            depth = Mathf.Clamp(depth, 0f, 1f);
            depth *= 65535f;
            return (ushort)Mathf.RoundToInt(depth);
        }

        // Token: 0x0600469B RID: 18075 RVA: 0x0021094E File Offset: 0x0020ED4E
        private static float HiveShortToFloat(ushort depth)
        {
            return (float)depth / 65535f;
        }

        // Token: 0x0600469C RID: 18076 RVA: 0x00210958 File Offset: 0x0020ED58
        private bool CanHaveHive(int ind)
        {
            Building building = this.map.edificeGrid[ind];
            if (building != null && !HiveGrid.CanCoexistWithSnow(building.def))
            {
                return false;
            }
            TerrainDef terrainDef = this.map.terrainGrid.TerrainAt(ind);
            return terrainDef == null || terrainDef.holdSnow;
        }

        // Token: 0x0600469D RID: 18077 RVA: 0x002109B5 File Offset: 0x0020EDB5
        public static bool CanCoexistWithSnow(ThingDef def)
        {
            return def.category != ThingCategory.Building || def.Fillage != FillCategory.Full;
        }

        // Token: 0x0600469E RID: 18078 RVA: 0x002109D4 File Offset: 0x0020EDD4
        public void AddDepth(IntVec3 c, float depthToAdd)
        {
            int num = this.map.cellIndices.CellToIndex(c);
            float num2 = this.depthGrid[num];
            if (num2 <= 0f && depthToAdd < 0f)
            {
                return;
            }
            if (num2 >= 0.999f && depthToAdd > 1f)
            {
                return;
            }
            if (!this.CanHaveHive(num))
            {
                this.depthGrid[num] = 0f;
                return;
            }
            float num3 = num2 + depthToAdd;
            num3 = Mathf.Clamp(num3, 0f, 1f);
            float num4 = num3 - num2;
            this.totalDepth += (double)num4;
            if (Mathf.Abs(num4) > 0.0001f)
            {
                this.depthGrid[num] = num3;
                this.CheckVisualOrPathCostChange(c, num2, num3);
            }
        }

        // Token: 0x0600469F RID: 18079 RVA: 0x00210A90 File Offset: 0x0020EE90
        public void SetDepth(IntVec3 c, float newDepth)
        {
            int num = this.map.cellIndices.CellToIndex(c);
            if (!this.CanHaveHive(num))
            {
                this.depthGrid[num] = 0f;
                return;
            }
            newDepth = Mathf.Clamp(newDepth, 0f, 1f);
            float num2 = this.depthGrid[num];
            this.depthGrid[num] = newDepth;
            float num3 = newDepth - num2;
            this.totalDepth += (double)num3;
            this.CheckVisualOrPathCostChange(c, num2, newDepth);
        }

        // Token: 0x060046A0 RID: 18080 RVA: 0x00210B0C File Offset: 0x0020EF0C
        private void CheckVisualOrPathCostChange(IntVec3 c, float oldDepth, float newDepth)
        {
            if (!Mathf.Approximately(oldDepth, newDepth))
            {
                if (Mathf.Abs(oldDepth - newDepth) > 0.15f || Rand.Value < 0.0125f)
                {
                    this.map.mapDrawer.MapMeshDirty(c, MapMeshFlag.Snow, true, false);
                    this.map.mapDrawer.MapMeshDirty(c, MapMeshFlag.Things, true, false);
                }
                else if (newDepth == 0f)
                {
                    this.map.mapDrawer.MapMeshDirty(c, MapMeshFlag.Snow, true, false);
                }
                if (HiveUtility.GetHiveCategory(oldDepth) != HiveUtility.GetHiveCategory(newDepth))
                {
                    this.map.pathGrid.RecalculatePerceivedPathCostAt(c);
                }
            }
        }

        // Token: 0x060046A1 RID: 18081 RVA: 0x00210BB6 File Offset: 0x0020EFB6
        public float GetDepth(IntVec3 c)
        {
            if (!c.InBounds(this.map))
            {
                return 0f;
            }
            return this.depthGrid[this.map.cellIndices.CellToIndex(c)];
        }

        // Token: 0x060046A2 RID: 18082 RVA: 0x00210BE7 File Offset: 0x0020EFE7
        public HiveCategory GetCategory(IntVec3 c)
        {
            return HiveUtility.GetHiveCategory(this.GetDepth(c));
        }

        // Token: 0x04003028 RID: 12328
        private Map map;

        // Token: 0x04003029 RID: 12329
        private float[] depthGrid;

        // Token: 0x0400302A RID: 12330
        private double totalDepth;

        // Token: 0x0400302B RID: 12331
        public const float MaxDepth = 1f;
    }
}
