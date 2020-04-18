using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using AvP.settings;
using AvP.ExtensionMethods;
using HunterMarkingSystem;
using HunterMarkingSystem.ExtensionMethods;

namespace AvP.HarmonyInstance
{

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) })]
    public static class AvP_PawnGenerator_GeneratePawn_Yautja_Patch
    {
        public static void Postfix(PawnGenerationRequest request, ref Pawn __result)
        {
            if (__result.isYautja(out Comp_Yautja yautja))
            {
                if (__result.Markable(out Comp_Markable markable))
                {

                    Backstory pawnStoryC = __result.story.childhood;
                    Backstory pawnStoryA = __result.story.adulthood ?? null;

                    bool hasunblooded = __result.health.hediffSet.HasHediff(markable.Props.cultureDef.UnbloodedHediff);
                    bool hasbloodedUM = __result.health.hediffSet.HasHediff(markable.Props.cultureDef.UnmarkedHediff);
                    bool hasbloodedM = __result.health.hediffSet.hediffs.Any<Hediff>(x => x.def.defName.StartsWith(markable.Props.cultureDef.MarkedHediff.defName));

                    if (!hasunblooded && !hasbloodedUM && !hasbloodedM)
                    {
                        HediffDef hediffDef;
                        if (pawnStoryA != null)
                        {
                            if (pawnStoryA != AvPExtensions.bsDefUnblooded.backstory && __result.kindDef.race == YautjaDefOf.AvP_Alien_Yautja)
                            {
                                Rand.PushState();
                                int hdi = Rand.RangeInclusive(0, yautja.Props.bloodedDefs.Count-1);
                                Rand.PopState();
                                hediffDef = yautja.Props.bloodedDefs[hdi];

                                if (hediffDef != null)
                                {
                                    /*
                                    PawnKindDef pawnKindDef = YautjaBloodedUtility.RandomMarked(hediffDef);
                                    if (_Yautja != null)
                                    {
                                        _Yautja.MarkHedifflabel = pawnKindDef.LabelCap;
                                        _Yautja.MarkedhediffDef = hediffDef;
                                        _Yautja.predator = pawnKindDef.RaceProps.predator;
                                        _Yautja.BodySize = pawnKindDef.RaceProps.baseBodySize;
                                        _Yautja.combatPower = pawnKindDef.combatPower;
                                        //    Log.Message(string.Format("{0}: {1}", hediffDef.stages[0].label, pawnKindDef.LabelCap));
                                    }
                                    */
                                }

                            }
                            else
                            {
                                hediffDef = markable.Props.cultureDef.UnbloodedHediff;
                            }
                        }
                        else
                        {
                            hediffDef = markable.Props.cultureDef.UnbloodedHediff;
                        }
                        foreach (var item in __result.RaceProps.body.AllParts)
                        {
                            if (item.def == BodyPartDefOf.Head)
                            {

                                __result.health.AddHediff(hediffDef, item);
                            }
                        }
                    }
                    else
                    {
                        //    Log.Message(string.Format("new pawn has hasunblooded:{0}, hasbloodedUM:{1}, hasbloodedM:{2}", hasunblooded, hasbloodedUM, hasbloodedM));
                    }
                }
                if (request.Faction.leader == null && request.Faction != Faction.OfPlayerSilentFail && request.KindDef.race == YautjaDefOf.AvP_Alien_Yautja)
                {
                    Rand.PushState();
                    bool upgradeWeapon = Rand.Chance(0.5f);
                    Rand.PopState();
                    if (__result.equipment.Primary != null && upgradeWeapon)
                    {
                        __result.equipment.Primary.TryGetQuality(out QualityCategory weaponQuality);
                        if (weaponQuality != QualityCategory.Legendary)
                        {
                            Thing Weapon = __result.equipment.Primary;
                            CompQuality Weapon_Quality = Weapon.TryGetComp<CompQuality>();
                            if (Weapon_Quality != null)
                            {
                                Weapon_Quality.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                            }
                        }

                    }
                    else if (__result.apparel.WornApparelCount > 0 && !upgradeWeapon)
                    {
                        foreach (var item in __result.apparel.WornApparel)
                        {
                            item.TryGetQuality(out QualityCategory gearQuality);
                            float upgradeChance = 0.5f;
                            Rand.PushState();
                            bool upgradeGear = Rand.Chance(upgradeChance);
                            Rand.PopState();
                            if (gearQuality != QualityCategory.Legendary)
                            {
                                CompQuality Gear_Quality = item.TryGetComp<CompQuality>();
                                if (Gear_Quality != null)
                                {
                                    if (upgradeGear)
                                    {
                                        Gear_Quality.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            
        }
    }

}