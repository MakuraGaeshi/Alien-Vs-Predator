using RRYautja;
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
                    x = pawn.mindState.duty.focus.Cell.x + 4,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x - 4,
                    z = pawn.mindState.duty.focus.Cell.z
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z - 4
                },
                new IntVec3
                {
                    x = pawn.mindState.duty.focus.Cell.x,
                    z = pawn.mindState.duty.focus.Cell.z - 4
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
                    if (c.InBounds(pawn.Map))
                    {

                        Building edifice = c.GetEdifice(pawn.Map);
                        if (edifice != null && (edifice.def.passability == Traversability.Impassable || edifice.def.IsDoor) && edifice.def.size == IntVec2.One && edifice.def != ThingDefOf.CollapsedRocks && pawn.CanReserve(edifice, 1, -1, null, false) && XenomorphUtil.DistanceBetween(edifice.Position, pawn.mindState.duty.focus.Cell) <= MiningRange)
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
            IntVec3 hiveCell = pawn.mindState.duty.focus.Cell;
            CellRect eggRect = new CellRect(hiveCell.x - 1, hiveCell.z - 1, 3, 3);
            foreach (var c in eggRect)
            {
                if (c.GetFirstHaulable(pawn.Map) is Thing h && !(h is Pawn) && !(h is Building_XenoEgg) && !(h is Building_XenomorphCocoon))
                {
                    if (pawn.CanReserve(h, 1, -1, null, false))
                    {
                        if (!XenomorphUtil.CanHaulAside(pawn, h, hiveCell, ClearingRange, out IntVec3 c2))
                        {
                            return null;
                        }
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
            for (int i = 0; i < 40; i++)
            {
                IntVec3 hiveCell = pawn.mindState.duty.focus.Cell;
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
