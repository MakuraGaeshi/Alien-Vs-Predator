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

            base.PostPostApplyDamage(dinfo, totalDamageDealt);

        }
    }
    public class CompProperties_Neomorph : CompProperties
    {
        public CompProperties_Neomorph()
        {
            this.compClass = typeof(Comp_Neomorph);
        }

    }

    public class Comp_Neomorph : ThingComp
    {
        public CompProperties_Neomorph Props
        {
            get
            {
                return (CompProperties_Neomorph)this.props;
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



            base.PostPostApplyDamage(dinfo, totalDamageDealt);

        }
    }
}
