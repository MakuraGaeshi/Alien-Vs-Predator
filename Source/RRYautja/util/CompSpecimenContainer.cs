using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RRYautja
{
    public class CompProperties_SpecimenContainer: CompProperties
    {
        public CompProperties_SpecimenContainer()
        {
            this.compClass = typeof(CompSpecimenContainer);
        }
        public List<ThingDef> acceptableThings = new List<ThingDef>();
    }

    // Token: 0x02000002 RID: 2
    public class CompSpecimenContainer : ThingComp, IThingHolder
    {
        public CompProperties_SpecimenContainer Props => (CompProperties_SpecimenContainer)this.props;

        bool HasSpecimen => innerContainer.Any;
        bool AcceptableThing(ThingDef thing) => Props.acceptableThings.Contains(thing);
        bool CanAcceptThing(Thing thing) => AcceptableThing(thing.def) && !HasSpecimen;
        // Token: 0x06007F70 RID: 32624 RVA: 0x00056C78 File Offset: 0x00054E78
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            string FloatMenuOptionLabel = string.Empty;
            if (HasSpecimen)
            {
                Thing contained = innerContainer.First();
                FloatMenuOptionLabel = contained.Label;
                FloatMenuOption useopt = new FloatMenuOption(FloatMenuOptionLabel, delegate ()
                {
                    if (selPawn.CanReserveAndReach(this.parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
                    {
                        this.TryStartUseJob(selPawn);
                    }
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield return useopt;
            }
            foreach (var item in CompFloatMenuOptions(selPawn))
            {
                yield return item;
            }
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        public void TryStartUseJob(Pawn user)
        {
            if (!user.CanReserveAndReach(this.parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
            {
                return;
            }
            /*
            Job job = new Job(this.Props.useJob, this.parent);
            user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            */
        }

        // Token: 0x06007F71 RID: 32625 RVA: 0x00056C80 File Offset: 0x00054E80
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });

        }
        public ThingOwner innerContainer;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {

            }
        }
        

    }
}
