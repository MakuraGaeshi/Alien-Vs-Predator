﻿using Verse;

namespace RRYautja
{
    // Token: 0x02000024 RID: 36
    public class Verb_UseEquipment_Thrown : Verb_UseEquipment
    {
        // Token: 0x060000E5 RID: 229 RVA: 0x000081A4 File Offset: 0x000063A4
        public void PostCastShot(bool inResult, out bool outResult)
        {
            outResult = inResult;
                ThingWithComps weapon = CasterPawn.equipment.Primary;
                CasterPawn.equipment.Notify_EquipmentRemoved(weapon);
        }

        protected override bool TryCastShot()
        {
            bool result = base.TryCastShot();
            if (result)
            {
                PostCastShot(result,out  result);
            }
            return result;
        }

    }
}
