using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RRYautja.ExtensionMethods
{
    [StaticConstructorOnStartup]
    public static class AvPExtensions
    {

        public static bool isYautja(this Pawn p)
        {
            return p.def == YautjaDefOf.RRY_Alien_Yautja;
        }

        public static bool isUnblooded(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_Unblooded"));
        }

        public static bool isBlooded(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_Blooded"));
        }

        public static bool isBloodUnmarked(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_BloodedUM"));
        }

        public static bool isBloodMarked(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("RRY_Hediff_BloodedM"));
        }


        public static bool isBloodable(this Pawn p)
        {
            return p.TryGetComp<Comp_Yautja>() != null;
        }

        public static Comp_Yautja BloodStatus(this Pawn p)
        {
            if (p.isBloodable())
            {
                return p.TryGetComp<Comp_Yautja>();
            }
            return null;
        }

        public static BloodStatusMode CurBloodStatus(this Pawn p)
        {
            if (p.isBloodable())
            {
                if (p.isUnblooded())
                {
                    return BloodStatusMode.Unblooded;
                }
                if (p.isBloodUnmarked())
                {
                    return BloodStatusMode.Unmarked;
                }
                if (p.isBloodMarked())
                {
                    return BloodStatusMode.Marked;
                }
                return BloodStatusMode.None;
            }
            return BloodStatusMode.NoComp;
        }

        public static bool isCocooned(this Pawn p)
        {
            return p.health.hediffSet.HasHediff(XenomorphDefOf.RRY_Hediff_Cocooned);
        }

        public static bool isXenomorph(this Pawn p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }

        public static bool isNeomorph(this Pawn p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Neomorph;
        }

        public static bool isXenomorph(this PawnKindDef p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }

        public static bool isNeomorph(this PawnKindDef p)
        {
            return p.RaceProps.FleshType == XenomorphRacesDefOf.RRY_Neomorph;
        }

        public static bool isXenomorph(this ThingDef p)
        {
            return p.race.FleshType == XenomorphRacesDefOf.RRY_Xenomorph;
        }

        public static bool isNeomorph(this ThingDef p)
        {
            return p.race.FleshType == XenomorphRacesDefOf.RRY_Neomorph;
        }

        public static bool isPotentialHost(this Pawn p)
        {
            return XenomorphUtil.isInfectablePawn(p) && !p.isXenomorph() && !p.isNeomorph();
        }

        public static bool isPotentialHost(this PawnKindDef p)
        {
            return XenomorphUtil.isInfectablePawnKind(p) && !p.isXenomorph() && !p.isNeomorph();
        }

        public static bool isPotentialHost(this ThingDef p)
        {
            return XenomorphUtil.isInfectableThing(p) && !p.isXenomorph() && !p.isNeomorph();
        }

        public static bool isHost(this Pawn p)
        {
            return p.isNeoHost() || p.isXenoHost();
        }
        public static bool isXenoHost(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("XenomorphImpregnation") || x.def.defName.Contains("FaceHuggerInfection"));
        }
        public static bool isNeoHost(this Pawn p)
        {
            return p.health.hediffSet.hediffs.Any(x =>  x.def.defName.Contains("NeomorphImpregnation"));
        }

        public static bool isWorthyKill(this Pawn p)
        {
            bool predatory = p.kindDef.RaceProps.predator;
            bool fighter = p.kindDef.isFighter;
            bool leader = p.kindDef.factionLeader;
            bool human = p.def.defName.Contains("Human");
            bool humanlike = p.RaceProps.Humanlike;
            bool combatpower50 = p.kindDef.combatPower >= 50;
            bool combatpower100 = p.kindDef.combatPower >= 100;
            return YautjaBloodedUtility.WorthyKill(p.kindDef);
        }

        public static PawnKindDef resultingXenomorph(this Pawn p)
        {
            PawnKindDef kindDef = null;
            if (!p.isPotentialHost() || p.BodySize > 0.63f)
            {
                return null;
            }
            bool human = p.def.defName.Contains("Human") || YautjaBloodedUtility.GetMark(p.kindDef) == YautjaDefOf.RRY_Hediff_BloodedMHuman;
            bool yautja = p.def.defName.Contains("Yautja");
            bool thrumbo = p.def.defName.Contains("Human") || YautjaBloodedUtility.GetMark(p.kindDef) == YautjaDefOf.RRY_Hediff_BloodedMThrumbo;
            bool hound = YautjaBloodedUtility.GetMark(p.kindDef) == YautjaDefOf.RRY_Hediff_BloodedMHound;

            bool humanlike = p.RaceProps.Humanlike;

            bool large = humanlike ? p.BodySize > 1f : p.BodySize > 4f;
            bool small = p.BodySize > 1f;

            bool predalienEmbryo = p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("XenomorphImpregnation")) && ((XenoHediffWithComps)p.health.hediffSet.hediffs.Find(x => x.def.defName.Contains("XenomorphImpregnation"))).TryGetComp<HediffComp_XenoSpawner>().predalienImpregnation;
            bool royaleEmbryo = p.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("XenomorphImpregnation")) && ((XenoHediffWithComps)p.health.hediffSet.hediffs.Find(x => x.def.defName.Contains("XenomorphImpregnation"))).TryGetComp<HediffComp_XenoSpawner>().RoyaleEmbryo;

            if (humanlike)
            {
                if (human)
                {
                    if (predalienEmbryo)
                    {
                        kindDef = p.kindDef.isFighter ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                    }
                    else
                    {
                        if (royaleEmbryo)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Queen;
                        }
                        else
                        {
                            if (large)
                            {
                                kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                            }
                            else
                            {
                                if (small)
                                {
                                    kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                                else
                                {
                                    kindDef = p.kindDef.isFighter ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (yautja)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
                    }
                    else
                    {
                        if (royaleEmbryo)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Queen;
                        }
                        else
                        {
                            if (large)
                            {
                                kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                            }
                            else
                            {
                                if (small)
                                {
                                    kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                                else
                                {
                                    kindDef = p.kindDef.RaceProps.predator ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (large)
                {
                    if (thrumbo)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Thrumbomorph;
                    }
                    else
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    }
                }
                else
                {
                    if (hound)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                        }
                        else
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                    }
                }
            }

            bool selected = Find.Selector.SingleSelectedThing == p;
            if (selected && Prefs.DevMode)
            {
                Log.Message(string.Format("{0} will spawn from {1}", kindDef, p.LabelShortCap));
            }
            return kindDef;
        }

        public static PawnKindDef resultingXenomorph(this PawnKindDef p)
        {
            PawnKindDef kindDef = null;
            if (!p.isPotentialHost() || p.RaceProps.baseBodySize > 0.63f)
            {
                return null;
            }
            bool human = p.race.defName.Contains("Human") || YautjaBloodedUtility.GetMark(p) == YautjaDefOf.RRY_Hediff_BloodedMHuman;
            bool yautja = p.race.defName.Contains("Yautja");
            bool thrumbo = p.race.defName.Contains("Human") || YautjaBloodedUtility.GetMark(p) == YautjaDefOf.RRY_Hediff_BloodedMThrumbo;
            bool hound = YautjaBloodedUtility.GetMark(p) == YautjaDefOf.RRY_Hediff_BloodedMHound;

            bool humanlike = p.RaceProps.Humanlike;

            bool large = humanlike ? p.RaceProps.baseBodySize > 1f : p.RaceProps.baseBodySize > 4f;
            bool small = p.RaceProps.baseBodySize > 1f;

            if (humanlike)
            {
                if (human)
                {
                    if (large)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                        else
                        {
                            kindDef = p.isFighter ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                    }
                }
                else
                {
                    if (yautja)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Predalien;
                    }
                    else
                    {
                        if (large)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                        }
                        else
                        {
                            if (small)
                            {
                                kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                            }
                            else
                            {
                                kindDef = p.RaceProps.predator ? XenomorphDefOf.RRY_Xenomorph_Warrior : XenomorphDefOf.RRY_Xenomorph_Drone;
                            }
                        }
                    }
                }
            }
            else
            {
                if (large)
                {
                    if (thrumbo)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Thrumbomorph;
                    }
                    else
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Warrior;
                    }
                }
                else
                {
                    if (hound)
                    {
                        kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Runner;
                        }
                        else
                        {
                            kindDef = XenomorphDefOf.RRY_Xenomorph_Drone;
                        }
                    }
                }
            }
            return kindDef;
        }

        public static List<PawnKindDef> resultingXenomorph(this ThingDef p)
        {
            List<PawnKindDef> kindDef = new List<PawnKindDef>();
            if (!p.isPotentialHost() || p.race.baseBodySize < 0.63f)
            {
                return null;
            }
            bool human = p.defName.Contains("Human");
            bool yautja = p.defName.Contains("Alien_Yautja");
            bool thrumbo = p.defName.Contains("Thrumbo");
            bool hound = p.defName.Contains("Hound") || p.defName.Contains("hound") || p.defName.Contains("Dog") || p.defName.Contains("dog") || p.defName.Contains("Wolf") || p.defName.Contains("wolf") || p.defName.Contains("Warg") || p.defName.Contains("warg") || p.description.Contains("Hound") || p.description.Contains("hound") || p.description.Contains("Dog") || p.description.Contains("dog") || p.description.Contains("Wolf") || p.description.Contains("wolf") || p.description.Contains("Warg") || p.description.Contains("warg");

            bool humanlike = p.race.Humanlike;

            bool large = humanlike ? p.race.baseBodySize > 1f : p.race.baseBodySize >= 3.5f;
            bool small = p.race.baseBodySize > 1f;

            if (humanlike)
            {
                if (human)
                {
                    if (large)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                        }
                        else
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                        }
                    }
                }
                else
                {
                    if (yautja)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Predalien);
                    }
                    else
                    {
                        if (large)
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                        }
                        else
                        {
                            if (small)
                            {
                                kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                            }
                            else
                            {
                                kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                                kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                            }
                        }
                    }
                }
            }
            else
            {
                if (large)
                {
                    if (thrumbo)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Thrumbomorph);
                    }
                    else
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Warrior);
                    }
                }
                else
                {
                    if (hound)
                    {
                        kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Runner);
                    }
                    else
                    {
                        if (small)
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Runner);
                        }
                        else
                        {
                            kindDef.Add(XenomorphDefOf.RRY_Xenomorph_Drone);
                        }
                    }
                }
            }
            return kindDef;
        }

        public static HediffDef unbloodedDef = YautjaDefOf.RRY_Hediff_Unblooded;
        public static HediffDef unmarkedDef = YautjaDefOf.RRY_Hediff_BloodedUM;
        public static HediffDef markedDef = YautjaDefOf.RRY_Hediff_BloodedM;
        public static AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_YoungBlood");
        public static AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_Blooded");
        public static AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodA");
        public static AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodB");

        // Token: 0x02000D68 RID: 3432
        public enum BloodStatusMode
        {
            NoComp,
            None,
            Unblooded,
            Unmarked,
            Marked
        }

    }

}
