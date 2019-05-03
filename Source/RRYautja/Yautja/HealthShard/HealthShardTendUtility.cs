﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RRYautja
{
    // Token: 0x0200000E RID: 14
    public static class HealthShardTendUtility
    {
        // Token: 0x0600002F RID: 47 RVA: 0x00002870 File Offset: 0x00000A70
        public static void DoTend(Pawn doctor, Pawn patient, Cloakgen medkit)
        {
            bool flag = patient.health.HasHediffsNeedingTend(false);
            if (flag)
            {
                bool flag2 = medkit != null && medkit.Destroyed;
                if (flag2)
                {
                    Log.Warning("Tried to use destroyed medkit.", false);
                    medkit = null;
                }
                float num = HealthShardTendUtility.CalculateBaseTendQuality(doctor, patient, medkit.kitComp.Props.medicine ?? null);
                HealthShardTendUtility.GetOptimalHediffsToTendWithSingleTreatment(patient, medkit.kitComp.Props.medicine != null, HealthShardTendUtility.tmpHediffsToTend, null);
                for (int i = 0; i < HealthShardTendUtility.tmpHediffsToTend.Count; i++)
                {
                    HealthShardTendUtility.tmpHediffsToTend[i].Tended(num, i);
                }
                bool flag3 = doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null;
                if (flag3)
                {
                    patient.mindState.timesGuestTendedToByPlayer++;
                }
                bool flag4 = doctor != null && doctor.IsColonistPlayerControlled;
                if (flag4)
                {
                    patient.records.AccumulateStoryEvent(StoryEventDefOf.TendedByPlayer);
                }
                bool flag5 = doctor != null && doctor.RaceProps.Humanlike && patient.RaceProps.Animal;
                if (flag5)
                {
                    bool flag6 = RelationsUtility.TryDevelopBondRelation(doctor, patient, 0.004f);
                    if (flag6)
                    {
                        bool flag7 = doctor.Faction != null && doctor.Faction != patient.Faction;
                        if (flag7)
                        {
                            InteractionWorker_RecruitAttempt.DoRecruit(doctor, patient, 1f, false);
                        }
                    }
                }
                patient.records.Increment(RecordDefOf.TimesTendedTo);
                bool flag8 = doctor != null;
                if (flag8)
                {
                    doctor.records.Increment(RecordDefOf.TimesTendedOther);
                }
                bool flag9 = doctor == patient && !doctor.Dead;
                if (flag9)
                {
                    doctor.mindState.Notify_SelfTended();
                }
                bool flag10 = medkit.kitComp.Props.medicine != null;
                if (flag10)
                {
                    bool flag11 = patient.Spawned || (doctor != null && doctor.Spawned);
                    if (flag11)
                    {
                        bool flag12 = medkit.kitComp.Props.medicine != null && StatExtension.GetStatValueAbstract(medkit.kitComp.Props.medicine, StatDefOf.MedicalPotency, null) > StatExtension.GetStatValueAbstract(ThingDefOf.MedicineIndustrial, StatDefOf.MedicalPotency, null);
                        if (flag12)
                        {
                            SoundStarter.PlayOneShot(SoundDefOf.TechMedicineUsed, new TargetInfo(patient.Position, patient.Map, false));
                        }
                    }
                }
            }
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00002870 File Offset: 0x00000A70
        public static void UseShard(Pawn doctor, Pawn patient, Cloakgen medkit)
        {
            bool flag = patient.health.HasHediffsNeedingTend(false);
            Log.Message(string.Format("UseShard flag: {0}, doctor: {1}, patient: {2}", flag, doctor ,patient));
            if (flag)
            {
                Hediff hediff = HealthShardTendUtility.FindLifeThreateningHediff(patient);
                if (hediff != null)
                {
                    Log.Message(string.Format("hediff: {0}", hediff));
                    medkit.UseKit();
                    HealthShardTendUtility.Cure(hediff);
                    return;
                }
                if (HealthUtility.TicksUntilDeathDueToBloodLoss(patient) < 2500)
                {
                    Log.Message(string.Format("TicksUntilDeathDueToBloodLoss: {0}", HealthUtility.TicksUntilDeathDueToBloodLoss(patient)));
                    Hediff hediff2 = HealthShardTendUtility.FindMostBleedingHediff(patient);
                    if (hediff2 != null)
                    {
                        Log.Message(string.Format("hediff2: {0}", hediff2));
                        medkit.UseKit();
                        HealthShardTendUtility.Cure(hediff2);
                        return;
                    }
                }
                Hediff_Injury hediff_Injury3 = HealthShardTendUtility.FindInjury(patient, null);
                if (hediff_Injury3 != null)
                {
                    Log.Message(string.Format("hediff2: {0}", hediff_Injury3));
                    medkit.UseKit();
                    HealthShardTendUtility.Cure(hediff_Injury3);
                    return;
                }
            }
        }

        // Token: 0x06002ACE RID: 10958 RVA: 0x00142BE4 File Offset: 0x00140FE4
        private static Hediff FindLifeThreateningHediff(Pawn pawn)
        {
            Hediff hediff = null;
            float num = -1f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                Log.Message(string.Format("name: {0}, {1} of {2}", hediffs[i], i, hediffs.Count));
                if (hediffs[i].Visible && hediffs[i].def.everCurableByItem)
                {
                    Log.Message(string.Format("name: {0}, {1} of {2} (hediffs[i].Visible && hediffs[i].def.everCurableByItem)", hediffs[i], i, hediffs.Count));
                    if (!hediffs[i].FullyImmune())
                    {
                        Log.Message(string.Format("name: {0}, {1} of {2} !hediffs[i].FullyImmune()", hediffs[i], i, hediffs.Count));
                        HediffStage curStage = hediffs[i].CurStage;
                        bool flag = curStage != null && curStage.lifeThreatening;
                        bool flag2 = hediffs[i].def.lethalSeverity >= 0f && hediffs[i].Severity / hediffs[i].def.lethalSeverity >= 0.8f;
                        if (flag || flag2)
                        {
                            Log.Message(string.Format("name: {0}, {1} of {2} flag || flag2", hediffs[i], i, hediffs.Count));
                            float num2 = (hediffs[i].Part == null) ? 999f : hediffs[i].Part.coverageAbsWithChildren;
                            if (hediff == null || num2 > num)
                            {
                                Log.Message(string.Format("name: {0}, {1} of {2} hediff == null || num2 > num", hediffs[i], i, hediffs.Count));
                                hediff = hediffs[i];
                                num = num2;
                            }
                        }
                    }
                }
            }
            return hediff;
        }

        // Token: 0x06002ACF RID: 10959 RVA: 0x00142D28 File Offset: 0x00141128
        private static Hediff FindMostBleedingHediff(Pawn pawn)
        {
            float num = 0f;
            Hediff hediff = null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i].Visible && hediffs[i].def.everCurableByItem)
                {
                    float bleedRate = hediffs[i].BleedRate;
                    if (bleedRate > 0f && (bleedRate > num || hediff == null))
                    {
                        num = bleedRate;
                        hediff = hediffs[i];
                    }
                }
            }
            return hediff;
        }

        private static Hediff_Injury FindInjury(Pawn pawn, IEnumerable<BodyPartRecord> allowedBodyParts = null)
        {
            Hediff_Injury hediff_Injury = null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff_Injury hediff_Injury2 = hediffs[i] as Hediff_Injury;
                if (hediff_Injury2 != null && hediff_Injury2.Visible && hediff_Injury2.def.everCurableByItem)
                {
                    if (allowedBodyParts == null || allowedBodyParts.Contains(hediff_Injury2.Part))
                    {
                        if (hediff_Injury == null || hediff_Injury2.Severity > hediff_Injury.Severity)
                        {
                            hediff_Injury = hediff_Injury2;
                        }
                    }
                }
            }
            return hediff_Injury;
        }
        private static void Cure(Hediff hediff)
        {
            Pawn pawn = hediff.pawn;
            pawn.health.RemoveHediff(hediff);
            if (hediff.def.cureAllAtOnceIfCuredByItem)
            {
                int num = 0;
                for (; ; )
                {
                    num++;
                    if (num > 10000)
                    {
                        break;
                    }
                    Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff.def, false);
                    if (firstHediffOfDef == null)
                    {
                        goto Block_3;
                    }
                    pawn.health.RemoveHediff(firstHediffOfDef);
                }
                Log.Error("Too many iterations.", false);
                Block_3:;
            }
            Messages.Message("MessageHediffCuredByItem".Translate(hediff.LabelBase.CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent, true);
        }
        // Token: 0x06000030 RID: 48 RVA: 0x00002B04 File Offset: 0x00000D04
        public static float CalculateBaseTendQuality(Pawn doctor, Pawn patient, ThingDef medicine)
        {
            float medicinePotency = (medicine == null) ? 0.3f : StatExtension.GetStatValueAbstract(medicine, StatDefOf.MedicalPotency, null);
            float medicineQualityMax = (medicine == null) ? 0.7f : StatExtension.GetStatValueAbstract(medicine, StatDefOf.MedicalQualityMax, null);
            return HealthShardTendUtility.CalculateBaseTendQuality(doctor, patient, medicinePotency, medicineQualityMax);
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00002B50 File Offset: 0x00000D50
        public static float CalculateBaseTendQuality(Pawn doctor, Pawn patient, float medicinePotency, float medicineQualityMax)
        {
            bool flag = doctor != null;
            float num;
            if (flag)
            {
                num = StatExtension.GetStatValue(doctor, StatDefOf.MedicalTendQuality, true);
            }
            else
            {
                num = 0.75f;
            }
            num *= medicinePotency;
            Building_Bed building_Bed = (patient != null) ? RestUtility.CurrentBed(patient) : null;
            bool flag2 = building_Bed != null;
            if (flag2)
            {
                num += StatExtension.GetStatValue(building_Bed, StatDefOf.MedicalTendQualityOffset, true);
            }
            bool flag3 = doctor == patient && doctor != null;
            if (flag3)
            {
                num *= 0.7f;
            }
            return Mathf.Clamp(num, 0f, medicineQualityMax);
        }

        // Token: 0x06000032 RID: 50 RVA: 0x00002BD8 File Offset: 0x00000DD8
        public static void GetOptimalHediffsToTendWithSingleTreatment(Pawn patient, bool usingMedicine, List<Hediff> outHediffsToTend, List<Hediff> tendableHediffsInTendPriorityOrder = null)
        {
            outHediffsToTend.Clear();
            HealthShardTendUtility.tmpHediffs.Clear();
            bool flag = tendableHediffsInTendPriorityOrder != null;
            if (flag)
            {
                HealthShardTendUtility.tmpHediffs.AddRange(tendableHediffsInTendPriorityOrder);
            }
            else
            {
                List<Hediff> hediffs = patient.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    bool flag2 = hediffs[i].TendableNow(false);
                    if (flag2)
                    {
                        HealthShardTendUtility.tmpHediffs.Add(hediffs[i]);
                    }
                }
                HealthShardTendUtility.SortByTendPriority(HealthShardTendUtility.tmpHediffs);
            }
            bool flag3 = GenCollection.Any<Hediff>(HealthShardTendUtility.tmpHediffs);
            if (flag3)
            {
                Hediff hediff = HealthShardTendUtility.tmpHediffs[0];
                outHediffsToTend.Add(hediff);
                HediffCompProperties_TendDuration hediffCompProperties_TendDuration = hediff.def.CompProps<HediffCompProperties_TendDuration>();
                bool flag4 = hediffCompProperties_TendDuration != null && hediffCompProperties_TendDuration.tendAllAtOnce;
                if (flag4)
                {
                    for (int j = 0; j < HealthShardTendUtility.tmpHediffs.Count; j++)
                    {
                        bool flag5 = HealthShardTendUtility.tmpHediffs[j] != hediff && HealthShardTendUtility.tmpHediffs[j].def == hediff.def;
                        if (flag5)
                        {
                            outHediffsToTend.Add(HealthShardTendUtility.tmpHediffs[j]);
                        }
                    }
                }
                else
                {
                    bool flag6 = hediff is Hediff_Injury && usingMedicine;
                    if (flag6)
                    {
                        float num = hediff.Severity;
                        for (int k = 0; k < HealthShardTendUtility.tmpHediffs.Count; k++)
                        {
                            bool flag7 = HealthShardTendUtility.tmpHediffs[k] != hediff;
                            if (flag7)
                            {
                                Hediff_Injury hediff_Injury = HealthShardTendUtility.tmpHediffs[k] as Hediff_Injury;
                                bool flag8 = hediff_Injury != null;
                                if (flag8)
                                {
                                    float severity = hediff_Injury.Severity;
                                    bool flag9 = num + severity <= 20f;
                                    if (flag9)
                                    {
                                        num += severity;
                                        outHediffsToTend.Add(hediff_Injury);
                                    }
                                }
                            }
                        }
                    }
                }
                HealthShardTendUtility.tmpHediffs.Clear();
            }
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00002DE4 File Offset: 0x00000FE4
        public static void SortByTendPriority(List<Hediff> hediffs)
        {
            bool flag = hediffs.Count > 1;
            if (flag)
            {
                HealthShardTendUtility.tmpHediffsWithTendPriority.Clear();
                for (int i = 0; i < hediffs.Count; i++)
                {
                    HealthShardTendUtility.tmpHediffsWithTendPriority.Add(new Pair<Hediff, float>(hediffs[i], hediffs[i].TendPriority));
                }
                GenCollection.SortByDescending<Pair<Hediff, float>, float, float>(HealthShardTendUtility.tmpHediffsWithTendPriority, (Pair<Hediff, float> x) => x.Second, (Pair<Hediff, float> x) => x.First.Severity);
                hediffs.Clear();
                for (int j = 0; j < HealthShardTendUtility.tmpHediffsWithTendPriority.Count; j++)
                {
                    hediffs.Add(HealthShardTendUtility.tmpHediffsWithTendPriority[j].First);
                }
                HealthShardTendUtility.tmpHediffsWithTendPriority.Clear();
            }
        }

        // Token: 0x04000015 RID: 21
        public const float NoMedicinePotency = 0.3f;

        // Token: 0x04000016 RID: 22
        public const float NoMedicineQualityMax = 0.7f;

        // Token: 0x04000017 RID: 23
        public const float NoDoctorTendQuality = 0.75f;

        // Token: 0x04000018 RID: 24
        public const float SelfTendQualityFactor = 0.7f;

        // Token: 0x04000019 RID: 25
        private const float ChanceToDevelopBondRelationOnTended = 0.004f;

        // Token: 0x0400001A RID: 26
        private static List<Hediff> tmpHediffsToTend = new List<Hediff>();

        // Token: 0x0400001B RID: 27
        private static List<Hediff> tmpHediffs = new List<Hediff>();

        // Token: 0x0400001C RID: 28
        private static List<Pair<Hediff, float>> tmpHediffsWithTendPriority = new List<Pair<Hediff, float>>();
    }
}
