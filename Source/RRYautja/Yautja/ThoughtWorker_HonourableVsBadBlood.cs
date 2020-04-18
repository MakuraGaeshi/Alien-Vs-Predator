using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x02000207 RID: 519
    public class ThoughtWorker_HonourableVsBadBlood : ThoughtWorker
    {
        // Token: 0x06000A02 RID: 2562 RVA: 0x0004F2B0 File Offset: 0x0004D6B0
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (p == other || other == null)
            {
                return false;
            }
            if (!p.RaceProps.Humanlike || !other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.AvP_Alien_Yautja || other.kindDef.race != YautjaDefOf.AvP_Alien_Yautja)
            {
                return false;
            }
            if (p.story.adulthood == null)
            {
                return false;
            }
            if (p.story.adulthood.identifier.Contains("AvP_Yautja_BadBlood"))
            {
                return false;
            }
            if (RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (other.story.adulthood==null)
            {
                return false;
            }
            if (other.story.adulthood.identifier.Contains("AvP_Yautja_BadBlood"))
            {
                return true;
            }
            return false;
        }
    }

    // Token: 0x02000207 RID: 519
    public class ThoughtWorker_BadBloodVsHonourable : ThoughtWorker
    {
        // Token: 0x06000A02 RID: 2562 RVA: 0x0004F2B0 File Offset: 0x0004D6B0
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (p == other || other == null)
            {
                return false;
            }
            if (!p.RaceProps.Humanlike || !other.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.kindDef.race != YautjaDefOf.AvP_Alien_Yautja || other.kindDef.race != YautjaDefOf.AvP_Alien_Yautja)
            {
                return false;
            }
            if (other.story.adulthood == null)
            {
                return false;
            }
            if (other.story.adulthood.identifier.Contains("AvP_Yautja_BadBlood"))
            {
                return false;
            }
            if (RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (p.story.adulthood == null)
            {
                return false;
            }
            if (p.story.adulthood.identifier.Contains("AvP_Yautja_BadBlood"))
            {
                return true;
            }
            return false;
        }
    }
}
