using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000D4F RID: 3407
    public class DamageWorker_Acid : DamageWorker_AddInjury
    {
        // Token: 0x06004BA7 RID: 19367 RVA: 0x002357D4 File Offset: 0x00233BD4
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            Pawn pawn = victim as Pawn;
            if (pawn != null && pawn.Faction == Faction.OfPlayer)
            {
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
            Map map = victim.Map;
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (!damageResult.deflected && !dinfo.InstantPermanentInjury)
            {
             //   victim.TryAttachFire(Rand.Range(0.15f, 0.25f));
            }
            if (victim.Destroyed && map != null && pawn == null)
            {
                foreach (IntVec3 c in victim.OccupiedRect())
                {
                    FilthMaker.MakeFilth(c, map, ThingDefOf.Filth_Ash, 1);
                }
                Plant plant = victim as Plant;
                if (plant != null && victim.def.plant.IsTree && plant.LifeStage != PlantLifeStage.Sowing && victim.def != ThingDefOf.BurnedTree)
                {
                    DeadPlant deadPlant = (DeadPlant)GenSpawn.Spawn(ThingDefOf.BurnedTree, victim.Position, map, WipeMode.Vanish);
                    deadPlant.Growth = plant.Growth;
                }
                
            }
            return damageResult;
        }

        // Token: 0x06004B85 RID: 19333 RVA: 0x002344F0 File Offset: 0x002328F0
        private void CheckApplySpreadDamage(DamageInfo dinfo, Thing t)
        {
            if (Rand.Chance(0.5f))
            {
                dinfo.SetAmount((float)Mathf.CeilToInt(dinfo.Amount * Rand.Range(0.35f, 0.7f)));
                t.TakeDamage(dinfo);
            }
        }
        // Token: 0x06004BA8 RID: 19368 RVA: 0x00235924 File Offset: 0x00233D24
        public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, bool canThrowMotes)
        {
            base.ExplosionAffectCell(explosion, c, damagedThings, canThrowMotes);
            if (this.def == XenomorphDefOf.RRY_AcidDamage && Rand.Chance(FireUtility.ChanceToStartFireIn(c, explosion.Map)))
            {
            //    FireUtility.TryStartFireIn(c, explosion.Map, Rand.Range(0.2f, 0.6f));
            }
        }
    }
}
