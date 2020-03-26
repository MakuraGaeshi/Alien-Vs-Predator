using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class PawnsArrivalModeWorker_DropThroughRoofNearPower : PawnsArrivalModeWorker
    {
        // Token: 0x060015AA RID: 5546 RVA: 0x000A92C8 File Offset: 0x000A76C8
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
        //    Log.Message("PawnsArrivalModeWorker_DropThroughRoofNearPower");
            Map map = (Map)parms.target;
            List<Building> list = new List<Building>();
            List<Building> outdoor = new List<Building>();
            List<Building> indoor = new List<Building>();
            try
            {
                list = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null || x.TryGetComp<CompPowerBattery>() != null);
                if (!list.NullOrEmpty())
                {
                    indoor = list.FindAll(x => x.GetRoom() !=null && x.GetRoom().OpenRoofCount == 0);
                    if (!indoor.NullOrEmpty())
                    {
                        string indoors = string.Empty;
                        indoor.ForEach(x => indoors += x.LabelCap);
                    //    Log.Message(string.Format("Indoor Targets: {0}", indoors));
                    }
                    outdoor = list.FindAll(x => !indoor.Contains(x));
                    if (!outdoor.NullOrEmpty())
                    {
                        string outdoors = string.Empty;
                        outdoor.ForEach(x => outdoors += x.LabelCap);
                    //    Log.Message(string.Format("Outdoor Targets: {0}", outdoors));
                    }
                }
                else
                {
                    Log.Warning("target building list NullOrEmpty");
                }
            }
            catch (Exception)
            {
                Log.Error("something went wrong while trying to find targets");
                throw;
            }
            /*
            */
            if (list.NullOrEmpty())
            {
                Log.Warning(string.Format("No Targets Found for {0}, aborting", this));
                return;
            }
            else
            for (int i = 0; i < pawns.Count; i++)
            {
                Building target = list.RandomElement();
                IntVec3 dropCenter = target.Position;
                if (outdoor.Contains(target))
                {

                    DropThroughRoofUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), parms.podOpenDelay, true, false, true);
                }
                else
                {

                    DropThroughRoofUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), parms.podOpenDelay, false, false, true);
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

    public class PawnsArrivalModeWorker_RandomDropThroughRoof : PawnsArrivalModeWorker
    {
        // Token: 0x060015AA RID: 5546 RVA: 0x000A92C8 File Offset: 0x000A76C8
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            for (int i = 0; i < pawns.Count; i++)
            {
                IntVec3 dropCenter = DropCellFinder.RandomDropSpot(map);
                DropThroughRoofUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), parms.podOpenDelay, true, false, true);
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
