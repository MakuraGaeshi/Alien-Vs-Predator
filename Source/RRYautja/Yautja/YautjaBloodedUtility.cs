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
        public static AlienRace.BackstoryDef bsDefUnblooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_YoungBlood");
        public static AlienRace.BackstoryDef bsDefBlooded = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_Blooded");
        public static AlienRace.BackstoryDef bsDefBadbloodA = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodA");
        public static AlienRace.BackstoryDef bsDefBadblooBd = DefDatabase<AlienRace.BackstoryDef>.GetNamed("RRY_Yautja_BadBloodB");

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
                    //return "HediffGrowthMode_Growing".Translate();
                case BloodStatusMode.Unmarked:
                    return "BloodStatus_Unmarked".Translate(pawn.LabelShortCap);
                    //return "HediffGrowthMode_Stable".Translate();
                case BloodStatusMode.Marked:
                    return "BloodStatus_Marked".Translate();
                    //return "HediffGrowthMode_Remission".Translate();
                default:
                    throw new ArgumentException();
            }
        }

        public static bool BloodStatus(Pawn pawn, out Hediff BloodHD)
        {
            HediffSet hediffSet = pawn.health.hediffSet;
            BodyPartRecord part = pawn.RaceProps.body.AllParts.Find(x => x.def == BodyPartDefOf.Head);
            HediffDef hediffDef;
            bool result = false;
            BloodHD = null;
            bool hasbloodedM = hediffSet.hediffs.Any<Hediff>(x => x.def.defName.StartsWith(markedDef.defName));
            if (hasbloodedM)
            {
                BloodHD = hediffSet.hediffs.Find(x => x.def.defName.Contains(markedDef.defName));
            //    Log.Message(string.Format("hasbloodedM: {0}", BloodHD));
                result = true;
            }
            bool hasunblooded = hediffSet.HasHediff(unbloodedDef);
            if (hasunblooded)
            {
                BloodHD = hediffSet.GetFirstHediffOfDef(unbloodedDef);
                if (!PlayerKnowledgeDatabase.IsComplete(YautjaConceptDefOf.RRY_Concept_Unblooded) && pawn.IsColonist && pawn.Spawned && (pawn.Map != null|| pawn.MapHeld != null))
                {
                    LessonAutoActivator.TeachOpportunity(YautjaConceptDefOf.RRY_Concept_Unblooded, OpportunityType.Important);
                }
                //    Log.Message(string.Format("hasunblooded: {0}", BloodHD));
                result = false;
            }
            bool hasbloodedUM = hediffSet.HasHediff(unmarkedDef);
            if (hasbloodedUM)
            {
                BloodHD = hediffSet.GetFirstHediffOfDef(unmarkedDef);
                if (!PlayerKnowledgeDatabase.IsComplete(YautjaConceptDefOf.RRY_Concept_Blooding) && pawn.IsColonist && pawn.Spawned && (pawn.Map != null || pawn.MapHeld != null))
                {
                    LessonAutoActivator.TeachOpportunity(YautjaConceptDefOf.RRY_Concept_Blooding, OpportunityType.Important);
                }
                //    Log.Message(string.Format("hasbloodedUM: {0}", BloodHD));
                result = true;
            }
            if (BloodHD==null)
            {
            //    Log.Message(string.Format("1"));
                Backstory pawnStoryC = pawn.story.childhood;
                Backstory pawnStoryA = pawn.story.adulthood ?? null;

                if (pawnStoryA != null)
                {
                //    Log.Message(string.Format("2"));
                    if (pawnStoryA != bsDefUnblooded.backstory)
                    {
                    //    Log.Message(string.Format("3"));
                        Comp_Yautja _Yautja = pawn.TryGetComp<Comp_Yautja>();
                        hediffDef = _Yautja.Props.bloodedDefs.RandomElement();

                        if (hediffDef != null)
                        {
                        //    Log.Message(string.Format("4"));
                            PawnKindDef pawnKindDef = YautjaBloodedUtility.RandomMarked(hediffDef);
                            if (_Yautja != null)
                            {
                            //    Log.Message(string.Format("5"));
                                _Yautja.MarkHedifflabel = pawnKindDef.LabelCap;
                                _Yautja.MarkedhediffDef = hediffDef;
                                _Yautja.predator = pawnKindDef.RaceProps.predator;
                                _Yautja.BodySize = pawnKindDef.RaceProps.baseBodySize;
                                _Yautja.combatPower = pawnKindDef.combatPower;
                            }
                        }

                    }
                    else
                    {
                    //    Log.Message(string.Format("6"));
                        hediffDef = unbloodedDef;
                    }
                }
                else
                {
                //    Log.Message(string.Format("7"));
                    hediffDef = unbloodedDef;
                }
            //    Log.Message(string.Format("8"));
                pawn.health.AddHediff(hediffDef, part);
            //    Log.Message(string.Format("9"));
                BloodHD = hediffSet.GetFirstHediffOfDef(hediffDef);
            //    Log.Message(string.Format("10"));
                result = true;
            //    Log.Message(string.Format("11"));
            }
            else
            {
            //    Log.Message(string.Format("12"));
                Comp_Yautja _Yautja = pawn.TryGetComp<Comp_Yautja>();
            //    Log.Message(string.Format("12 a {0}", BloodHD.def));
                if (hasbloodedM)
                {
                    PawnKindDef pawnKindDef = YautjaBloodedUtility.RandomMarked(BloodHD.def);
                //    Log.Message(string.Format("12 B {0}", pawnKindDef));
                    if (_Yautja != null && _Yautja.MarkHedifflabel.NullOrEmpty())
                    {
                    //    Log.Message(string.Format("13 "));
                        _Yautja.MarkHedifflabel = pawnKindDef.LabelCap;
                    //    Log.Message(string.Format("13 a"));
                        _Yautja.MarkedhediffDef = BloodHD.def;
                    //    Log.Message(string.Format("13 b"));
                        _Yautja.predator = pawnKindDef.RaceProps.predator;
                    //    Log.Message(string.Format("13 c"));
                        _Yautja.BodySize = pawnKindDef.RaceProps.baseBodySize;
                    //    Log.Message(string.Format("13 d"));
                        _Yautja.combatPower = pawnKindDef.combatPower;
                    //    Log.Message(string.Format("13 f"));
                    }
                }
            }
        //    Log.Message(string.Format("14"));
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
        
        public static List<Pair<ThingDef, HediffDef>> Bloolist = new List<Pair<ThingDef, HediffDef>>
        { // XenomorphRacesDefOf XenomorphDefOf
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Queen.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Predalien.race, YautjaDefOf.RRY_Hediff_BloodedMPredalien),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Warrior.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Drone.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Runner.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(XenomorphDefOf.RRY_Xenomorph_Neomorph.race, YautjaDefOf.RRY_Hediff_BloodedMXenomorph),
            new Pair<ThingDef, HediffDef>(YautjaDefOf.RRY_Alien_Yautja, YautjaDefOf.RRY_Hediff_BloodedMBadBlood),
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
        

        public static PawnKindDef RandomMarked(HediffDef markedDef)
        {
        //    Log.Message(string.Format("{0}", markedDef));
            if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen)
            {
                return XenomorphDefOf.RRY_Xenomorph_Queen;
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMXenomorph)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => x.race.defName.Contains("Xenomorph") && !x.race.defName.Contains("FaceHugger") && !x.race.defName.Contains("Predalien") && !x.race.defName.Contains("Queen")).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMMechanoid)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => ((x.race.defName.Contains("Mech") || x.race.defName.Contains("Droid") || x.race.defName.Contains("Android") || x.race.defName.Contains("ChjDroid") || x.race.defName.Contains("ChjAndroid")) && x.RaceProps.IsMechanoid || (x.defaultFactionType != null && x.defaultFactionType == Faction.OfMechanoids.def)) && !x.race.race.body.defName.Contains("AIRobot")).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMPredalien)
            {
                return XenomorphDefOf.RRY_Xenomorph_Predalien;
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMBadBlood)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => x.race == YautjaDefOf.RRY_Alien_Yautja && x.defaultFactionType==YautjaDefOf.RRY_Yautja_BadBloodFaction).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMHumanlike)
            {
                List<PawnKindDef> list =  DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => x.race != ThingDefOf.Human && x.RaceProps.Humanlike && (/*!x.race.defName.Contains("Yautja") &&*/ !x.race.defName.Contains("Mech") && !x.race.defName.Contains("Droid") && !x.race.defName.Contains("Android") && !x.race.defName.Contains("ChjDroid") && !x.race.defName.Contains("ChjAndroid")) && !x.factionLeader);
                if (!list.NullOrEmpty())
                {
                    return list.RandomElement();
                }
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => x.race != ThingDefOf.Human && x.RaceProps.Humanlike && ( /*!x.race.defName.Contains("Yautja") &&*/ !x.race.defName.Contains("Mech") && !x.race.defName.Contains("Droid") && !x.race.defName.Contains("Android") && !x.race.defName.Contains("ChjDroid") && !x.race.defName.Contains("ChjAndroid")) && (x.factionLeader || (x.isFighter && x.combatPower > (100)))).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMHuman)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => x.race == ThingDefOf.Human && !x.factionLeader).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => x.race == ThingDefOf.Human && (x.factionLeader || (x.isFighter && x.combatPower > (100)))).RandomElement();
            }
            /*
            else if (other.kindDef.race == YautjaDefOf.RRY_Alien_Yautja && !other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead && (other.Faction.PlayerGoodwill > 0 || other.Faction.IsPlayer)) 
            {
                markedDef == YautjaDefOf.RRY_Hediff_BloodedMBadBlood;
            }
            */
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMGroTye)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => !x.race.defName.Contains("Xenomorph") && x.defName.Contains("Megasloth")).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMCrusher)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => !x.race.defName.Contains("Xenomorph") && (x.defName.Contains("Rhinoceros") || x.defName.Contains("Elephant"))).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMThrumbo)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("Thrumbo")).RandomElement();
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedMHound)
            {
                List<PawnKindDef> list = DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => !x.race.defName.Contains("Xenomorph") && !x.RaceProps.Humanlike && ((x.defName.Contains("Wolf") || x.race.description.Contains("Wolf") || x.race.description.Contains("wolf") || x.race.description.Contains("wolves")) || (x.defName.Contains("Hound") || x.defName.Contains("hound") || x.race.description.Contains("Hound") || x.race.description.Contains("hound") || x.race.description.Contains("hounds")) || (x.race.defName.Contains("Dog") || x.race.description.Contains("Dog") || x.race.description.Contains("dog") || x.race.description.Contains("dogs"))) && ((x.RaceProps.predator == true && x.combatPower > 50) || (x.RaceProps.predator == false && x.combatPower > 100)));
                PawnKindDef kindDef = list.RandomElement();
                return kindDef;
            }
            else if (markedDef == YautjaDefOf.RRY_Hediff_BloodedM)
            {
                return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => (!x.RaceProps.IsMechanoid && x.defaultFactionType != Faction.OfMechanoids.def) && !x.race.defName.Contains("Xenomorph") && (!x.defName.Contains("Rhinoceros") && !x.defName.Contains("Elephant") && !x.defName.Contains("Thrumbo"))&& !x.defName.Contains("Megasloth") && ((!x.defName.Contains("Wolf") && !x.race.description.Contains("Wolf") && !x.race.description.Contains("wolf") && !x.race.description.Contains("wolves")) && (!x.defName.Contains("Hound") && !x.defName.Contains("hound") && !x.race.description.Contains("Hound") && !x.race.description.Contains("hound") && !x.race.description.Contains("hounds")) && (!x.race.defName.Contains("Dog") && !x.race.description.Contains("Dog") && !x.race.description.Contains("dog") && !x.race.description.Contains("dogs"))) && !x.RaceProps.Humanlike && (x.combatPower > 100 || (x.RaceProps.predator == true && x.combatPower > 50))).RandomElement();
            }
            return null;
        }
    }
}
