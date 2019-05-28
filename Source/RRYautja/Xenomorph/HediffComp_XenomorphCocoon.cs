using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_XenomorphCocoon : HediffCompProperties
    {
        public HediffCompProperties_XenomorphCocoon()
        {
            this.compClass = typeof(HediffComp_XenomorphCocoon);
        }


    }
    public class HediffComp_XenomorphCocoon : HediffComp
    {
        public HediffCompProperties_XenomorphCocoon XenoProps
        {
            get
            {
                return this.props as HediffCompProperties_XenomorphCocoon;
            }
        }

        public bool eggsPresent;
        public bool eggsReachable;
        public Thing closestReachableEgg;
        public Thing closestReachableCocoontoEgg;

        public bool cocoonsPresent;
        public bool cocoonsReachable;
        public bool cocoonOccupied;
        public Thing closestReachableCocoon;

        public bool hivelikesPresent;
        public bool hivelikesReachable;
        public Thing closestReachableHivelike;

        public Thing cocoonThing;
        public Thing eggThing;
        public Thing hiveThing;

        public int ticksSinceHeal;
        public int healIntervalTicks = 60;
        private float BloodlossSev = 0f;
        private float MalnutritionSev = 0f;
        private float NutritionNeed = 0f;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            BloodlossSev = Pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss) ? Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss).Severity : 0f;
            MalnutritionSev = Pawn.health.hediffSet.HasHediff(HediffDefOf.Malnutrition) ? Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition).Severity : 0f;
            NutritionNeed = Pawn.needs.food.ShowOnNeedList ? Pawn.needs.food.CurLevel : 0f;

            eggsPresent = XenomorphUtil.EggsPresent(Pawn.Map);
            eggsReachable = !XenomorphUtil.ClosestReachableEgg(Pawn).DestroyedOrNull();
            closestReachableEgg = XenomorphUtil.ClosestReachableEgg(Pawn);

            hivelikesPresent = XenomorphUtil.HivelikesPresent(Pawn.Map);
            hivelikesReachable = !XenomorphUtil.ClosestReachableHivelike(Pawn).DestroyedOrNull();
            closestReachableHivelike = XenomorphUtil.ClosestReachableHivelike(Pawn);

            cocoonsPresent = XenomorphUtil.EmptyCocoonsPresent(Pawn.Map);
            cocoonsReachable = !XenomorphUtil.ClosestReachableEmptyCocoon(Pawn).DestroyedOrNull();
            closestReachableCocoon = XenomorphUtil.ClosestReachableEmptyCocoon(Pawn);

            if (Pawn.CurrentBed() == null && Pawn.Map!=null)
            {
                ThingDef named = XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon;
                int num = (named.Size.x > named.Size.z) ? named.Size.x : named.Size.z;
                CellRect mapRect;
                IntVec3 intVec = Pawn.Position;
                mapRect = new CellRect(intVec.x, intVec.z, num, num);
                GenPlace.TryPlaceThing(TryMakeCocoon(mapRect, Pawn.Map, named), intVec, Pawn.Map, ThingPlaceMode.Direct);
                closestReachableCocoon = XenomorphUtil.ClosestReachableEmptyCocoon(Pawn);
                ((Building_XenomorphCocoon)closestReachableCocoon).owners.Add(Pawn);
                Pawn.Position = closestReachableCocoon.Position;
                Pawn.mindState.Notify_TuckedIntoBed();
            }
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
            cellRect = new CellRect(mapRect.BottomLeft.x + 1, mapRect.BottomLeft.z + 1, 2, 1);
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
            building_XenomorphCocoon.SetPositionDirect(cellRect.CenterCell);
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
            return building_XenomorphCocoon;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            bool selected = Pawn.Map != null ? Find.Selector.SelectedObjects.Contains(Pawn) : false;
            if (this.ticksSinceHeal > this.healIntervalTicks && Pawn.CurrentBed() == null || (Pawn.CurrentBed() != null&& !(Pawn.CurrentBed() is Building_XenomorphCocoon)))
            {
             //   Pawn.health.RemoveHediff(this.parent);
            }
            else
            {
                this.ticksSinceHeal++;
                bool flag = this.ticksSinceHeal > this.healIntervalTicks;
                if (false)
                {
                    bool flag2 = Pawn.health.hediffSet.HasNaturallyHealingInjury() && Pawn.health.hediffSet.BleedRateTotal>0.1f;
                    if (flag2)
                    {
                        this.ticksSinceHeal = 0;
                        float num = 8f;
                        Hediff_Injury hediff_Injury = GenCollection.RandomElement<Hediff_Injury>(from x in Pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
                                                                                                 where HediffUtility.CanHealNaturally(x) && x.def != XenomorphDefOf.RRY_FaceHuggerInfection && x.def != XenomorphDefOf.RRY_HiddenNeomorphImpregnation && x.def != XenomorphDefOf.RRY_HiddenXenomorphImpregnation && x.def != XenomorphDefOf.RRY_NeomorphImpregnation && x.def != XenomorphDefOf.RRY_XenomorphImpregnation && x.def != XenomorphDefOf.RRY_Hediff_Cocooned && x.def.isBad && x.Bleeding
                                                                                                 select x);
                        hediff_Injury.Heal(num * Pawn.HealthScale * 0.05f);
                        string text = string.Format("{0} healed.", Pawn.LabelCap);
                    }
                }
                /*
                HediffSet curhediffSet = Pawn.health.hediffSet;
                foreach (var item in hediffSetOnAdd.hediffs)
                {
                    if (curhediffSet.hediffs.Contains(item))
                    {
                        Hediff hediff = curhediffSet.hediffs.Find(x => x.def == item.def);
                        if (hediff!=null  && (hediff.def != XenomorphDefOf.RRY_FaceHuggerInfection && hediff.def != XenomorphDefOf.RRY_HiddenNeomorphImpregnation && hediff.def != XenomorphDefOf.RRY_HiddenXenomorphImpregnation && hediff.def != XenomorphDefOf.RRY_NeomorphImpregnation && hediff.def != XenomorphDefOf.RRY_XenomorphImpregnation && hediff.def != XenomorphDefOf.RRY_Hediff_Cocooned && hediff.def.isBad))
                        {
                            if (selected) Log.Message(string.Format("{0}, {1}: {2}, {3}: {4}", Pawn.LabelShortCap, hediff, hediff.Severity, item, item.Severity));
                            hediff.Severity = item.Severity;
                        }
                    }

                }
                */
                if (Pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
                {
                    Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss).Severity = BloodlossSev;
                }
                if (Pawn.health.hediffSet.HasHediff(HediffDefOf.Malnutrition))
                {
                    Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition).Severity = MalnutritionSev;
                }
                if (Pawn.health.hediffSet.HasHediff(HediffDefOf.WoundInfection))
                {
                    foreach (var item in Pawn.health.hediffSet.GetHediffs<Hediff_Injury>())
                    {

                    }
                }
            }

        }
        

    }
}
