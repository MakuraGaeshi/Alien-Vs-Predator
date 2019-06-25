using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x02000190 RID: 400
    public class LordToil_DefendHiveLoc : LordToil
    {
        // Token: 0x06000861 RID: 2145 RVA: 0x00047713 File Offset: 0x00045B13
        public LordToil_DefendHiveLoc(IntVec3 baseCenter)
        {
            this.baseCenter = baseCenter;
        }

        // Token: 0x1700015B RID: 347
        // (get) Token: 0x06000862 RID: 2146 RVA: 0x00047722 File Offset: 0x00045B22
        public override IntVec3 FlagLoc
        {
            get
            {
                return this.baseCenter;
            }
        }

        // Token: 0x06000863 RID: 2147 RVA: 0x0004772C File Offset: 0x00045B2C
        public override void UpdateAllDuties()
        {
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
            {
                this.lord.ownedPawns[i].mindState.duty = new PawnDuty(XenomorphDefOf.RRY_DefendHiveLoc, this.baseCenter, -1f);
            }
        }

        // Token: 0x04000396 RID: 918
        public IntVec3 baseCenter;
    }
}
