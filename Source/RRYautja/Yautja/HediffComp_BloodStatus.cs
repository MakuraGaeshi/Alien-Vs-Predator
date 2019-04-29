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
        public string MarkHedifftype;
        public string MarkHedifflabel;
        public bool predator;
        public float combatPower;
        public float BodySize;


        private HediffDef MarkedhediffDefp;
        private Corpse corpsep;
        private Pawn pawnp;
        private string MarkHedifftypep;
        private string MarkHedifflabelp;
        private bool predatorp;
        private float combatPowerp;
        private float BodySizep;

        //    Scribe_Defs.Look<HediffDef>(ref MarkedhediffDef, "MarkedhediffDef");
        //    Scribe_Deep.Look<TaleReference>(ref this.taleRef, "taleRef", new object[0]);
        //    Scribe_References.Look<Lord>(ref this.lord, "defenseLord", false);
        //    Scribe_Values.Look<float>(ref this.pointsLeft, "mechanoidPointsLeft", 0f, false);
        public override void CompExposeData()
        {
            base.CompExposeData();
        //    Scribe_Values.Look<HediffDef>(ref MarkedhediffDef, "MarkedhediffDef", Props.MarkedhediffDef, true);
            Scribe_Defs.Look<HediffDef>(ref this.HediffProps.MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.HediffProps.corpse, "corpseRef", true);
            Scribe_References.Look<Pawn>(ref this.HediffProps.pawn, "pawnRef", true);
            Scribe_Values.Look<String>(ref this.HediffProps.MarkHedifftype, "thisMarktype");
            Scribe_Values.Look<String>(ref this.HediffProps.MarkHedifflabel, "thislabel");
            Scribe_Values.Look<bool>(ref this.HediffProps.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.HediffProps.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.HediffProps.BodySize, "thisBodySize");

            /*
             
            Scribe_Defs.Look<HediffDef>(ref MarkedhediffDef, "MarkedhediffDef");
            Scribe_References.Look<Corpse>(ref this.corpse, "corpseRef");//, Props.corpse);//
            Scribe_References.Look<Pawn>(ref this.pawn, "pawnRef");//, Props.pawn);
            Scribe_Values.Look<String>(ref this.MarkHedifftype, "thisMarktype");//, Props.Marklabel);
            Scribe_Values.Look<String>(ref this.MarkHedifflabel, "thislabel");//, Props.Marklabel);
            Scribe_Values.Look<bool>(ref this.predator, "thisPred");
            Scribe_Values.Look<float>(ref this.combatPower, "thiscombatPower");
            Scribe_Values.Look<float>(ref this.BodySize, "thisBodySize");
            */
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            Log.Message(string.Format("CompPostPostAdd: {0} added to {1}", this.parent.LabelCap, this.Pawn.Name.ToStringShort));
            //Log.Message(string.Format("CompPostPostAdd: this.Props:{0}", this.HediffProps.ToStringSafe()));
            /*
            this.MarkedhediffDefp = this.MarkedhediffDef;
            this.corpsep= this.corpse;
            this.pawnp= this.pawn;
            this.MarkHedifftypep= this.MarkHedifftype;
            this.MarkHedifflabelp= this.MarkHedifflabel;
            this.predatorp= this.predator;
            this.combatPowerp= this.combatPower;
            this.BodySizep= this.BodySize;
            */

            //this.parent.CurStage.label = this.HediffProps.pawn.KindLabel;
            base.CompPostPostAdd(dinfo);
        }

        public override void CompPostMake()
        {
            Log.Message(string.Format("CompPostMake: {0} added to {1}", this.parent.LabelCap, this.Pawn.Name.ToStringShort));

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

            //  this.parent.CurStage.label = this.HediffProps.pawn.KindLabel;
            //  this.parent.def.stages[0].label = this. this.MarkHedifftype;
            //  this.parent.def.stages[1].label = this.MarkHedifflabel;
            //  MarkedhediffDef = DefDatabase<HediffDef>.GetNamed(MarkHedifftype);

        }
        public override void CompPostPostRemoved()
        {
            
            base.CompPostPostRemoved();
            /*
            Hediff bloodedUM = Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM);
            BodyPartRecord part = Pawn.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_Hediff_BloodedUM).Part;
            Pawn.health.RemoveHediff(bloodedUM);
            Hediff blooded = HediffMaker.MakeHediff(hediffDef, Pawn, null);
            Pawn.health.AddHediff(blooded, part, null);
            */
        }
        public BloodStatusMode bloodStatus;
    }


    // Token: 0x02000D5A RID: 3418
    public class HediffCompProperties_MarkedYautja : HediffCompProperties
    {
        // Token: 0x06004C0D RID: 19469 RVA: 0x00237094 File Offset: 0x00235494
        public HediffCompProperties_MarkedYautja()
        {
            this.compClass = typeof(HediffComp_MarkedYautja);
        }


    }
    public class HediffComp_MarkedYautja : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_MarkedYautja Props
        {
            get
            {
                return (HediffCompProperties_MarkedYautja)this.props;
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
