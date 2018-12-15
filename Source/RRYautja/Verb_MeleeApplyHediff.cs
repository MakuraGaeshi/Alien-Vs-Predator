using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000A0B RID: 2571
	public class Verb_MeleeApplyHediff : Verb_MeleeAttack
	{
		// Token: 0x060039CE RID: 14798 RVA: 0x001B7F74 File Offset: 0x001B6374
		protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
		{
			DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
			if (this.tool == null)
			{
				Log.ErrorOnce("Attempted to apply melee hediff without a tool", 38381735, false);
				return damageResult;
			}
			Pawn pawn = target.Thing as Pawn;
			if (pawn == null)
			{
				Log.ErrorOnce("Attempted to apply melee hediff without pawn target", 78330053, false);
				return damageResult;
			}
			HediffSet hediffSet = pawn.health.hediffSet;
			BodyPartTagDef bodypartTagTarget = this.verbProps.bodypartTagTarget;
			foreach (BodyPartRecord part in hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, bodypartTagTarget, null))
			{
				damageResult.AddHediff(pawn.health.AddHediff(this.tool.hediff, part, null, null));
				damageResult.AddPart(pawn, part);
				damageResult.wounded = true;
			}
			return damageResult;
		}

		// Token: 0x060039CF RID: 14799 RVA: 0x001B8064 File Offset: 0x001B6464
		public override bool IsUsableOn(Thing target)
		{
			return target is Pawn;
		}
	}
}
