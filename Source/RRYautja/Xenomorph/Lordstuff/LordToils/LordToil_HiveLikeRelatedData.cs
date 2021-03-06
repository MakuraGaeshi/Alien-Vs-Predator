﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x02000196 RID: 406
	public class LordToil_HiveLikeRelatedData : LordToilData
	{
		// Token: 0x06000883 RID: 2179 RVA: 0x000482A8 File Offset: 0x000466A8
		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.assignedHiveLikes.RemoveAll((KeyValuePair<Pawn, HiveLike> x) => x.Key.Destroyed);
			}
			Scribe_Collections.Look<Pawn, HiveLike>(ref this.assignedHiveLikes, "assignedHives", LookMode.Reference, LookMode.Reference);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.assignedHiveLikes.RemoveAll((KeyValuePair<Pawn, HiveLike> x) => x.Value == null);
            }
            Scribe_Values.Look(ref HiveLoc, "HiveLoc");
            Scribe_References.Look(ref HiveQueen, "HiveQueen");
            Scribe_TargetInfo.Look(ref HiveFocus, "HiveFocus");
        }
        
		public Dictionary<Pawn, HiveLike> assignedHiveLikes = new Dictionary<Pawn, HiveLike>();
        public LocalTargetInfo HiveFocus;
        public IntVec3 HiveLoc;
        public Pawn HiveQueen;
    }
}
