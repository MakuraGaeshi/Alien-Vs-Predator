using RimWorld;
using RRYautja.ExtensionMethods;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RRYautja
{
    // RRYautja.JobGiver_EnterHiveTunnel
    public class JobGiver_EnterHiveTunnel : ThinkNode_JobGiver
    {
        // Token: 0x060004C5 RID: 1221 RVA: 0x00030B6C File Offset: 0x0002EF6C
        protected override Job TryGiveJob(Pawn pawn)
        {
            MapComponent_HiveGrid _HiveGrid = pawn.Map.GetComponent<MapComponent_HiveGrid>();
            HiveLike Tunnel = null;
            Tunnel = JobGiver_EnterHiveTunnel.FindMyTunnel(pawn);
            if (Tunnel == null)
            {
                return null;
            }
            if (Tunnel.hiveDormant && !_HiveGrid.HiveGuardlist.Contains(pawn))
            {
                return new Job(XenomorphDefOf.RRY_Job_EnterHiveTunnel, Tunnel);
            }
            bool flag1 = Tunnel.def == XenomorphDefOf.RRY_XenomorphHive;
            bool flag2 = Tunnel.def == XenomorphDefOf.RRY_XenomorphHive_Child;
            bool flag3 = Tunnel.Map.mapPawns.AllPawnsSpawned.Any(x => x.isPotentialHost() && pawn.TryGetComp<Comp_Xenomorph>().IsAcceptablePreyFor(pawn, x, true));
            if (flag3 || Tunnel == null || (Tunnel != null && Tunnel.spawnedPawns.Contains(pawn) && (Tunnel.Position.Roofed(Tunnel.Map) || flag1)) || !pawn.CanReach(Tunnel, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                return null;
            }
            return new Job(XenomorphDefOf.RRY_Job_EnterHiveTunnel, Tunnel);
        }

        // Token: 0x060004C6 RID: 1222 RVA: 0x00030C6C File Offset: 0x0002F06C
        public static HiveLike FindMyTunnel(Pawn pawn)
        {
            if (pawn.GetLord() != null && pawn.GetLord() is Lord lord)
            {
                if (lord.LordJob is LordJob_DefendAndExpandHiveLike hivejob)
                {
                    if (lord.CurLordToil is LordToil_DefendAndExpandHiveLike hivetoil)
                    {
                        if (hivetoil.Data.assignedHiveLikes.TryGetValue(pawn) != null)
                        {
                            return hivetoil.Data.assignedHiveLikes.TryGetValue(pawn);
                        }
                    }
                }
                if (lord.CurLordToil is LordToil_DefendHiveLikeAggressively hivetoilA)
                {
                    if (hivetoilA.Data.assignedHiveLikes.TryGetValue(pawn) != null)
                    {
                        return hivetoilA.Data.assignedHiveLikes.TryGetValue(pawn);
                    }
                }
            }
            return null;
        }
    }
}
