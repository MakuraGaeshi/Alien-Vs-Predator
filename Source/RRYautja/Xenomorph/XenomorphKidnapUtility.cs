using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RRYautja
{
    // Token: 0x020000D7 RID: 215
    public static class XenomorphKidnapUtility
    {
        public static bool eggsPresent;
        public static bool eggsReachable;
        public static Thing closestReachableEgg;
        public static Thing closestReachableCocoontoEgg;

        public static bool cocoonsPresent;
        public static bool cocoonsReachable;
        public static Thing closestReachableCocoon;

        public static bool hivelikesPresent;
        public static bool hivelikesReachable;
        public static Thing closestReachableHivelike;

        public static bool hiveslimepresent;
        public static bool hiveslimeReachable;
        public static Thing closestreachablehiveslime;

        public static bool hiveshippresent;
        public static bool hiveshipReachable;
        public static Thing closestreachablehiveship;

        public static Thing cocoonThing;
        public static Thing eggThing;
        public static Thing hiveThing;

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

        public static bool TryFindGoodHiveLoc(Pawn pawn, Pawn victim, out IntVec3 c)
        {
            Map map = pawn.Map;
            c = IntVec3.Invalid;
            bool selected = map != null ? Find.Selector.SelectedObjects.Contains(pawn) && (Prefs.DevMode) : false;
            ThingDef named = victim.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;

            hiveshippresent = XenomorphUtil.HiveShipPresent(map);
            hiveshipReachable = !XenomorphUtil.ClosestReachableHiveShip(pawn).DestroyedOrNull();
            closestreachablehiveship = XenomorphUtil.ClosestReachableHiveShip(pawn);
            if (c == IntVec3.Invalid && hiveshippresent && hiveshipReachable)
            {
                if (Prefs.DevMode && DebugSettings.godMode) Log.Message(string.Format("(c == IntVec3.Invalid && hiveshippresent && hiveshipReachable)"));
                c = closestreachablehiveship.Position;
                if (Prefs.DevMode && DebugSettings.godMode) Log.Message(string.Format("(c == {0})",c));
                return true;
            }

            hivelikesPresent = XenomorphUtil.HivelikesPresent(map);
            hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull();
            closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(pawn);
            if ((hivelikesPresent && hivelikesReachable))
            {
                List<ThingDef_HiveLike> hivedefs = DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == pawn.Faction.def);

                if (XenomorphUtil.TotalSpawnedHivelikeCount(map) > 0)
                {
                    if (XenomorphUtil.TotalSpawnedParentHivelikeCount(map) > 0)
                    {
                        hiveThing = XenomorphUtil.TotalSpawnedParentHivelikeCount(map) > 1 ? XenomorphUtil.SpawnedParentHivelikes(map).RandomElement() : XenomorphUtil.ClosestReachableHivelike(pawn, XenomorphUtil.SpawnedParentHivelikes(map));
                        c = hiveThing.Position;
                        if (c != IntVec3.Invalid)
                        {
                            return true;
                        }
                    }
                    /*
                    if (XenomorphUtil.TotalSpawnedChildHivelikeCount(map) > 0)
                    {
                        hiveThing = XenomorphUtil.TotalSpawnedParentHivelikeCount(map) > 1 ? XenomorphUtil.SpawnedParentHivelikes(map).RandomElement() : XenomorphUtil.ClosestReachableHivelike(pawn, XenomorphUtil.SpawnedParentHivelikes(map));
                        c = hiveThing.Position;
                        return true;
                    }
                    */
                }
            }
            
            hiveslimepresent = XenomorphUtil.HiveSlimePresent(map);
            hiveslimeReachable = !XenomorphUtil.ClosestReachableHiveSlime(pawn).DestroyedOrNull();
            closestreachablehiveslime = XenomorphUtil.ClosestReachableHiveSlime(pawn);
            if (c == IntVec3.Invalid && hiveslimepresent && hiveslimeReachable)
            {
                c = closestreachablehiveslime.Position;
                if (c != IntVec3.Invalid)
                {
                    return true;
                }
            }
            
            eggsPresent = XenomorphUtil.EggsPresent(map);
            eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
            closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);
            if (c == IntVec3.Invalid && (eggsPresent && eggsReachable && XenomorphUtil.SpawnedEggsNeedHosts(map).Count > 0))
            {
                eggThing = XenomorphUtil.SpawnedEggsNeedHosts(map).Count > 1 ? XenomorphUtil.SpawnedEggsNeedHosts(map).RandomElement() : XenomorphUtil.ClosestReachableEggNeedsHost(pawn);
                c = eggThing.Position;
                if (c != IntVec3.Invalid)
                {
                    return true;
                }
            }

            cocoonsPresent = XenomorphUtil.CocoonsPresent(map, named);
            cocoonsReachable = !XenomorphUtil.ClosestReachableCocoon(pawn, named).DestroyedOrNull();
            closestReachableCocoon = XenomorphUtil.ClosestReachableCocoon(pawn, named);
            if (c == IntVec3.Invalid && cocoonsPresent && cocoonsReachable)
            {
                c = closestReachableCocoon.Position;
                if (c != IntVec3.Invalid)
                {
                    return true;
                }
            }

            if (c == IntVec3.Invalid)
            {
                if (!InfestationLikeCellFinder.TryFindCell(out c, map, false))
                {
                    if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                    {
                        return false;
                    }
                    if (c != IntVec3.Invalid)
                    {
                        return true;
                    }
                }
                else
                {

                    if (pawn.GetLord() != null && pawn.GetLord() is Lord lord)
                    {
                        Log.Message(string.Format("TryFindGoodHiveLoc pawn.GetLord() != null"));
                    }
                    
                    if (pawn.mindState.duty.def != OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike && pawn.mindState.duty.def != OGHiveLikeDefOf.RRY_DefendHiveLikeAggressively)
                    {
                        pawn.mindState.duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, c, 40f);

                        c = RCellFinder.RandomWanderDestFor(pawn, c, 5f, null, Danger.Some);
                        return true;
                    }
                    else
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, c, 5f, null, Danger.Some);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool TryFindGoodHiveLoc(Pawn pawn, out IntVec3 c, bool allowFogged = false)
        {
            Map map = pawn.Map;
            c = IntVec3.Invalid;
            bool selected = map != null ? Find.Selector.SelectedObjects.Contains(pawn) && (Prefs.DevMode) : false;
            ThingDef named = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;

            hivelikesPresent = XenomorphUtil.HivelikesPresent(map);
            hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull();
            closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(pawn);
            if ((hivelikesPresent && hivelikesReachable))
            {
                List<ThingDef_HiveLike> hivedefs = DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == pawn.Faction.def);

                if (XenomorphUtil.TotalSpawnedHivelikeCount(map) > 0)
                {
                    if (XenomorphUtil.TotalSpawnedParentHivelikeCount(map) > 0)
                    {
                        hiveThing = XenomorphUtil.TotalSpawnedParentHivelikeCount(map) > 1 ? XenomorphUtil.SpawnedParentHivelikes(map).RandomElement() : XenomorphUtil.ClosestReachableHivelike(pawn, XenomorphUtil.SpawnedParentHivelikes(map));
                        c = hiveThing.Position;
                        if (c != IntVec3.Invalid)
                        {
                            return true;
                        }
                    }
                    /*
                    if (XenomorphUtil.TotalSpawnedChildHivelikeCount(map) > 0)
                    {
                        hiveThing = XenomorphUtil.TotalSpawnedParentHivelikeCount(map) > 1 ? XenomorphUtil.SpawnedParentHivelikes(map).RandomElement() : XenomorphUtil.ClosestReachableHivelike(pawn, XenomorphUtil.SpawnedParentHivelikes(map));
                        c = hiveThing.Position;
                        return true;
                    }
                    */
                }
            }

            hiveslimepresent = XenomorphUtil.HiveSlimePresent(map);
            hiveslimeReachable = !XenomorphUtil.ClosestReachableHiveSlime(pawn).DestroyedOrNull();
            closestreachablehiveslime = XenomorphUtil.ClosestReachableHiveSlime(pawn);
            if (c == IntVec3.Invalid && hiveslimepresent && hiveslimeReachable)
            {
                c = closestreachablehiveslime.Position;
                if (c != IntVec3.Invalid)
                {
                    return true;
                }
            }

            hiveshippresent = XenomorphUtil.HiveShipPresent(map);
            hiveshipReachable = !XenomorphUtil.ClosestReachableHiveShip(pawn).DestroyedOrNull();
            closestreachablehiveship = XenomorphUtil.ClosestReachableHiveShip(pawn);
            if (c == IntVec3.Invalid && hiveshippresent && hiveshipReachable)
            {
                c = closestreachablehiveship.Position;
                if (c != IntVec3.Invalid)
                {
                    return true;
                }
            }

            eggsPresent = XenomorphUtil.EggsPresent(map);
            eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
            closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);
            if (c == IntVec3.Invalid && (eggsPresent && eggsReachable && XenomorphUtil.SpawnedEggsNeedHosts(map).Count > 0))
            {
                eggThing = XenomorphUtil.SpawnedEggsNeedHosts(map).Count > 1 ? XenomorphUtil.SpawnedEggsNeedHosts(map).RandomElement() : XenomorphUtil.ClosestReachableEggNeedsHost(pawn);
                c = eggThing.Position;
                if (c != IntVec3.Invalid)
                {
                    return true;
                }
            }

            cocoonsPresent = XenomorphUtil.CocoonsPresent(map, named);
            cocoonsReachable = !XenomorphUtil.ClosestReachableCocoon(pawn, named).DestroyedOrNull();
            closestReachableCocoon = XenomorphUtil.ClosestReachableCocoon(pawn, named);
            if (c == IntVec3.Invalid && cocoonsPresent && cocoonsReachable)
            {
                c = closestReachableCocoon.Position;
                if (c != IntVec3.Invalid)
                {
                    return true;
                }
            }

            if (c == IntVec3.Invalid)
            {
                if (!InfestationLikeCellFinder.TryFindCell(out c, map, false))
                {
                    if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                    {
                        return false;
                    }
                    if (c != IntVec3.Invalid)
                    {
                        return true;
                    }
                }
                else
                {

                    if (pawn.GetLord() != null && pawn.GetLord() is Lord lord)
                    {
                        Log.Message(string.Format("TryFindGoodHiveLoc pawn.GetLord() != null"));
                    }

                    if (pawn.mindState.duty.def != OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike && pawn.mindState.duty.def != OGHiveLikeDefOf.RRY_DefendHiveLikeAggressively)
                    {
                        pawn.mindState.duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, c, 40f);

                        c = RCellFinder.RandomWanderDestFor(pawn, c, 5f, null, Danger.Some);
                        return true;
                    }
                    else
                    {
                        c = RCellFinder.RandomWanderDestFor(pawn, c, 5f, null, Danger.Some);
                        return true;
                    }
                }
            }
            return false;
        }



        // Token: 0x060004D2 RID: 1234 RVA: 0x00031074 File Offset: 0x0002F474
        public static bool TryFindGoodImpregnateVictim(Pawn kidnapper, float maxDist, out Pawn victim, List<Thing> disallowed = null)
        {
            if (!kidnapper.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || !kidnapper.Map.reachability.CanReachMapEdge(kidnapper.Position, TraverseParms.For(kidnapper, Danger.Some, TraverseMode.ByPawn, false)))
            {
            //    Log.Message(string.Format("TryFindGoodImpregnateVictim \n{0} incapable of job", kidnapper.LabelShortCap));
                victim = null;
                return false;
            }
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn = t as Pawn;
                bool cocoonFlag = !pawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
                bool pawnFlag = ((XenomorphUtil.isInfectablePawn(pawn))) && !XenomorphUtil.IsXenomorph(pawn) && pawn.gender == Gender.Female && pawn.Downed && (pawn.Faction == null || pawn.Faction.HostileTo(kidnapper.Faction) || kidnapper.Faction == null);
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
