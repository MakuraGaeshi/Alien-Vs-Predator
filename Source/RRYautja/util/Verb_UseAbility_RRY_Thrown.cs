using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AbilityUser
{
    // Token: 0x02000024 RID: 36
    public class Verb_UseAbility_RRY_Thrown : Verb_UseAbility
    {
        // Token: 0x060000E5 RID: 229 RVA: 0x000081A4 File Offset: 0x000063A4
        public override void PostCastShot(bool inResult, out bool outResult)
        {
            outResult = inResult;
                ThingWithComps weapon = CasterPawn.equipment.Primary;
                CasterPawn.equipment.Notify_EquipmentRemoved(weapon);
        }
        
    }
}
