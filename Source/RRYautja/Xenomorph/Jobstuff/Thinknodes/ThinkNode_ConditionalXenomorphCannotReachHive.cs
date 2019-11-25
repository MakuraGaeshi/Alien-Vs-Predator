using RRYautja.ExtensionMethods;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001DB RID: 475
    public class ThinkNode_ConditionalXenomorphCannotReachHive : ThinkNode_Conditional
    {
        // Token: 0x06000992 RID: 2450 RVA: 0x0004DBA2 File Offset: 0x0004BFA2
        protected override bool Satisfied(Pawn pawn)
        {
            IntVec3 c;
            if (!pawn.isXenomorph())
            {
                return false;
            }
            c = pawn.mindState.duty.focus.Cell;
            if (!pawn.xenomorph().HiveLoc.IsValid || pawn.xenomorph().HiveLoc == IntVec3.Zero)
            {
                pawn.xenomorph().HiveLoc = c;
            }
            bool result = !pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly, true, TraverseMode.PassAllDestroyableThingsNotWater) || (c.Filled(pawn.Map) && !c.GetFirstBuilding(pawn.Map).def.defName.Contains("RRY_Xenomorph_Hive"));

            if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode && DebugSettings.godMode) Log.Message(string.Format("{0}: {1} = Result: {2}", this, pawn.LabelShortCap, result));
            return result;
        }
    }
}
