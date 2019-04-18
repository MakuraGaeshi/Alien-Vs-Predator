using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RRYautja
{
	// Token: 0x02000A0D RID: 2573
	public class Verb_MeleeAttackFacehugger : Verb_MeleeAttack
    {
		// Token: 0x060039DE RID: 14814 RVA: 0x001B8078 File Offset: 0x001B6478
		private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
        {
            Pawn hitPawn = (Pawn)target;
            var effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(XenomorphDefOf.RRY_FaceHuggerInfection);
            if (effectOnPawn == null)
            {
                effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(XenomorphDefOf.RRY_XenomorphImpregnation);
            }
            if (effectOnPawn == null)
            {
                effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(XenomorphDefOf.RRY_HiddenXenomorphImpregnation);
            }
            if (Rand.Value * 100 > 50 & effectOnPawn == null && hitPawn.kindDef.race.race.baseBodySize>0.7f)
            {
                infect = true;
            }
            float damAmount = this.verbProps.AdjustedMeleeDamageAmount(this, base.CasterPawn);
			float armorPenetration = this.verbProps.AdjustedArmorPenetration(this, base.CasterPawn);
			DamageDef damDef = this.verbProps.meleeDamageDef;
			BodyPartGroupDef bodyPartGroupDef = null;
			HediffDef hediffDef = null;
			damAmount = Rand.Range(damAmount * 0.8f, damAmount * 1.2f);
			if (base.CasterIsPawn)
			{
				bodyPartGroupDef = this.verbProps.AdjustedLinkedBodyPartsGroup(this.tool);
				if (damAmount >= 1f)
				{
					if (base.HediffCompSource != null)
					{
						hediffDef = base.HediffCompSource.Def;
					}
				}
				else
				{
					damAmount = 1f;
					damDef = DamageDefOf.Blunt;
				}
			}
			ThingDef source;
			if (base.EquipmentSource != null)
			{
				source = base.EquipmentSource.def;
			}
			else
			{
				source = base.CasterPawn.def;
			}
			Vector3 direction = (target.Thing.Position - base.CasterPawn.Position).ToVector3();
			DamageDef def = damDef;
			float num = damAmount;
			float num2 = armorPenetration;
			Thing caster = this.caster;
            DamageInfo mainDinfo = new DamageInfo(def, num, num2, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
			mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
			mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
			mainDinfo.SetWeaponHediff(hediffDef);
			mainDinfo.SetAngle(direction);
			yield return mainDinfo;
			if (this.surpriseAttack && ((this.verbProps.surpriseAttack != null && !this.verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>()) || (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>())))
			{
				IEnumerable<ExtraMeleeDamage> extraDamages = Enumerable.Empty<ExtraMeleeDamage>();
				if (this.verbProps.surpriseAttack != null && this.verbProps.surpriseAttack.extraMeleeDamages != null)
				{
					extraDamages = extraDamages.Concat(this.verbProps.surpriseAttack.extraMeleeDamages);
				}
				if (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>())
				{
					extraDamages = extraDamages.Concat(this.tool.surpriseAttack.extraMeleeDamages);
				}
				foreach (ExtraMeleeDamage extraDamage in extraDamages)
				{
					int extraDamageAmount = GenMath.RoundRandom(extraDamage.AdjustedDamageAmount(this, base.CasterPawn));
					float extraDamageArmorPenetration = extraDamage.AdjustedArmorPenetration(this, base.CasterPawn);
					def = extraDamage.def;
					num2 = (float)extraDamageAmount;
					num = extraDamageArmorPenetration;
					caster = this.caster;
					DamageInfo extraDinfo = new DamageInfo(def, num2, num, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
					extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
					extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
					extraDinfo.SetWeaponHediff(hediffDef);
					extraDinfo.SetAngle(direction);
					yield return extraDinfo;
				}
			}
			yield break;
		}

		// Token: 0x060039DF RID: 14815 RVA: 0x001B80A4 File Offset: 0x001B64A4
		protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            Pawn hitPawn = (Pawn)target;
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
			foreach (DamageInfo dinfo in this.DamageInfosToApply(target))
			{
				if (target.ThingDestroyed)
				{
					break;
				}
				result = target.Thing.TakeDamage(dinfo);
            }
            if (infect)
            {
                foreach (var part in hitPawn.RaceProps.body.AllParts.Where(x => x.def.defName == "Head"))
                {
                    Hediff hediff = HediffMaker.MakeHediff(XenomorphDefOf.RRY_FaceHuggerInfection, hitPawn, null);
                    HediffComp_XenoFacehugger comp = hediff.TryGetComp<HediffComp_XenoFacehugger>();
                    comp.Props.Instigator = CasterPawn;
                    hitPawn.health.AddHediff(hediff, part, null);
                    //MoteMaker.ThrowText(hitPawn.Position.ToVector3(), hitPawn.Map, "Xeno_Facehugger_Success".Translate(Def.AddHediffChance), 12f);
                    caster.DeSpawn();
                }
            }
            return result;
		}

		// Token: 0x0400253C RID: 9532
		private const float MeleeDamageRandomFactorMin = 0.8f;

		// Token: 0x0400253D RID: 9533
		private const float MeleeDamageRandomFactorMax = 1.2f;
        public bool infect = false;
	}
}
