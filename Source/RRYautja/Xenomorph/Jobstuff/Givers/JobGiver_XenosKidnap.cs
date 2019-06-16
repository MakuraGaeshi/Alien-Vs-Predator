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
                    bool selected = pawn.Map != null ? Find.Selector.SelectedObjects.Contains(pawn) && (Prefs.DevMode) : false;
                    //    if (selected) Log.Message(string.Format("TargetB == {0}", c));
                    if (c != IntVec3.Invalid && t != null)
                    {
                        return new Job(XenomorphDefOf.RRY_Job_XenomorphKidnap)
                        {
                            targetA = t,
                            targetB = c,
                            count = 1
                        };
                    }
                }
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

        // Token: 0x0600000C RID: 12 RVA: 0x00002D80 File Offset: 0x00000F80
        private static void ClearMapRect(CellRect mapRect, Map map)
        {
            foreach (IntVec3 intVec in mapRect)
            {
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    thingList[i].Destroy(0);
                }
            }
        }
        // Token: 0x0600000D RID: 13 RVA: 0x00002DF8 File Offset: 0x00000FF8
        private static Building_XenomorphCocoon TryMakeCocoon(CellRect mapRect, Map map, ThingDef thingDef)
        {
            mapRect.ClipInsideMap(map);
            CellRect cellRect;
            cellRect = new CellRect(mapRect.BottomLeft.x + 1, mapRect.TopRight.z + 1, 2, 1);
            cellRect.ClipInsideMap(map);
            IsMapRectClear(cellRect, map);
            foreach (IntVec3 intVec in cellRect)
            {
                List<Thing> thingList = GridsUtility.GetThingList(intVec, map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag = !thingList[i].def.destroyable;
                    if (flag)
                    {
                        return null;
                    }
                }
            }
            Building_XenomorphCocoon building_XenomorphCocoon = (Building_XenomorphCocoon)ThingMaker.MakeThing(thingDef, null);
            building_XenomorphCocoon.SetPositionDirect(cellRect.RandomCell);
            bool flag2 = Rand.Value < 0.5f;
            if (flag2)
            {
                flag2 = Rand.Value < 0.5f;
                if (flag2)
                {
                    building_XenomorphCocoon.Rotation = Rot4.West;
                }
                else
                {
                    building_XenomorphCocoon.Rotation = Rot4.East;
                }
            }
            else
            {
                flag2 = Rand.Value < 0.5f;
                if (flag2)
                {
                    building_XenomorphCocoon.Rotation = Rot4.South;
                }
                else
                {
                    building_XenomorphCocoon.Rotation = Rot4.North;
                }
            }
        //    Log.Message(string.Format("Rotation: {0}", building_XenomorphCocoon.Rotation));
            return building_XenomorphCocoon;
        }

        // Token: 0x040002AB RID: 683
        public const float VictimSearchRadiusInitial = 8f;

        // Token: 0x040002AC RID: 684
        private const float VictimSearchRadiusOngoing = 18f;
    }
}
