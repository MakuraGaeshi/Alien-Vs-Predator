using System;
using System.Collections.Generic;
using System.Linq;
using AbilityUser;
using RRYautja.settings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RRYautja
{
	
	// Token: 0x02000024 RID: 36
	public class VerbProperties_EquipmentAbility : VerbProperties_Ability
    {

    }

    // Token: 0x02000025 RID: 37
    public class Verb_UseEquipment : Verb_UseAbility
    {
        public new VerbProperties_EquipmentAbility UseAbilityProps
        {
            get
            {
                return (VerbProperties_EquipmentAbility)this.verbProps;
            }
        }
        // Token: 0x1700002C RID: 44
        // (get) Token: 0x060000E4 RID: 228 RVA: 0x00007E7B File Offset: 0x0000607B
        protected override int ShotsPerBurst
        {
            get
            {
                return this.verbProps.burstShotCount;
            }
        }

        public new ThingWithComps EquipmentSource
        {
            get
            {
                return AbilityUserComp.AbilityData.TemporaryWeaponPowers.Contains(Ability)? CasterPawn.equipment.Primary : null ;
            }
        }
        // Token: 0x060000E5 RID: 229 RVA: 0x00007E88 File Offset: 0x00006088
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            VerbProperties verbProps = this.verbProps;
            float? num;
            if (verbProps == null)
            {
                num = null;
            }
            else
            {
                ThingDef defaultProjectile = verbProps.defaultProjectile;
                if (defaultProjectile == null)
                {
                    num = null;
                }
                else
                {
                    ProjectileProperties projectile = defaultProjectile.projectile;
                    num = ((projectile != null) ? new float?(projectile.explosionRadius) : null);
                }
            }
            float result = num ?? 1f;
            bool flag = this.UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties != null;
            if (flag)
            {
                bool showRangeOnSelect = this.UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties.showRangeOnSelect;
                if (showRangeOnSelect)
                {
                    result = (float)this.UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties.range;
                }
            }
            return result;
        }

        private void DebugMessage(string s)
        {
            bool flag = this.debugMode;
            if (flag)
            {
                Log.Message(s, false);
            }
        }
        private void ThrowDebugText(string text)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, text, -1f);
            }
        }
        // Token: 0x06006520 RID: 25888 RVA: 0x001B9057 File Offset: 0x001B7457
        private void ThrowDebugText(string text, IntVec3 c)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(c.ToVector3Shifted(), this.caster.Map, text, -1f);
            }
        }
        protected new virtual void UpdateTargets()
        {
            TargetsAoE.Clear();
            if (UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                //Log.Message("AoE Called");
                if (UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast AoE-Ability without defining a target class");

                var targets = new List<Thing>();

                //Handle TargetAoE start location.
                var aoeStartPosition = caster.PositionHeld;
                if (!UseAbilityProps.TargetAoEProperties.startsFromCaster)
                    aoeStartPosition = currentTarget.Cell;

                //Handle friendly fire targets.
                if (!UseAbilityProps.TargetAoEProperties.friendlyFire)
                {
                    targets = caster.Map.listerThings.AllThings.Where(x =>
                        x.Position.InHorDistOf(aoeStartPosition, UseAbilityProps.TargetAoEProperties.range) &&
                        UseAbilityProps.TargetAoEProperties.targetClass.IsAssignableFrom(x.GetType()) &&
                        x.Faction.HostileTo(Faction.OfPlayer)).ToList();
                }
                else if (UseAbilityProps.TargetAoEProperties.targetClass == typeof(Plant) ||
                         UseAbilityProps.TargetAoEProperties.targetClass == typeof(Building))
                {
                    targets = caster.Map.listerThings.AllThings.Where(x =>
                        x.Position.InHorDistOf(aoeStartPosition, UseAbilityProps.TargetAoEProperties.range) &&
                        UseAbilityProps.TargetAoEProperties.targetClass.IsAssignableFrom(x.GetType())).ToList();
                    foreach (var targ in targets)
                    {
                        var tinfo = new LocalTargetInfo(targ);
                        TargetsAoE.Add(tinfo);
                    }
                    return;
                }
                else
                {
                    targets.Clear();
                    targets = caster.Map.listerThings.AllThings.Where(x =>
                        x.Position.InHorDistOf(aoeStartPosition, UseAbilityProps.TargetAoEProperties.range) &&
                        UseAbilityProps.TargetAoEProperties.targetClass.IsAssignableFrom(x.GetType()) &&
                        (x.HostileTo(Faction.OfPlayer) || UseAbilityProps.TargetAoEProperties.friendlyFire)).ToList();
                }

                var maxTargets = UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties.maxTargets;
                var randTargets = new List<Thing>(targets.InRandomOrder());
                for (var i = 0; i < maxTargets && i < randTargets.Count(); i++)
                {
                    var tinfo = new TargetInfo(randTargets[i]);
                    if (UseAbilityProps.targetParams.CanTarget(tinfo))
                        TargetsAoE.Add(new LocalTargetInfo(randTargets[i]));
                }
            }
            else
            {
                TargetsAoE.Clear();
                TargetsAoE.Add(currentTarget);
            }
        }

        protected new bool? TryLaunchProjectile(ThingDef projectileDef, LocalTargetInfo launchTarget)
        {
            DebugMessage(launchTarget.ToString());
            var flag = TryFindShootLineFromTo(caster.Position, launchTarget, out var shootLine);
            if (verbProps.stopBurstWithoutLos && !flag)
            {
                DebugMessage("Targeting cancelled");
                return false;
            }
            var drawPos = caster.DrawPos;
            var projectile2 = (Projectile_AbilityBase)GenSpawn.Spawn(projectileDef, shootLine.Source, caster.Map);
            projectile2.extraDamages = UseAbilityProps.extraDamages;
            projectile2.localSpawnThings = UseAbilityProps.thingsToSpawn;
            verbProps.soundCast?.PlayOneShot(new TargetInfo(caster.Position, caster.Map, false));
            verbProps.soundCastTail?.PlayOneShotOnCamera();
            if (DebugViewSettings.drawShooting)
                MoteMaker.ThrowText(caster.DrawPos, caster.Map, "ToHit", -1f);
			

            if (this.verbProps.forcedMissRadius > 0.5f)
            {
                float num = VerbUtility.CalculateAdjustedForcedMiss(this.verbProps.forcedMissRadius, this.currentTarget.Cell - this.caster.Position);
                if (num > 0.5f)
                {
                    int max = GenRadial.NumCellsInRadius(num);
                    int num2 = Rand.Range(0, max);
                    if (num2 > 0)
                    {
                        IntVec3 c = this.currentTarget.Cell + GenRadial.RadialPattern[num2];
                        this.ThrowDebugText("ToRadius");
                        this.ThrowDebugText("Rad\nDest", c);
                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        if (Rand.Chance(0.5f))
                        {
                            projectileHitFlags = ProjectileHitFlags.All;
                        }
                        if (!this.canHitNonTargetPawnsNow)
                        {
                            projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                        }
                    //    projectile2.Launch(CasterPawn, drawPos, c, this.currentTarget, projectileHitFlags, caster, null);
                        projectile2.Launch(caster, Ability.Def, drawPos, c, projectileHitFlags, null,
                            UseAbilityProps.hediffsToApply,
                            UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                        return true;
                    }
                }
            }
            ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef targetCoverDef = (randomCoverToMissInto == null) ? null : randomCoverToMissInto.def;
            if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                this.ThrowDebugText("ToWild" + ((!this.canHitNonTargetPawnsNow) ? string.Empty : "\nchntp"));
                this.ThrowDebugText("Wild\nDest", shootLine.Dest);
                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                //    projectile2.Launch(CasterPawn, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags2, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, shootLine.Dest, projectileHitFlags2, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                return true;
            }
            if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
            {
                this.ThrowDebugText("ToCover" + ((!this.canHitNonTargetPawnsNow) ? string.Empty : "\nchntp"));
                this.ThrowDebugText("Cover\nDest", randomCoverToMissInto.Position);
                ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }
            //    projectile2.Launch(CasterPawn, drawPos, randomCoverToMissInto, this.currentTarget, projectileHitFlags3, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, randomCoverToMissInto, projectileHitFlags3, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                return true;
            }
            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (this.canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }
            this.ThrowDebugText("ToHit" + ((!this.canHitNonTargetPawnsNow) ? string.Empty : "\nchntp"));
            if (this.currentTarget.Thing != null)
            {
            //    projectile2.Launch(CasterPawn, drawPos, this.currentTarget, this.currentTarget, projectileHitFlags4, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, currentTarget, projectileHitFlags4, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                this.ThrowDebugText("Hit\nDest", this.currentTarget.Cell);
            }
            else
            {
            //    projectile2.Launch(CasterPawn, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags4, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, shootLine.Dest, projectileHitFlags4, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                this.ThrowDebugText("Hit\nDest", shootLine.Dest);
            }
			/*
            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (this.canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }
            DebugMessage(launchTarget.ToString());
            projectile2.Launch(caster, Ability.Def, drawPos, launchTarget, projectileHitFlags4, null,
                UseAbilityProps.hediffsToApply,
                UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
			*/
            return true;
        }

        public void CriticalOverheatExplosion()
        {
            Map map = this.caster.Map;
            if (this.Projectile.projectile.explosionEffect != null)
            {
                Effecter effecter = this.Projectile.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(this.EquipmentSource.Position, map, false), new TargetInfo(this.EquipmentSource.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = this.caster.Position;
            Map map2 = map;
            float explosionRadius = this.Projectile.projectile.explosionRadius;
            DamageDef damageDef = this.Projectile.projectile.damageDef;
            Thing launcher = this.EquipmentSource;
            int DamageAmount = this.Projectile.projectile.GetDamageAmount(this.EquipmentSource, null);
            float ArmorPenetration = this.Projectile.projectile.GetArmorPenetration(this.EquipmentSource, null);
            SoundDef soundExplode = this.Projectile.projectile.soundExplode;
            ThingDef equipmentDef = this.EquipmentSource.def;
            ThingDef def = this.EquipmentSource.def;
            Thing thing = this.EquipmentSource;
            ThingDef postExplosionSpawnThingDef = this.Projectile.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.Projectile.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.Projectile.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.Projectile.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
            return;
        }

        public override void WarmupComplete()
        {
            if (verbTracker == null)
                verbTracker = CasterPawn.verbTracker;
            burstShotsLeft = ShotsPerBurst;
            state = VerbState.Bursting;
            TryCastNextBurstShot();
            //Find.BattleLog.Add(new BattleLogEntry_RangedFire(this.caster,
            //    (!this.currentTarget.HasThing) ? null : this.currentTarget.Thing,
            //    (base.EquipmentSource == null) ? null : base.EquipmentSource.def, this.Projectile,
            //    this.ShotsPerBurst > 1));
        }
		
        // Token: 0x040000A2 RID: 162
        public List<LocalTargetInfo> TargetsAoE = new List<LocalTargetInfo>();

        // Token: 0x040000A3 RID: 163
        public Action<Thing> timeSavingActionVariable = null;

        // Token: 0x040000A5 RID: 165
        private bool debugMode = false;
    }
}
