﻿using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001EA RID: 490
    public class ThinkNode_ConditionalAnyInfectableSpawnedNearby : ThinkNode_Conditional
    {
        // Token: 0x060009B3 RID: 2483 RVA: 0x0004DFD8 File Offset: 0x0004C3D8
        protected override bool Satisfied(Pawn pawn)
        {
            bool result;
            if (pawn.Spawned && XenomorphUtil.IsXenomorph(pawn))
            {
                bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) : false;
                if (!XenomorphUtil.EggsPresent(pawn.Map))
                {
                    if (selected) Log.Message(string.Format("pawn.Spawned && XenomorphUtil.IsXenomorph(pawn) && !XenomorphUtil.EggsPresent(pawn.Map)"));
                    List<Pawn> list = pawn.Map.mapPawns.AllPawns.Where((Pawn x) => !x.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) && !x.Downed && XenomorphUtil.isInfectablePawn(x) && (!x.InBed() || (x.InBed() && !(x.CurrentBed() is Building_XenomorphCocoon))) && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.NoPassClosedDoors)).ToList();
                    if (selected) Log.Message(string.Format("found {0}", list.Count));
                    result = list.Any<Pawn>(x => x.Spawned);
                }
                else
                {
                    if (selected) Log.Message(string.Format("pawn.Spawned && XenomorphUtil.IsXenomorph(pawn) && XenomorphUtil.EggsPresent(pawn.Map)"));
                    List<Pawn> list = pawn.Map.mapPawns.AllPawns.Where((Pawn x) => !x.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned) && !x.Downed && XenomorphUtil.isInfectablePawn(x) && (!x.InBed() || (x.InBed() && !(x.CurrentBed() is Building_XenomorphCocoon))) && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.NoPassClosedDoors)).ToList();
                    result = list.Any<Pawn>(x => x.Spawned && XenomorphUtil.DistanceBetween(XenomorphUtil.ClosestReachableEgg(x).Position, x.Position) > 3f);
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

    }
}