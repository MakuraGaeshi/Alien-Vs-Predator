using System;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x02000172 RID: 370
    public class LordJob_XenoKidnap : LordJob
    {
        // Token: 0x17000137 RID: 311
        // (get) Token: 0x060007B4 RID: 1972 RVA: 0x00043C82 File Offset: 0x00042082
        public override bool GuiltyOnDowned
        {
            get
            {
                return true;
            }
        }

        // Token: 0x060007B5 RID: 1973 RVA: 0x00043C88 File Offset: 0x00042088
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_KidnapCover lordToil_KidnapCover = new LordToil_KidnapCover();
            lordToil_KidnapCover.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_KidnapCover);
            LordToil_KidnapCover lordToil_KidnapCover2 = new LordToil_KidnapCover();
            lordToil_KidnapCover2.cover = false;
            lordToil_KidnapCover2.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_KidnapCover2);
            Transition transition = new Transition(lordToil_KidnapCover, lordToil_KidnapCover2, false, true);
            transition.AddTrigger(new Trigger_TicksPassed(1200));
            stateGraph.AddTransition(transition, false);
            return stateGraph;
        }

        // Token: 0x060007B6 RID: 1974 RVA: 0x00043CED File Offset: 0x000420ED
        public override void ExposeData()
        {
        }
    }
}
