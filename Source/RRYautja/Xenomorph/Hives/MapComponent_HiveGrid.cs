﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RRYautja;
using UnityEngine;
using Verse;
using static RRYautja.HiveUtility;

namespace RRYautja
{
    // Token: 0x02000067 RID: 103
    public class MapComponent_HiveGrid : MapComponent
    {
        // Token: 0x06000217 RID: 535 RVA: 0x0000B430 File Offset: 0x00009630
        public MapComponent_HiveGrid(Map map) : base(map)
        {
            this.map = map;
            this.depthGrid = new float[map.cellIndices.NumGridCells];
        }

        // Token: 0x0400011D RID: 285
        public MapComponent_HiveGrid HiveGrid;
        // Token: 0x06000218 RID: 536 RVA: 0x0000B490 File Offset: 0x00009690
        public override void FinalizeInit()
        {
            base.FinalizeInit();
        }

        // Token: 0x06000219 RID: 537 RVA: 0x0000B49A File Offset: 0x0000969A
        public override void MapGenerated()
        {
            base.MapGenerated();
        }

        // Token: 0x0600021A RID: 538 RVA: 0x0000B4A4 File Offset: 0x000096A4
        public override void ExposeData()
        {
            MapExposeUtility.ExposeUshort(this.map, (IntVec3 c) => MapComponent_HiveGrid.HiveFloatToShort(this.GetDepth(c)), delegate (IntVec3 c, ushort val)
            {
                this.depthGrid[this.map.cellIndices.CellToIndex(c)] = MapComponent_HiveGrid.HiveShortToFloat(val);
            }, "depthGrid");
            base.ExposeData();
        }

        // Token: 0x0600021B RID: 539 RVA: 0x0000B4C0 File Offset: 0x000096C0
        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
        //    this.HiveGrid.Regenerate();
        }

        // Token: 0x0600021C RID: 540 RVA: 0x0000B528 File Offset: 0x00009728
        public override void MapComponentTick()
        {
            base.MapComponentTick();
        }

        internal float[] DepthGridDirect_Unsafe
        {
            get
            {
                return this.depthGrid;
            }
        }

        public float TotalDepth
        {
            get
            {
                return (float)this.totalDepth;
            }
        }
        

        private static ushort HiveFloatToShort(float depth)
        {
            depth = Mathf.Clamp(depth, 0f, 1f);
            depth *= 65535f;
            return (ushort)Mathf.RoundToInt(depth);
        }

        private static float HiveShortToFloat(ushort depth)
        {
            return (float)depth / 65535f;
        }

        private bool CanHaveHive(int ind)
        {
            Building building = this.map.edificeGrid[ind];
            if (building != null && !MapComponent_HiveGrid.CanCoexistWithHive(building.def))
            {
                return false;
            }
            TerrainDef terrainDef = this.map.terrainGrid.TerrainAt(ind);
            return terrainDef == null || terrainDef.holdSnow;
        }

        public static bool CanCoexistWithHive(ThingDef def)
        {
            return def.category != ThingCategory.Building || def.Fillage != FillCategory.Full;
        }

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

        private void CheckVisualOrPathCostChange(IntVec3 c, float oldDepth, float newDepth)
        {
            if (!Mathf.Approximately(oldDepth, newDepth))
            {
                if (Mathf.Abs(oldDepth - newDepth) > 0.15f || Rand.Value < 0.0125f)
                {
                    this.map.mapDrawer.MapMeshDirty(c, (Verse.MapMeshFlag)ExtensionMethods.MapMeshFlag.Hive, true, false);
                    this.map.mapDrawer.MapMeshDirty(c, (Verse.MapMeshFlag)ExtensionMethods.MapMeshFlag.Hive, true, false);
                }
                else if (newDepth == 0f)
                {
                    this.map.mapDrawer.MapMeshDirty(c, (Verse.MapMeshFlag)ExtensionMethods.MapMeshFlag.Hive, true, false);
                }
                if (HiveUtility.GetSnowCategory(oldDepth) != HiveUtility.GetSnowCategory(newDepth))
                {
                    this.map.pathGrid.RecalculatePerceivedPathCostAt(c);
                }
            }
        }

        public float GetDepth(IntVec3 c)
        {
            if (!c.InBounds(this.map))
            {
                return 0f;
            }
            return this.depthGrid[this.map.cellIndices.CellToIndex(c)];
        }

        public HiveCategory GetCategory(IntVec3 c)
        {
            return HiveUtility.GetSnowCategory(this.GetDepth(c));
        }
        
        private float[] depthGrid;

        private double totalDepth;

        public const float MaxDepth = 1f;

    }
}
