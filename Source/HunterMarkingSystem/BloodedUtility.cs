using System;
using System.Collections.Generic;
using RimWorld;
using System.Text;
using Verse;
using HunterMarkingSystem.ExtensionMethods;

namespace HunterMarkingSystem
{
    public static class HMSUtility
    {

        // Token: 0x02000D68 RID: 3432
        public enum BloodStatusMode
        {
            NoComp,
            None,
            Unblooded,
            Unmarked,
            Marked
        }

        // Token: 0x06004C44 RID: 19524 RVA: 0x0023802D File Offset: 0x0023642D
        public static string GetLabel(this BloodStatusMode m, Pawn pawn)
        {
            if (pawn.Markable(out Comp_Markable Markable))
            {

                switch (m)
                {
                    case BloodStatusMode.Unblooded:
                        return "HMS_BloodStatus_Unblooded".Translate(pawn.LabelShortCap, Markable.markDataSelf.MarkScore);
                    case BloodStatusMode.Unmarked:
                        return "HMS_BloodStatus_Unmarked".Translate(pawn.LabelShortCap, Markable.markDataKillNew.Label, Markable.markDataKillNew.raceDef.LabelCap, Markable.markDataKillNew.MarkScore);
                    case BloodStatusMode.Marked:
                        return "HMS_BloodStatus_Marked".Translate(pawn.LabelShortCap, Markable.markDataKill.MarkDef, Markable.markDataKill.raceDef.LabelCap, Markable.markDataKill.MarkScore);
                    default:
                        return "HMS_BloodStatus_Uninitalised".Translate(pawn.LabelShortCap);
                }
            }
            return "HMS_BloodStatus_Uninitalised".Translate(pawn.LabelShortCap);
        }

        public static HediffDef GetMark(Pawn x)
        {
            List<HediffDef> list = GetMarks(x.def);
            HediffDef hediff = GetMark(x.def);

            if (x.RaceProps.Humanlike)
            {
                if (x.kindDef.factionLeader)
                {
                    if (list.Any(y => y.defName.Contains("Worthy")))
                    {
                        hediff = list.Find(y=> y.defName.Contains("Worthy"));
                    }
                }
            }

            return hediff;
        }

        public static List<HediffDef> GetMarks(Pawn x)
        {
            return GetMarks(x.def);
        }

        public static HediffDef GetMark(PawnKindDef x)
        {
            return GetMark(x.race);
        }
        public static List<HediffDef> GetMarks(PawnKindDef x)
        {
            return GetMarks(x.race);
        }

        public static HediffDef GetMark(ThingDef x)
        {
        //    Log.Warning(string.Format("GetMark for {0}", x.defName));
            bool ArtificalPawn = x.race.FleshType.defName.Contains("Flesh_Construct") || x.race.FleshType.defName.Contains("Deamon") || x.race.body.defName.Contains("AIRobot") || x.race.IsMechanoid || (UtilChjAndroids.ChjAndroid && UtilChjAndroids.isChjAndroid(x)) || (UtilTieredAndroids.TieredAndroid && UtilTieredAndroids.isAtlasAndroid(x));
        //    Log.Warning(string.Format("ArtificalPawn {0}", ArtificalPawn));
            if (x.HasModExtension<MarkDefExtension>())
            {
                if (!x.GetModExtension<MarkDefExtension>().hediffs.NullOrEmpty())
                {
                //    Log.Message(string.Format("Default hediff for {0} = {1}", x.LabelCap, x.GetModExtension<MarkDefExtension>().hediffs[0].stages[0].label.CapitalizeFirst()));
                    return x.GetModExtension<MarkDefExtension>().hediffs[0];
                }
                else
                {
                    Log.Warning(string.Format("MarkDefExtension found for {0} but no Hediffs found", x.defName));
                }
            }
            else
            {
            //    Log.Warning(string.Format("No MarkDefExtension found for {0}", x.defName));
            }
            if (ArtificalPawn)
            {
                return HMSDefOf.HMS_Hediff_BloodedMMechanoid;
            }
            else
            if (x.race.Humanlike)
            {
                if (x.defName.Contains("Human") || x == ThingDefOf.Human || x.race.meatDef == ThingDefOf.Meat_Human)
                {
                    return HMSDefOf.HMS_Hediff_BloodedMHuman;
                    return HMSDefOf.HMS_Hediff_BloodedMWorthyHuman;
                }
                else
                {
                    return HMSDefOf.HMS_Hediff_BloodedMHumanlike;
                    return HMSDefOf.HMS_Hediff_BloodedMWorthyHumanlike;
                }
            }
            else
            {

                if (x.defName.Contains("Xenomorph") && !x.defName.Contains("FaceHugger") && !x.defName.Contains("Predalien") && !x.defName.Contains("Queen"))
                {
                    return HMSDefOf.HMS_Hediff_BloodedMXenomorph;
                }
                else if (x.defName.Contains("GroTye") || x.defName.Contains("Megasloth"))
                {
                    return HMSDefOf.HMS_Hediff_BloodedMGroTye;
                }
                else if (!x.defName.Contains("Xenomorph") && (x.defName.Contains("Rhinoceros") || x.defName.Contains("Elephant")))
                {
                    return HMSDefOf.HMS_Hediff_BloodedMCrusher;
                }
                else if (x.defName.Contains("Thrumbo") && !x.race.Humanlike)
                {
                    return HMSDefOf.HMS_Hediff_BloodedMThrumbo;
                }
                else if (!x.race.Humanlike && ((x.defName.Contains("Wolf") || x.description.Contains("Wolf") || x.description.Contains("wolf") || x.description.Contains("wolves")) || (x.defName.Contains("Hound") || x.defName.Contains("hound") || x.description.Contains("Hound") || x.description.Contains("hound") || x.description.Contains("hounds")) || (x.defName.Contains("Dog") || x.description.Contains("Dog") || x.description.Contains("dog") || x.description.Contains("dogs"))) && ((x.race.predator == true)))
                {
                    return HMSDefOf.HMS_Hediff_BloodedMHound;
                }
                else
                {
                    return HMSDefOf.HMS_Hediff_BloodedM;
                }
            }
            return null;
        }

        public static List<HediffDef> GetMarks(ThingDef x)
        {
            //    Log.Message(string.Format("{0}", markedDef)); 
            List<HediffDef> Marks = new List<HediffDef>();
            if (x.HasModExtension<MarkDefExtension>())
            {
                return x.GetModExtension<MarkDefExtension>().hediffs;
            }
            else if (x.defName.Contains("GroTye") || x.defName.Contains("Megasloth"))
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMGroTye);
            }
            else if ((x.defName.Contains("Rhinoceros") || x.defName.Contains("Elephant")))
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMCrusher);
            }
            else if (!x.defName.Contains("Human") && ((x.defName.Contains("WraithGuard") || x.defName.Contains("Necron") || x.defName.Contains("Mech") || x.defName.Contains("Droid") || x.defName.Contains("Android") || x.defName.Contains("ChjDroid") || x.defName.Contains("ChjAndroid")) || x.race.IsMechanoid/* || (x.defaultFactionType != null && x.defaultFactionType.defName.Contains("Mechanoid"))*/) && !x.race.body.defName.Contains("AIRobot"))
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMMechanoid);
            }
            else if (x.defName.Contains("Human"))
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMHuman);
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMWorthyHuman);
            }
            else if (!x.defName.Contains("Human") && x.race.Humanlike && (!x.defName.Contains("Mech") && !x.defName.Contains("Droid") && !x.defName.Contains("Android") && !x.defName.Contains("ChjDroid") && !x.defName.Contains("ChjAndroid")))
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMHumanlike);
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMWorthyHumanlike);
            }
            else if (x.defName.Contains("Thrumbo") && !x.race.Humanlike)
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMThrumbo);
            }
            else if (!x.race.Humanlike && ((x.defName.Contains("Wolf") || x.description.Contains("Wolf") || x.description.Contains("wolf") || x.description.Contains("wolves")) || (x.defName.Contains("Hound") || x.defName.Contains("hound") || x.description.Contains("Hound") || x.description.Contains("hound") || x.description.Contains("hounds")) || (x.defName.Contains("Dog") || x.description.Contains("Dog") || x.description.Contains("dog") || x.description.Contains("dogs"))) && ((x.race.predator == true)))
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedMHound);
            }
            else
            {
                Marks.Add(HMSDefOf.HMS_Hediff_BloodedM);
            }
            return Marks;
        }
    }
}
