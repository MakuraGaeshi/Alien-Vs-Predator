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

                if (pawn.kindDef.race == YautjaDefOf.RRY_Alien_Yautja && pawnStoryA != null)
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
      

        public static PawnKindDef RandomMarked(HediffDef markedDef)
        {
            List<PawnKindDef> kinddefs = DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(x => GetMark(x) == markedDef);
            if (kinddefs.NullOrEmpty())
            {
                return null;
            }
            return kinddefs.RandomElement();
        }

        public static HediffDef GetMark(PawnKindDef x)
        {
            //    Log.Message(string.Format("{0}", markedDef)); 
            if (x == XenomorphDefOf.RRY_Xenomorph_Queen)
            {
                return YautjaDefOf.RRY_Hediff_BloodedMXenomorphQueen;
            }
            else if (x.race.defName.Contains("Xenomorph") && !x.race.defName.Contains("FaceHugger") && !x.race.defName.Contains("Predalien") && !x.race.defName.Contains("Queen"))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMXenomorph;
            }
            else if (!x.race.defName.Contains("Human") && ((x.race.defName.Contains("WraithGuard") || x.race.defName.Contains("Necron") || x.race.defName.Contains("Mech") || x.race.defName.Contains("Droid") || x.race.defName.Contains("Android") || x.race.defName.Contains("ChjDroid") || x.race.defName.Contains("ChjAndroid")) || x.RaceProps.IsMechanoid/* || (x.defaultFactionType != null && x.defaultFactionType.defName.Contains("Mechanoid"))*/) && !x.race.race.body.defName.Contains("AIRobot"))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMMechanoid;
            }
            else if (x == XenomorphDefOf.RRY_Xenomorph_Predalien)
            {
                return YautjaDefOf.RRY_Hediff_BloodedMPredalien;
            }
            else if (x.race == YautjaDefOf.RRY_Alien_Yautja && x.defaultFactionType == YautjaDefOf.RRY_Yautja_BadBloodFaction)
            {
                return YautjaDefOf.RRY_Hediff_BloodedMBadBlood;
            }
            else if (x.race.defName.Contains("Human") && !x.factionLeader)
            {
                return YautjaDefOf.RRY_Hediff_BloodedMHuman;
            }
            else if (x.race.defName.Contains("Human") && (x.factionLeader || (x.isFighter && x.combatPower > (100))))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMWorthyHuman;
            }
            else if (!x.race.defName.Contains("Human") && x.RaceProps.Humanlike && (/*!x.race.defName.Contains("Yautja") &&*/ !x.race.defName.Contains("Mech") && !x.race.defName.Contains("Droid") && !x.race.defName.Contains("Android") && !x.race.defName.Contains("ChjDroid") && !x.race.defName.Contains("ChjAndroid")) && !x.factionLeader)
            {
                return YautjaDefOf.RRY_Hediff_BloodedMHumanlike;
            }
            else if (!x.race.defName.Contains("Human") && x.RaceProps.Humanlike && ( /*!x.race.defName.Contains("Yautja") &&*/ !x.race.defName.Contains("Mech") && !x.race.defName.Contains("Droid") && !x.race.defName.Contains("Android") && !x.race.defName.Contains("ChjDroid") && !x.race.defName.Contains("ChjAndroid")) && (x.factionLeader || (x.isFighter && x.combatPower > (100))))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMWorthyHumanlike;
            }
            /*
            else if (x.race == YautjaDefOf.RRY_Alien_Yautja && !other.story.adulthood.identifier.StartsWith("Yautja_BadBlood") && other.Dead && (other.Faction.PlayerGoodwill > 0 || other.Faction.IsPlayer)) 
            {
                return YautjaDefOf.RRY_Hediff_BloodedMBadBlood;
            }
            */
            else if (x.race.defName.Contains("GroTye") || x.defName.Contains("Megasloth"))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMGroTye;
            }
            else if (!x.race.defName.Contains("Xenomorph") && (x.defName.Contains("Rhinoceros") || x.defName.Contains("Elephant")))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMCrusher;
            }
            else if (!x.race.defName.Contains("Xenomorph") && x.defName.Contains("Thrumbo"))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMThrumbo;
            }
            else if (!x.race.defName.Contains("Xenomorph") && !x.RaceProps.Humanlike && ((x.defName.Contains("Wolf") || x.race.description.Contains("Wolf") || x.race.description.Contains("wolf") || x.race.description.Contains("wolves")) || (x.defName.Contains("Hound") || x.defName.Contains("hound") || x.race.description.Contains("Hound") || x.race.description.Contains("hound") || x.race.description.Contains("hounds")) || (x.race.defName.Contains("Dog") || x.race.description.Contains("Dog") || x.race.description.Contains("dog") || x.race.description.Contains("dogs"))) && ((x.RaceProps.predator == true && x.combatPower > 50) || (x.RaceProps.predator == false && x.combatPower > 100)))
            {
                return YautjaDefOf.RRY_Hediff_BloodedMHound;
            }
            else if ((!x.RaceProps.IsMechanoid && (x.defaultFactionType == null || (x.defaultFactionType != null && !x.defaultFactionType.defName.Contains("Mechanoid")))) && !x.race.defName.Contains("Xenomorph") && !x.RaceProps.Humanlike && (x.combatPower > 100 || (x.RaceProps.predator == true && x.combatPower > 50)))
            {
                return YautjaDefOf.RRY_Hediff_BloodedM;
            }
            return null;
        }

        public static bool WorthyKill(PawnKindDef x)
        {
            if (GetMark(x)!=null)
            {
                return true;
            }
            return false;
        }
    }
}
