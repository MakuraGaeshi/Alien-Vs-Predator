using RRYautja;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000F0 RID: 240 
    public class JobGiver_KidnapNearby : ThinkNode_JobGiver
    {
        // Token: 0x06000523 RID: 1315 RVA: 0x000337F4 File Offset: 0x00031BF4
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_KidnapNearby jobGiver_RescueNearby = (JobGiver_KidnapNearby)base.DeepCopy(resolve);
            jobGiver_RescueNearby.radius = this.radius;
            return jobGiver_RescueNearby;
        }

        // Token: 0x06000524 RID: 1316 RVA: 0x0003381C File Offset: 0x00031C1C
        protected override Job TryGiveJob(Pawn pawn)
        {
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn3 = (Pawn)t;
                float eggDist= 9999f;
                Thing egg = XenomorphUtil.ClosestReachableEgg(pawn3);
                if (egg != null)
                {
                    eggDist = XenomorphUtil.DistanceBetween(pawn3.Position, egg.Position);
                }
                else return false;
                if (eggDist==9999f || eggDist<=5)
                {
                    return false;
                }
                return pawn3.Downed && pawn3.Faction != pawn.Faction && XenomorphUtil.isInfectablePawn(pawn3) && pawn.CanReserve(pawn3, 1, -1, null, false) && !pawn3.IsForbidden(pawn) && !GenAI.EnemyIsNear(pawn3, 25f);
            };
            Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), this.radius, validator, null, 0, -1, false, RegionType.Set_Passable, false);
            if (pawn2 == null)
            {
                return null;
            }
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
            if (thing == null)
            {
                return null;
                thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, false);
                if (thing == null)
                {
                    return null;
                }
            }
            return new Job(XenomorphDefOf.XenomorphKidnap, pawn2, thing)
            {
                count = 1
            };
        }

        // Token: 0x040002D2 RID: 722
        private float radius = 30f;

        // Token: 0x040002D3 RID: 723
        private const float MinDistFromEnemy = 25f;
    }
}
