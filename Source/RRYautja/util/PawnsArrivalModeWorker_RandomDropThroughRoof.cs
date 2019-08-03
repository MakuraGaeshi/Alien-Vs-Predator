using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x020004B2 RID: 1202
    public class PawnsArrivalModeWorker_RandomDropThroughRoof : PawnsArrivalModeWorker
    {
        // Token: 0x060015AA RID: 5546 RVA: 0x000A92C8 File Offset: 0x000A76C8
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            for (int i = 0; i < pawns.Count; i++)
            {
                IntVec3 dropCenter = DropCellFinder.RandomDropSpot(map);
                DropPodUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), parms.podOpenDelay, true, false, true);
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
