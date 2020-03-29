using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection;
using UnityEngine;
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "DamageInfosToApply")]
    public static class AvP_Verb_MeleeAttackDamage_DamageInfosToApply_Facehugger_Patch
    {
        [HarmonyPostfix]
        public static void DamageInfosToApply_ForceWeapon_Postfix(ref Verb_MeleeAttackDamage __instance, LocalTargetInfo target, ref IEnumerable<DamageInfo> __result)
        {
            if (__instance!=null)
            {
                if (target==null)
                {
                    return;
                }
                if (!target.HasThing)
                {
                    return;
                }
                if (target.Pawn== null)
                {
                    return;
                }
                if (__instance.CasterIsPawn)
                {
                    Pawn Attacker = __instance.CasterPawn;
                    Pawn hitPawn = target.Pawn;
                    if (__instance.CasterPawn.isXenomorph())
                    {
                        if (Attacker.def == XenomorphRacesDefOf.RRY_Xenomorph_FaceHugger)
                        {
                            if (!hitPawn.isPotentialHost())
                            {
                                return;
                            }
                            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
                            DamageDef def = __instance.verbProps.meleeDamageDef;
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
                                        bool flag2 = wornApparel[i].def.apparel.CoversBodyPart(Head(hitPawn));
                                        if (flag2)
                                        {
                                            armour += wornApparel[i].def.statBases.GetStatOffsetFromList(def.armorCategory.armorRatingStat);
                                        }
                                    }
                                }
                            }
                            float InfecterRoll = (Rand.Value * 100) * (1 - tgtdodge);
                            float InfectionDefence = 50 + tgtmelee + (armour * 10);
                            if ((InfecterRoll > InfectionDefence || hitPawn.Downed) && !hitPawn.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Anesthetic))
                            {

                                foreach (var part in hitPawn.RaceProps.body.AllParts.Where(x => x.def.defName.Contains("Head") && !x.def.defName.Contains("Claw")))
                                {
                                    Hediff hediff = HediffMaker.MakeHediff(XenomorphDefOf.RRY_FaceHuggerInfection, hitPawn, null);
                                    Comp_Facehugger _Facehugger = Attacker.TryGetComp<Comp_Facehugger>();
                                    HediffComp_XenoFacehugger comp = hediff.TryGetComp<HediffComp_XenoFacehugger>();
                                    comp.instigator = Attacker;
                                    comp.instigatorKindDef = Attacker.kindDef;
                                    comp.royaleHugger = _Facehugger.RoyaleHugger;
                                    comp.previousImpregnations = _Facehugger.Impregnations;
                                    hitPawn.health.AddHediff(hediff, part, null);
                                    string text = TranslatorFormattedStringExtensions.Translate("Xeno_Facehugger_Attach", hitPawn.LabelShort, part.LabelShortCap);
                                    MoteMaker.ThrowText(hitPawn.Position.ToVector3(), hitPawn.Map, text, 5f);
                                    if (Attacker.Spawned)
                                    {
                                        Attacker.DeSpawn();
                                    }
                                    comp.TryAcceptThing(Attacker);
                                    //    comp.GetDirectlyHeldThings().TryAdd(CasterPawn);

                                }
                            }

                        }
                    }
                }
            }
        }

        public static BodyPartRecord Head(Pawn hitPawn)
        {
            return hitPawn.RaceProps.body.AllParts.Where(x => x.def.defName.Contains("Head")).First();
        }

    }
    /*
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "ApplyMeleeDamageToTarget")]
    public static class AM_Verb_MeleeAttackDamage_ApplyMeleeDamageToTarget_ForceWeapon_Patch
    {
        [HarmonyPrefix]
        public static void ApplyMeleeDamageToTarget_ForceWeapon_Prefix(DamageWorker.DamageResult __instance ,LocalTargetInfo target)
        {
            
        }
    }
    */
}