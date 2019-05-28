﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x02000195 RID: 405
	public abstract class LordToil_HiveLikeRelated : LordToil
	{
		// Token: 0x0600087C RID: 2172 RVA: 0x00047540 File Offset: 0x00045940
		public LordToil_HiveLikeRelated()
		{
			this.data = new LordToil_HiveLikeRelatedData();
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600087D RID: 2173 RVA: 0x00047553 File Offset: 0x00045953
		private LordToil_HiveLikeRelatedData Data
		{
			get
			{
				return (LordToil_HiveLikeRelatedData)this.data;
			}
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00047560 File Offset: 0x00045960
		protected void FilterOutUnspawnedHiveLikes()
		{
			this.Data.assignedHiveLikes.RemoveAll((KeyValuePair<Pawn, HiveLike> x) => x.Value == null || !x.Value.Spawned);
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x00047590 File Offset: 0x00045990
		protected HiveLike GetHiveLikeFor(Pawn pawn)
		{
            if (this.Data.assignedHiveLikes.TryGetValue(pawn, out HiveLike hive))
            {
                return hive;
            }
            hive = this.FindClosestHiveLike(pawn);
			if (hive != null)
			{
				this.Data.assignedHiveLikes.Add(pawn, hive);
			}
			return hive;
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x000475D8 File Offset: 0x000459D8
		private HiveLike FindClosestHiveLike(Pawn pawn)
		{
            ThingDef hiveDef = null;
            List<ThingDef_HiveLike> hivedefs = DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == pawn.Faction.def);
            foreach (ThingDef_HiveLike hivedef in hivedefs)
            {
            //    Log.Message(string.Format("LordToil_HiveLikeRelated found hiveDef: {0} for {1}", hiveDef, pawn));
                if (hivedef.Faction == pawn.Faction.def)
                {
                    hiveDef = hivedef;
                //    Log.Message(string.Format("LordToil_HiveLikeRelated set hiveDef: {0} for {1}", hiveDef, pawn));
                    break;
                }
            }
			return (HiveLike)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(hiveDef), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 30f, (Thing x) => x.Faction == pawn.Faction, null, 0, 30, false, RegionType.Set_Passable, false);
		}
	}
}
