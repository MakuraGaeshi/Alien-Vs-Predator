using RRYautja;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    public abstract class ThinkNode_ConditionalHive : ThinkNode_Conditional
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalHive thinkNode_ConditionalHive = (ThinkNode_ConditionalHive)base.DeepCopy(resolve);
            return thinkNode_ConditionalHive;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            bool result = !HiveGrid(pawn).Hivelist.NullOrEmpty();
            return result;
        }
        
        public static HiveLike Tunnel(Pawn pawn)
        {
            if (pawn.GetLord() != null && pawn.GetLord() is Lord lord)
            {
                if (lord.LordJob is LordJob_DefendAndExpandHiveLike hivejob)
                {
                    if (lord.CurLordToil is LordToil_DefendAndExpandHiveLike hivetoil)
                    {
                        if (hivetoil.Data.assignedHiveLikes.TryGetValue(pawn) != null)
                        {
                            return hivetoil.Data.assignedHiveLikes.TryGetValue(pawn);
                        }
                    }
                }
                if (lord.CurLordToil is LordToil_DefendHiveLikeAggressively hivetoilA)
                {
                    if (hivetoilA.Data.assignedHiveLikes.TryGetValue(pawn) != null)
                    {
                        return hivetoilA.Data.assignedHiveLikes.TryGetValue(pawn);
                    }
                }
            }
            return null;
        }

        public static MapComponent_HiveGrid HiveGrid(Pawn pawn)
        {
            return pawn.Map.GetComponent<MapComponent_HiveGrid>();
        }
    }

    public class ThinkNode_ConditionalHiveGuard : ThinkNode_ConditionalHive
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalHiveGuard thinkNode_ConditionalHive = (ThinkNode_ConditionalHiveGuard)base.DeepCopy(resolve);
            return thinkNode_ConditionalHive;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            bool result = HiveGrid(pawn).HiveGuardlist.Contains(pawn);
            if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode && DebugSettings.godMode) Log.Message(string.Format("{0} HiveGuard: {1} = Result: {2}", this, pawn.LabelShortCap, result));
            return result;
        }
    }

    public class ThinkNode_ConditionalHiveWorker : ThinkNode_ConditionalHive
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalHiveWorker thinkNode_ConditionalHive = (ThinkNode_ConditionalHiveWorker)base.DeepCopy(resolve);
            return thinkNode_ConditionalHive;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            bool result = HiveGrid(pawn).HiveWorkerlist.Contains(pawn);
            if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode && DebugSettings.godMode) Log.Message(string.Format("{0} HiveWorker: {1} = Result: {2}", this, pawn.LabelShortCap, result));
            return result;
        }
    }

    public class ThinkNode_ConditionalHiveActive : ThinkNode_ConditionalHive
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalHiveActive thinkNode_ConditionalHive = (ThinkNode_ConditionalHiveActive)base.DeepCopy(resolve);
            return thinkNode_ConditionalHive;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            bool result = !Tunnel(pawn).hiveDormant;
            if (Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode && DebugSettings.godMode) Log.Message(string.Format("{0} hiveDormant: {1} = Result: {2}", this, pawn.LabelShortCap, result));
            return result;
        }
    }

}
