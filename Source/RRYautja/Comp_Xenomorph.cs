using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RRYautja
{
    public class CompProperties_Xenomorph : CompProperties
    {
        public CompProperties_Xenomorph()
        {
            this.compClass = typeof(Comp_Xenomorph);
        }

    }

    public class Comp_Xenomorph : ThingComp
    {
        public CompProperties_Xenomorph Props
        {
            get
            {
                return (CompProperties_Xenomorph)this.props;
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {

            Pawn other = dinfo.Instigator as Pawn;
            Pawn pawn = base.parent as Pawn;

            if (other.kindDef.race==YautjaDefOf.Alien_Yautja)
            {
                if (pawn.Dead)
                {
                    if (other.health.hediffSet.HasHediff(YautjaDefOf.RRYUnblooded))
                    {
                        foreach (var part in other.RaceProps.body.AllParts.Where(x => x.def.defName == "Head"))
                        {
                            Hediff unblooded = other.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRYUnblooded);
                            other.health.RemoveHediff(unblooded);
                            Hediff blooded = HediffMaker.MakeHediff(YautjaDefOf.RRYBloodedUM, other, null); 
                            other.health.AddHediff(blooded, part, null);
                        }
                    }
                }
                
            }
            base.PostPostApplyDamage(dinfo, totalDamageDealt);

        }
    }
}
