using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000E4 RID: 228
    public class JobGiver_GetFoodXenomorph : ThinkNode_JobGiver
    {
        // Token: 0x060004FB RID: 1275 RVA: 0x00032140 File Offset: 0x00030540
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_GetFoodXenomorph jobGiver_GetFood = (JobGiver_GetFoodXenomorph)base.DeepCopy(resolve);
            jobGiver_GetFood.minCategory = this.minCategory;
            jobGiver_GetFood.forceScanWholeMap = this.forceScanWholeMap;
            return jobGiver_GetFood;
        }

        // Token: 0x060004FC RID: 1276 RVA: 0x00032174 File Offset: 0x00030574
        public override float GetPriority(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food == null)
            {
                return 0f;
            }
            if (pawn.needs.food.CurCategory < HungerCategory.Starving && FoodUtility.ShouldBeFedBySomeone(pawn))
            {
                return 0f;
            }
            if (food.CurCategory < this.minCategory)
            {
                return 0f;
            }
            if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
            {
                return 9.5f;
            }
            return 0f;
        }

        // Token: 0x060004FD RID: 1277 RVA: 0x000321F8 File Offset: 0x000305F8
        protected override Job TryGiveJob(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food == null || food.CurCategory < this.minCategory)
            {
                return null;
            }
            bool flag;
            if (pawn.AnimalOrWildMan())
            {
                flag = true;
            }
            else
            {
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
                flag = (firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f);
            }
        //    bool flag2 = pawn.needs.food.CurCategory == HungerCategory.Starving;
            bool flag2 = true;
            bool desperate = flag2;
            Thing thing = null;
            ref Thing foodSource = ref thing;
            ThingDef thingDef = null;
            ref ThingDef foodDef = ref thingDef;
            bool canRefillDispenser = true;
            bool canUseInventory = true;
            bool allowCorpse;
            if (pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_FaceHugger)
            { allowCorpse = false; }
            else  allowCorpse = true; 
            bool flag3 = this.forceScanWholeMap;
            if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out foodSource, out foodDef, canRefillDispenser, canUseInventory, false, allowCorpse, false, pawn.IsWildMan(), flag3))
            {
                return null;
            }
            Pawn pawn2 = thing as Pawn;
            if (pawn2 != null)
            {
                if (pawn.kindDef==XenomorphDefOf.RRY_Xenomorph_FaceHugger)
                {
                    return new Job(XenomorphDefOf.PredatorHuntXenomorph, pawn2)
                    {
                        killIncappedTarget = false
                    };
                }
                else
                return new Job(XenomorphDefOf.PredatorHuntXenomorph, pawn2)
                {   
                    killIncappedTarget = true
                };
            }
            if (thing is Plant && thing.def.plant.harvestedThingDef == thingDef)
            {
                return new Job(JobDefOf.Harvest, thing);
            }
            Building_NutrientPasteDispenser building_NutrientPasteDispenser = thing as Building_NutrientPasteDispenser;
            if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
            {
                Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
                if (building != null)
                {
                    ISlotGroupParent hopperSgp = building as ISlotGroupParent;
                    Job job = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                    if (job != null)
                    {
                        return job;
                    }
                }
                thing = FoodUtility.BestFoodSourceOnMap(pawn, pawn, flag2, out thingDef, FoodPreferability.MealLavish, false, !pawn.IsTeetotaler(), false, false, false, false, false, false, this.forceScanWholeMap);
                if (thing == null)
                {
                    return null;
                }
            }
            float nutrition = FoodUtility.GetNutrition(thing, thingDef);
            return new Job(JobDefOf.Ingest, thing)
            {
                count = FoodUtility.WillIngestStackCountOf(pawn, thingDef, nutrition)
            };
        }

        // RimWorld.FoodUtility
        private static Pawn BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
        {
            if (predator.meleeVerbs.TryGetMeleeVerb(null) == null)
            {
                return null;
            }
            bool flag = false;
            float summaryHealthPercent = predator.health.summaryHealth.SummaryHealthPercent;
            if (summaryHealthPercent < 0.25f)
            {
                flag = true;
            }
            tmpPredatorCandidates.Clear();
            int maxRegionsToScan = GetMaxRegionsToScan(predator, forceScanWholeMap);
            if (maxRegionsToScan < 0)
            {
                tmpPredatorCandidates.AddRange(predator.Map.mapPawns.AllPawnsSpawned);
            }
            else
            {
                TraverseParms traverseParms = TraverseParms.For(predator, Danger.Deadly, TraverseMode.ByPawn, false);
                RegionTraverser.BreadthFirstTraverse(predator.Position, predator.Map, (Region from, Region to) => to.Allows(traverseParms, true), delegate (Region x)
                {
                    List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                    for (int j = 0; j < list.Count; j++)
                    {
                        tmpPredatorCandidates.Add((Pawn)list[j]);
                    }
                    return false;
                }, 999999, RegionType.Set_Passable);
            }
            Pawn pawn = null;
            float num = 0f;
            bool tutorialMode = TutorSystem.TutorialMode;
            for (int i = 0; i < tmpPredatorCandidates.Count; i++)
            {
                Pawn pawn2 = tmpPredatorCandidates[i];
                if (predator.GetRoom(RegionType.Set_Passable) == pawn2.GetRoom(RegionType.Set_Passable))
                {
                    if (predator != pawn2)
                    {
                        if (!flag || pawn2.Downed)
                        {
                            if (FoodUtility.IsAcceptablePreyFor(predator, pawn2))
                            {
                                if (predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
                                {
                                    if (!pawn2.IsForbidden(predator))
                                    {
                                        if (!tutorialMode || pawn2.Faction != Faction.OfPlayer)
                                        {
                                            float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2);
                                            if (preyScoreFor > num || pawn == null)
                                            {
                                                num = preyScoreFor;
                                                pawn = pawn2;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            tmpPredatorCandidates.Clear();
            return pawn;
        }

        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey)
        {
            if (!prey.RaceProps.canBePredatorPrey)
            {
                return false;
            }
            if (!prey.RaceProps.IsFlesh)
            {
                return false;
            }
            if (!Find.Storyteller.difficulty.predatorsHuntHumanlikes && prey.RaceProps.Humanlike)
            {
                return false;
            }
            if (prey.BodySize > predator.RaceProps.maxPreyBodySize)
            {
                return false;
            }
            if (!prey.Downed)
            {
                if (prey.kindDef.combatPower > 2f * predator.kindDef.combatPower)
                {
                    return false;
                }
                float num = prey.kindDef.combatPower * prey.health.summaryHealth.SummaryHealthPercent * prey.ageTracker.CurLifeStage.bodySizeFactor;
                float num2 = predator.kindDef.combatPower * predator.health.summaryHealth.SummaryHealthPercent * predator.ageTracker.CurLifeStage.bodySizeFactor;
                if (num >= num2)
                {
                    return false;
                }
            }
            return (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) && (predator.Faction == null || prey.HostFaction == null || predator.HostileTo(prey)) && (predator.Faction != Faction.OfPlayer || prey.Faction != Faction.OfPlayer) && (!predator.RaceProps.herdAnimal || predator.def != prey.def);
        }

        private static int GetMaxRegionsToScan(Pawn getter, bool forceScanWholeMap)
        {
            if (getter.RaceProps.Humanlike)
            {
                return -1;
            }
            if (forceScanWholeMap)
            {
                return -1;
            }
            if (getter.Faction == Faction.OfPlayer)
            {
                return 100;
            }
            return 30;
        }

        private static List<Pawn> tmpPredatorCandidates = new List<Pawn>();

        // Token: 0x040002BD RID: 701
        private HungerCategory minCategory;

        // Token: 0x040002BE RID: 702
        public bool forceScanWholeMap;
    }
}
