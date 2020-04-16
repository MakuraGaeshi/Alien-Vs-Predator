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
        public Pawn hitPawn
        {
            get
            {
                return (Pawn)this.currentTarget;
            }
        }

        public BodyPartRecord Head
        {
            get
            {
                return hitPawn.RaceProps.body.AllParts.Where(x => x.def.defName.Contains("Head")).First();
            }
        }

        // Token: 0x06005F6B RID: 24427 RVA: 0x0020F288 File Offset: 0x0020D488
        private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
        {
            Pawn hitPawn = (Pawn)target;
            DamageDef def = this.verbProps.meleeDamageDef;
            bool flag = XenomorphUtil.isInfectablePawn(hitPawn);
            float tgtmelee = 0f;
            float tgtdodge = 0f;
            float armourBlunt = 0f;
            float armourSharp = 0f;
            float armourHeat = 0f;
            float armour = 0f;
            if (hitPawn.RaceProps.Humanlike) tgtmelee = hitPawn.skills.GetSkill(SkillDefOf.Melee).Level;
            if (hitPawn.RaceProps.Humanlike) tgtdodge = hitPawn.GetStatValue(StatDefOf.MeleeDodgeChance);
            if (hitPawn.RaceProps.Humanlike)
            {
                if (hitPawn.apparel.WornApparel.Count > 0 && hitPawn.apparel.WornApparel is List<Apparel> wornApparel)
                {
                    for (int i = 0; i < wornApparel.Count; i++)
                    {
                        bool flag2 = wornApparel[i].def.apparel.CoversBodyPart(Head);
                        if (flag2)
                        {
                            armour += wornApparel[i].def.statBases.GetStatOffsetFromList(def.armorCategory.armorRatingStat);
                        }
                    }
                }
            }
            float InfecterRoll = (Rand.Value * 100) * (1 - tgtdodge);
            float InfectionDefence = 50 + tgtmelee + (armour * 10);
            if ((InfecterRoll > InfectionDefence || hitPawn.Downed) && flag && hitPawn is Pawn && !hitPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Anesthetic))
            {
                infect = true;
            }
            else
            {
                infect = false;
            }
            float num = this.verbProps.AdjustedMeleeDamageAmount(this, this.CasterPawn);
            float armorPenetration = this.verbProps.AdjustedArmorPenetration(this, this.CasterPawn);
            BodyPartGroupDef bodyPartGroupDef = null;
            HediffDef hediffDef = null;
            num = Rand.Range(num * 0.8f, num * 1.2f);
            if (this.CasterIsPawn)
            {
                bodyPartGroupDef = this.verbProps.AdjustedLinkedBodyPartsGroup(this.tool);
                if (num >= 1f)
                {
                    if (base.HediffCompSource != null)
                    {
                        hediffDef = base.HediffCompSource.Def;
                    }
                }
                else
                {
                    num = 1f;
                    def = DamageDefOf.Blunt;
                }
            }
            ThingDef source;
            if (base.EquipmentSource != null)
            {
                source = base.EquipmentSource.def;
            }
            else
            {
                source = this.CasterPawn.def;
            }
            Vector3 direction = (target.Thing.Position - this.CasterPawn.Position).ToVector3();
            DamageInfo damageInfo = new DamageInfo(def, num, armorPenetration, -1f, this.caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
            damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
            damageInfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
            damageInfo.SetWeaponHediff(hediffDef);
            damageInfo.SetAngle(direction);
            yield return damageInfo;
            if (this.tool != null && this.tool.extraMeleeDamages != null)
            {
                foreach (ExtraDamage extraDamage in this.tool.extraMeleeDamages)
                {
                    if (Rand.Chance(extraDamage.chance))
                    {
                        num = extraDamage.amount;
                        num = Rand.Range(num * 0.8f, num * 1.2f);
                        damageInfo = new DamageInfo(extraDamage.def, num, extraDamage.AdjustedArmorPenetration(this, this.CasterPawn), -1f, this.caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                        damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                        damageInfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                        damageInfo.SetWeaponHediff(hediffDef);
                        damageInfo.SetAngle(direction);
                        yield return damageInfo;
                    }
                }
                List<ExtraDamage>.Enumerator enumerator = default(List<ExtraDamage>.Enumerator);
            }
            if (this.surpriseAttack && ((this.verbProps.surpriseAttack != null && !this.verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>()) || (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>())))
            {
                IEnumerable<ExtraDamage> enumerable = Enumerable.Empty<ExtraDamage>();
                if (this.verbProps.surpriseAttack != null && this.verbProps.surpriseAttack.extraMeleeDamages != null)
                {
                    enumerable = enumerable.Concat(this.verbProps.surpriseAttack.extraMeleeDamages);
                }
                if (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>())
                {
                    enumerable = enumerable.Concat(this.tool.surpriseAttack.extraMeleeDamages);
                }
                foreach (ExtraDamage extraDamage2 in enumerable)
                {
                    int num2 = GenMath.RoundRandom(extraDamage2.AdjustedDamageAmount(this, this.CasterPawn));
                    float armorPenetration2 = extraDamage2.AdjustedArmorPenetration(this, this.CasterPawn);
                    DamageInfo damageInfo2 = new DamageInfo(extraDamage2.def, (float)num2, armorPenetration2, -1f, this.caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                    damageInfo2.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                    damageInfo2.SetWeaponBodyPartGroup(bodyPartGroupDef);
                    damageInfo2.SetWeaponHediff(hediffDef);
                    damageInfo2.SetAngle(direction);
                    yield return damageInfo2;
                }
                IEnumerator<ExtraDamage> enumerator2 = null;
            }
            yield break;
        }
		// Token: 0x060039DF RID: 14815 RVA: 0x001B80A4 File Offset: 0x001B64A4
		protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
            Pawn hitPawn = (Pawn)target;
            if (infect && hitPawn != null && !XenomorphUtil.IsInfectedPawn(hitPawn) && !hitPawn.Dead && hitPawn.RaceProps.body.AllParts.Any(x => x.def.defName.Contains("Head") && !x.def.defName.Contains("Claw")))
            {
                foreach (var part in hitPawn.RaceProps.body.AllParts.Where(x => x.def.defName.Contains("Head") && !x.def.defName.Contains("Claw")))
                {
                    Hediff hediff = HediffMaker.MakeHediff(XenomorphDefOf.RRY_FaceHuggerInfection, hitPawn, null);
                    Comp_Facehugger _Facehugger = CasterPawn.TryGetComp<Comp_Facehugger>();
                    HediffComp_XenoFacehugger comp = hediff.TryGetComp<HediffComp_XenoFacehugger>();
                    comp.instigator = CasterPawn;
                    comp.instigatorKindDef = CasterPawn.kindDef;
                    comp.royaleHugger = _Facehugger.RoyaleHugger;
                    comp.previousImpregnations = _Facehugger.Impregnations;
                    hitPawn.health.AddHediff(hediff, part, null);
                    string text = TranslatorFormattedStringExtensions.Translate("Xeno_Facehugger_Attach", hitPawn.LabelShort, part.LabelShortCap);
                    MoteMaker.ThrowText(hitPawn.Position.ToVector3(), hitPawn.Map, text, 5f);
                    if (CasterPawn.Spawned)
                    {
                        CasterPawn.DeSpawn();
                    }
                    comp.TryAcceptThing(CasterPawn);
                    //    comp.GetDirectlyHeldThings().TryAdd(CasterPawn);

                    infect = false;
                }
                return result;
            }
            foreach (DamageInfo dinfo in this.DamageInfosToApply(target))
            {
                if (target.ThingDestroyed)
                {
                    break;
                }
                result = target.Thing.TakeDamage(dinfo);
            }
            return result;
        }


        public PawnKindDef HuggerKindDef = XenomorphDefOf.RRY_Xenomorph_FaceHugger;
        public PawnKindDef RoyaleKindDef = XenomorphDefOf.RRY_Xenomorph_RoyaleHugger;
        // Token: 0x0400253C RID: 9532
        private const float MeleeDamageRandomFactorMin = 0.8f;

		// Token: 0x0400253D RID: 9533
		private const float MeleeDamageRandomFactorMax = 1.2f;
        public bool infect = false;
	}
}
