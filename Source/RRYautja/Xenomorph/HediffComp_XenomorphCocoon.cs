using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_XenomorphCocoon : HediffCompProperties
    {
        public HediffCompProperties_XenomorphCocoon()
        {
            this.compClass = typeof(HediffComp_XenomorphCocoon);
        }


    }
    public class HediffComp_XenomorphCocoon : HediffComp
    {
        public HediffCompProperties_XenomorphCocoon XenoProps
        {
            get
            {
                return this.props as HediffCompProperties_XenomorphCocoon;
            }
        }
        
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            ThingDef thingDef = Pawn.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
            if (Pawn.CurrentBed() == null)
            {
                Pawn.health.RemoveHediff(this.parent);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Find.TickManager.TicksGame % 300 == 0)
            {
                if (Pawn.CurrentBed() == null)
                {
                    Pawn.health.RemoveHediff(this.parent);
                }
            }
        }

    }
}
