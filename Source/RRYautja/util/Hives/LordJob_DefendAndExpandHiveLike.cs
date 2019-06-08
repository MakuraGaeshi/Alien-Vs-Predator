using System;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x0200016E RID: 366
	public class LordJob_DefendAndExpandHiveLike : LordJob
	{
		// Token: 0x06000791 RID: 1937 RVA: 0x00042F24 File Offset: 0x00041324
		public LordJob_DefendAndExpandHiveLike()
		{

		}

        // Token: 0x06000792 RID: 1938 RVA: 0x00042F2C File Offset: 0x0004132C
        public LordJob_DefendAndExpandHiveLike(bool aggressive)
        {
            this.aggressive = aggressive;
        }

        // Token: 0x06000792 RID: 1938 RVA: 0x00042F2C File Offset: 0x0004132C
        public LordJob_DefendAndExpandHiveLike(bool aggressive, Faction faction, IntVec3 vec3, float radius)
        {
            this.aggressive = aggressive;
            this.radius = radius;
            this.faction = faction;
            this.flagLoc = vec3;
        }

        // Token: 0x17000130 RID: 304
        // (get) Token: 0x06000793 RID: 1939 RVA: 0x00042F3B File Offset: 0x0004133B
        public override bool CanBlockHostileVisitors
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000794 RID: 1940 RVA: 0x00042F3E File Offset: 0x0004133E
		public override bool AddFleeToil
		{
			get
			{
				return false;
			}
		}

        public override void LordJobTick()
        {
            base.LordJobTick();

        }


        // Token: 0x06000795 RID: 1941 RVA: 0x00042F44 File Offset: 0x00041344
        public override StateGraph CreateGraph()
		{
            
			StateGraph stateGraph = new StateGraph();
            LordToil_DefendAndExpandHiveLike lordToil_DefendAndExpandHiveLike = new LordToil_DefendAndExpandHiveLike
            {
                distToHiveToAttack = radius,
                
            };
            stateGraph.StartingToil = lordToil_DefendAndExpandHiveLike;

            LordToil_DefendHiveLikeAggressively lordToil_DefendHiveAggressively = new LordToil_DefendHiveLikeAggressively
            {
                distToHiveToAttack = radius
            };
            stateGraph.AddToil(lordToil_DefendHiveAggressively);

			LordToil_XenomrophAssaultColony lordToil_XenomrophAssaultColony = new LordToil_XenomrophAssaultColony(false);
			stateGraph.AddToil(lordToil_XenomrophAssaultColony);
            
			Transition transition = new Transition(lordToil_DefendAndExpandHiveLike, (!aggressive) ? (LordToil)lordToil_DefendHiveAggressively : (LordToil)lordToil_XenomrophAssaultColony, false, true);
			transition.AddTrigger(new Trigger_PawnHarmed(0.5f, true, null));
			transition.AddTrigger(new Trigger_PawnLostViolently(false));
			transition.AddTrigger(new Trigger_Memo(HiveLike.MemoAttackedByEnemy));
			transition.AddTrigger(new Trigger_Memo(HiveLike.MemoBurnedBadly));
			transition.AddTrigger(new Trigger_Memo(HiveLike.MemoDestroyedNonRoofCollapse));
			transition.AddTrigger(new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
			transition.AddPostAction(new TransitionAction_EndAllJobs());
			stateGraph.AddTransition(transition, false);
            
			Transition transition2 = new Transition(lordToil_DefendAndExpandHiveLike, lordToil_XenomrophAssaultColony, false, true);
			Transition transition3 = transition2;
			float chance = 0.05f;
			Faction parentFaction = base.Map.ParentFaction;
			transition3.AddTrigger(new Trigger_PawnHarmed(chance, false, parentFaction));
			transition2.AddPostAction(new TransitionAction_EndAllJobs());
			stateGraph.AddTransition(transition2, false);

			Transition transition4 = new Transition(lordToil_DefendHiveAggressively, lordToil_XenomrophAssaultColony, false, true);
			Transition transition5 = transition4;
			chance = 0.5f;
			parentFaction = base.Map.ParentFaction;
			transition5.AddTrigger(new Trigger_PawnHarmed(chance, false, parentFaction));
			transition4.AddPostAction(new TransitionAction_EndAllJobs());
			stateGraph.AddTransition(transition4, false);

			Transition transition6 = new Transition(lordToil_DefendAndExpandHiveLike, lordToil_DefendAndExpandHiveLike, true, true);
			transition6.AddTrigger(new Trigger_Memo(HiveLike.MemoDeSpawned));
			stateGraph.AddTransition(transition6, false);

			Transition transition7 = new Transition(lordToil_DefendHiveAggressively, lordToil_DefendHiveAggressively, true, true);
			transition7.AddTrigger(new Trigger_Memo(HiveLike.MemoDeSpawned));
			stateGraph.AddTransition(transition7, false);

			Transition transition8 = new Transition(lordToil_XenomrophAssaultColony, lordToil_DefendAndExpandHiveLike, false, true);
			transition8.AddSource(lordToil_DefendHiveAggressively);
			transition8.AddTrigger(new Trigger_TicksPassedWithoutHarmOrMemos(1200, new string[]
			{
				HiveLike.MemoAttackedByEnemy,
				HiveLike.MemoBurnedBadly,
				HiveLike.MemoDestroyedNonRoofCollapse,
				HiveLike.MemoDeSpawned,
				HediffGiver_Heat.MemoPawnBurnedByAir
			}));
			transition8.AddPostAction(new TransitionAction_EndAttackBuildingJobs());
			stateGraph.AddTransition(transition8, false);
			return stateGraph;
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x00043166 File Offset: 0x00041566
		public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.aggressive, "aggressive", false, false);
		}

		// Token: 0x04000346 RID: 838
		private bool aggressive;
        private float radius = 40f;
        private Faction faction;
        private IntVec3 flagLoc;
    }
}
