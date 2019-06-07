﻿using RRYautja;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200018F RID: 399
	public class LordToil_DefendAndExpandHiveLike : LordToil_HiveLikeRelated
    {
        public bool hivelikesPresent;
        public bool eggsPresent;
        public bool cocoonsHumanoidPresent;
        public bool cocoonsAnimalPresent;

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
        
        public bool hivelikesReachable;
        public Thing closestReachableHivelike;

        public Thing cocoonThing;
        public Thing eggThing;
        public Thing hiveThing;


        // Token: 0x06000860 RID: 2144 RVA: 0x00047694 File Offset: 0x00045A94
        public override void UpdateAllDuties()
        {
            hivelikesPresent = XenomorphUtil.HivelikesPresent(Map);
            eggsPresent = XenomorphUtil.EggsPresent(Map);
            base.FilterOutUnspawnedHiveLikes();
			for (int i = 0; i < this.lord.ownedPawns.Count; i++)
			{
                Pawn pawn = this.lord.ownedPawns[i];
                PawnDuty duty;
                if (hivelikesPresent)
                {
                    HiveLike hiveFor = base.GetHiveLikeFor(this.lord.ownedPawns[i]);
                    if (hiveFor.parentHiveLike != null)
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, hiveFor.parentHiveLike, this.distToHiveToAttack);
                    }
                    else
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, hiveFor, this.distToHiveToAttack);
                    }
                }
                else
                {
                    ThingDef named = pawn.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;

                    cocoonsPresent = XenomorphUtil.CocoonsPresent(pawn.Map, named);
                    eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
                    closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);

                    hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull();
                    closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(pawn);

                    cocoonsReachable = !XenomorphUtil.ClosestReachableCocoon(pawn, named).DestroyedOrNull();
                    closestReachableCocoon = XenomorphUtil.ClosestReachableCocoon(pawn, named);

                    emptycocoonsPresent = XenomorphUtil.EmptyCocoonsPresent(pawn.Map, named);
                    emptycocoonsReachable = !XenomorphUtil.ClosestReachableEmptyCocoon(pawn, named).DestroyedOrNull();
                    emptyclosestReachableCocoon = XenomorphUtil.ClosestReachableEmptyCocoon(pawn, named);
                    
                    if (eggsPresent)
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, closestReachableEgg, this.distToHiveToAttack);
                    }
                    else if (cocoonsPresent)
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, closestReachableCocoon, this.distToHiveToAttack);
                    }
                    else if (InfestationLikeCellFinder.TryFindCell(out IntVec3 c, Map, false))
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, c, this.distToHiveToAttack);
                    }
                    else
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, pawn, this.distToHiveToAttack);
                    }
                }
				this.lord.ownedPawns[i].mindState.duty = duty;
			}
		}

		// Token: 0x04000395 RID: 917
		public float distToHiveToAttack = 60f;
	}
}
