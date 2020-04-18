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
using RRYautja.settings;
using RRYautja.ExtensionMethods;

namespace RRYautja.HarmonyInstance
{
	// Disallows stripping of the Wristblade
	[HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_LostBodyPart")]
	public static class AvP_Pawn_ApparelTracker_Notify_LostBodyPart_Patch
	{
		[HarmonyPostfix]
		public static void Notify_LostBodyPart_Postfix(Pawn_ApparelTracker __instance)
		{
			if (__instance == null || __instance.WornApparel == null || __instance.WornApparel.Count == 0 || __instance.pawn==null || !__instance.pawn.RaceProps.Humanlike || (__instance.pawn.Map == null && __instance.pawn.MapHeld == null))
			{
				return;
			}
			AvP_Pawn_ApparelTracker_Notify_LostBodyPart_Patch.tmpApparel.Clear();
			for (int i = 0; i < __instance.pawn.apparel.WornApparel.Count; i++)
			{
				AvP_Pawn_ApparelTracker_Notify_LostBodyPart_Patch.tmpApparel.Add(__instance.pawn.apparel.WornApparel[i]);
			}
			for (int j = 0; j < AvP_Pawn_ApparelTracker_Notify_LostBodyPart_Patch.tmpApparel.Count; j++)
			{
				Apparel apparel = AvP_Pawn_ApparelTracker_Notify_LostBodyPart_Patch.tmpApparel[j];
				CompHediffApparel hdApp = apparel.TryGetComp<CompHediffApparel>();
				if (!HasPartsToWear(__instance.pawn, apparel))
				{
					//	Log.Message(string.Format("{0} no longer has parts for {1}", __instance.pawn, apparel));
					Rand.PushState();
					apparel.HitPoints -= Rand.RangeInclusive(0, apparel.MaxHitPoints);
					Rand.PopState();
					if (apparel.HitPoints > 0) 
						__instance.TryDrop(apparel); 
					else 
						__instance.Remove(apparel);
				}
			}
		}
		public static bool HasPartsToWear(Pawn p, Apparel apparel)
		{
			List<Hediff> hediffs = p.health.hediffSet.hediffs;
			bool flag = false;
			for (int j = 0; j < hediffs.Count; j++)
			{
				if (hediffs[j] is Hediff_MissingPart)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return true;
			}
			IEnumerable<BodyPartRecord> notMissingParts = p.health.hediffSet.GetNotMissingParts();
			CompHediffApparel hdApp = apparel.TryGetComp<CompHediffApparel>();
			List<BodyPartGroupDef> groups = new List<BodyPartGroupDef>();
			foreach (BodyPartRecord rec in hdApp.MyGetPartsToAffect(p))
			{
				groups.AddRange(rec.groups);
			}
			int i;
			for (i = 0; i < groups.Count; i++)
			{
				if (notMissingParts.Any((BodyPartRecord x) => x.IsInGroup(groups[i])))
				{
					return true;
				}
			}
			return false;
		}
		private static List<Apparel> tmpApparel = new List<Apparel>();
	}
}
