using System;

namespace Verse
{
    // Token: 0x02000CFD RID: 3325
    public enum HiveCategory : byte
    {
        // Token: 0x0400322C RID: 12844
        None,
        // Token: 0x0400322D RID: 12845
        Dusting,
        // Token: 0x0400322E RID: 12846
        Thin,
        // Token: 0x0400322F RID: 12847
        Medium,
        // Token: 0x04003230 RID: 12848
        Thick
    }

    // Token: 0x02000CFE RID: 3326
    public static class HiveUtility
    {
        // Token: 0x060049BF RID: 18879 RVA: 0x0022958C File Offset: 0x0022798C
        public static HiveCategory GetHiveCategory(float snowDepth)
        {
            if (snowDepth < 0.03f)
            {
                return HiveCategory.None;
            }
            if (snowDepth < 0.25f)
            {
                return HiveCategory.Dusting;
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

        // Token: 0x060049C0 RID: 18880 RVA: 0x002295C4 File Offset: 0x002279C4
        public static string GetDescription(HiveCategory category)
        {
            switch (category)
            {
                case HiveCategory.None:
                    return "SnowNone".Translate();
                case HiveCategory.Dusting:
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

        // Token: 0x060049C1 RID: 18881 RVA: 0x0022962C File Offset: 0x00227A2C
        public static int MovementTicksAddOn(HiveCategory category)
        {
            switch (category)
            {
                case HiveCategory.None:
                    return 0;
                case HiveCategory.Dusting:
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

        // Token: 0x060049C2 RID: 18882 RVA: 0x0022965C File Offset: 0x00227A5C
        public static void AddSnowRadial(IntVec3 center, Map map, float radius, float depth)
        {
            int num = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = center + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map))
                {
                    float lengthHorizontal = (center - intVec).LengthHorizontal;
                    float num2 = 1f - lengthHorizontal / radius;
                    map.snowGrid.AddDepth(intVec, num2 * depth);
                }
            }
        }
    }
}
