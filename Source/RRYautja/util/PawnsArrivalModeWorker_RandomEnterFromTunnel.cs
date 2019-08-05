using RRYautja;
using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x020004B2 RID: 1202
    public class PawnsArrivalModeWorker_RandomEnterFromTunnel : PawnsArrivalModeWorker
    {
        // Token: 0x060015AA RID: 5546 RVA: 0x000A92C8 File Offset: 0x000A76C8
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
			List<Pair<List<Pawn>, IntVec3>> list = PawnsArrivalModeWorkerUtility.SplitIntoRandomGroupsNearMapEdge(pawns, map, false);
			PawnsArrivalModeWorkerUtility.SetPawnGroupsInfo(parms, list);
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list[i].First.Count; j++)
				{
                    IntVec3 dropCenter = XenomorphUtil.SpawnedChildHivelikes(map).RandomElement().Position;
                    IntVec3 loc = CellFinder.RandomClosewalkCellNear(dropCenter, map, 8, null);
					GenSpawn.Spawn(list[i].First[j], loc, map, parms.spawnRotation, WipeMode.Vanish, false);
				}
			}
        }

        // Token: 0x060015AB RID: 5547 RVA: 0x000A931B File Offset: 0x000A771B
        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            parms.podOpenDelay = 0;
            parms.spawnRotation = Rot4.Random;
            return true;
        }
    }
}
