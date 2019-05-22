using RRYautja;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    // Token: 0x020006FF RID: 1791
    public class Returning_Projectile : Projectile
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.timesBounced, "timesBounced");
            Scribe_Values.Look<bool>(ref this.canBounce, "canBounce");
            Scribe_References.Look<Pawn>(ref this.OriginalPawn, "OriginalPawnRef");//, Props.pawn);
            Scribe_References.Look<Thing>(ref this.OriginalWeapon, "OriginalWeaponRef");//, Props.pawn);
            Scribe_References.Look<Projectile>(ref this.OriginalProjectile, "OriginalProjectileRef");//, Props.pawn);

            /*
            Scribe_Defs.Look<HediffDef>(ref MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef");//, Props.corpse);//
            Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef");//, Props.pawn);
            Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");//, Props.Marklabel);
            Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");//, Props.Marklabel);
            Scribe_Values.Look<bool>(ref this.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
            */
        }
        public bool canBounce = false;

        public int ExtraTargets
        {
            get
            {
                int count = 0;
                bool enablebounce = false;
                if (OriginalPawn.Faction == Faction.OfPlayer)
                {
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
                }

                if (OriginalPawn.apparel.WornApparelCount>0)
                {
                    foreach (var item in OriginalPawn.apparel.WornApparel)
                    {
                        if (item.def.defName.Contains("RRY_Apparel_") && item.def.defName.Contains("BioMask"))
                        {
                            enablebounce = true;
                            this.canBounce = enablebounce;
                        }
                    }
                }

                if (!enablebounce)
                {
                    count = 0;
                }
                return count;
            }
        }

        public Pawn OriginalPawn;
        public Thing OriginalWeapon;
        public Projectile OriginalProjectile;
        public int timesBounced = 0;
        
        // Token: 0x06002721 RID: 10017 RVA: 0x0012A314 File Offset: 0x00128714
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
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
            this.DeSpawn();
        }

        public override void Tick()
        {
            base.Tick();
            /*
            if (this.usedTarget.Thing.Position != this.intendedTarget.Thing.Position)
            {
                this.Launch(launcher, this.ExactPosition, usedTarget, intendedTarget, ProjectileHitFlags.IntendedTarget, launcher);
            }
            */
        }

        public virtual void PostPostImpactEffects(Thing hitThing)
        {
            if (hitThing is Pawn hitPawn && !hitPawn.Dead && !hitPawn.Downed)
            {
                Hediff hediff = HediffMaker.MakeHediff(YautjaDefOf.RRY_Hediff_BouncedProjectile, hitPawn);
                Hediff_Bouncer bouncer = (Hediff_Bouncer)hediff;
                if (this.timesBounced==0)
                {
                    this.OriginalPawn = (Pawn)launcher;
                    this.OriginalWeapon = launcher;
                    this.OriginalProjectile = this;
                }
                bouncer.OriginalPawn = this.OriginalPawn;
                bouncer.OriginalWeapon = this.OriginalWeapon;
                bouncer.OriginalProjectile = this.OriginalProjectile;
                bouncer.TimesBounced = this.timesBounced;
                if (hitPawn!=OriginalPawn) hitPawn.health.AddHediff(bouncer);
                else
                {
                    Projectile projectile2 = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("RRY_SmartDisk_Returning"), null);
                    if (OriginalPawn.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
                    {
                        projectile2 = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("RRY_SmartDisk_Returning"), null);
                    }
                    GenSpawn.Spawn(projectile2, base.PositionHeld, launcher.Map, 0);
                    projectile2.Launch(this, base.PositionHeld.ToVector3(), launcher, launcher, ProjectileHitFlags.IntendedTarget, launcher);
                }
            }
            /*
            else if (timesBounced < ExtraTargets)
            {
                Thing thing = GenClosest.ClosestThingReachable(this.ExactPosition.ToIntVec3(), OriginalPawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), Verse.AI.PathEndMode.Touch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 10f, x => (x.Faction.HostileTo(this.OriginalPawn.Faction)), null, 0, -1, false, RegionType.Set_Passable, false);

                if (thing!=null)
                {
                    timesBounced++;
                    this.Launch(launcher, this.ExactPosition, thing, thing, ProjectileHitFlags.IntendedTarget, launcher);
                }
                else
                {
                    Projectile projectile2 = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("RRY_SmartDisk_Returning"), null);
                    GenSpawn.Spawn(projectile2, base.PositionHeld, launcher.Map, 0);
                    projectile2.Launch(this, base.PositionHeld.ToVector3(), launcher, launcher, ProjectileHitFlags.IntendedTarget, launcher);
                }
            }
            */
            else
            {
                Projectile projectile2 = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("RRY_SmartDisk_Returning"), null);
                if (OriginalPawn.kindDef.race != YautjaDefOf.RRY_Alien_Yautja)
                {
                    projectile2 = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("RRY_SmartDisk_Returning"), null);
                }
                GenSpawn.Spawn(projectile2, base.PositionHeld, launcher.Map, 0);
                projectile2.Launch(this, base.PositionHeld.ToVector3(), launcher, launcher, ProjectileHitFlags.IntendedTarget, launcher);
            }
        }

    }
}
