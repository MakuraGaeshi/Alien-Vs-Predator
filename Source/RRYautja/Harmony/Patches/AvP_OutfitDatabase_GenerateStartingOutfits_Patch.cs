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

namespace AvP.HarmonyInstance
{
    
    [HarmonyPatch(typeof(OutfitDatabase), "GenerateStartingOutfits")]
    public static class AvP_OutfitDatabase_GenerateStartingOutfits_Patch
    {
        public static void Postfix(OutfitDatabase __instance)
		{
			__instance.MakeNewOutfit().label = "AvP_Yautja_OutfitAnything".Translate();
			Outfit outfit = __instance.MakeNewOutfit();
			outfit.label = "AvP_Yautja_OutfitWorker".Translate();
			outfit.filter.SetDisallowAll(null, null);
			outfit.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (thingDef.apparel != null && thingDef.apparel.defaultOutfitTags != null && thingDef.apparel.defaultOutfitTags.Contains("Worker"))
				{
					if (AlienRace.RaceRestrictionSettings.apparelRestrictionDict.ContainsKey(key: thingDef))
					{
						if (!AlienRace.RaceRestrictionSettings.apparelRestrictionDict.TryGetValue(key: thingDef).NullOrEmpty())
						{
							if (AlienRace.RaceRestrictionSettings.apparelRestrictionDict.TryGetValue(key: thingDef).Contains(YautjaDefOf.AvP_Alien_Yautja))
							{
								outfit.filter.SetAllow(thingDef, true);
							}
						}
					}
					if (AlienRace.RaceRestrictionSettings.apparelWhiteDict.ContainsKey(key: thingDef))
					{
						if (!AlienRace.RaceRestrictionSettings.apparelWhiteDict.TryGetValue(key: thingDef).NullOrEmpty())
						{
							if (AlienRace.RaceRestrictionSettings.apparelWhiteDict.TryGetValue(key: thingDef).Contains(YautjaDefOf.AvP_Alien_Yautja))
							{
								outfit.filter.SetAllow(thingDef, true);
							}
						}
					}
				}
			}
			Outfit outfit2 = __instance.MakeNewOutfit();
			outfit2.label = "AvP_Yautja_OutfitSoldier".Translate();
			outfit2.filter.SetDisallowAll(null, null);
			outfit2.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
			foreach (ThingDef thingDef2 in DefDatabase<ThingDef>.AllDefs)
			{
				if (thingDef2.apparel != null && thingDef2.apparel.defaultOutfitTags != null && thingDef2.apparel.defaultOutfitTags.Contains("Soldier"))
				{
					if (AlienRace.RaceRestrictionSettings.apparelRestrictionDict.ContainsKey(key: thingDef2))
					{
						if (!AlienRace.RaceRestrictionSettings.apparelRestrictionDict.TryGetValue(key: thingDef2).NullOrEmpty())
						{
							if (AlienRace.RaceRestrictionSettings.apparelRestrictionDict.TryGetValue(key: thingDef2).Contains(YautjaDefOf.AvP_Alien_Yautja))
							{
								outfit2.filter.SetAllow(thingDef2, true);
							}
						}
					}
					if (AlienRace.RaceRestrictionSettings.apparelWhiteDict.ContainsKey(key: thingDef2))
					{
						if (!AlienRace.RaceRestrictionSettings.apparelWhiteDict.TryGetValue(key: thingDef2).NullOrEmpty())
						{
							if (AlienRace.RaceRestrictionSettings.apparelWhiteDict.TryGetValue(key: thingDef2).Contains(YautjaDefOf.AvP_Alien_Yautja))
							{
								outfit2.filter.SetAllow(thingDef2, true);
							}
						}
					}
				}
			}
			Outfit outfit3 = __instance.MakeNewOutfit();
			outfit3.label = "AvP_Yautja_OutfitNudist".Translate();
			outfit3.filter.SetDisallowAll(null, null);
			outfit3.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
			foreach (ThingDef thingDef3 in DefDatabase<ThingDef>.AllDefs)
			{
				if (thingDef3.apparel != null && !thingDef3.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) && !thingDef3.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
				{
					if (AlienRace.RaceRestrictionSettings.apparelRestrictionDict.ContainsKey(key: thingDef3))
					{
						if (!AlienRace.RaceRestrictionSettings.apparelRestrictionDict.TryGetValue(key: thingDef3).NullOrEmpty())
						{
							if (AlienRace.RaceRestrictionSettings.apparelRestrictionDict.TryGetValue(key: thingDef3).Contains(YautjaDefOf.AvP_Alien_Yautja))
							{
								outfit3.filter.SetAllow(thingDef3, true);
							}
						}
					}
					if (AlienRace.RaceRestrictionSettings.apparelWhiteDict.ContainsKey(key: thingDef3))
					{
						if (!AlienRace.RaceRestrictionSettings.apparelWhiteDict.TryGetValue(key: thingDef3).NullOrEmpty())
						{
							if (AlienRace.RaceRestrictionSettings.apparelWhiteDict.TryGetValue(key: thingDef3).Contains(YautjaDefOf.AvP_Alien_Yautja))
							{
								outfit3.filter.SetAllow(thingDef3, true);
							}
						}
					}
				}
			}

			Outfit outfit4 = __instance.MakeNewOutfit();
			outfit4.label = "AvP_USCM_OutfitSmartgunner".Translate();
			outfit4.filter.SetDisallowAll(null, null);
			outfit4.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
			foreach (ThingDef thingDef4 in DefDatabase<ThingDef>.AllDefs)
			{
				if (thingDef4.IsApparel)
				{
					if ((ApparelUtility.CanWearTogether(USCMDefOf.AvP_USCM_Armour_M56CombatHarness, thingDef4, BodyDefOf.Human) && ApparelUtility.CanWearTogether(USCMDefOf.AvP_USCM_Equipment_HeadMountedSight, thingDef4, BodyDefOf.Human)) || USCMDefOf.AvP_USCM_Armour_M56CombatHarness == thingDef4 || USCMDefOf.AvP_USCM_Equipment_HeadMountedSight == thingDef4)
					{
						if (thingDef4.apparel != null && thingDef4.apparel.defaultOutfitTags != null && (thingDef4.apparel.defaultOutfitTags.Contains("SmartGunOperator") || thingDef4.apparel.defaultOutfitTags.Contains("Soldier")))
						{
							outfit4.filter.SetAllow(thingDef4, true);
						}
					}
				}
				
			}

		}
    }
    
}