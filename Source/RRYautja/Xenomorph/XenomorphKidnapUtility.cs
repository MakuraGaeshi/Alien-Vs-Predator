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
                Thing egg = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
                return XenomorphUtil.DistanceBetween(pawn.Position, egg.Position)>3 && XenomorphUtil.isInfectablePawn(pawn) && pawn.Downed && (pawn.Faction == null || pawn.Faction.HostileTo(kidnapper.Faction)) && kidnapper.CanReserve(pawn, 1, -1, null, false) && (disallowed == null || !disallowed.Contains(pawn));
            };
            victim = (Pawn)GenClosest.ClosestThingReachable(kidnapper.Position, kidnapper.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some, false), maxDist, validator, null, 0, -1, false, RegionType.Set_Passable, false);
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
    }
}
