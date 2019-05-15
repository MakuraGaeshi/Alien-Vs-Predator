using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using static RRYautja.YautjaBloodedUtility;

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
        public HediffDef MarkedhediffDef;
        public Corpse corpse;
        public Pawn pawn;
        public string MarkHedifftype;
        public string MarkHedifflabel;
        public bool predator;
        public float combatPower;
        public float BodySize;
    }
    public class HediffComp_BloodedYautja : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_BloodedYautja HediffProps
        {
            get
            {
                try
                {
                    return (HediffCompProperties_BloodedYautja)this.props;
                }
                catch
                {
                    return new HediffCompProperties_BloodedYautja();
                }
            }
        }
        public HediffDef MarkedhediffDef;
        public Corpse corpse;
        public Pawn pawn;
        public PawnKindDef pawnKindDef;
        public string MarkHedifftype;
        public string MarkHedifflabel;
        public bool predator;
        public float combatPower;
        public float BodySize;

        //    Scribe_Defs.Look<HediffDef>(ref MarkedhediffDef, "MarkedhediffDef");
        //    Scribe_Deep.Look<TaleReference>(ref this.taleRef, "taleRef", new object[0]);
        //    Scribe_References.Look<Lord>(ref this.lord, "defenseLord", false);
        //    Scribe_Values.Look<float>(ref this.pointsLeft, "mechanoidPointsLeft", 0f, false);
        public override void CompExposeData()
        {
            base.CompExposeData();
        }



        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
        }
        public override string CompLabelInBracketsExtra
        {
            get
            {
                Comp_Yautja _Yautja = this.Pawn.TryGetComp<Comp_Yautja>();
                if (_Yautja != null)
                {
                    return _Yautja.MarkHedifflabel;
                }
                return null;
            }
        }

        /*
        public override string CompLabelInBracketsExtra
        {
            get
            {
                Comp_Yautja _Yautja = this.Pawn.TryGetComp<Comp_Yautja>();


                return null;
                if (_Yautja!=null)
                {
                    return _Yautja.markCorpse.InnerPawn.KindLabel;
                }
                return this.bloodStatus.GetLabel(HediffProps.pawn);
                return this.HediffProps.pawn.KindLabel;
            }
        }
        */
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
        }
    }


    // Token: 0x02000D5A RID: 3418
    public class HediffCompProperties_MarkedYautja : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_MarkedYautja()
        {
            this.compClass = typeof(HediffComp_MarkedYautja);
        }
        public HediffDef MarkedhediffDef;
        public Corpse corpse;
        public Pawn pawn;
        public PawnKindDef pawnKindDef;
        public string MarkHedifftype;
        public string MarkHedifflabel;
        public bool predator;
        public float combatPower;
        public float BodySize;

    }
    public class HediffComp_MarkedYautja : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_MarkedYautja Props
        {
            get
            {
                try
                {
                    return (HediffCompProperties_MarkedYautja)this.props;
                }
                catch
                {
                    return new HediffCompProperties_MarkedYautja();
                }
            }
        }
        public HediffDef MarkedhediffDef;
        public Corpse corpse;
        public Pawn pawn;
        public PawnKindDef pawnKindDef;
        public string MarkHedifftype;
        public string MarkHedifflabel;
        public bool predator;
        public float combatPower;
        public float BodySize;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Defs.Look<HediffDef>(ref this.MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef", true);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef", true);
            Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");
            Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");
            Scribe_Values.Look<bool>(ref this.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                Comp_Yautja _Yautja = this.Pawn.TryGetComp<Comp_Yautja>();
                if (_Yautja != null)
                {
                    return _Yautja.MarkHedifflabel;
                }
                if (MarkHedifflabel!=null)
                {
                    return MarkHedifflabel;
                }
                return null;
            }
        }


        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (MarkedhediffDef == null)
            {
                MarkedhediffDef = this.Def;
                corpse = null;
                pawn = null;
                pawnKindDef = YautjaBloodedUtility.RandomMarked(MarkedhediffDef);
                Log.Message(string.Format("{0}: {1}", MarkedhediffDef, pawnKindDef));
                MarkHedifftype = null;
                MarkHedifflabel = pawnKindDef.LabelCap;
                predator = pawnKindDef.RaceProps.predator;
                combatPower = pawnKindDef.combatPower;
                BodySize = pawnKindDef.race.race.baseBodySize;
                Comp_Yautja _Yautja = this.Pawn.TryGetComp<Comp_Yautja>();
                if (_Yautja != null)
                {
                    _Yautja.MarkHedifflabel = MarkHedifflabel;
                    _Yautja.pawn = pawn;
                    _Yautja.corpse = corpse;
                    _Yautja.MarkedhediffDef = MarkedhediffDef;
                    _Yautja.MarkHedifflabel = MarkHedifflabel;
                    _Yautja.predator = predator;
                    _Yautja.BodySize = BodySize;
                    _Yautja.combatPower = combatPower;
                }
            }

        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

        }
    }
}
