using System;
using System.Collections.Generic;
using RimWorld;
using System.Text;
using Verse;

namespace RRYautja
{
    public static class YautjaBloodedUtility
    {
        public static HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public static HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
        public static HediffDef markedDef = YautjaDefOf.RRY_Hediff_BloodedM;

        // Token: 0x02000D68 RID: 3432
        public enum BloodStatusMode
        {
            // Token: 0x040033DB RID: 13275
            Unblooded,
            // Token: 0x040033DC RID: 13276
            Unmarked,
            // Token: 0x040033DD RID: 13277
            Marked
        }

        // Token: 0x06004C44 RID: 19524 RVA: 0x0023802D File Offset: 0x0023642D
        public static string GetLabel(this BloodStatusMode m, Pawn pawn)
        {
            switch (m)
            {
                case BloodStatusMode.Unblooded:
                    return "BloodStatus_Unblooded".Translate();
                    return "HediffGrowthMode_Growing".Translate();
                case BloodStatusMode.Unmarked:
                    return "BloodStatus_Unmarked".Translate(pawn.LabelShortCap);
                    return "HediffGrowthMode_Stable".Translate();
                case BloodStatusMode.Marked:
                    return "BloodStatus_Marked".Translate();
                    return "HediffGrowthMode_Remission".Translate();
                default:
                    throw new ArgumentException();
            }
        }

        public static bool BloodStatus(Pawn pawn, out Hediff BloodHD)
        {
            HediffSet hediffSet = pawn.health.hediffSet;
            bool result = false;
            BloodHD = null;

            bool hasbloodedM = hediffSet.hediffs.Any<Hediff>(x => x.def.defName.StartsWith(markedDef.defName));
            if (hasbloodedM)
            {
                foreach (var item in hediffSet.hediffs)
                {
                    if (item.def.defName.StartsWith(markedDef.defName))
                    {
                        BloodHD = item;
                        result = true;
                        break;
                    }
                }
            }
            bool hasunblooded = hediffSet.HasHediff(unbloodedDef);
            if (hasunblooded)
            {
                BloodHD = hediffSet.GetFirstHediffOfDef(unbloodedDef);
                result = false;

            }
            bool hasbloodedUM = hediffSet.HasHediff(unmarkedDef);
            if (hasbloodedUM)
            {
                BloodHD = hediffSet.GetFirstHediffOfDef(unmarkedDef);
                result = true;

            }
            return result;
        }
        public static bool BloodStatus(Pawn pawn)
        {
            HediffSet hediffSet = pawn.health.hediffSet;
            bool result = false;

            bool hasbloodedM = hediffSet.hediffs.Any<Hediff>(x => x.def.defName.StartsWith(markedDef.defName));
            if (hasbloodedM)
            {
                foreach (var item in hediffSet.hediffs)
                {
                    if (item.def.defName.StartsWith(markedDef.defName))
                    {
                        result = true;
                        break;
                    }
                }
            }
            bool hasunblooded = hediffSet.HasHediff(unbloodedDef);
            if (hasunblooded)
            {
                result = false;

            }
            bool hasbloodedUM = hediffSet.HasHediff(unmarkedDef);
            if (hasbloodedUM)
            {
                result = true;

            }
            return result;
        }

        public static bool Marked(Pawn pawn, out Hediff BloodHD)
        {
            HediffSet hediffSet = pawn.health.hediffSet;
            bool result = false;
            BloodHD = null;
            //  bool hasbloodedM = (hediffSet.hediffs.FindAll(x => x.def.defName.StartsWith(YautjaBloodedUtility.markedDef.defName)).FirstOrFallback());
            bool hasbloodedM = hediffSet.hediffs.Any<Hediff>(x => x.def.defName.StartsWith(markedDef.defName));
            if (hasbloodedM)
            {
                foreach (var item in hediffSet.hediffs)
                {
                    if (item.def.defName.StartsWith(markedDef.defName))
                    {
                        BloodHD = item;
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public static bool bloodmatch(HediffDef hediffDef, Corpse corpse)
        {
            foreach (var item in Bloodlist)
            {
                if (hediffDef == item.First)
                {
                    if (corpse.InnerPawn.kindDef.race == item.Second)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static ThingDef bloodForMark(HediffDef hediffDef)
        {
            //Log.Message(string.Format("hediffDef {0}", hediffDef));
            foreach (var item in Bloodlist)
            {
            //    Log.Message(string.Format("checking against {0}", item.First));
                if (hediffDef == item.First)
                {
                //    Log.Message(string.Format("returning {0}", item.Second));
                    return item.Second;
                }
            }
            return null;
        }

        public static ThingDef bloodForMark(Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(YautjaDefOf.RRY_Hediff_BloodedUM))
            {
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
                foreach (var item in Bloodlist)
                {
                    if (hediff.def == item.First)
                    {
                        return item.Second;
                    }
                }
            }
            return null;
        }

        public static List<Pair<ThingDef, HediffDef>> Bloolist = new List<Pair<ThingDef, HediffDef>>
        { // XenomorphRacesDefOf XenomorphDefOf
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Queen.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Predalien.race, YautjaDefOf.RRY_Hediff_BloodedMPredalien),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Warrior.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Drone.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Runner.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Neomorph.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(YautjaDefOf.Alien_Yautja, YautjaDefOf.RRY_Hediff_BloodedMBadBlood),
            new Pair<ThingDef, HediffDef>(ThingDefOf.Human, YautjaDefOf.RRY_Hediff_BloodedMHuman),
            new Pair<ThingDef, HediffDef>(ThingDefOf.Human, YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman),
            new Pair<ThingDef, HediffDef>(ThingDefOf.Human, YautjaDefOf.RRY_Hediff_BloodedMHumanlike),
            new Pair<ThingDef, HediffDef>(ThingDefOf.Human, YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike),
            new Pair<ThingDef, HediffDef>(YautjaDefOf.RRY_Yautja_Hound, YautjaDefOf.RRY_Hediff_BloodedMHound),
            new Pair<ThingDef, HediffDef>(YautjaDefOf.RRY_Rynath, YautjaDefOf.RRY_Hediff_BloodedMCrusher),
            new Pair<ThingDef, HediffDef>(ThingDefOf.Thrumbo, YautjaDefOf.RRY_Hediff_BloodedMGroTye),

            /*
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMXenomorph, XenomorphDefOf.RRY_Xenomorph_Warrior.race),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMPredalien, XenomorphDefOf.RRY_Xenomorph_Predalien.race),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMBadBlood, YautjaDefOf.Alien_Yautja),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMHuman, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMHumanlike, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMHound, YautjaDefOf.RRY_Yautja_Hound),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMCrusher, YautjaDefOf.RRY_Rynath),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMGroTye, ThingDefOf.Thrumbo),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedM, XenomorphDefOf.RRY_Xenomorph_Drone.race)
            */
        };

        public static List<Pair<HediffDef, ThingDef>> Bloodlist = new List<Pair<HediffDef, ThingDef>>
        {
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen, XenomorphDefOf.RRY_Xenomorph_Queen.race),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMXenomorph, XenomorphDefOf.RRY_Xenomorph_Warrior.race),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMPredalien, XenomorphDefOf.RRY_Xenomorph_Predalien.race),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMBadBlood, YautjaDefOf.Alien_Yautja),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMHuman, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMHumanlike, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike, ThingDefOf.Human),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMHound, YautjaDefOf.RRY_Yautja_Hound),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMCrusher, YautjaDefOf.RRY_Rynath),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedMGroTye, ThingDefOf.Thrumbo),
            new Pair<HediffDef, ThingDef>(YautjaDefOf.RRY_Hediff_BloodedM, XenomorphDefOf.RRY_Xenomorph_Drone.race)
        };
    }
}
