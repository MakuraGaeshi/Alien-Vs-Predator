using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_UnbloodedYautja : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_UnbloodedYautja()
        {
            this.compClass = typeof(HediffComp_UnbloodedYautja);
        }

        // Token: 0x040033C3 RID: 13251
    }
    // Token: 0x02000D5B RID: 3419
    public class HediffComp_UnbloodedYautja : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_UnbloodedYautja Props
		{
			get
			{
				return (HediffCompProperties_UnbloodedYautja)this.props;
			}
		}


    }

    // Token: 0x02000D5A RID: 3418
    public class HediffCompProperties_BloodedYautja : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_BloodedYautja()
        {
            this.compClass = typeof(HediffComp_BloodedYautja);
        }
        
    }
    public class HediffComp_BloodedYautja : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_BloodedYautja Props
        {
            get
            {
                return (HediffCompProperties_BloodedYautja)this.props;
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            base.Pawn.story.adulthood.identifier = "Yautja_Blooded";
            Hediff bloodedUM = Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
            BodyPartRecord part = Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM).Part;
            Pawn.health.RemoveHediff(bloodedUM);
            Hediff blooded = HediffMaker.MakeHediff(YautjaDefOf.RRY_Hediff_BloodedM, Pawn, null);
            Pawn.health.AddHediff(blooded, part, null);

        }
    }


    // Token: 0x02000D5A RID: 3418
    public class HediffCompProperties_BloodedYautjaM : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_BloodedYautjaM()
        {
            this.compClass = typeof(HediffComp_BloodedYautjaM);
        }


    }
    public class HediffComp_BloodedYautjaM : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_BloodedYautjaM Props
        {
            get
            {
                return (HediffCompProperties_BloodedYautjaM)this.props;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

        }
    }
}
