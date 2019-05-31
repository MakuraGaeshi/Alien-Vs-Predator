using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RRYautja
{
    class XenomorphUtil
    {
        // Token: 0x060000A8 RID: 168 RVA: 0x00007234 File Offset: 0x00005434
        public static HashSet<Thing> XenomorphCocoonsFor(Map map, Thing t)
        {
            HashSet<Thing> wildCocoons = map.GetComponent<MapComponent_XenomorphCocoonTracker>().WildCocoons;
            bool flag = (wildCocoons != null || wildCocoons.Count > 0) && t.Faction != Faction.OfPlayerSilentFail;
            HashSet<Thing> result;
            if (flag)
            {
                result = wildCocoons;
            }
            else
            {
                HashSet<Thing> domesticCocoons = map.GetComponent<MapComponent_XenomorphCocoonTracker>().DomesticCocoons;
                bool flag2 = (domesticCocoons != null || domesticCocoons.Count > 0) && t.Faction == Faction.OfPlayerSilentFail;
                if (flag2)
                {
                    result = new HashSet<Thing>(from x in domesticCocoons
                                                where ForbidUtility.InAllowedArea(x.PositionHeld, t as Pawn)
                                                select x);
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }
        public static bool isInfectablePawn(Pawn pawn)
        {
            if (pawn.Dead) return false;
            if (pawn.RaceProps.IsMechanoid) return false;
            if (!pawn.RaceProps.IsFlesh) return false;
            if (IsInfectedPawn(pawn)) return false;
            if (IsXenomorph(pawn)) return false;
            if (IsXenomorphFaction(pawn)) return false;
            if (pawn.BodySize < 0.65f) return false;
            return true;
        }
        public static bool isInfectablePawn(Pawn pawn, bool allowinfected = false)
        {
            if (pawn.Dead) return false;
            if (pawn.RaceProps.IsMechanoid) return false;
            if (!pawn.RaceProps.IsFlesh) return false;
            if (!allowinfected && IsInfectedPawn(pawn)) return false;
            if (IsXenomorph(pawn)) return false;
            if (IsXenomorphFaction(pawn)) return false;
            if (pawn.BodySize < 0.65f) return false;
            return true;
        }
        public static bool isInfectablePawnKind(PawnKindDef pawn)
        {
            if (pawn.RaceProps.IsMechanoid) return false;
            if (!pawn.RaceProps.IsFlesh) return false;
            if (pawn.race.defName.Contains("RRY_Xenomorph_")) return false;
            if (pawn.RaceProps.baseBodySize < 0.65f) return false;
            return true;
        }
        public static bool isXenomorphInfectedPawn(Pawn pawn)
        {
            HediffSet hediffSet = pawn.health.hediffSet;
            if (hediffSet.HasHediff(XenomorphDefOf.RRY_FaceHuggerInfection, false)) return true;
            if (hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenXenomorphImpregnation, false)) return true;
            if (hediffSet.HasHediff(XenomorphDefOf.RRY_XenomorphImpregnation, false)) return true;
            return false;
        }
        public static bool IsXenomorphPawn(Pawn pawn)
        {
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_FaceHugger) return true;
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Predalien) return true;
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Runner) return true;
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Drone) return true;
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Warrior) return true;
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen) return true;
            return false;
        }
        public static bool IsXenomorphCorpse(Corpse corpse)
        {
            if (corpse.InnerPawn.kindDef == XenomorphDefOf.RRY_Xenomorph_FaceHugger) return true;
            if (corpse.InnerPawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Predalien) return true;
            if (corpse.InnerPawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Runner) return true;
            if (corpse.InnerPawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Drone) return true;
            if (corpse.InnerPawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Warrior) return true;
            if (corpse.InnerPawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen) return true;
            return false;
        }
        public static bool isNeomorphInfectedPawn(Pawn pawn)
        {
            HediffSet hediffSet = pawn.health.hediffSet;
            if (hediffSet.HasHediff(XenomorphDefOf.RRY_HiddenNeomorphImpregnation, false)) return true;
            if (hediffSet.HasHediff(XenomorphDefOf.RRY_NeomorphImpregnation, false)) return true;
            return false;
        }
        public static bool IsNeomorphPawn(Pawn pawn)
        {
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Neomorph) return true;
            return false;
        }

        public static bool IsNeomorphCorpse(Corpse corpse)
        {
            if (corpse.InnerPawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Neomorph) return true;
            return false;
        }

        public static bool IsInfectedPawn(Pawn pawn)
        {
            if (isXenomorphInfectedPawn(pawn) || isNeomorphInfectedPawn(pawn)) return true;
            return false;
        }
        public static bool IsXenomorph(Pawn pawn)
        {
            if (IsXenomorphPawn(pawn) || IsNeomorphPawn(pawn)) return true;
            return false;
        }
        public static bool IsXenoCorpse(Corpse corpse)
        {
            if (IsXenomorphCorpse(corpse) || IsNeomorphCorpse(corpse)) return true;
            return false;
        }
        public static bool IsXenomorphFaction(Pawn pawn)
        {
            if (pawn.Faction == Find.FactionManager.FirstFactionOfDef(XenomorphDefOf.RRY_Xenomorph)) return true;
            return false;
        }
        public static bool QueenPresent(Map map, out Pawn Queen)
        {
            foreach (var p in map.mapPawns.AllPawnsSpawned)
            {
                if (p.kindDef == XenomorphDefOf.RRY_Xenomorph_Queen)
                {
                    Queen = p;
                    return true;
                }
            }
            Queen = null;
            return false;
        }

        // Egg stuff
        public static bool EggsPresent(Map map)
        {
            return TotalSpawnedEggCount(map) > 0;
        }
        public static Thing ClosestReachableEgg(Pawn pawn)
        {
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableEggNeedsHost(Pawn pawn)
        {
            List<Thing> list = SpawnedEggsNeedHosts(pawn.Map);
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, (x => list.Contains(x)), null, 0, -1, false, RegionType.Set_Passable, false);

            return thing;
        }
        public static int TotalSpawnedEggCount(Map map)
        {
            return map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized).Count;
        }
        public static int TotalSpawnedEggNeedHostsCount(Map map)
        {
            return SpawnedEggsNeedHosts(map).Count;
        }
        public static List<Thing> SpawnedEggs(Map map)
        {
            return map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_EggXenomorphFertilized);
        }
        public static int TotalSpawnedThingCount(ThingDef thing, Map map)
        {
            return map.listerThings.ThingsOfDef(thing).Count;
        }
        public static List<Thing> SpawnedThings(ThingDef thing, Map map)
        {
            return map.listerThings.ThingsOfDef(thing);
        }
        public static List<Thing> SpawnedEggsNeedHosts(Map map)
        {
            List<Thing> list = new List<Thing>();
            foreach (var item in SpawnedEggs(map))
            {
                Pawn host = (Pawn)GenClosest.ClosestThingReachable(item.Position, item.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 1, x => XenomorphUtil.isInfectablePawn(((Pawn)x)), null, 0, -1, false, RegionType.Set_Passable, false);
                if (host == null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        // Cocoon stuff
        public static bool CocoonsPresent(Map map, ThingDef t)
        {
            return TotalSpawnedCocoonCount(map, t) > 0;
        }
        public static bool EmptyCocoonsPresent(Map map, ThingDef t)
        {
            return TotalSpawnedEmptyCocoonCount(map, t) > 0;
        }
        public static Thing ClosestReachableCocoon(Pawn pawn, ThingDef t)
        {
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableEmptyCocoon(Pawn pawn, ThingDef t)
        {
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, (x => (x is Building_XenomorphCocoon XC && XC.AnyUnoccupiedSleepingSlot)), null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableCocoonToEgg(Thing egg, ThingDef t)
        {
            Thing thing = GenClosest.ClosestThingReachable(egg.Position, egg.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 10f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableEmptyCocoonToEgg(Thing egg, ThingDef t)
        {
            Thing thing = GenClosest.ClosestThingReachable(egg.Position, egg.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 10f, (x => (x is Building_XenomorphCocoon XC && XC.AnyUnoccupiedSleepingSlot && XC.owners.NullOrEmpty())), null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableCocoonThatEggNeedsHost(Pawn pawn, ThingDef t)
        {
            Thing thing;
            List<Thing> list = SpawnedEggsNeedHosts(pawn.Map);
            thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, (x => list.Contains(x)), null, 0, -1, false, RegionType.Set_Passable, false);
            thing = GenClosest.ClosestThingReachable(thing.Position, thing.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 10f, (x => (x is Building_XenomorphCocoon XC)), null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableEmptyCocoonThatEggNeedsHost(Pawn pawn, ThingDef t)
        {
            Thing thing;
            List<Thing> list = SpawnedEggsNeedHosts(pawn.Map);
            List<Thing> cocoonlist = SpawnedEmptyCocoons(pawn.Map, t);

            thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, (x => list.Contains(x)), null, 0, -1, false, RegionType.Set_Passable, false);
            thing = GenClosest.ClosestThingReachable(thing.Position, thing.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 10f, (x => cocoonlist.Contains(x) && (x is Building_XenomorphCocoon XC && XC.AnyUnoccupiedSleepingSlot)), null, 0, -1, false, RegionType.Set_Passable, false);

            return thing;
        }

        public static int TotalSpawnedCocoonCount(Map map, ThingDef t)
        {
            return map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon).Count;
        }
        public static List<Thing> SpawnedCocoons(Map map, ThingDef t)
        {
            return map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon);
        }
        public static int TotalSpawnedEmptyCocoonCount(Map map, ThingDef t)
        {
            return SpawnedEmptyCocoons(map, t).Count;
        }
        public static List<Thing> SpawnedEmptyCocoons(Map map, ThingDef t)
        {
            return SpawnedCocoons(map, t).FindAll(x => (x is Building_XenomorphCocoon XC && XC.AnyUnoccupiedSleepingSlot));
        }


        // Hivelike stuff
        public static bool HivelikesPresent(Map map)
        {
            return TotalSpawnedHivelikeCount(map) > 0;
        }
        public static Thing ClosestReachableHivelike(Pawn pawn)
        {
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_XenomorphHive), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableHivelike(ThingDef hiveDef, Pawn pawn)
        {
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(hiveDef), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            return thing;
        }
        public static Thing ClosestReachableHivelikeThatEggNeedsHost(Pawn pawn)
        {
            List<Thing> list = SpawnedEggsNeedHosts(pawn.Map);
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, (x => list.Contains(x)), null, 0, -1, false, RegionType.Set_Passable, false);

            return thing;
        }
        public static int TotalSpawnedHivelikeCount(Map map)
        {
            return map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_XenomorphHive).Count;
        }
        public static List<Thing> SpawnedHivelikes(Map map)
        {
            return map.listerThings.ThingsOfDef(XenomorphDefOf.RRY_XenomorphHive);
        }
        public static List<Thing> SpawnedHivelikes(ThingDef hiveDef, Map map)
        {
            return map.listerThings.ThingsOfDef(hiveDef);
        }

        // space between / distance between
        public static float DistanceBetween(IntVec3 a, IntVec3 b)
        {
            double distance = GetDistance(a.x, a.z, b.x, b.z);
            return (float)distance;
        }
        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
    }
    [StaticConstructorOnStartup]
    class XenomorphStaticUtil
    {
        static void Main()
        {
            foreach (var item in DefDatabase<PawnKindDef>.AllDefsListForReading)
            {
                if (XenomorphUtil.isInfectablePawnKind(item))
                {
                    Log.Message(string.Format("Xenomorph Host: {0}", item.LabelCap));
                }
            }
        }
    }
}
