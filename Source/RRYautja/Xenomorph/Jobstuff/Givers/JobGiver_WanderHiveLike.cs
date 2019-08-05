using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x020000A5 RID: 165
	public class JobGiver_WanderHiveLike : JobGiver_Wander
    {
        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_WanderHiveLike jobGiver_WanderHiveLike = (JobGiver_WanderHiveLike)base.DeepCopy(resolve);
            jobGiver_WanderHiveLike.wanderRadius = this.wanderRadius;
            return jobGiver_WanderHiveLike;
        }
        // Token: 0x0600041F RID: 1055 RVA: 0x0002CD08 File Offset: 0x0002B108
        public JobGiver_WanderHiveLike()
		{
			this.wanderRadius = 7.5f;
			this.ticksBetweenWandersRange = new IntRange(125, 200);
		}

        private HiveLike FindClosestHiveLike(Pawn pawn)
        {
            ThingDef hiveDef = null;
            List<ThingDef_HiveLike> hivedefs = DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == pawn.Faction.def);
        //    Log.Message(string.Format("hivedefs found: {0}", hivedefs.Count));
            foreach (ThingDef_HiveLike hivedef in hivedefs)
            {
            //    Log.Message(string.Format("LordToil_HiveLikeRelated found hiveDef: {0} for {1}", hiveDef, pawn));
                if (hivedef.Faction == pawn.Faction.def)
                {
                    hiveDef = hivedef;
                //    Log.Message(string.Format("LordToil_HiveLikeRelated set hiveDef: {0} for {1}", hiveDef, pawn));
                    return (HiveLike)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(hiveDef), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 30f, (Thing x) => x.Faction == pawn.Faction, null, 0, 30, false, RegionType.Set_Passable, false);
                }
            }
            return null;
        }

        // Token: 0x06000420 RID: 1056 RVA: 0x0002CD30 File Offset: 0x0002B130
        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            HiveLike hivelike = pawn.mindState.duty!=null && pawn.mindState.duty.focus!=null ? pawn.mindState.duty.focus.Thing as HiveLike: null;
            if (hivelike!=null)
            {
                return hivelike.Position;
            }
            /*
            if (hivelike==null)
            {
                hivelike = FindClosestHiveLike(pawn);
            }
            */
            if (hivelike == null || !hivelike.Spawned)
            {
                if (XenomorphUtil.HivelikesPresent(pawn.Map))
                {
                    return XenomorphUtil.ClosestReachableHivelike(pawn).Position;
                }
                if (!XenomorphKidnapUtility.hiveslimepresent)
                {
                    if (XenomorphKidnapUtility.TryFindGoodHiveLoc(pawn, out IntVec3 c))
                    {
                        return c;
                    }
                }
                else if (!XenomorphUtil.ClosestReachableHiveSlime(pawn).DestroyedOrNull())
                {
                    return XenomorphUtil.ClosestReachableHiveSlime(pawn).Position;
                }
                return pawn.Position;
            }
            //    Log.Message(string.Format("JobGiver_WanderHiveLike hivelike.Position: {0}", hivelike.Position));
            return hivelike.Position;
		}
	}
}
