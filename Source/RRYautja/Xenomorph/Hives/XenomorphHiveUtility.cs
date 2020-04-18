using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace RRYautja
{
    // Token: 0x02000CFE RID: 3326
    public static class XenomorphHiveUtility
    {

        // Token: 0x06002688 RID: 9864 RVA: 0x00124A2B File Offset: 0x00122E2B
        public static int TotalSpawnedHiveLikesCount(Map map)
        {
            return map.listerThings.ThingsOfDef(XenomorphDefOf.AvP_Xenomorph_Hive).Count;
        }

        // Token: 0x06002689 RID: 9865 RVA: 0x00124A44 File Offset: 0x00122E44
        public static bool AnyHiveLikePreventsClaiming(Thing thing)
        {
            if (!thing.Spawned)
            {
                return false;
            }
            int num = GenRadial.NumCellsInRadius(2f);
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = thing.Position + GenRadial.RadialPattern[i];
                if (c.InBounds(thing.Map) && c.GetFirstThing(thing.Map, null) != null)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x0600268A RID: 9866 RVA: 0x00124ABC File Offset: 0x00122EBC
        public static void Notify_HiveLikeDespawned(HiveLike hivelike, Map map)
        {
            int num = GenRadial.NumCellsInRadius(2f);
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = hivelike.Position + GenRadial.RadialPattern[i];
                if (c.InBounds(map))
                {
                    List<Thing> thingList = c.GetThingList(map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].Faction == hivelike.OfFaction && !XenomorphHiveUtility.AnyHiveLikePreventsClaiming(thingList[j]))
                        {
                            thingList[j].SetFaction(null, null);
                        }
                    }
                }
            }
        }


        // Token: 0x040015BC RID: 5564
        private const float HivePreventsClaimingInRadius = 2f;
        public static HiveCategory GetHiveCategory(float snowDepth)
        {
            if (snowDepth < 0.03f)
            {
                return HiveCategory.None;
            }
            if (snowDepth < 0.25f)
            {
                return HiveCategory.Inital;
            }
            if (snowDepth < 0.5f)
            {
                return HiveCategory.Thin;
            }
            if (snowDepth < 0.75f)
            {
                return HiveCategory.Medium;
            }
            return HiveCategory.Thick;
        }

        public static GooCategory GetGooCategory(float snowDepth)
        {
            if (snowDepth < 0.03f)
            {
                return GooCategory.None;
            }
            if (snowDepth < 0.25f)
            {
                return GooCategory.Inital;
            }
            if (snowDepth < 0.5f)
            {
                return GooCategory.Thin;
            }
            if (snowDepth < 0.75f)
            {
                return GooCategory.Medium;
            }
            return GooCategory.Thick;
        }

        public static string GetDescription(HiveCategory category)
        {
            switch (category)
            {
                case HiveCategory.None:
                    return "SnowNone".Translate();
                case HiveCategory.Inital:
                    return "SnowDusting".Translate();
                case HiveCategory.Thin:
                    return "SnowThin".Translate();
                case HiveCategory.Medium:
                    return "SnowMedium".Translate();
                case HiveCategory.Thick:
                    return "SnowThick".Translate();
                default:
                    return "Unknown snow";
            }
        }
        public static string GetDescription(GooCategory category)
        {
            switch (category)
            {
                case GooCategory.None:
                    return "SnowNone".Translate();
                case GooCategory.Inital:
                    return "SnowDusting".Translate();
                case GooCategory.Thin:
                    return "SnowThin".Translate();
                case GooCategory.Medium:
                    return "SnowMedium".Translate();
                case GooCategory.Thick:
                    return "SnowThick".Translate();
                default:
                    return "Unknown snow";
            }
        }

        public static int MovementTicksAddOn(HiveCategory category)
        {
            switch (category)
            {
                case HiveCategory.None:
                    return 0;
                case HiveCategory.Inital:
                    return 0;
                case HiveCategory.Thin:
                    return 4;
                case HiveCategory.Medium:
                    return 8;
                case HiveCategory.Thick:
                    return 12;
                default:
                    return 0;
            }
        }

        public static void AddHiveRadial(IntVec3 center, Map map, float radius, float depth)
        {
            int num = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = center + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map))
                {
                    float lengthHorizontal = (center - intVec).LengthHorizontal;
                    float num2 = 1f - lengthHorizontal / radius;

                    MapComponent_HiveGrid _HiveGrid = map.GetComponent<MapComponent_HiveGrid>();
                    _HiveGrid.AddDepth(intVec, num2 * depth);
                }
            }
        }


        public static void AddGooRadial(IntVec3 center, Map map, float radius, float depth)
        {
            int num = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = center + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map))
                {
                    float lengthHorizontal = (center - intVec).LengthHorizontal;
                    float num2 = 1f - lengthHorizontal / radius;

                    MapComponent_GooGrid _HiveGrid = map.GetComponent<MapComponent_GooGrid>();
                    _HiveGrid.AddDepth(intVec, num2 * depth);
                }
            }
        }

        public enum HiveCategory : byte
        {
            // Token: 0x0400322C RID: 12844
            None,
            // Token: 0x0400322D RID: 12845
            Inital,
            // Token: 0x0400322E RID: 12846
            Thin,
            // Token: 0x0400322F RID: 12847
            Medium,
            // Token: 0x04003230 RID: 12848
            Thick
        }
        public enum GooCategory : byte
        {
            // Token: 0x0400322C RID: 12844
            None,
            // Token: 0x0400322D RID: 12845
            Inital,
            // Token: 0x0400322E RID: 12846
            Thin,
            // Token: 0x0400322F RID: 12847
            Medium,
            // Token: 0x04003230 RID: 12848
            Thick
        }
    }
}
