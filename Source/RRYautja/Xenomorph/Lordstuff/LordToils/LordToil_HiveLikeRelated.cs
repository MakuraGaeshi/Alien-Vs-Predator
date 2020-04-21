﻿using AvP;
using System;
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
		public LordToil_HiveLikeRelatedData Data
		{
			get
			{
				return (LordToil_HiveLikeRelatedData)this.data;
			}
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00047560 File Offset: 0x00045960
		protected void FilterOutUnspawnedHiveLikes()
		{
			this.Data.assignedHiveLikes.RemoveAll((KeyValuePair<Pawn, XenomorphHive> x) => x.Value == null || !x.Value.Spawned);
		}

        // Token: 0x0600087F RID: 2175 RVA: 0x00047590 File Offset: 0x00045990
        protected XenomorphHive GetHiveLikeFor(Pawn pawn)
        {
            if (this.Data.assignedHiveLikes.TryGetValue(pawn, out XenomorphHive hive))
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
        private XenomorphHive FindClosestHiveLike(Pawn pawn)
		{
            return (XenomorphHive)XenomorphUtil.ClosestReachableHivelike(pawn);
        }
	}
}
