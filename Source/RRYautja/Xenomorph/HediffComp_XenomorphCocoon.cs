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

        private List<Rot4> Rotlist = new List<Rot4>
        {
            Rot4.North,
            Rot4.South,
            Rot4.East,
            Rot4.West
        };

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            ThingDef thingDef = Pawn.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
            if (Pawn.CurrentBed() == null)
            {
                Rot4 rot = Rotlist.RandomElement();
                ThingDef named = Pawn.RaceProps.Humanlike ? XenomorphDefOf.RRY_Xenomorph_Humanoid_Cocoon : XenomorphDefOf.RRY_Xenomorph_Animal_Cocoon;
                Thing thing = ThingMaker.MakeThing(named);
                GenSpawn.Spawn(thing, Pawn.Position, Pawn.Map, rot, WipeMode.Vanish, false);
                this.Pawn.jobs.Notify_TuckedIntoBed((Building_XenomorphCocoon)XenomorphUtil.ClosestReachableEmptyCocoon(Pawn, named));
                this.Pawn.mindState.Notify_TuckedIntoBed();
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
