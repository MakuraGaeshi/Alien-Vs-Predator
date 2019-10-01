using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_Xenomorph : HediffCompProperties
    {

    }
    public class HediffComp_Xenomorph : HediffComp
    {
        public HediffCompProperties_Xenomorph XenoProps
        {
            get
            {
                return this.props as HediffCompProperties_Xenomorph;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

        }
        

    }
}
