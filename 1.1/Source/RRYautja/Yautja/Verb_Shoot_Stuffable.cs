using System;
using RimWorld;

namespace Verse
{
    // Token: 0x0200102D RID: 4141
    public class Verb_Shoot_Stuffable : Verb_Launch_Stuffable_Projectile
    {
        // Token: 0x1700104D RID: 4173
        // (get) Token: 0x060064FA RID: 25850 RVA: 0x001B9186 File Offset: 0x001B7586
        protected override int ShotsPerBurst
        {
            get
            {
                return this.verbProps.burstShotCount;
            }
        }

        // Token: 0x060064FB RID: 25851 RVA: 0x001B9194 File Offset: 0x001B7594
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (this.currentTarget.Thing is Pawn pawn && !pawn.Downed && base.CasterIsPawn && base.CasterPawn.skills != null)
            {
                float num = (!pawn.HostileTo(this.caster)) ? 20f : 170f;
                float num2 = this.verbProps.AdjustedFullCycleTime(this, base.CasterPawn);
                base.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
            }
        }

        // Token: 0x060064FC RID: 25852 RVA: 0x001B9234 File Offset: 0x001B7634
        protected override bool TryCastShot()
        {
            bool flag = base.TryCastShot();
            if (flag && base.CasterIsPawn)
            {
                base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            return flag;
        }
    }
}
