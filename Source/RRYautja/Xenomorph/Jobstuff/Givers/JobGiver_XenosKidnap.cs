using RRYautja;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x020000D6 RID: 214
    public class JobGiver_XenosKidnap : ThinkNode_JobGiver
    {
        private List<Rot4> Rotlist = new List<Rot4>
        {
            Rot4.North,
            Rot4.South,
            Rot4.East,
            Rot4.West
        };

        // Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_XenosKidnap jobGiver_XenosKidnap = (JobGiver_XenosKidnap)base.DeepCopy(resolve);
            jobGiver_XenosKidnap.HuntingRange = this.HuntingRange;
            return jobGiver_XenosKidnap;
        }


        private float HuntingRange = 9999f;

        // Token: 0x060004D1 RID: 1233 RVA: 0x0003100C File Offset: 0x0002F40C
        protected override Job TryGiveJob(Pawn pawn)
        {
            float Searchradius = HuntingRange;
            IntVec3 c = IntVec3.Invalid;
            if (XenomorphKidnapUtility.TryFindGoodKidnapVictim(pawn, Searchradius, out Pawn t, null) && !GenAI.InDangerousCombat(pawn))
            {
                if (XenomorphKidnapUtility.TryFindGoodHiveLoc(pawn, t, out c))
                {
                    ThingDef namedA = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;
                    ThingDef namedB = XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
                    bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) && (Prefs.DevMode) : false;
                    if (c != IntVec3.Invalid && t != null && pawn.CanReach(c, PathEndMode.ClosestTouch, Danger.Deadly, true, TraverseMode.PassAllDestroyableThings))
                    {
                        Predicate<IntVec3> validator = delegate (IntVec3 y)
                        {
                            bool adjacent = c.AdjacentTo8WayOrInside(y);
                            bool filled = y.Filled(pawn.Map);
                            bool edifice = y.GetEdifice(pawn.Map).DestroyedOrNull();
                            bool building = y.GetFirstBuilding(pawn.Map).DestroyedOrNull();
                            bool thingA = y.GetFirstThing(pawn.Map, namedA).DestroyedOrNull();
                            bool thingB = y.GetFirstThing(pawn.Map, namedB).DestroyedOrNull();
                            //Log.Message(string.Format("{0} {1} {2} {3} {4} {5}", y, !adjacent, !filled, edifice, building, thing));
                            return !adjacent && !filled && edifice && building && thingA && thingB;
                        };

                        bool b = RCellFinder.TryFindRandomCellNearWith(c, validator, pawn.Map, out IntVec3 lc,6,12);
                        
                        return new Job(XenomorphDefOf.RRY_Job_XenomorphKidnap)
                        {
                            targetA = t,
                            targetB = lc,
                            count = 1
                        };
                    }
                    else
                    {
                        if (Find.Selector.SelectedObjects.Contains(pawn)) Log.Message(string.Format("{0} something went wrong", this));
                    }
                }
                else
                {
                    if (Find.Selector.SelectedObjects.Contains(pawn)) Log.Message(string.Format("{0} No Cocooning spot Found", this));
                }
            }
            else
            {
                if (Find.Selector.SelectedObjects.Contains(pawn)) Log.Message(string.Format("{0} No Victim Found", this));
            }
            return null;
        }
        
        // Token: 0x0600000B RID: 11 RVA: 0x00002C9C File Offset: 0x00000E9C
        private static bool IsMapRectClear(CellRect mapRect, Map map)
        {
            foreach (IntVec3 intVec in mapRect)
            {
                bool flag = !map.pathGrid.WalkableFast(intVec);
                if (flag)
                {
                    return false;
                }
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag2 = thingList[i].def.category == (ThingCategory)3 || thingList[i].def.category == (ThingCategory)1 || thingList[i].def.category == (ThingCategory)10;
                    if (flag2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        // Token: 0x040002AB RID: 683
        public const float VictimSearchRadiusInitial = 8f;

        // Token: 0x040002AC RID: 684
        private const float VictimSearchRadiusOngoing = 18f;
    }
}
