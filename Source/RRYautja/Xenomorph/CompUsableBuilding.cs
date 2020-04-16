using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x0200025E RID: 606
    public class CompProperties_UsableBuilding : CompProperties_Usable
    {
        // Token: 0x06000ACC RID: 2764 RVA: 0x00056399 File Offset: 0x00054799
        public CompProperties_UsableBuilding()
        {
            this.compClass = typeof(CompUsableBuilding);
        }
        
    }
    // Token: 0x02000774 RID: 1908
    public class CompUsableBuilding : CompUsable
    {
        public new CompProperties_UsableBuilding Props
        {
            get
            {
                return (CompProperties_UsableBuilding)this.props;
            }
        }

        // Token: 0x06002A4B RID: 10827 RVA: 0x00138F78 File Offset: 0x00137378
        public  void TryStartUseJob(Pawn user)
        {
            if (!user.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false))
            {
                return;
            }
            string text;
            if (!this.CanBeUsedBy(user, out text))
            {
                return;
            }
            Job job = new Job(this.Props.useJob, this.parent);
            user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

        // Token: 0x06002A4A RID: 10826 RVA: 0x00138F4C File Offset: 0x0013734C
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            string failReason;
            if (!this.CanBeUsedBy(myPawn, out failReason))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + ((failReason == null) ? string.Empty : (" (" + failReason + ")")), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.CanReach(this.parent, PathEndMode.InteractionCell, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            /*
            else if (!myPawn.CanReserve(this.parent, 1, -1, null, false))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn)  + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            */
            else if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else
            {
                FloatMenuOption useopt = new FloatMenuOption(this.FloatMenuOptionLabel(myPawn), delegate ()
                {
                    if (myPawn.CanReach(this.parent, PathEndMode.InteractionCell, Danger.Deadly, false))
                    {
                        foreach (CompUseEffect compUseEffect in this.parent.GetComps<CompUseEffect>())
                        {
                            if (compUseEffect.SelectedUseOption(myPawn))
                            {
                                return;
                            }
                        }
                        this.TryStartUseJob(myPawn);
                    }
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield return useopt;
            }
            yield break;
        }
        
        // Token: 0x06002A4D RID: 10829 RVA: 0x00139094 File Offset: 0x00137494
        private bool CanBeUsedBy(Pawn p, out string failReason)
        {
            List<ThingComp> allComps = this.parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                CompUseEffect compUseEffect = allComps[i] as CompUseEffect;
                if (compUseEffect != null && !compUseEffect.CanBeUsedBy(p, out failReason))
                {
                    return false;
                }
            }
            failReason = null;
            return true;
        }
    }
}
