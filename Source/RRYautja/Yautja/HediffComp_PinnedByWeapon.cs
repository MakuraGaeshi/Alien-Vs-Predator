using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class HediffCompProperties_PinnedByWeapon : HediffCompProperties
    {
        public HediffCompProperties_PinnedByWeapon()
        {
            this.compClass = typeof(HediffComp_PinnedByWeapon);
        }

    }
    public class HediffComp_PinnedByWeapon : HediffComp
    {
        public HediffCompProperties_PinnedByWeapon XenoProps
        {
            get
            {
                return this.props as HediffCompProperties_PinnedByWeapon;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            
            if (base.Pawn!=null)
            {
                if (base.Pawn.Position != null)
                {
                    if (base.Pawn.Map != null)
                    {
                        bool present = false;
                        List<Thing> thingList = GridsUtility.GetThingList(base.Pawn.Position, base.Pawn.Map);
                        foreach (var item in thingList.Where(x => x.TryGetComp<Comp_PinningWeapon>()!=null))
                        {
                            present = true;
                        }
                        if (!present)
                        {
                            Pawn.health.RemoveHediff(this.parent);
                        }
                    }
                }
            }
        }


    }
    public class CompProperties_PinningWeapon : CompProperties
    {
        public CompProperties_PinningWeapon()
        {
            this.compClass = typeof(Comp_PinningWeapon);
        }

    }

    public class Comp_PinningWeapon : ThingComp
    {
        public CompProperties_PinningWeapon Props
        {
            get
            {
                return (CompProperties_PinningWeapon)this.props;
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
