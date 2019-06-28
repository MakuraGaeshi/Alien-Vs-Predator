using RRYautja;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200018F RID: 399
	public class LordToil_DefendAndExpandHiveLike : LordToil_HiveLikeRelated
    {
        public bool eggsPresent;
        public bool cocoonsHumanoidPresent;
        public bool cocoonsAnimalPresent;

        public bool eggsReachable;
        public Thing closestReachableEgg;
        public Thing closestReachableCocoontoEgg;
        
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

        public LocalTargetInfo myFocus;
        bool QueenPresent;


        // Token: 0x06000860 RID: 2144 RVA: 0x00047694 File Offset: 0x00045A94
        public override void UpdateAllDuties()
        {
            QueenPresent = XenomorphUtil.QueenPresent(Map, out Pawn Queen);
            if (QueenPresent)
            {
                if (this.Data.HiveQueen.DestroyedOrNull()) this.Data.HiveQueen = Queen;
            }
            eggsPresent = XenomorphUtil.EggsPresent(Map);
            base.FilterOutUnspawnedHiveLikes();
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
			{
                Pawn pawn = this.lord.ownedPawns[i];
                PawnDuty duty;
                if (XenomorphUtil.HivelikesPresent(Map))
                {
                    HiveLike hiveFor = base.GetHiveLikeFor(this.lord.ownedPawns[i]);
                    if (hiveFor.parentHiveLike != null)
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, hiveFor.parentHiveLike, this.distToHiveToAttack);
                        this.Data.HiveLoc = hiveFor.parentHiveLike.Position;
                    }
                    else if (hiveFor!=null)
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, hiveFor, this.distToHiveToAttack);
                        this.Data.HiveLoc = hiveFor.Position;
                    }
                    else
                    {
                   //     Log.Message(string.Format("hives present but not found, we dun fucked up boss"));
                        duty = null;
                    }
                }
                else
                {
                    if (XenomorphUtil.HiveSlimePresent(Map))
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, XenomorphUtil.ClosestReachableHiveSlime(pawn), this.distToHiveToAttack);
                        this.Data.HiveLoc = XenomorphUtil.ClosestReachableHiveSlime(pawn).Position;
                    }
                    else if (XenomorphKidnapUtility.TryFindGoodHiveLoc(pawn, out IntVec3 c))
                    {
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, c, this.distToHiveToAttack);
                        this.Data.HiveLoc = c;
                    }
                    else
                    {
                        duty = null;
                    }
                    /*
                    ThingDef named = pawn.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
                    cocoonsPresent = XenomorphUtil.CocoonsPresent(pawn.Map, named);
                    eggsReachable = !XenomorphUtil.ClosestReachableEgg(pawn).DestroyedOrNull();
                    closestReachableEgg = XenomorphUtil.ClosestReachableEgg(pawn);

                    hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(pawn).DestroyedOrNull();
                    closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(pawn);

                    cocoonsReachable = !XenomorphUtil.ClosestReachableCocoon(pawn, named).DestroyedOrNull();
                    closestReachableCocoon = XenomorphUtil.ClosestReachableCocoon(pawn, named);
                    
                    if (XenomorphUtil.EggsPresent(Map))
                    {
                   //     Log.Message(string.Format("eggsPresent: {0}", closestReachableEgg.Position));
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, closestReachableEgg, this.distToHiveToAttack);
                    }
                    else if (cocoonsPresent)
                    {
                   //     Log.Message(string.Format("cocoonsPresent: {0}", closestReachableCocoon.Position));
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, closestReachableCocoon, this.distToHiveToAttack);
                    }
                    else if (myFocus.Cell != IntVec3.Zero)
                    {
                   //     Log.Message(string.Format("myFocus {0}", myFocus.Cell));
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, myFocus, this.distToHiveToAttack);
                    }
                    else if (InfestationLikeCellFinder.TryFindCell(out IntVec3 c, Map, false))
                    {
                   //     Log.Message(string.Format("InfestationLikeCellFinder: {0}", c));
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, c, this.distToHiveToAttack);
                    }
                    else
                    {
                   //     Log.Message(string.Format("pawn: {0}", pawn.Position));
                        duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendAndExpandHiveLike, pawn, this.distToHiveToAttack);
                    }
                    */
                }
				this.lord.ownedPawns[i].mindState.duty = duty;
                if (duty != null)
                {
                    if (duty.focus != null && duty.focus != IntVec3.Invalid && duty.focus != IntVec3.Zero)
                    {
                        myFocus = duty.focus;
                    }
                }
            }
		}

		// Token: 0x04000395 RID: 917
		public float distToHiveToAttack = 60f;
	}
}
