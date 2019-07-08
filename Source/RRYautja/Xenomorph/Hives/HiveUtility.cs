using System;
using Verse;

namespace RRYautja
{
    // Token: 0x02000CFE RID: 3326
    public static class HiveUtility
    {
        public static HiveCategory GetSnowCategory(float snowDepth)
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
    }
}
