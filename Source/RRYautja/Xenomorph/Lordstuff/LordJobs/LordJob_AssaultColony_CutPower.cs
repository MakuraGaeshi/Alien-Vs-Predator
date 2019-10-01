using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x0200016C RID: 364
    public class LordJob_AssaultColony_CutPower : LordJob 
    {
        // Token: 0x06000787 RID: 1927 RVA: 0x00042796 File Offset: 0x00040B96
        public LordJob_AssaultColony_CutPower()
        {
        }

        // Token: 0x06000788 RID: 1928 RVA: 0x000427B4 File Offset: 0x00040BB4
        public LordJob_AssaultColony_CutPower(Faction assaulterFaction, bool canKidnap = true, bool canTimeoutOrFlee = true, bool sappers = false, bool useAvoidGridSmart = false, bool canSteal = true)
        {
            this.assaulterFaction = assaulterFaction;
            this.canKidnap = canKidnap;
            this.canTimeoutOrFlee = canTimeoutOrFlee;
            this.sappers = sappers;
            this.useAvoidGridSmart = useAvoidGridSmart;
            this.canSteal = canSteal;
        }

        // Token: 0x1700012F RID: 303
        // (get) Token: 0x06000789 RID: 1929 RVA: 0x00042809 File Offset: 0x00040C09
        public override bool GuiltyOnDowned
        {
            get
            {
                return true;
            }
        }

        // Token: 0x0600078A RID: 1930 RVA: 0x0004280C File Offset: 0x00040C0C
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil lordToil = null;
            if (this.sappers)
            {
                lordToil = new LordToil_AssaultColonySappers();
                if (this.useAvoidGridSmart)
                {
                    lordToil.useAvoidGrid = true;
                }
                stateGraph.AddToil(lordToil);
                Transition transition = new Transition(lordToil, lordToil, true, true);
                transition.AddTrigger(new Trigger_PawnLost());
                stateGraph.AddTransition(transition, false);
                Transition transition2 = new Transition(lordToil, lordToil, true, false);
                transition2.AddTrigger(new Trigger_PawnHarmed(1f, false, null));
                transition2.AddPostAction(new TransitionAction_CheckForJobOverride());
                stateGraph.AddTransition(transition2, false);
            }
            LordToil lordToil2 = new LordToil_AssaultColony_CutPower(false);
            if (this.useAvoidGridSmart)
            {
                lordToil2.useAvoidGrid = true;
            }
            stateGraph.AddToil(lordToil2);
            LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, false);
            lordToil_ExitMap.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_ExitMap);
            if (this.sappers)
            {
                Transition transition3 = new Transition(lordToil, lordToil2, false, true);
                transition3.AddTrigger(new Trigger_NoFightingSappers());
                stateGraph.AddTransition(transition3, false);
            }
            if (this.assaulterFaction.def.humanlikeFaction)
            {
                if (this.canTimeoutOrFlee)
                {
                    Transition transition4 = new Transition(lordToil2, lordToil_ExitMap, false, true);
                    if (lordToil != null)
                    {
                        transition4.AddSource(lordToil);
                    }
                    transition4.AddTrigger(new Trigger_TicksPassed((!this.sappers) ? LordJob_AssaultColony_CutPower.AssaultTimeBeforeGiveUp.RandomInRange : LordJob_AssaultColony_CutPower.SapTimeBeforeGiveUp.RandomInRange));
                    transition4.AddPreAction(new TransitionAction_Message("MessageRaidersGivenUpLeaving".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
                    stateGraph.AddTransition(transition4, false);
                    Transition transition5 = new Transition(lordToil2, lordToil_ExitMap, false, true);
                    if (lordToil != null)
                    {
                        transition5.AddSource(lordToil);
                    }
                    FloatRange floatRange = new FloatRange(0.25f, 0.35f);
                    float randomInRange = floatRange.RandomInRange;
                    transition5.AddTrigger(new Trigger_FractionColonyDamageTaken(randomInRange, 900f));
                    transition5.AddPreAction(new TransitionAction_Message("MessageRaidersSatisfiedLeaving".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
                    stateGraph.AddTransition(transition5, false);
                }
                if (this.canKidnap)
                {
                    LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_Kidnap().CreateGraph()).StartingToil;
                    Transition transition6 = new Transition(lordToil2, startingToil, false, true);
                    if (lordToil != null)
                    {
                        transition6.AddSource(lordToil);
                    }
                    transition6.AddPreAction(new TransitionAction_Message("MessageRaidersKidnapping".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
                    transition6.AddTrigger(new Trigger_KidnapVictimPresent());
                    stateGraph.AddTransition(transition6, false);
                }
                if (this.canSteal)
                {
                    LordToil startingToil2 = stateGraph.AttachSubgraph(new LordJob_Steal().CreateGraph()).StartingToil;
                    Transition transition7 = new Transition(lordToil2, startingToil2, false, true);
                    if (lordToil != null)
                    {
                        transition7.AddSource(lordToil);
                    }
                    transition7.AddPreAction(new TransitionAction_Message("MessageRaidersStealing".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
                    transition7.AddTrigger(new Trigger_HighValueThingsAround());
                    stateGraph.AddTransition(transition7, false);
                }
            }
            Transition transition8 = new Transition(lordToil2, lordToil_ExitMap, false, true);
            if (lordToil != null)
            {
                transition8.AddSource(lordToil);
            }
            transition8.AddTrigger(new Trigger_BecameNonHostileToPlayer());
            transition8.AddPreAction(new TransitionAction_Message("MessageRaidersLeaving".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
            stateGraph.AddTransition(transition8, false);
            return stateGraph;
        }

        // Token: 0x0600078B RID: 1931 RVA: 0x00042C00 File Offset: 0x00041000
        public override void ExposeData()
        {
            Scribe_References.Look<Faction>(ref this.assaulterFaction, "assaulterFaction", false);
            Scribe_Values.Look<bool>(ref this.canKidnap, "canKidnap", true, false);
            Scribe_Values.Look<bool>(ref this.canTimeoutOrFlee, "canTimeoutOrFlee", true, false);
            Scribe_Values.Look<bool>(ref this.sappers, "sappers", false, false);
            Scribe_Values.Look<bool>(ref this.useAvoidGridSmart, "useAvoidGridSmart", false, false);
            Scribe_Values.Look<bool>(ref this.canSteal, "canSteal", true, false);
        }

        // Token: 0x0400033C RID: 828
        private Faction assaulterFaction;

        // Token: 0x0400033D RID: 829
        private bool canKidnap = true;

        // Token: 0x0400033E RID: 830
        private bool canTimeoutOrFlee = true;

        // Token: 0x0400033F RID: 831
        private bool sappers;

        // Token: 0x04000340 RID: 832
        private bool useAvoidGridSmart;

        // Token: 0x04000341 RID: 833
        private bool canSteal = true;

        // Token: 0x04000342 RID: 834
        private static readonly IntRange AssaultTimeBeforeGiveUp = new IntRange(26000, 38000);

        // Token: 0x04000343 RID: 835
        private static readonly IntRange SapTimeBeforeGiveUp = new IntRange(33000, 38000);
    }
}
