using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RRYautja
{
    // Token: 0x020000D7 RID: 215
    public static class XenomorphKidnapUtility
    {
        // Token: 0x060004D2 RID: 1234 RVA: 0x00031074 File Offset: 0x0002F474
        public static bool TryFindGoodKidnapVictim(Pawn kidnapper, float maxDist, out Pawn victim, List<Thing> disallowed = null)
        {
            if (!kidnapper.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || !kidnapper.Map.reachability.CanReachMapEdge(kidnapper.Position, TraverseParms.For(kidnapper, Danger.Some, TraverseMode.ByPawn, false)))
            {
                victim = null;
                return false;
            }
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn = t as Pawn;
                bool cocoonFlag = !pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
                bool pawnFlag = ((XenomorphUtil.isInfectablePawn(pawn, true))) && pawn.Downed && (pawn.Faction == null || pawn.Faction.HostileTo(kidnapper.Faction));
                return  cocoonFlag && pawnFlag && kidnapper.CanReserve(pawn, 1, -1, null, false) && (disallowed == null || !disallowed.Contains(pawn));
            };
            victim = (Pawn)GenClosest.ClosestThingReachable(kidnapper.Position, kidnapper.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some, false), maxDist, validator, null, 0, -1, false, RegionType.Set_Passable, false);
            return victim != null;
        }

        // Token: 0x060004D2 RID: 1234 RVA: 0x00031074 File Offset: 0x0002F474
        public static bool TryFindGoodImpregnateVictim(Pawn kidnapper, float maxDist, out Pawn victim, List<Thing> disallowed = null)
        {
            if (!kidnapper.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)) // || !kidnapper.Map.reachability.CanReachMapEdge(kidnapper.Position, TraverseParms.For(kidnapper, Danger.Some, TraverseMode.ByPawn, false)))
            {
            //    Log.Message(string.Format("TryFindGoodImpregnateVictim \n{0} incapable of job", kidnapper.LabelShortCap));
                victim = null;
                return false;
            }
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn = t as Pawn;
                bool cocoonFlag = !pawn.InBed() || (pawn.InBed() && (pawn.CurrentBed() is Building_XenomorphCocoon));
                bool pawnFlag = ((XenomorphUtil.isInfectablePawn(pawn)) || (XenomorphUtil.isInfectablePawn(pawn, false))) && pawn.Downed && (pawn.Faction == null || pawn.Faction.HostileTo(kidnapper.Faction) || kidnapper.Faction == null);
            //    Log.Message(string.Format(" cocoonFlag; {0} \n pawnFlag: {1}", cocoonFlag, pawnFlag));
                return (cocoonFlag && pawnFlag) && (kidnapper.CanReserve(pawn, 1, -1, null, false) && (disallowed == null || !disallowed.Contains(pawn))) && pawn != kidnapper && pawn.gender == Gender.Female;
            };
        //    Log.Message(string.Format("TryFindGoodImpregnateVictim \nvalidator {0}", validator));
            victim = (Pawn)GenClosest.ClosestThingReachable(kidnapper.Position, kidnapper.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some, false), maxDist, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        //    Log.Message(string.Format("TryFindGoodImpregnateVictim \nvictim {0}", victim));
            return victim != null;
        }

        // Token: 0x060004D3 RID: 1235 RVA: 0x0003113C File Offset: 0x0002F53C
        public static Pawn ReachableWoundedGuest(Pawn searcher)
        {
            List<Pawn> list = searcher.Map.mapPawns.SpawnedPawnsInFaction(searcher.Faction);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i];
                if (pawn.guest != null && !pawn.IsPrisoner && pawn.Downed && searcher.CanReserveAndReach(pawn, PathEndMode.OnCell, Danger.Some, 1, -1, null, false))
                {
                    return pawn;
                }
            }
            return null;
        }

        // Token: 0x0600000B RID: 11 RVA: 0x00002C9C File Offset: 0x00000E9C
        private static bool IsMapRectClear(CellRect mapRect, Map map)
        {
            foreach (IntVec3 intVec in mapRect)
            {
                bool flag = !map.pathGrid.WalkableFast(intVec);
                if (flag)
                {
                    return false;
                }
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag2 = thingList[i].def.category == (ThingCategory)3 || thingList[i].def.category == (ThingCategory)1 || thingList[i].def.category == (ThingCategory)10;
                    if (flag2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        public static List<IntVec3> XenoCocoonLocations(IntVec3 center, float radius, Map map)
        {
            int num = GenRadial.NumCellsInRadius(radius);
            List<IntVec3> list = new List<IntVec3>();
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = center + GenRadial.RadialPattern[i];
                CellRect rect = new CellRect(intVec.x, intVec.z, 1, 1);
                if (intVec.InBounds(map))
                {
                    if (IsMapRectClear(rect, map))
                    {
                        list.Add(intVec);
                    }
                }
            }
            return list;
        }

    }
}
