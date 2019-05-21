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
        // Token: 0x060004D1 RID: 1233 RVA: 0x0003100C File Offset: 0x0002F40C
        protected override Job TryGiveJob(Pawn pawn)
        {
            IntVec3 c;
            if (!RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn)&&(!XenomorphUtil.EggsPresent(pawn.Map)|| (XenomorphUtil.EggsPresent(pawn.Map) && XenomorphUtil.ClosestReachableEgg(pawn) == null)))
            {
                return null;
            }
            if (XenomorphUtil.EggsPresent(pawn.Map)&& XenomorphUtil.ClosestReachableEgg(pawn) != null)
            {
                if (XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count > 1)
                {
                    Thing eggThing = XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).RandomElement();
                    Thing cocoonThing = null;
                    if (XenomorphUtil.CocoonsPresent(pawn.Map))
                    {
                        cocoonThing = GenClosest.ClosestThingReachable(eggThing.Position, eggThing.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 3f, null, null, 0, -1, false, RegionType.Set_Passable, false);

                    }
                    c = cocoonThing!=null ? cocoonThing.Position : eggThing.Position;
                }
                else if (XenomorphUtil.SpawnedEggsNeedHosts(pawn.Map).Count == 1)
                {
                    Thing eggThing = XenomorphUtil.ClosestReachableEggNeedsHost(pawn);
                    Thing cocoonThing = null;
                    if (XenomorphUtil.CocoonsPresent(pawn.Map))
                    {
                        cocoonThing = GenClosest.ClosestThingReachable(eggThing.Position, eggThing.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 3f, null, null, 0, -1, false, RegionType.Set_Passable, false);

                    }
                    c = cocoonThing != null ? cocoonThing.Position : eggThing.Position;
                }
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
            Pawn t;
            if (XenomorphKidnapUtility.TryFindGoodKidnapVictim(pawn, 60f, out t, null) && !GenAI.InDangerousCombat(pawn))
            {
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
