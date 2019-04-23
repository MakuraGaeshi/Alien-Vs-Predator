using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x0200044C RID: 1100
    public class RoomRoleWorker_TrophyRoom : RoomRoleWorker
    {
        // Token: 0x06001350 RID: 4944 RVA: 0x00094598 File Offset: 0x00092998
        public override float GetScore(Room room)
        {
            int num = 0;
            List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
            {
                Thing thing = containedAndAdjacentThings[i];
                if (thing.def.category == ThingCategory.Building)
                {
                    List<JoyGiverDef> allDefsListForReading = DefDatabase<JoyGiverDef>.AllDefsListForReading;
                    for (int j = 0; j < allDefsListForReading.Count; j++)
                    {
                        if (allDefsListForReading[j].thingDefs != null && allDefsListForReading[j].thingDefs.Contains(thing.def))
                        {
                            num++;
                            break;
                        }
                    }
                }
            }
            return (float)num * 5f;
        }
    }
}
