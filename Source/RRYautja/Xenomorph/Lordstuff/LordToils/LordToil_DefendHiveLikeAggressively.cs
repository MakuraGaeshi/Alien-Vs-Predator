using System;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000191 RID: 401
    public class LordToil_DefendHiveLikeAggressively : LordToil_HiveLikeRelated
    {
        // Token: 0x06000865 RID: 2149 RVA: 0x000477A4 File Offset: 0x00045BA4
        public override void UpdateAllDuties()
        {
            base.FilterOutUnspawnedHiveLikes();
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
            {
                HiveLike hiveFor = base.GetHiveLikeFor(this.lord.ownedPawns[i]);
                PawnDuty duty;
                if (hiveFor.parentHiveLike!=null)
                {
                    duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendHiveLikeAggressively, hiveFor.parentHiveLike, this.distToHiveToAttack);
                }
                else
                {
                    duty = new PawnDuty(OGHiveLikeDefOf.RRY_DefendHiveLikeAggressively, hiveFor, this.distToHiveToAttack);
                }
                this.lord.ownedPawns[i].mindState.duty = duty;
            }
        }

        // Token: 0x04000397 RID: 919
        public float distToHiveToAttack = 100f;
    }
}
