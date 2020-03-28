using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000C1C RID: 3100
	public class JobGiver_Xenomorph_AIFightEnemies : JobGiver_AIFightEnemies
	{
		// Token: 0x060047EB RID: 18411 RVA: 0x00033D97 File Offset: 0x00031F97
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_Xenomorph_AIFightEnemies jobGiver_Xenomorph_AIFightEnemies = (JobGiver_Xenomorph_AIFightEnemies)base.DeepCopy(resolve);
			jobGiver_Xenomorph_AIFightEnemies.targetAcquireRadius = this.targetAcquireRadius;
			jobGiver_Xenomorph_AIFightEnemies.targetKeepRadius = this.targetKeepRadius;
			jobGiver_Xenomorph_AIFightEnemies.needLOSToAcquireNonPawnTargets = this.needLOSToAcquireNonPawnTargets;
			jobGiver_Xenomorph_AIFightEnemies.chaseTarget = this.chaseTarget;
			return jobGiver_Xenomorph_AIFightEnemies;
		}
		// Token: 0x060047D3 RID: 18387 RVA: 0x0018CBD0 File Offset: 0x0018ADD0
		protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
		{
			Thing enemyTarget = pawn.mindState.enemyTarget;
			bool allowManualCastWeapons = !pawn.IsColonist;
			Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
			if (verb == null)
			{
				dest = IntVec3.Invalid;
				return false;
			}
			return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
			{
				caster = pawn,
				target = enemyTarget,
				verb = verb,
				maxRangeFromTarget = verb.verbProps.range,
				wantCoverFromTarget = (verb.verbProps.range > 5f)
			}, out dest);
		}

		// Token: 0x060047EC RID: 18412 RVA: 0x0018CEF0 File Offset: 0x0018B0F0
		protected override Job TryGiveJob(Pawn pawn)
		{
			this.UpdateEnemyTarget(pawn);
			Thing enemyTarget = pawn.mindState.enemyTarget;
			if (enemyTarget == null)
			{
				return null;
			}
			Pawn pawn2 = enemyTarget as Pawn;
			if (pawn2 != null && pawn2.IsInvisible())
			{
				return null;
			}
			bool allowManualCastWeapons = !pawn.IsColonist;
			Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
			if (verb == null)
			{
				return null;
			}
			if (verb.verbProps.IsMeleeAttack)
			{
				return this.MeleeAttackJob(enemyTarget);
			}
			bool flag = CoverUtility.CalculateOverallBlockChance(pawn, enemyTarget.Position, pawn.Map) > 0.01f;
			bool flag2 = pawn.Position.Standable(pawn.Map) && pawn.Map.pawnDestinationReservationManager.CanReserve(pawn.Position, pawn, pawn.Drafted);
			bool flag3 = verb.CanHitTarget(enemyTarget);
			bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;
			if ((flag && flag2 && flag3) || (flag4 && flag3))
			{
				return JobMaker.MakeJob(JobDefOf.Wait_Combat, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true);
			}
			IntVec3 intVec;
			if (!this.TryFindShootingPosition(pawn, out intVec))
			{
				return null;
			}
			if (intVec == pawn.Position)
			{
				return JobMaker.MakeJob(JobDefOf.Wait_Combat, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true);
			}
			Job job = JobMaker.MakeJob(JobDefOf.Goto, intVec);
			job.expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange;
			job.checkOverrideOnExpire = true;
			return job;
		}
		protected override Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
			if (this.needLOSToAcquireNonPawnTargets)
			{
				targetScanFlags |= TargetScanFlags.NeedLOSToNonPawns;
			}
			if (this.PrimaryVerbIsIncendiary(pawn))
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			return (Thing)AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, (Thing x) => this.ExtraTargetValidator(pawn, x), 0f, this.targetAcquireRadius, this.GetFlagPosition(pawn), this.GetFlagRadius(pawn), false, true);
		}

		// Token: 0x060047F1 RID: 18417 RVA: 0x0018D25C File Offset: 0x0018B45C
		private bool PrimaryVerbIsIncendiary(Pawn pawn)
		{
			if (pawn.equipment != null && pawn.equipment.Primary != null)
			{
				List<Verb> allVerbs = pawn.equipment.Primary.GetComp<CompEquippable>().AllVerbs;
				for (int i = 0; i < allVerbs.Count; i++)
				{
					if (allVerbs[i].verbProps.isPrimary)
					{
						return allVerbs[i].IsIncendiary();
					}
				}
			}
			return false;
		}

		// Token: 0x04002F8C RID: 12172
		public float targetAcquireRadius = 56f;

		// Token: 0x04002F8D RID: 12173
		public float targetKeepRadius = 65f;

		// Token: 0x04002F8E RID: 12174
		public bool needLOSToAcquireNonPawnTargets;

		// Token: 0x04002F8F RID: 12175
		public bool chaseTarget;
	}
}
