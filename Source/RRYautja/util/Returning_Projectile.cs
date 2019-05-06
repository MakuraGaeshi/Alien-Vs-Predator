using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    // Token: 0x020006FF RID: 1791
    public class Returning_Projectile : Projectile
    {
        public int ExtraTargets
        {
            get
            {
                int count = 0;
                if (YautjaDefOf.RRY_YautjaRanged_Basic.IsFinished)
                {
                    count++;
                }
                if (YautjaDefOf.RRY_YautjaRanged_Med.IsFinished)
                {
                    count++;
                }
                if (YautjaDefOf.RRY_YautjaRanged_Adv.IsFinished)
                {
                    count++;
                }
                return count;
            }
        }

        public int remaningTargets;
        
        // Token: 0x06002721 RID: 10017 RVA: 0x0012A314 File Offset: 0x00128714
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
            PostPostImpactEffects(hitThing);
        }

        public virtual void PostPostImpactEffects(Thing hitThing)
        {
            LocalTargetInfo targetInfo = new LocalTargetInfo(launcher.Position);
            Projectile projectile2 = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("RRY_SmartDisk_Returning"), null);
            GenSpawn.Spawn(projectile2, base.PositionHeld, launcher.Map, 0);
            projectile2.Launch(this, base.PositionHeld.ToVector3(), launcher, launcher, ProjectileHitFlags.IntendedTarget, launcher);
        }

    }
}
