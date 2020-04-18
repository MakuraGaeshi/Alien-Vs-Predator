using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AvP
{
    public class CompProperties_SmartgunSystem : CompProperties
    {
        public CompProperties_SmartgunSystem()
        {
            this.compClass = typeof(CompSmartgunSystem);
        }
        public float WarmUpReduction = 1f;
        public bool WarmUpReductionQualityAffected = true;
        public ThingDef TargeterDef;
        public ThingDef HarnessDef;
    }

    // Token: 0x02000002 RID: 2
    public class CompSmartgunSystem : ThingComp
    {
        public CompProperties_SmartgunSystem Props => (CompProperties_SmartgunSystem)this.props;
        public float originalwarmupTime;
        public CompEquippable equippable => this.parent.TryGetComp<CompEquippable>();
        public CompQuality quality => this.parent.TryGetComp<CompQuality>();
        public Pawn pawn => this.equippable?.PrimaryVerb?.CasterPawn;

        public bool hasTargheter
        {
            get
            {
                bool result = false;
                if (Props.TargeterDef != null)
                {
                    if (Props.TargeterDef != null && Props.TargeterDef.IsApparel)
                    {
                        if (pawn == null || !pawn.RaceProps.Humanlike)
                        {
                            return false;
                        }
                        result = pawn.apparel.WornApparel.Any(x => x.def == Props.TargeterDef);
                    }
                }
                return result;
            }
        }

        public float AdjustedWarmup
        {
            get
            {
                bool qualityaffected = Props.WarmUpReductionQualityAffected && quality != null;
                float qualitymod = qualityaffected ? 0.2f + ((float)quality.Quality / 10) : 1f;
                float reductionmod = Props.WarmUpReduction * qualitymod;
                return originalwarmupTime * (1f - reductionmod);
            }
        }

        public bool hasHarness
        {
            get
            {
                bool result = false;
                if (Props.HarnessDef != null)
                {
                    if (Props.HarnessDef != null && Props.HarnessDef.IsApparel)
                    {
                        if (pawn == null || !pawn.RaceProps.Humanlike)
                        {
                            return false;
                        }
                        result = pawn.apparel.WornApparel.Any(x => x.def == Props.HarnessDef);
                    }
                }
                return result;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.originalwarmupTime, "originalwarmupTime");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.originalwarmupTime = this.parent.def.Verbs[0].warmupTime;
            }
        }
        

    }
}
