using RimWorld;
using System;
using Verse;

namespace RRYautja
{
    // Token: 0x02000D8F RID: 3471
    public class HediffGiver_Synth : HediffGiver
    {
        private bool done = false;
        // Token: 0x06004D1C RID: 19740 RVA: 0x0023C685 File Offset: 0x0023AA85
        public override void OnIntervalPassed(Pawn pawn, Hediff cause) 
        {
            if (pawn.def == USCMDefOf.AvP_Synth)
            {
                // Rand.MTBEventOccurs(this.mtbDays, 60000f, 60f)
                BodyPartRecord record = pawn.RaceProps.body.AllParts.Find(x => x.def.defName == "AvP_BehavorialInhibitor");
                float chance = 1f - (pawn.health.hediffSet.GetPartHealth(record) / record.def.GetMaxHealth(pawn));
                //    Log.Message(string.Format("{0}, {1}", pawn.LabelShortCap, chance));
                bool has = (pawn.health.hediffSet.HasHediff(USCMDefOf.AvP_Damaged_Inhibitor) && pawn.health.hediffSet.HasHediff(USCMDefOf.AvP_Defective_Inhibitor));
                if (!done && Rand.Chance(chance))
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);
                    done = true;
                }
                if (Rand.MTBEventOccurs(this.mtbDays, 60000f, 60f) && Rand.Chance(chance) && base.TryApply(pawn, null))
                {
                    //    Log.Message(string.Format("Fire Berserk for {0}, Chance: {1}", pawn.LabelShortCap, chance));
                    /*
                    if (!has)
                    {
                        pawn.health.AddHediff(USCMDefOf.AvP_Damaged_Inhibitor, record);
                    }
                    */
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);
                    //    base.SendLetter(pawn, cause);
                }
                
            }
        }

        // Token: 0x04003434 RID: 13364
        public float mtbDays;
    }
}
