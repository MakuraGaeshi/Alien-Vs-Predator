﻿using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x020000A4 RID: 164
    public class JobGiver_MineRandomNearHive : ThinkNode_JobGiver
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_MineRandomNearHive jobGiver_MineRandomNearHive = (JobGiver_MineRandomNearHive)base.DeepCopy(resolve);
            jobGiver_MineRandomNearHive.MiningRange = this.MiningRange;
            return jobGiver_MineRandomNearHive;
        }

        private int MiningRange = 5;

        // Token: 0x0600041E RID: 1054 RVA: 0x0002CA54 File Offset: 0x0002AE54
        protected override Job TryGiveJob(Pawn pawn)
        {
            List<IntVec3> pillarLoc = new List<IntVec3>()
            {
                // Cardinals
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 3,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 3,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z + 3
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z - 3
                },
                // Corners
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 2,
                    z = pawn.mindState.duty.focus.Cell.z + 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 2,
                    z = pawn.mindState.duty.focus.Cell.z + 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 2,
                    z = pawn.mindState.duty.focus.Cell.z - 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 2,
                    z = pawn.mindState.duty.focus.Cell.z - 2
                }
            };
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            if (region == null)
            {
                return null;
            }
            for (int i = 0; i < 40; i++)
            {
                IntVec3 randomCell = region.RandomCell;
                for (int j = 0; j < 4; j++)
                {
                    IntVec3 c = randomCell + GenAdj.CardinalDirections[j];
                    if (c.InBounds(pawn.Map) && c.Roofed(pawn.Map))
                    {

                        Building edifice = c.GetEdifice(pawn.Map);
                        if (edifice != null && (edifice.def.passability == Traversability.Impassable || edifice.def.IsDoor) && edifice.def.size == IntVec2.One && edifice.def != ThingDefOf.CollapsedRocks && edifice.def != XenomorphDefOf.RRY_Xenomorph_HiveWall && pawn.CanReserve(edifice, 1, -1, null, false) && XenomorphUtil.DistanceBetween(edifice.Position, pawn.mindState.duty.focus.Cell) <= MiningRange)
                        {
                            if (!pillarLoc.Contains(edifice.Position))
                            {
                                return new Job(JobDefOf.Mine, edifice)
                                {
                                    ignoreDesignations = true
                                };
                            }
                        }
                        /*
                        else if (edifice==null)
                        {
                            if (!pillarLoc.Contains(edifice.Position))
                            {
                                return new Job(JobDefOf.Mine, edifice)
                                {
                                    ignoreDesignations = true
                                };
                            }
                        }
                        */
                    }
                }
            }
            return null;
        }

    }

    // Token: 0x020000A4 RID: 164
    public class JobGiver_MineOutHive : ThinkNode_JobGiver
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_MineOutHive jobGiver_MineOutHive = (JobGiver_MineOutHive)base.DeepCopy(resolve);
            jobGiver_MineOutHive.MiningRange = this.MiningRange;
            return jobGiver_MineOutHive;
        }

        private int MiningRange = 5;

        private float CellsInScanRadius
        {
            get
            {
                return (float)GenRadial.NumCellsInRadius(MiningRange);
            }
        }

        // Token: 0x0600041E RID: 1054 RVA: 0x0002CA54 File Offset: 0x0002AE54
        protected override Job TryGiveJob(Pawn pawn)
        {
            Map map = pawn.Map;
            List<IntVec3> pillarLoc = new List<IntVec3>()
            {
                // Cardinals
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 3,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 3,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z + 3
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z - 3
                },
                // Corners
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 2,
                    z = pawn.mindState.duty.focus.Cell.z + 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 2,
                    z = pawn.mindState.duty.focus.Cell.z + 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x + 2,
                    z = pawn.mindState.duty.focus.Cell.z - 2
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 2,
                    z = pawn.mindState.duty.focus.Cell.z - 2
                }
            };
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            bool flag1 = pawn.mindState.duty.focus.Cell.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_XenomorphHive) != null;
            bool flag2 = pawn.mindState.duty.focus.Cell.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_XenomorphHive_Child) != null;
            bool flag3 = pawn.mindState.duty.focus.Cell.GetFirstThing(pawn.Map, XenomorphDefOf.RRY_Hive_Slime) != null;
            if (pawn.GetLord() is Lord L && L != null)
            {
                if ((L.CurLordToil is LordToil_DefendAndExpandHiveLike || L.CurLordToil is LordToil_DefendHiveLikeAggressively) && L.CurLordToil is LordToil_HiveLikeRelated THL)
                {
                    if (THL != null)
                    {
                        if (THL.Data!=null)
                        {
                            HiveLike hive = THL.Data.assignedHiveLikes.TryGetValue(pawn);
                            if (hive!=null)
                            {
                                flag1 = hive.def == XenomorphDefOf.RRY_XenomorphHive;
                                flag2 = hive.def == XenomorphDefOf.RRY_XenomorphHive_Child;
                            }
                        }
                    }
                }
            }
            if (region == null)
            {
                return null;
            }
            if (flag2)
            {
                MiningRange = 3;
            }
            for (int i = 0; i < GenRadial.NumCellsInRadius(MiningRange); i++)
            {
                IntVec3 c = pawn.mindState.duty.focus.Cell + GenRadial.RadialPattern[MiningRange];
                if (!pillarLoc.Contains(c))
                {
                    Building edifice = c.GetEdifice(pawn.Map);
                    if (edifice != null && (edifice.def.passability == Traversability.Impassable /* || edifice.def.IsDoor */) && edifice.def.size == IntVec2.One && edifice.def != ThingDefOf.CollapsedRocks && edifice.def != XenomorphDefOf.RRY_Xenomorph_HiveWall && pawn.CanReserve(edifice, 1, -1, null, false) && XenomorphUtil.DistanceBetween(edifice.Position, pawn.mindState.duty.focus.Cell) <= MiningRange)
                    {
                        if (!pillarLoc.Contains(edifice.Position))
                        {
                            return new Job(JobDefOf.Mine, edifice)
                            {
                                ignoreDesignations = true
                            };
                        }
                    }
                }
            }

            return null;
        }

    }

    // Token: 0x020000A4 RID: 164
    public class JobGiver_ClearHiveEggZone : ThinkNode_JobGiver
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_ClearHiveEggZone jobGiver_ClearHiveEggZone = (JobGiver_ClearHiveEggZone)base.DeepCopy(resolve);
            jobGiver_ClearHiveEggZone.ClearingRange = this.ClearingRange;
            return jobGiver_ClearHiveEggZone;
        }
        
        private int ClearingRange = 2;

        // Token: 0x0600041E RID: 1054 RVA: 0x0002CA54 File Offset: 0x0002AE54
        protected override Job TryGiveJob(Pawn pawn)
        {
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            if (region == null)
            {
                return null;
            }
            
            IntVec3 hiveCell;
            if (!XenomorphUtil.ClosestReachableParentHivelike(pawn).DestroyedOrNull())
            {
                hiveCell = XenomorphUtil.ClosestReachableParentHivelike(pawn).Position;
            }
            else if (!XenomorphUtil.ClosestReachableHiveSlime(pawn).DestroyedOrNull())
            {
                hiveCell = XenomorphUtil.ClosestReachableHiveSlime(pawn).Position;
            }
            else
            {
                hiveCell = pawn.mindState.duty.focus.Cell;
            }
            foreach (var c in GenAdj.AdjacentCellsAndInside)
            {
                if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode) Log.Message(string.Format("{0} TryGiveJob {1} @ {2}", this, pawn.LabelShortCap, (hiveCell + c)));
                if ((hiveCell+c).GetFirstHaulable(pawn.Map) is Thing h && !(h is Pawn) && !(h is Building_XenoEgg) && !(h is Building_XenomorphCocoon))
                {
                    if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode) Log.Message(string.Format("{0} TryGiveJob {1} @ {2} 1", this, pawn.LabelShortCap, (hiveCell + c)));
                    if (pawn.CanReserve(h, 1, -1, null, false))
                    {
                        if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode) Log.Message(string.Format("{0} TryGiveJob {1} @ {2} 2", this, pawn.LabelShortCap, (hiveCell + c)));
                        if (!XenomorphUtil.CanHaulAside(pawn, h, hiveCell, ClearingRange, out IntVec3 c2))
                        {
                            if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode) Log.Message(string.Format("{0} TryGiveJob {1} @ {2} 3", this, pawn.LabelShortCap, (hiveCell + c)));
                            return null;
                        }
                        if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode) Log.Message(string.Format("{0} TryGiveJob {1} @ {2} 4", this, pawn.LabelShortCap, (hiveCell + c)));
                        return new Job(JobDefOf.HaulToCell, h, c2)
                        {
                            count = 99999,
                            haulOpportunisticDuplicates = false,
                            haulMode = HaulMode.ToCellNonStorage,
                            ignoreDesignations = true
                        };
                    }
                }
            }
            return null;
        }
    }
    // Token: 0x020000A4 RID: 164
    public class JobGiver_ClearNearHive : ThinkNode_JobGiver
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_ClearNearHive jobGiver_ClearNearHive = (JobGiver_ClearNearHive)base.DeepCopy(resolve);
            jobGiver_ClearNearHive.minClearingRange = this.minClearingRange;
            jobGiver_ClearNearHive.maxClearingRange = this.maxClearingRange;
            return jobGiver_ClearNearHive;
        }

        private int minClearingRange = 1;
        private int maxClearingRange = 5;

        // Token: 0x0600041E RID: 1054 RVA: 0x0002CA54 File Offset: 0x0002AE54
        protected override Job TryGiveJob(Pawn pawn)
        {
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            if (region == null)
            {
                return null;
            }
            IntVec3 hiveCell;
            if (!XenomorphUtil.ClosestReachableParentHivelike(pawn).DestroyedOrNull())
            {
                hiveCell = XenomorphUtil.ClosestReachableParentHivelike(pawn).Position;
            }
            else if (!XenomorphUtil.ClosestReachableHiveSlime(pawn).DestroyedOrNull())
            {
                hiveCell = XenomorphUtil.ClosestReachableHiveSlime(pawn).Position;
            }
            else
            {
                hiveCell = pawn.mindState.duty.focus.Cell;
            }
            for (int i = 0; i < 40; i++)
            {
                IntVec3 randomCell = region.RandomCell;

                for (int j = 0; j < 8; j++)
                {
                    IntVec3 c = randomCell + GenAdj.AdjacentCellsAround[j];
                    if (c.InBounds(pawn.Map))
                    {

                        Thing thing = c.GetFirstHaulable(pawn.Map);
                    //    Log.Message(string.Format("thing: {0}", thing));
                        if (thing != null && (thing.def.passability == Traversability.Impassable || thing.def.IsDoor) && thing.def.size == IntVec2.One && thing.def != ThingDefOf.CollapsedRocks && pawn.CanReserve(thing, 1, -1, null, false) && XenomorphUtil.DistanceBetween(thing.Position, pawn.mindState.duty.focus.Cell) >= minClearingRange && XenomorphUtil.DistanceBetween(thing.Position, pawn.mindState.duty.focus.Cell) <= maxClearingRange)
                        {
                            return XenomorphUtil.HaulAsideJobFor(pawn, thing, hiveCell, maxClearingRange);
                        }
                    }
                }
            }
            return null;
        }
    }
}
