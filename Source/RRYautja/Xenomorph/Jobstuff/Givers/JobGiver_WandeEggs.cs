using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x020000A5 RID: 165
	public class JobGiver_WanderEggs : JobGiver_Wander
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_WanderEggs jobGiver_WanderEggs = (JobGiver_WanderEggs)base.DeepCopy(resolve);
            jobGiver_WanderEggs.wanderRadius = this.wanderRadius;
            return jobGiver_WanderEggs;
        }
        // Token: 0x0600041F RID: 1055 RVA: 0x0002CD08 File Offset: 0x0002B108
        public JobGiver_WanderEggs()
		{
			this.wanderRadius = 7.5f;
			this.ticksBetweenWandersRange = new IntRange(125, 200);
		}

        private HiveLike FindClosestEgg(Pawn pawn)
        {
            return (HiveLike)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XenomorphDefOf.RRY_EggXenomorphFertilized), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false), 30f, (Thing x) => x.Faction == pawn.Faction, null, 0, 30, false, RegionType.Set_Passable, false);
        }

        // Token: 0x06000420 RID: 1056 RVA: 0x0002CD30 File Offset: 0x0002B130
        protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			HiveLike hivelike = pawn.mindState.duty.focus.Thing as HiveLike;
            if (hivelike==null)
            {
                hivelike = FindClosestEgg(pawn);
            }
			if (hivelike == null || !hivelike.Spawned)
            {
            //    Log.Message(string.Format("JobGiver_WanderHiveLike pawn.Position: {0}", pawn.Position));
                return pawn.Position;
            }
        //    Log.Message(string.Format("JobGiver_WanderHiveLike hivelike.Position: {0}", hivelike.Position));
            return hivelike.Position;
		}
	}
}
