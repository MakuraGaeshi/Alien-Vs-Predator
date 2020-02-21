using System;
using Verse;

namespace RimWorld
{
    // Token: 0x0200074A RID: 1866
    public class CompProperties_MaintainableLike : CompProperties
    {
        // Token: 0x0600292F RID: 10543 RVA: 0x001389BE File Offset: 0x00136DBE
        public CompProperties_MaintainableLike()
        {
            this.compClass = typeof(CompMaintainableLike);
        }

        // Token: 0x040016D7 RID: 5847
        public int ticksHealthy = 1000;

        // Token: 0x040016D8 RID: 5848
        public int ticksNeedsMaintenance = 1000;

        // Token: 0x040016D9 RID: 5849
        public int damagePerTickRare = 10;
    }

    // Token: 0x02000749 RID: 1865
    public class CompMaintainableLike : ThingComp
    {
        // Token: 0x17000645 RID: 1605
        // (get) Token: 0x06002926 RID: 10534 RVA: 0x00138837 File Offset: 0x00136C37
        public CompProperties_MaintainableLike Props
        {
            get
            {
                return (CompProperties_MaintainableLike)this.props;
            }
        }

        private HiveLike hiveLike
        {
            get
            {
                return (HiveLike)parent;
            }
        }

        public CompSpawnerHiveLikes compSpawner
        {
            get
            {
                return hiveLike.TryGetComp<CompSpawnerHiveLikes>();
            }
        }
        // Token: 0x17000646 RID: 1606
        // (get) Token: 0x06002927 RID: 10535 RVA: 0x00138844 File Offset: 0x00136C44
        public MaintainableStage CurStage
        {
            get
            {
                if (this.ticksSinceMaintain < this.Props.ticksHealthy)
                {
                    return MaintainableStage.Healthy;
                }
                if (this.ticksSinceMaintain < this.Props.ticksHealthy + this.Props.ticksNeedsMaintenance)
                {
                    return MaintainableStage.NeedsMaintenance;
                }
                return MaintainableStage.Damaging;
            }
        }

        // Token: 0x17000647 RID: 1607
        // (get) Token: 0x06002928 RID: 10536 RVA: 0x00138884 File Offset: 0x00136C84
        private bool Active
        {
            get
            {
                return compSpawner.canSpawnHiveLikes;
            }
        }

        // Token: 0x06002929 RID: 10537 RVA: 0x001388AC File Offset: 0x00136CAC
        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticksSinceMaintain, "ticksSinceMaintain", 0, false);
        }

        // Token: 0x0600292A RID: 10538 RVA: 0x001388C0 File Offset: 0x00136CC0
        public override void CompTick()
        {
            base.CompTick();
            if (this.Active)
            {
                this.ticksSinceMaintain++;
                if (Find.TickManager.TicksGame % 250 == 0)
                {
                    this.CheckTakeDamage();
                }
            }
        }

        // Token: 0x0600292B RID: 10539 RVA: 0x001388FD File Offset: 0x00136CFD
        public override void CompTickRare()
        {
            base.CompTickRare();
            if (this.Active)
            {
                this.ticksSinceMaintain += 250;
                this.CheckTakeDamage();
            }
        }

        // Token: 0x0600292C RID: 10540 RVA: 0x0013892C File Offset: 0x00136D2C
        private void CheckTakeDamage()
        {
            if (this.CurStage == MaintainableStage.Damaging)
            {
                this.parent.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, (float)this.Props.damagePerTickRare, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
            }
        }

        // Token: 0x0600292D RID: 10541 RVA: 0x00138976 File Offset: 0x00136D76
        public void Maintained()
        {
            this.ticksSinceMaintain = 0;
        }

        // Token: 0x0600292E RID: 10542 RVA: 0x00138980 File Offset: 0x00136D80
        public override string CompInspectStringExtra()
        {
            MaintainableStage curStage = this.CurStage;
            if (curStage == MaintainableStage.NeedsMaintenance)
            {
                return "DueForMaintenance".Translate();
            }
            if (curStage != MaintainableStage.Damaging)
            {
                return null;
            }
            return "DeterioratingDueToLackOfMaintenance".Translate();
        }

        // Token: 0x040016D6 RID: 5846
        public int ticksSinceMaintain;
    }
}
