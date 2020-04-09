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

namespace RRYautja
{
    
    // FilthMaker.TryMakeFilth
    [HarmonyPatch(typeof(Pawn_HealthTracker), "DropBloodFilth")]
    public static class AvP_Pawn_HealthTracker_DropBloodFilth_Xenoblood_Patch
    {
        [HarmonyPrefix]
        public static bool Patch_Pawn_HealthTracker_DropBloodFilth(Pawn_HealthTracker __instance)
        {
            Pawn pawn = Main.Pawn_HealthTracker_GetPawn(__instance);
            bool flag = pawn.isXenomorph() && (pawn.Spawned || pawn.ParentHolder is Pawn_CarryTracker) && pawn.SpawnedOrAnyParentSpawned && pawn.RaceProps.BloodDef != null;
            bool result;
            if (flag)
            {
				AvP_Pawn_HealthTracker_DropBloodFilth_Xenoblood_Patch.TryMakeFilth(pawn.PositionHeld, pawn.MapHeld, XenomorphDefOf.RRY_FilthBloodXenomorph_Active, pawn.LabelIndefinite(), 1);
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

		// Token: 0x06004C56 RID: 19542 RVA: 0x00198798 File Offset: 0x00196998
		public static bool TryMakeFilth(IntVec3 c, Map map, ThingDef filthDef, string source, int count = 1, FilthSourceFlags additionalFlags = FilthSourceFlags.None)
		{
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				flag |= AvP_Pawn_HealthTracker_DropBloodFilth_Xenoblood_Patch.TryMakeFilth(c, map, filthDef, Gen.YieldSingle<string>(source), true, additionalFlags);
			}
			return flag;
		}
		// Token: 0x06004C58 RID: 19544 RVA: 0x001987D8 File Offset: 0x001969D8
		private static bool TryMakeFilth(IntVec3 c, Map map, ThingDef filthDef, IEnumerable<string> sources, bool shouldPropagate, FilthSourceFlags additionalFlags = FilthSourceFlags.None)
		{
			Filth filth = (Filth)(from t in c.GetThingList(map)
								  where t.def == filthDef
								  select t).FirstOrDefault<Thing>();
			if (!c.Walkable(map) || (filth != null && !filth.CanBeThickened))
			{
				if (shouldPropagate)
				{
					List<IntVec3> list = GenAdj.AdjacentCells8WayRandomized();
					for (int i = 0; i < 8; i++)
					{
						IntVec3 c2 = c + list[i];
						if (c2.InBounds(map) && AvP_Pawn_HealthTracker_DropBloodFilth_Xenoblood_Patch.TryMakeFilth(c2, map, filthDef, sources, false, FilthSourceFlags.None))
						{
							return true;
						}
					}
				}
				if (filth != null)
				{
					filth.AddSources(sources);
				}
				return false;
			}
			if (filth != null)
			{
				filth.ThickenFilth();
				filth.AddSources(sources);
			}
			else
			{
				if (!FilthMaker.CanMakeFilth(c, map, filthDef, additionalFlags))
				{
					return false;
				}
				Filth filth2 = (Filth)ThingMaker.MakeThing(filthDef, null);
				filth2.AddSources(sources);
				Filth_AddAcidDamage filth_ = filth2 as Filth_AddAcidDamage;
				filth_.destroyTick = 600; 
				GenSpawn.Spawn(filth2, c, map, WipeMode.Vanish);
			}
		//	FilthMonitor.Notify_FilthSpawned();
			return true;
		}

	}

}