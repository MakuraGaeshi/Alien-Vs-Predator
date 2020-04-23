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
            MapComponent_HiveGrid HiveGrid = Map.GetComponent<MapComponent_HiveGrid>();
            base.FilterOutUnspawnedHiveLikes();
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
            {
                Pawn p = this.lord.ownedPawns[i];
                float dist = p.Map !=null && p.Map.skyManager.CurSkyGlow < 0.5f ? this.distToHiveToAttack *2 : this.distToHiveToAttack;
                XenomorphHive Hive = base.GetHiveLikeFor(p);
                DutyDef dutyDef = XenomorphDefOf.AvP_Xenomorph_DefendHiveLoc;
                PawnDuty duty = null;
                if (Hive != null)
                {
                    dutyDef = XenomorphDefOf.AvP_Xenomorph_DefendAndExpandHive;
                }
                if (HiveGrid.HiveGuardlist.Contains(p))
                {
                    if (Hive.hasQueen)
                    {
                        dist = 16f;
                        if (Hive.Queen.Map!=null && Hive.Queen.Map == p.Map)
                        {
                            dutyDef = XenomorphDefOf.AvP_Xenomorph_DefendHiveLoc;
                        }
                        else
                        {
                            dutyDef = XenomorphDefOf.AvP_Xenomorph_DefendHiveLoc;
                        }
                    }
                    else
                    {
                        dist = 24f;
                        dutyDef = XenomorphDefOf.AvP_Xenomorph_DefendHiveLoc;
                    }
                }
                else if (HiveGrid.HiveWorkerlist.Contains(p))
                {
                    if (HiveGrid.potentialHosts.Any(x=> !x.Cocooned() && x.Downed))
                    {
                    //    duty = new PawnDuty(XenomorphDefOf.R, hiveFor, 16f);
                    }
                }
                if (duty == null && dutyDef !=null)
                {
                    duty = new PawnDuty(dutyDef, Hive.Position, dist);
                }
                if (duty != null)
                {
                    p.mindState.duty = duty;
                }
            }
        }

        // Token: 0x04000395 RID: 917
        public float distToHiveToAttack = 60f;
	}
}
