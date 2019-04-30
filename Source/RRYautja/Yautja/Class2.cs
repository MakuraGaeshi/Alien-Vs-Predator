using RRYautja;
using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x02000548 RID: 1352
    [StaticConstructorOnStartup]
    public abstract class RRY_Thought : IExposable
    {
        // Token: 0x1700038D RID: 909
        // (get) Token: 0x06001949 RID: 6473
        public abstract int CurStageIndex { get; }

        // Token: 0x1700038E RID: 910
        // (get) Token: 0x0600194A RID: 6474 RVA: 0x0004E80A File Offset: 0x0004CC0A
        public ThoughtStage CurStage
        {
            get
            {
                return this.def.stages[this.CurStageIndex];
            }
        }

        // Token: 0x1700038F RID: 911
        // (get) Token: 0x0600194B RID: 6475 RVA: 0x0004E822 File Offset: 0x0004CC22
        public virtual bool VisibleInNeedsTab
        {
            get
            {
                return this.CurStage.visible;
            }
        }

        // Token: 0x17000390 RID: 912
        // (get) Token: 0x0600194C RID: 6476 RVA: 0x0004E82F File Offset: 0x0004CC2F
        public virtual string LabelCap
        {
            get
            {
                Comp_Yautja _Yautja = pawn.TryGetComp<Comp_Yautja>();
                if (_Yautja!=null)
                {
                    
                }
                return this.CurStage.label.CapitalizeFirst();
            }
        }

        // Token: 0x17000391 RID: 913
        // (get) Token: 0x0600194D RID: 6477 RVA: 0x0004E841 File Offset: 0x0004CC41
        protected virtual float BaseMoodOffset
        {
            get
            {
                return this.CurStage.baseMoodEffect;
            }
        }

        // Token: 0x17000392 RID: 914
        // (get) Token: 0x0600194E RID: 6478 RVA: 0x0004E84E File Offset: 0x0004CC4E
        public string LabelCapSocial
        {
            get
            {
                if (this.CurStage.labelSocial != null)
                {
                    return this.CurStage.labelSocial.CapitalizeFirst();
                }
                return this.LabelCap;
            }
        }

        // Token: 0x17000393 RID: 915
        // (get) Token: 0x0600194F RID: 6479 RVA: 0x0004E878 File Offset: 0x0004CC78
        public string Description
        {
            get
            {
                string description = this.CurStage.description;
                if (description != null)
                {
                    return description;
                }
                return this.def.description;
            }
        }

        // Token: 0x17000394 RID: 916
        // (get) Token: 0x06001950 RID: 6480 RVA: 0x0004E8A4 File Offset: 0x0004CCA4
        public Texture2D Icon
        {
            get
            {
                if (this.def.Icon != null)
                {
                    return this.def.Icon;
                }
                if (this.MoodOffset() > 0f)
                {
                    return RRY_Thought.DefaultGoodIcon;
                }
                return RRY_Thought.DefaultBadIcon;
            }
        }

        // Token: 0x06001951 RID: 6481 RVA: 0x0004E8E3 File Offset: 0x0004CCE3
        public virtual void ExposeData()
        {
            Scribe_Defs.Look<ThoughtDef>(ref this.def, "def");
        }

        // Token: 0x06001952 RID: 6482 RVA: 0x0004E8F8 File Offset: 0x0004CCF8
        public virtual float MoodOffset()
        {
            if (this.CurStage == null)
            {
                Log.Error(string.Concat(new object[]
                {
                    "CurStage is null while ShouldDiscard is false on ",
                    this.def.defName,
                    " for ",
                    this.pawn
                }), false);
                return 0f;
            }
            float num = this.BaseMoodOffset;
            if (this.def.effectMultiplyingStat != null)
            {
                num *= this.pawn.GetStatValue(this.def.effectMultiplyingStat, true);
            }
            return num;
        }

        // Token: 0x06001953 RID: 6483 RVA: 0x0004E980 File Offset: 0x0004CD80
        public virtual bool GroupsWith(RRY_Thought other)
        {
            return this.def == other.def;
        }

        // Token: 0x06001954 RID: 6484 RVA: 0x0004E990 File Offset: 0x0004CD90
        public virtual void Init()
        {
        }

        // Token: 0x06001955 RID: 6485 RVA: 0x0004E992 File Offset: 0x0004CD92
        public override string ToString()
        {
            return "(" + this.def.defName + ")";
        }

        // Token: 0x04000F24 RID: 3876
        public Pawn pawn;

        // Token: 0x04000F25 RID: 3877
        public ThoughtDef def;


        String desc1 = string.Format("I've proven myself by killing a ");

        String desc2 = string.Format(" and marked myself with its blood. I feel amazing.");

        // Token: 0x04000F26 RID: 3878
        private static readonly Texture2D DefaultGoodIcon = ContentFinder<Texture2D>.Get("Things/Mote/ThoughtSymbol/GenericGood", true);

        // Token: 0x04000F27 RID: 3879
        private static readonly Texture2D DefaultBadIcon = ContentFinder<Texture2D>.Get("Things/Mote/ThoughtSymbol/GenericBad", true);
    }
}
