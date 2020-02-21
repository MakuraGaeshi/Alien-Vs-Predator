using RimWorld;
using Verse;

namespace RRYautja
{

    // Token: 0x02000025 RID: 37
    public class Verb_ShootEquipment : Verb_UseEquipment
    {
        protected override int ShotsPerBurst
        {
            get
            {
                return this.verbProps.burstShotCount;
            }
        }

        // Token: 0x0600219C RID: 8604 RVA: 0x000CC050 File Offset: 0x000CA250
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            Pawn pawn = this.currentTarget.Thing as Pawn;
            if (pawn != null && !pawn.Downed && this.CasterIsPawn && this.CasterPawn.skills != null)
            {
                float num = pawn.HostileTo(this.caster) ? 170f : 20f;
                float num2 = this.verbProps.AdjustedFullCycleTime(this, this.CasterPawn);
                this.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
            }
        }

        // Token: 0x0600219D RID: 8605 RVA: 0x000CC0DB File Offset: 0x000CA2DB
        protected override bool TryCastShot()
        {
            bool flag = base.TryCastShot();
            if (flag && this.CasterIsPawn)
            {
                this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            return flag;
        }
    }
}
