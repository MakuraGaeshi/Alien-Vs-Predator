using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using RRYautja.settings;
using RRYautja.ExtensionMethods;
using AlienRace;

namespace RRYautja
{

    [HarmonyPatch(typeof(AlienRace.HarmonyPatches), "ThoughtsFromIngestingPostfix")]
    public static class AvP_AlienRace_HarmonyPatches_ThoughtsFromIngestingPostfix_Patch
    {
        [HarmonyPrefix]
        public static bool ThoughtsFromIngestingPostPrefix(Pawn ingester, Thing foodSource)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "ThoughtsFromIngesting")]
    public static class AvP_FoodUtility_ThoughtsFromIngesting_Patch
    {
        [HarmonyPostfix]
        public static void ThoughtsFromIngestingPostfix(Pawn ingester, Thing foodSource, ref List<ThoughtDef> __result)
        {
            if (ingester.story.traits.HasTrait(tDef: AlienDefOf.Xenophobia) && ingester.story.traits.DegreeOfTrait(tDef: AlienDefOf.Xenophobia) == 1)
                if (__result.Contains(item: ThoughtDefOf.AteHumanlikeMeatDirect) && foodSource.def.ingestible.sourceDef != ingester.def)
                    __result.Remove(item: ThoughtDefOf.AteHumanlikeMeatDirect);
                else if (__result.Contains(item: ThoughtDefOf.AteHumanlikeMeatAsIngredient) &&
                         (foodSource.TryGetComp<CompIngredients>()?.ingredients.Any(predicate: td => FoodUtility.IsHumanlikeMeat(def: td) && td.ingestible.sourceDef != ingester.def) ?? false))
                    __result.Remove(item: ThoughtDefOf.AteHumanlikeMeatAsIngredient);
            if (!(ingester.def is ThingDef_AlienRace alienProps)) return;

            bool cannibal = ingester.story.traits.HasTrait(tDef: TraitDefOf.Cannibal);

            for (int i = 0; i < __result.Count; i++)
            {
                ThoughtDef thoughtDef = __result[index: i];
                ThoughtSettings settings = alienProps.alienRace.thoughtSettings;

                thoughtDef = settings.ReplaceIfApplicable(def: thoughtDef);

                if (thoughtDef == ThoughtDefOf.AteHumanlikeMeatDirect || thoughtDef == ThoughtDefOf.AteHumanlikeMeatDirectCannibal)
                    thoughtDef = settings.GetAteThought(race: foodSource.def.ingestible.sourceDef, cannibal: cannibal, ingredient: false);

                if (thoughtDef == ThoughtDefOf.AteHumanlikeMeatAsIngredient || thoughtDef == ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal)
                {
                    ThingDef race = null;
                    if (foodSource.TryGetComp<CompIngredients>() != null && foodSource.TryGetComp<CompIngredients>() is CompIngredients ingeds)
                    {
                        foreach (var item in ingeds.ingredients)
                        {
                            if (item.ingestible.sourceDef != null && item.ingestible.sourceDef.race != null && item.ingestible.sourceDef.race.Humanlike) race = (item.ingestible.sourceDef);
                        }
                    }
                    
                    if (race != null)
                        thoughtDef = settings.GetAteThought(race: race, cannibal: cannibal, ingredient: true);
                }

                __result[index: i] = thoughtDef;
            }
        }
    }

}