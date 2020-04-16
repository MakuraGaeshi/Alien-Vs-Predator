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
using AlienRace;

namespace RRYautja.HarmonyInstance
{
    [HarmonyPatch(typeof(RecordWorker_TimeInBedForMedicalReasons), "ShouldMeasureTimeNow")]
    public static class AvP_RecordWorker_TimeInBedForMedicalReasons_ShouldMeasureTimeNow_Synth_Patch
    {
        [HarmonyPrefix]
		public static bool CompatPatch_ShouldMeasureTimeNow(bool __result, ref Pawn pawn)
		{
			bool flag = pawn == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Pawn pawn2 = pawn;
				bool flag2 = ((pawn2 != null) ? pawn2.needs.TryGetNeed(NeedDefOf.Rest) : null) != null;
				bool flag3 = !flag2;
				if (flag3)
				{
					if (pawn.InBed())
					{
						if (!HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn))
						{
							bool flag4 = HealthAIUtility.ShouldSeekMedicalRest(pawn) && pawn.CurJob.restUntilHealed;
						}
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}
    }
    
}