using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x020000D6 RID: 214
    public class JobGiver_XenosKidnap : ThinkNode_JobGiver
    {
        public bool eggsPresent; 
        public bool eggsReachable;
        public Thing closestReachableEgg;
        public Thing closestReachableCocoontoEgg;

        public bool emptycocoonsPresent;
        public bool emptycocoonsReachable;
        public bool cocoonOccupied;
        public Thing emptyclosestReachableCocoon;

        public bool cocoonsPresent;
        public bool cocoonsReachable;
        public Thing closestReachableCocoon;

        public bool hivelikesPresent;
        public bool hivelikesReachable;
        public Thing closestReachableHivelike;

        public Thing cocoonThing;
        public Thing eggThing;
        public Thing hiveThing;

        private List<Rot4> Rotlist = new List<Rot4>
        {
            Rot4.North,
            Rot4.South,
            Rot4.East,
            Rot4.West
        };

        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_XenosKidnap jobGiver_XenosKidnap = (JobGiver_XenosKidnap)base.DeepCopy(resolve);
            jobGiver_XenosKidnap.HuntingRange = this.HuntingRange;
            return jobGiver_XenosKidnap;
        }


        private float HuntingRange = 9999f;

        // Token: 0x060004D1 RID: 1233 RVA: 0x0003100C File Offset: 0x0002F40C
        protected override Job TryGiveJob(Pawn pawn)
        {
            float Searchradius = HuntingRange;
            IntVec3 c = IntVec3.Invalid;
            if (XenomorphKidnapUtility.TryFindGoodKidnapVictim(pawn, Searchradius, out Pawn t, null) && !GenAI.InDangerousCombat(pawn))
            {
                bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) && (Prefs.DevMode) : false;
                ThingDef named = t.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
                eggsPresent = XenomorphUtil.EggsPresent(pawn.Map);
                eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
                closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);

                hivelikesPresent = XenomorphUtil.HivelikesPresent(pawn.Map);
                hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull();
                closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(pawn);

                cocoonsPresent = XenomorphUtil.CocoonsPresent(pawn.Map, named);
                cocoonsReachable = !XenomorphUtil.ClosestReachableCocoon(pawn, named).DestroyedOrNull();
                closestReachableCocoon = XenomorphUtil.ClosestReachableCocoon(pawn, named);

                emptycocoonsPresent = XenomorphUtil.EmptyCocoonsPresent(pawn.Map, named);
                emptycocoonsReachable = !XenomorphUtil.ClosestReachableEmptyCocoon(pawn, named).DestroyedOrNull();
                emptyclosestReachableCocoon = XenomorphUtil.ClosestReachableEmptyCocoon(pawn, named);

                if (selected && eggsPresent) Log.Message(string.Format("JobGiver_XenosKidnap for {3} eggsPresent: {0}, eggsReachable: {1}, closestReachableEgg: {2}", eggsPresent, eggsReachable, closestReachableEgg, pawn.LabelShortCap));
                if (selected && hivelikesPresent) Log.Message(string.Format("JobGiver_XenosKidnap for {3} hivelikesPresent: {0}, hivelikesReachable: {1}, closestReachableHivelike: {2}", hivelikesPresent, hivelikesReachable, closestReachableHivelike, pawn.LabelShortCap));
                if (selected && cocoonsPresent) Log.Message(string.Format("JobGiver_XenosKidnap for {3} cocoonsPresent: {0}, cocoonsReachable: {1}, closestReachableEgg: {2}", cocoonsPresent, cocoonsReachable, closestReachableCocoon, pawn.LabelShortCap));
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
                        c = hiveThing.Position;
                        int radius = 10;
                        IntVec3 intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && hiveThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !hiveThing.Position.Roofed(pawn.Map))) && !x.AdjacentTo8Way(hiveThing.Position) && XenomorphKidnapUtility.XenoCocoonLocations(hiveThing.Position, radius, pawn.Map).Contains(x)));
                        if (intVec == IntVec3.Invalid)
                        {
                            intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && hiveThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !hiveThing.Position.Roofed(pawn.Map))) && !x.AdjacentTo8Way(hiveThing.Position)));
                        }
                        c = intVec;
                    }
                }
                if (c == IntVec3.Invalid && !hivelikesPresent && (eggsPresent && eggsReachable && XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 0))
                {
                    if (selected) Log.Message(string.Format("JobGiver_XenosKidnap SpawnedEggsNeedHosts: {0}, EggCount: {1} for {2}", XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 0, XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count, pawn));
                    eggThing = XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 1 ? XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).RandomElement() : XenomorphUtil.ClosestReachableEggNeedsHost(pawn);

                    if (selected) Log.Message(string.Format("JobGiver_XenosKidnap eggThing: {0}, EggCount: {1} for {2}", eggThing, XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count, pawn));
                    c = eggThing.Position;
                    if (c == eggThing.Position)
                    {
                        int radius = 1;
                        int num = (named.Size.x > named.Size.z) ? named.Size.x : named.Size.z;
                        CellRect mapRect;
                        IntVec3 intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && eggThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !eggThing.Position.Roofed(pawn.Map)))));
                        mapRect = Rand.Chance(0.5f) ? new CellRect(intVec.x, intVec.z, num, num) : new CellRect(intVec.z, intVec.x, num, num);

                        while (!IsMapRectClear(mapRect, pawn.Map))
                        {
                            intVec = CellFinder.RandomClosewalkCellNear(c, pawn.Map, radius, (x => (x.Roofed(pawn.Map) && eggThing.Position.Roofed(pawn.Map) || (!x.Roofed(pawn.Map) && !eggThing.Position.Roofed(pawn.Map)))));
                            mapRect = new CellRect(intVec.x, intVec.z, num, num);
                            if (!IsMapRectClear(mapRect, pawn.Map)) radius++;
                            else
                            {
                                if (selected) Log.Message(string.Format("spot for cocoon found @ {0} which is {1} away from {2} @ {3}", intVec, radius, eggThing, eggThing.Position));
                            }
                            if (radius > 30)
                            {
                                break;
                            }
                        }
                        if (intVec != null)
                        {
                            //    GenPlace.TryPlaceThing(TryMakeCocoon(mapRect, pawn.Map, named), intVec, pawn.Map, ThingPlaceMode.Near);
                        }
                        c = intVec;
                    }
                    //GenPlace.TryPlaceThing(TryMakeCocoon(mapRect, pawn.Map, named), intVec, pawn.Map, ThingPlaceMode.Near);
                }
                if (c == IntVec3.Invalid && cocoonsPresent && !hivelikesPresent && !eggsPresent)
                {
                    if (selected) Log.Message(string.Format("cocoonsPresent: {0}", cocoonsPresent));
                    if (selected) Log.Message(string.Format("cocoonsReachable: {0}", cocoonsReachable));
                    c = RCellFinder.RandomWanderDestFor(pawn, closestReachableCocoon.Position, 5f, null, Danger.Some);
                    if (selected) Log.Message(string.Format("RCellFinder.RandomWanderDestFor(pawn, c, 5f, null, Danger.Some): {0}", c));
                }
                if (c == IntVec3.Invalid)
                {
                    if (!InfestationLikeCellFinder.TryFindCell(out c, out IntVec3 lc, pawn.Map, false))
                    {
                        if (selected) Log.Message(string.Format("no infestation cell found {0}\nfor {1} @ {2}", c, pawn, pawn.Position));
                        if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn) && (!XenomorphUtil.EggsPresent(pawn.Map) || (XenomorphUtil.EggsPresent(pawn.Map) && XenomorphUtil.ClosestReachableEgg(pawn) == null)))
                        {
                            if (selected) Log.Message(string.Format("no Exit cell found {0}\nfor {1} @ {2}", c, pawn, pawn.Position));
                            return null;
                        }
                    }
                    else
                    {
                        if (selected) Log.Message(string.Format("found infestation cell for {2} @ {3} \nlc: {0}, c:{1}", lc, c, pawn, pawn.Position));
                        if (pawn.GetLord()!=null && pawn.GetLord() is Lord lord)
                        {
                            if (selected) Log.Message(string.Format("found lord for {2} @ {3} doing\n LordJob: {0}, CurLordToil: {1}", lord.LordJob, lord.CurLordToil, pawn, pawn.Position));

                        }
                        if (pawn.mindState.duty.def != OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike && pawn.mindState.duty.def != OGHiveLikeDefOf.RRY_DefendHiveLikeAggressively)
                        {
                            pawn.mindState.duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, lc, 40f);

                            c = RCellFinder.RandomWanderDestFor(pawn, lc, 5f, null, Danger.Some);
                        }
                        else
                        {
                            c = RCellFinder.RandomWanderDestFor(pawn, lc, 5f, null, Danger.Some);
                        }
                    } 
                }
                if (selected) Log.Message(string.Format("TargetB == {0}", c));
                if (c != IntVec3.Invalid && t !=null)
                {
                    return new Job(XenomorphDefOf.RRY_Job_XenomorphKidnap)
                    {
                        targetA = t,
                        targetB = c,
                        count = 1
                    };
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
            cellRect = new CellRect(mapRect.BottomLeft.x + 1, mapRect.TopRight.z + 1, 2, 1);
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
            building_XenomorphCocoon.SetPositionDirect(cellRect.RandomCell);
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
        //    Log.Message(string.Format("Rotation: {0}", building_XenomorphCocoon.Rotation));
            return building_XenomorphCocoon;
        }

        // Token: 0x040002AB RID: 683
        public const float VictimSearchRadiusInitial = 8f;

        // Token: 0x040002AC RID: 684
        private const float VictimSearchRadiusOngoing = 18f;
    }
}
