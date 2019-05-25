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

        // Token: 0x060004D1 RID: 1233 RVA: 0x0003100C File Offset: 0x0002F40C
        protected override Job TryGiveJob(Pawn pawn)
        {
            eggsPresent = XenomorphUtil.EggsPresent(pawn.Map);
            eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
            closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);

            cocoonsPresent = XenomorphUtil.EmptyCocoonsPresent(pawn.Map);
            cocoonsReachable = !XenomorphUtil.ClosestReachableEmptyCocoon(pawn).DestroyedOrNull();// && XenomorphUtil.ClosestReachableEmptyCocoon(pawn)!=null;
            closestReachableCocoon = XenomorphUtil.ClosestReachableEmptyCocoon(pawn);

            Log.Message(string.Format("JobGiver_XenosKidnap EggsPresent: {0}, ReachableEgg: {1} for {2}, closestReachableCocoon: {3}", XenomorphUtil.EggsPresent(pawn.Map), XenomorphUtil.ClosestReachableEgg(pawn) != null, pawn , closestReachableCocoon));
            if (eggsPresent && eggsReachable)
            {
                if (XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 0)
                {
                    Log.Message(string.Format("JobGiver_XenosKidnap SpawnedEggsNeedHosts: {0}, EggCount: {1} for {2}", XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 0, XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count, pawn));
                    eggThing = XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 1 ? XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).RandomElement() : XenomorphUtil.ClosestReachableEggNeedsHost(pawn);

                    Log.Message(string.Format("JobGiver_XenosKidnap eggThing: {0}, EggCount: {1} for {2}", eggThing, XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count, pawn));
                    if (cocoonsPresent && cocoonsReachable)
                    {

                        Log.Message(string.Format("JobGiver_XenosKidnap {0} searching for cocoonThing for eggThing: {1}", pawn, eggThing.Position));
                        cocoonThing = XenomorphUtil.ClosestReachableEmptyCocoonToEgg(eggThing);
                        cocoonOccupied = cocoonThing!=null ? !(((Building_Bed)cocoonThing).AnyUnoccupiedSleepingSlot): true;


                        Log.Message(string.Format("JobGiver_XenosKidnap {0} set cocoonThing: {1} cocoonOccupied: {2} for eggThing: {3}", pawn, cocoonThing.Position, cocoonOccupied, eggThing.Position));

                    }
                    c = cocoonThing!=null&&!cocoonOccupied ? cocoonThing.Position : eggThing.Position;
                }
                /*
                else if (XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count == 1)
                {
                    Thing eggThing = XenomorphUtil.ClosestReachableEggNeedsHost(pawn);
                    Thing cocoonThing = null;
                    if (XenomorphUtil.CocoonsPresent(pawn.Map))
                    {
                        cocoonThing = GenClosest.ClosestThingReachable(eggThing.Position, eggThing.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 3f, null, null, 0, -1, false, RegionType.Set_Passable, false);
                        Log.Message(string.Format("JobGiver_XenosKidnap set cocoonThing: {0} for {1}", cocoonThing, pawn));

                    }
                    c = cocoonThing != null ? cocoonThing.Position : eggThing.Position;
                }
                */
                else
                {
                    ThingDef hiveDef = null;
                    List<ThingDef_HiveLike> hivedefs = DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == pawn.Faction.def);
                    foreach (ThingDef_HiveLike hivedef in hivedefs)
                    {
                        Log.Message(string.Format("JobGiver_MaintainHiveLikes found hiveDef: {0} for {1}", hiveDef, pawn));
                        if (hivedef.Faction == pawn.Faction.def)
                        {
                            hiveDef = hivedef;
                                Log.Message(string.Format("JobGiver_MaintainHiveLikes set hiveDef: {0} for {1}", hiveDef, pawn));

                            break;
                        }
                    }
                    if (XenomorphUtil.TotalSpawnedThingCount(hiveDef, pawn.Map)>0 && hiveDef != null)
                    {
                        c = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(hiveDef), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 30f, (Thing x) => x.Faction == pawn.Faction, null, 0, 30, false, RegionType.Set_Passable, false).Position;
                    }
                }

            }
            else
            {
                if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn) && (!XenomorphUtil.EggsPresent(pawn.Map) || (XenomorphUtil.EggsPresent(pawn.Map) && XenomorphUtil.ClosestReachableEgg(pawn) == null)))
                {
                    return null;
                }
            }
            Pawn t;
            Building_XenomorphCocoon b;
            if (XenomorphKidnapUtility.TryFindGoodKidnapVictim(pawn, 60f, out t, null) && !GenAI.InDangerousCombat(pawn))
            {
                Log.Message(string.Format("TargetB == {0}", c));
                b = (Building_XenomorphCocoon)c.GetFirstThing(t.Map, XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon);
                if (b!=null && b.AnyUnoccupiedSleepingSlot)
                {
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

        // Token: 0x040002AB RID: 683
        public const float VictimSearchRadiusInitial = 8f;

        // Token: 0x040002AC RID: 684
        private const float VictimSearchRadiusOngoing = 18f;
    }
}
