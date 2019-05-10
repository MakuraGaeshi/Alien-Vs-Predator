using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace RRYautja
{
    public class RRY_Xenomorph_AbilityUser : AbilityUser.CompAbilityUser
    {
       public override void CompTick()
        {
            base.CompTick();
            if (IsInitialized)
            {
                // any custom code
            }
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            // add Abilities
            this.AddPawnAbility(XenomorphDefOf.RRY_Ability_SpitAcid);
        }
        /**/
        public override bool TryTransformPawn()
        {
            if (Pawn.kindDef == XenomorphDefOf.RRY_Xenomorph_Drone)
            {
                return true;
            }
            return false;
        } /**/
    }
}
