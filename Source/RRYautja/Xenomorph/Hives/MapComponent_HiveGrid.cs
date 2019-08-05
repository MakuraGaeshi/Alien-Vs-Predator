using RimWorld;
using RRYautja.ExtensionMethods;
using System.Collections.Generic;
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
            this.potentialHosts = new List<Pawn>();
            this.nonpotentialHosts = new List<Pawn>();
            this.Queenlist = new List<Pawn>();
            this.Dronelist = new List<Pawn>();
            this.Warriorlist = new List<Pawn>();
            this.Runnerlist = new List<Pawn>();
            this.Predalienlist = new List<Pawn>();
            this.Thrumbomorphlist = new List<Pawn>();
            this.HiveGuardlist = new List<Pawn>();
            this.HiveWorkerlist = new List<Pawn>();
            this.Hivelist = new List<Thing>();
            this.HiveChildlist = new List<Thing>();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
        }
        
        public override void MapGenerated()
        {
            base.MapGenerated();
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            //    this.HiveGrid.Regenerate();
        }
        
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            /*
            if (true)
            {
                bool hiveship = XenomorphUtil.HiveShipPresent(this.map);
                bool hivetunnel = XenomorphUtil.HivelikesPresent(this.map);
                bool hiveslime = XenomorphUtil.HiveSlimePresent(this.map);
                if (!hiveship && !hivetunnel && !hiveslime)
                {
                    for (int i = 0; i < depthGrid.Length; i++)
                    {
                        if (depthGrid[i] > 0f)
                        {
                            HiveUtility.AddHiveRadial(this.map.cellIndices.IndexToCell(i), map, 1, -Rand.RangeSeeded(0.0001f,0.001f, AvPConstants.AvPSeed));
                        }
                    }
                }
            }
            */
            if (Find.TickManager.TicksGame % 500 == 0)
            {
            //    Log.Message(string.Format("MapComponentTick update lists"));
                potentialHosts = map.ViableHosts();
                nonpotentialHosts = map.InviableHosts();
                Queenlist = map.mapPawns.AllPawnsSpawned.FindAll(x => x.def == XenomorphRacesDefOf.RRY_Xenomorph_Queen);
                Dronelist = map.mapPawns.AllPawnsSpawned.FindAll(x => x.def == XenomorphRacesDefOf.RRY_Xenomorph_Drone);
                Warriorlist = map.mapPawns.AllPawnsSpawned.FindAll(x => x.def == XenomorphRacesDefOf.RRY_Xenomorph_Warrior);
                Runnerlist = map.mapPawns.AllPawnsSpawned.FindAll(x => x.def == XenomorphRacesDefOf.RRY_Xenomorph_Runner);
                Predalienlist = map.mapPawns.AllPawnsSpawned.FindAll(x => x.def == XenomorphRacesDefOf.RRY_Xenomorph_Predalien);
                Thrumbomorphlist = map.mapPawns.AllPawnsSpawned.FindAll(x => x.def == XenomorphRacesDefOf.RRY_Xenomorph_Thrumbomorph);
                Hivelist = map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_XenomorphHive);
                HiveChildlist = map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_XenomorphHive_Child);
                foreach (HiveLike item in HiveChildlist)
                {
                    if (!item.GetDirectlyHeldThings().NullOrEmpty())
                    {
                        IList<Thing> l = item.GetDirectlyHeldThings();
                        if (Hivelist != null)
                        {
                            int drone = 0;
                            int warrior = 0;
                            int runner = 0;
                            HiveLike main = (HiveLike)Hivelist.RandomElement();
                            foreach (Pawn i in l)
                            {
                                if (i.def == XenomorphRacesDefOf.RRY_Xenomorph_Runner)
                                {
                                    runner++;
                                    if (runner > 2)
                                    {
                                        main.GetDirectlyHeldThings().TryAddOrTransfer(i);
                                    }
                                }
                                if (i.def == XenomorphRacesDefOf.RRY_Xenomorph_Drone)
                                {
                                    drone++;
                                    if (drone > 2)
                                    {
                                        main.GetDirectlyHeldThings().TryAddOrTransfer(i);
                                    }
                                }
                                if (i.def == XenomorphRacesDefOf.RRY_Xenomorph_Warrior)
                                {
                                    warrior++;
                                    if (warrior > 2)
                                    {
                                        main.GetDirectlyHeldThings().TryAddOrTransfer(i);
                                    }
                                }
                                if (i.def == XenomorphRacesDefOf.RRY_Xenomorph_Predalien)
                                {
                                    main.GetDirectlyHeldThings().TryAddOrTransfer(i);
                                }
                                if (i.def == XenomorphRacesDefOf.RRY_Xenomorph_Queen)
                                {
                                    main.GetDirectlyHeldThings().TryAddOrTransfer(i);
                                }
                                if (i.def == XenomorphRacesDefOf.RRY_Xenomorph_Thrumbomorph)
                                {
                                    main.GetDirectlyHeldThings().TryAddOrTransfer(i);
                                }
                                if (!l.Contains(i))
                                {
                                    if (main.GetDirectlyHeldThings().Contains(i))
                                    {
                                        if (main.Lord != null)
                                        {
                                            i.switchLord(main.Lord);
                                        }
                                        else
                                        {
                                            //    Log.Message(string.Format("{0} lord is null", main, i.LabelShortCap));
                                        }
                                    }
                                    else
                                    {
                                        //   Log.Message(string.Format("{0} doesnt contain {1}", main, i.LabelShortCap));
                                    }
                                }
                                else
                                {
                                    //    Log.Message(string.Format("{0} still contains {1}", item, i.LabelShortCap));
                                }
                            }
                        }
                    }
                }
            }
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
            return terrainDef.passability != Traversability.Impassable;// terrainDef == null || terrainDef.holdSnow;
        }

        public static bool CanCoexistWithHive(ThingDef def)
        {
            return def.category != ThingCategory.Building || def.Fillage != FillCategory.Full || def == XenomorphDefOf.RRY_XenomorphCrashedShipPart;
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

        public override void ExposeData()
        {
            MapExposeUtility.ExposeUshort(this.map, (IntVec3 c) => MapComponent_HiveGrid.HiveFloatToShort(this.GetDepth(c)), delegate (IntVec3 c, ushort val)
            {
                this.depthGrid[this.map.cellIndices.CellToIndex(c)] = MapComponent_HiveGrid.HiveShortToFloat(val);
            }, "depthGrid");
            Scribe_Collections.Look<Pawn>(ref this.HiveGuardlist, "HiveGuardlist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.HiveWorkerlist, "HiveWorkerlist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.potentialHosts, "potentialHosts", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.nonpotentialHosts, "nonpotentialHosts", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.Queenlist, "Queenlist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.Dronelist, "Dronelist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.Warriorlist, "Warriorlist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.Runnerlist, "Runnerlist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.Predalienlist, "Predalienlist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.Thrumbomorphlist, "Thrumbomorphlist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Thing>(ref this.Hivelist, "Hivelist", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Thing>(ref this.HiveChildlist, "HiveChildlist", LookMode.Reference, new object[0]);
            base.ExposeData();
        }

        public MapComponent_HiveGrid HiveGrid;
        public List<Thing> Hivelist;
        public List<Thing> HiveChildlist;
        public List<Pawn> HiveGuardlist;
        public List<Pawn> HiveWorkerlist;
        public List<Pawn> potentialHosts;
        public List<Pawn> nonpotentialHosts;
        public List<Pawn> Queenlist;
        public List<Pawn> Dronelist;
        public List<Pawn> Warriorlist;
        public List<Pawn> Runnerlist;
        public List<Pawn> Predalienlist;
        public List<Pawn> Thrumbomorphlist;

        private float[] depthGrid;

        private double totalDepth;

        public const float MaxDepth = 1f;

    }
}
