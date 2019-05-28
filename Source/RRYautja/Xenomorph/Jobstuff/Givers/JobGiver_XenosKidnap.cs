using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000D6 RID: 214
    public class JobGiver_XenosKidnap : ThinkNode_JobGiver
    {
        IntVec3 c;

        public bool eggsPresent; 
        public bool eggsReachable;
        public Thing closestReachableEgg;
        public Thing closestReachableCocoontoEgg;

        public bool cocoonsPresent;
        public bool cocoonsReachable;
        public bool cocoonOccupied;
        public Thing closestReachableCocoon;

        public bool hivelikesPresent;
        public bool hivelikesReachable;
        public Thing closestReachableHivelike;

        public Thing cocoonThing;
        public Thing eggThing;
        public Thing hiveThing;

        // Token: 0x060004D1 RID: 1233 RVA: 0x0003100C File Offset: 0x0002F40C
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (XenomorphKidnapUtility.TryFindGoodKidnapVictim(pawn, 60f, out Pawn t, null) && !GenAI.InDangerousCombat(pawn))
            {
                bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) : false;
                eggsPresent = XenomorphUtil.EggsPresent(pawn.Map);
                eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
                closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);

                hivelikesPresent = XenomorphUtil.HivelikesPresent(pawn.Map);
                hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull();
                closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(pawn);

                cocoonsPresent = XenomorphUtil.EmptyCocoonsPresent(pawn.Map);
                cocoonsReachable = !XenomorphUtil.ClosestReachableEmptyCocoon(pawn).DestroyedOrNull();
                closestReachableCocoon = XenomorphUtil.ClosestReachableEmptyCocoon(pawn);

                if (selected) Log.Message(string.Format("JobGiver_XenosKidnap EggsPresent: {0}, ReachableEgg: {1} for {2}, closestReachableCocoon: {3}", XenomorphUtil.EggsPresent(pawn.Map), XenomorphUtil.ClosestReachableEgg(pawn) != null, pawn, closestReachableCocoon));
                if ((hivelikesPresent && hivelikesReachable))
                {
                    ThingDef hiveDef = null;
                    List<ThingDef_HiveLike> hivedefs = DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == pawn.Faction.def);
                    foreach (ThingDef_HiveLike hivedef in hivedefs)
                    {
                        if (selected) Log.Message(string.Format("JobGiver_XenosKidnap found hiveDef: {0} for {1}", hivedef, pawn));
                        if (hivedef.Faction == pawn.Faction.def)
                        {
                            hiveDef = hivedef;
                            if (selected) Log.Message(string.Format("JobGiver_XenosKidnap set hiveDef: {0} for {1}", hiveDef, pawn));

                            break;
                        }
                    }
                    if (XenomorphUtil.TotalSpawnedThingCount(hiveDef, pawn.Map) > 0 && hiveDef != null)
                    {
                        hiveThing = XenomorphUtil.TotalSpawnedThingCount(hiveDef, pawn.Map) > 1 ? XenomorphUtil.SpawnedHivelikes(hiveDef, pawn.Map).RandomElement() : XenomorphUtil.ClosestReachableHivelike(hiveDef, pawn);
                        if (cocoonsPresent && cocoonsReachable)
                        {
                            if (selected) Log.Message(string.Format("JobGiver_XenosKidnap {0} searching for cocoonThing for hiveThing: {1}", pawn, hiveThing.Position));
                            cocoonThing = XenomorphUtil.ClosestReachableEmptyCocoonToEgg(closestReachableHivelike);
                            cocoonOccupied = cocoonThing != null ? !(((Building_Bed)cocoonThing).AnyUnoccupiedSleepingSlot) : true;


                            if (selected) Log.Message(string.Format("JobGiver_XenosKidnap {0} set cocoonThing: {1} cocoonOccupied: {2} for hiveThing: {3}", pawn, cocoonThing.Position, cocoonOccupied, hiveThing.Position));
                        }
                        c = cocoonThing != null && !cocoonOccupied ? cocoonThing.Position : hiveThing.Position;
                        if (c == hiveThing.Position)
                        {
                            int radius = 2;
                            ThingDef named = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;
                            int num = (named.Size.x > named.Size.z) ? named.Size.x : named.Size.z;
                            CellRect mapRect;
                            IntVec3 intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && hiveThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !hiveThing.Position.Roofed(pawn.Map)))));
                            mapRect = new CellRect(intVec.x, intVec.z, num, num);
                            while (!IsMapRectClear(mapRect, pawn.Map))
                            {
                                intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && hiveThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !hiveThing.Position.Roofed(pawn.Map)))));
                                mapRect = new CellRect(intVec.x, intVec.z, num, num);
                                radius++;

                            }
                            if (intVec != null)
                            {
                                GenPlace.TryPlaceThing(TryMakeCocoon(mapRect, pawn.Map, named), intVec, pawn.Map, ThingPlaceMode.Near);
                            }
                            c = intVec;
                        }
                    }

                }
                else if ((eggsPresent && eggsReachable))
                {
                    if (XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 0)
                    {
                        if (selected) Log.Message(string.Format("JobGiver_XenosKidnap SpawnedEggsNeedHosts: {0}, EggCount: {1} for {2}", XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 0, XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count, pawn));
                        eggThing = XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 1 ? XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).RandomElement() : XenomorphUtil.ClosestReachableEggNeedsHost(pawn);

                        if (selected) Log.Message(string.Format("JobGiver_XenosKidnap eggThing: {0}, EggCount: {1} for {2}", eggThing, XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count, pawn));
                        if (cocoonsPresent && cocoonsReachable)
                        {

                            if (selected) Log.Message(string.Format("JobGiver_XenosKidnap {0} searching for cocoonThing for eggThing: {1}", pawn, eggThing.Position));
                            cocoonThing = XenomorphUtil.ClosestReachableEmptyCocoonToEgg(eggThing);
                            cocoonOccupied = cocoonThing != null ? !(((Building_Bed)cocoonThing).AnyUnoccupiedSleepingSlot) : true;


                            if (selected) Log.Message(string.Format("JobGiver_XenosKidnap {0} set cocoonThing: {1} cocoonOccupied: {2} for eggThing: {3}", pawn, cocoonThing.Position, cocoonOccupied, eggThing.Position));

                        }
                        c = cocoonThing != null && !cocoonOccupied ? cocoonThing.Position : eggThing.Position;
                        if (c == eggThing.Position)
                        {
                            int radius = 1;
                            ThingDef named = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;
                            int num = (named.Size.x > named.Size.z) ? named.Size.x : named.Size.z;
                            CellRect mapRect;
                            IntVec3 intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && eggThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !eggThing.Position.Roofed(pawn.Map)))));
                            mapRect = Rand.Chance(0.5f) ? new CellRect(intVec.x, intVec.z, num, num) : new CellRect(intVec.z, intVec.x, num, num);

                            while (!IsMapRectClear(mapRect, pawn.Map))
                            {
                                intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && eggThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !eggThing.Position.Roofed(pawn.Map)))));
                                mapRect = new CellRect(intVec.x, intVec.z, num, num);
                                radius++;

                            }
                            if (intVec != null)
                            {
                                GenPlace.TryPlaceThing(TryMakeCocoon(mapRect, pawn.Map, named), intVec, pawn.Map, ThingPlaceMode.Near);
                            }
                            c = intVec;
                        }
                        //GenPlace.TryPlaceThing(TryMakeCocoon(mapRect, pawn.Map, named), intVec, pawn.Map, ThingPlaceMode.Near);

                    }
                }
                else
                {
                    if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn) && (!XenomorphUtil.EggsPresent(pawn.Map) || (XenomorphUtil.EggsPresent(pawn.Map) && XenomorphUtil.ClosestReachableEgg(pawn) == null)))
                    {
                        return null;
                    }
                }
                Building_XenomorphCocoon b;
                if (selected) Log.Message(string.Format("TargetB == {0}", c));
                b = (Building_XenomorphCocoon)c.GetFirstThing(t.Map, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon);
                if (b!=null && b.AnyUnoccupiedSleepingSlot && b.owners.NullOrEmpty())
                {

                    b.owners.Add(t);
                    return new Job(XenomorphDefOf.RRY_Job_XenomorphKidnap)
                    {
                        targetA = t,
                        targetB = b,
                        count = 1
                    };
                }
                else
                return new Job(XenomorphDefOf.RRY_Job_XenomorphKidnap)
                {
                    targetA = t,
                    targetB = c,
                    count = 1
                };
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

        // Token: 0x0600000C RID: 12 RVA: 0x00002D80 File Offset: 0x00000F80
        private static void ClearMapRect(CellRect mapRect, Map map)
        {
            foreach (IntVec3 intVec in mapRect)
            {
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    thingList[i].Destroy(0);
                }
            }
        }
        // Token: 0x0600000D RID: 13 RVA: 0x00002DF8 File Offset: 0x00000FF8
        private static Building_XenomorphCocoon TryMakeCocoon(CellRect mapRect, Map map, ThingDef thingDef)
        {
            mapRect.ClipInsideMap(map);
            CellRect cellRect;
            cellRect = new CellRect(mapRect.BottomLeft.x + 1, mapRect.BottomLeft.z + 1, 2, 1);
            cellRect.ClipInsideMap(map);
            IsMapRectClear(cellRect, map);
            foreach (IntVec3 intVec in cellRect)
            {
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag = !thingList[i].def.destroyable;
                    if (flag)
                    {
                        return null;
                    }
                }
            }
            Building_XenomorphCocoon building_XenomorphCocoon = (Building_XenomorphCocoon)ThingMaker.MakeThing(thingDef, null);
            building_XenomorphCocoon.SetPositionDirect(cellRect.CenterCell);
            bool flag2 = Rand.Value < 0.5f;
            if (flag2)
            {
                flag2 = Rand.Value < 0.5f;
                if (flag2)
                {
                    building_XenomorphCocoon.Rotation = Rot4.West;
                }
                else
                {
                    building_XenomorphCocoon.Rotation = Rot4.East;
                }
            }
            else
            {
                flag2 = Rand.Value < 0.5f;
                if (flag2)
                {
                    building_XenomorphCocoon.Rotation = Rot4.South;
                }
                else
                {
                    building_XenomorphCocoon.Rotation = Rot4.North;
                }
            }
            Log.Message(string.Format("Rotation: {0}", building_XenomorphCocoon.Rotation));
            return building_XenomorphCocoon;
        }

        // Token: 0x040002AB RID: 683
        public const float VictimSearchRadiusInitial = 8f;

        // Token: 0x040002AC RID: 684
        private const float VictimSearchRadiusOngoing = 18f;
    }
}
