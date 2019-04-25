using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x0200044E RID: 1102
    public class RoomRoleWorker_EggChamber : RoomRoleWorker
    {
        // Token: 0x06001354 RID: 4948 RVA: 0x00094660 File Offset: 0x00092A60
        public override float GetScore(Room room)
        {
            int num = 0;
            List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
            {
                if (containedAndAdjacentThings[i].def == XenomorphDefOf.RRY_EggXenomorphFertilized)
                {
                    num++;
                }
            }
            return (float)num;
        }
    }
}
