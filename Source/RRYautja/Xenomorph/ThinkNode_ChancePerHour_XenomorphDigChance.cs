using System;

namespace Verse.AI
{
    // Token: 0x02000AE5 RID: 2789
    public class ThinkNode_ChancePerHour_XenomorphDigChance : ThinkNode_ChancePerHour
    {
        // Token: 0x06003E84 RID: 16004 RVA: 0x001D6DC4 File Offset: 0x001D51C4
        protected override float MtbHours(Pawn pawn)
        {
            Room room = pawn.GetRoom(RegionType.Set_Passable);
            if (room == null)
            {
                return 18f;
            }
            int num = (!room.IsHuge) ? room.CellCount : 9999;
            float num2 = GenMath.LerpDoubleClamped(2f, 25f, 6f, 1f, (float)num);
            return 18f / num2;
        }

        // Token: 0x040027BC RID: 10172
        private const float BaseMtbHours = 18f;
    }
}
