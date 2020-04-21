using AvP;
using System;
using Verse;
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
            MapComponent_HiveGrid hive = Map.GetComponent<MapComponent_HiveGrid>();
            if (!hive.Hivelist.NullOrEmpty() && !hive.HiveLoclist.NullOrEmpty())
            {

            }
            else
            {
                if (XenomorphKidnapUtility.TryFindGoodHiveLoc(lord.ownedPawns.RandomElement(), out _, null, true, false, true))
                {

                }
                else if (XenomorphKidnapUtility.TryFindGoodHiveLoc(lord.ownedPawns.RandomElement(), out _, null, true, true, true))
                {

                }
            }
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
            {
                PawnDuty duty;
                if (!hive.Hivelist.NullOrEmpty())
                {
                    XenomorphHive hiveFor = base.GetHiveLikeFor(this.lord.ownedPawns[i]);
                    if (hiveFor.parentHiveLike != null)
                    {
                        duty = new PawnDuty(XenomorphDefOf.AvP_Xenomorph_DefendHiveAggressively, hiveFor.parentHiveLike, this.distToHiveToAttack);
                    }
                    else
                    {
                        duty = new PawnDuty(XenomorphDefOf.AvP_Xenomorph_DefendHiveAggressively, hiveFor, this.distToHiveToAttack);
                    }
                }
                else if (!hive.HiveLoclist.NullOrEmpty())
                {
                    IntVec3 hiveloc = hive.HiveLoclist.RandomElement();

                    duty = new PawnDuty(XenomorphDefOf.AvP_Xenomorph_DefendHiveAggressively, hiveloc, this.distToHiveToAttack);
                }
                else
                {
                    duty = new PawnDuty(XenomorphDefOf.AvP_Xenomorph_AssaultColony_CutPower);
                }
                this.lord.ownedPawns[i].mindState.duty = duty;
            }
        }

        // Token: 0x04000397 RID: 919
        public float distToHiveToAttack = 100f;
    }
}
