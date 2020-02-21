using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RRYautja
{
    // Token: 0x02000067 RID: 103
    public class JobDriver_EnterHiveTunnel : JobDriver
    {
        // Token: 0x17000099 RID: 153
        // (get) Token: 0x060002E8 RID: 744 RVA: 0x0001C700 File Offset: 0x0001AB00
        public HiveLike Transporter
        {
            get
            {
                Thing thing = this.job.GetTarget(this.TransporterInd).Thing;
                if (thing == null)
                {
                    return null;
                }
                return (HiveLike)thing;
            }
        }

        // Token: 0x060002E9 RID: 745 RVA: 0x0001C735 File Offset: 0x0001AB35
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x060002EA RID: 746 RVA: 0x0001C738 File Offset: 0x0001AB38
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(this.TransporterInd);
            yield return Toils_Goto.GotoThing(this.TransporterInd, PathEndMode.Touch);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    HiveLike transporter = this.Transporter;
                    this.pawn.DeSpawn(DestroyMode.Vanish);
                    if (pawn.def==XenomorphRacesDefOf.RRY_Xenomorph_Queen)
                    {
                        transporter.GetDirectlyHeldQueens().TryAdd(this.pawn, false);
                    }
                    else
                    {
                        transporter.GetDirectlyHeldThings().TryAdd(this.pawn, false);
                    }
                }
            };
            yield break;
        }

        // Token: 0x0400020B RID: 523
        private TargetIndex TransporterInd = TargetIndex.A;
    }
}
