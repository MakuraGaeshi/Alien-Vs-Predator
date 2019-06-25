using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000E4 RID: 228
    public class JobGiver_XenoGetFood : ThinkNode_JobGiver
    {
        // Token: 0x060004FB RID: 1275 RVA: 0x00032140 File Offset: 0x00030540
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_XenoGetFood jobGiver_XenoGetFood = (JobGiver_XenoGetFood)base.DeepCopy(resolve);
            jobGiver_XenoGetFood.minCategory = this.minCategory;
            jobGiver_XenoGetFood.forceScanWholeMap = this.forceScanWholeMap;
            return jobGiver_XenoGetFood;
        }

        // Token: 0x060004FC RID: 1276 RVA: 0x00032174 File Offset: 0x00030574
        public override float GetPriority(Pawn pawn)
        {
            LifeStageDef stage = pawn.ageTracker.CurLifeStage;
            if (stage != pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def)
            {
                return 9.5f;
            }
            return 0f;
        }

        // Token: 0x060004FD RID: 1277 RVA: 0x000321F8 File Offset: 0x000305F8
        protected override Job TryGiveJob(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            LifeStageDef stage = pawn.ageTracker.CurLifeStage;
            if (stage == pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def)
            {
                return null;
            }
            bool flag;
            if (pawn.AnimalOrWildMan())
            {
                Log.Message(string.Format("XenoGetFood AnimalOrWildMan"));
                flag = true;
            }
            else
            {
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
                flag = (firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f);
            }
            bool flag2 = pawn.needs.food.CurCategory == HungerCategory.Starving;
            bool desperate = flag2;
            Thing thing;
            ThingDef thingDef;
            bool canRefillDispenser = false;
            bool canUseInventory = false;
            bool allowCorpse = flag;
            bool flag3 = this.forceScanWholeMap;
            if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out thing, out thingDef, canRefillDispenser, canUseInventory, false, allowCorpse, false, pawn.IsWildMan(), flag3))
            {
                return null;
            }
            float nutrition = FoodUtility.GetNutrition(thing, thingDef);
            return new Job(JobDefOf.Ingest, thing)
            {
                count = FoodUtility.WillIngestStackCountOf(pawn, thingDef, nutrition)
            };
        }

        // Token: 0x040002BD RID: 701
        private HungerCategory minCategory;

        // Token: 0x040002BE RID: 702
        public bool forceScanWholeMap;
    }
}
