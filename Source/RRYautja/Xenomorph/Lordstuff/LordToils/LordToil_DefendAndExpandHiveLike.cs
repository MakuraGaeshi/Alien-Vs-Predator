using AvP;
using AvP.ExtensionMethods;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200018F RID: 399
	public class LordToil_DefendAndExpandHiveLike : LordToil_HiveLikeRelated
    {
        
        // Token: 0x06000860 RID: 2144 RVA: 0x00047694 File Offset: 0x00045A94
        public override void UpdateAllDuties()
        {
            MapComponent_HiveGrid hive = Map.GetComponent<MapComponent_HiveGrid>();
            base.FilterOutUnspawnedHiveLikes();
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
            {
                Pawn p = this.lord.ownedPawns[i];
                HiveLike hiveFor = base.GetHiveLikeFor(p);
                PawnDuty duty = null;
                if (hive.HiveGuardlist.Contains(p))
                {
                    if (hiveFor.hasQueen)
                    {
                        duty = new PawnDuty(XenomorphDefOf.AvP_Xenomorph_DefendHiveLoc, hiveFor, 16f);
                    }
                    else
                    {

                    }
                }
                else if (hive.HiveWorkerlist.Contains(p))
                {
                    if (hive.potentialHosts.Any(x=> !x.Cocooned() && x.Downed))
                    {
                    //    duty = new PawnDuty(XenomorphDefOf.R, hiveFor, 16f);
                    }
                }







                if (duty == null)
                {
                    duty = new PawnDuty(XenomorphDefOf.AvP_Xenomorph_DefendAndExpandHive, hiveFor, this.distToHiveToAttack);
                }
                p.mindState.duty = duty;
            }
        }

        // Token: 0x04000395 RID: 917
        public float distToHiveToAttack = 60f;
	}
}
