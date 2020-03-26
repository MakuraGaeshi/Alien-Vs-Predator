using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001EA RID: 490 .skyManager.CurSkyGlow <= 0.5f
    public class ThinkNode_ConditionalSkyBrighter : ThinkNode_Conditional
    {
        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalSkyBrighter thinkNode_ConditionalSky = (ThinkNode_ConditionalSkyBrighter)base.DeepCopy(resolve);
            thinkNode_ConditionalSky.Brightness = this.Brightness;
            return thinkNode_ConditionalSky;
        }

        public float Brightness = 0.5f;

        // Token: 0x060009B3 RID: 2483 RVA: 0x0004DFD8 File Offset: 0x0004C3D8
        protected override bool Satisfied(Pawn pawn)
        {
            bool result = pawn.Map.skyManager.CurSkyGlow >= Brightness;
        //    if (Find.Selector.SelectedObjects.Contains(pawn)) Log.Message(string.Format("{0} Cur: {1} >= {2} Result: {3}", this, pawn.Map.skyManager.CurSkyGlow, Brightness, result));
            return result;
        }

    }
    // Token: 0x020001EA RID: 490 .skyManager.CurSkyGlow <= 0.5f
    public class ThinkNode_ConditionalSkyDarker : ThinkNode_Conditional
    {
        // Token: 0x060009B8 RID: 2488 RVA: 0x0004E07C File Offset: 0x0004C47C
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalSkyDarker thinkNode_ConditionalSky = (ThinkNode_ConditionalSkyDarker)base.DeepCopy(resolve);
            thinkNode_ConditionalSky.Brightness = this.Brightness;
            return thinkNode_ConditionalSky;
        }

        public float Brightness = 0.5f;

        // Token: 0x060009B3 RID: 2483 RVA: 0x0004DFD8 File Offset: 0x0004C3D8
        protected override bool Satisfied(Pawn pawn)
        {
            bool result = pawn.Map.skyManager.CurSkyGlow <= Brightness;
        //    if (Find.Selector.SelectedObjects.Contains(pawn)) Log.Message(string.Format("{0} Cur: {1} <= {2} Result: {3}", this, pawn.Map.skyManager.CurSkyGlow, Brightness, result));
            return result;
        }

    }
}
