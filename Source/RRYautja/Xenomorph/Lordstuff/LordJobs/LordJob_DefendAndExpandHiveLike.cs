using System;
using System.Collections.Generic;
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

            this.RunnerList = new List<Pawn>();
            this.DroneList = new List<Pawn>();
            this.WarriorList = new List<Pawn>();
            this.PreatorianList = new List<Pawn>();
            this.PredalienList = new List<Pawn>();
            this.QueenList = new List<Pawn>();
            this.ThrumbomorphList = new List<Pawn>();
            this.XenoList = new List<Pawn>();
            this.ScoutList = new List<Pawn>();
            this.WorkerList = new List<Pawn>();
            this.GuardList = new List<Pawn>();
        }

        // Token: 0x06000792 RID: 1938 RVA: 0x00042F2C File Offset: 0x0004132C
        public LordJob_DefendAndExpandHiveLike(bool aggressive)
        {
            this.aggressive = aggressive;
            this.RunnerList = new List<Pawn>();
            this.DroneList = new List<Pawn>();
            this.WarriorList = new List<Pawn>();
            this.PreatorianList = new List<Pawn>();
            this.PredalienList = new List<Pawn>();
            this.QueenList = new List<Pawn>();
            this.ThrumbomorphList = new List<Pawn>();
            this.XenoList = new List<Pawn>();
            this.ScoutList = new List<Pawn>();
            this.WorkerList = new List<Pawn>();
            this.GuardList = new List<Pawn>();
        }

        // Token: 0x06000792 RID: 1938 RVA: 0x00042F2C File Offset: 0x0004132C
        public LordJob_DefendAndExpandHiveLike(bool aggressive, Faction faction, IntVec3 vec3, float radius)
        {
            this.aggressive = aggressive;
            this.radius = radius;
            this.faction = faction;
            this.flagLoc = vec3;
            this.RunnerList = new List<Pawn>();
            this.DroneList = new List<Pawn>();
            this.WarriorList = new List<Pawn>();
            this.PreatorianList = new List<Pawn>();
            this.PredalienList = new List<Pawn>();
            this.QueenList = new List<Pawn>();
            this.ThrumbomorphList = new List<Pawn>();
            this.XenoList = new List<Pawn>();
            this.ScoutList = new List<Pawn>();
            this.WorkerList = new List<Pawn>();
            this.GuardList = new List<Pawn>();


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

        public override void Notify_PawnAdded(Pawn pawn)
        {
            if (pawn.def.race.FleshType == XenomorphRacesDefOf.AvP_Xenomorph)
            {
                if (!XenoList.Contains(pawn))
                {
                    XenoList.Add(pawn);
                }
            }
            if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_FaceHugger)
            {
                /*
                if (!hiveGrid.Dronelist.Contains(pawn))
                {
                    hiveGrid.Dronelist.Add(pawn);
                }
                */
            }
            else if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Runner)
            {
                if (!RunnerList.Contains(pawn))
                {
                    RunnerList.Add(pawn);
                }
                if (!ScoutList.Contains(pawn) && ScoutList.Count<Math.Max(1, RunnerList.Count/3))
                {

                }
            }
            else if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Drone)
            {
                if (!DroneList.Contains(pawn))
                {
                    DroneList.Add(pawn);
                }
            }
            else if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Warrior)
            {
                if (!WarriorList.Contains(pawn))
                {
                    WarriorList.Add(pawn);
                }
            }
            /*
            else if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Preatorian)
            {
                if (!PreatorianList.Contains(pawn))
                {
                    PreatorianList.Add(pawn);
                }
            }
            */
            else if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Predalien)
            {
                if (!PredalienList.Contains(pawn))
                {
                    PredalienList.Add(pawn);
                }
            }
            else if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Queen)
            {
                if (!QueenList.Contains(pawn))
                {
                    QueenList.Add(pawn);
                }
            }
            else if (pawn.def == XenomorphRacesDefOf.AvP_Xenomorph_Thrumbomorph)
            {
                if (!ThrumbomorphList.Contains(pawn))
                {
                    ThrumbomorphList.Add(pawn);
                }
            }
            base.Notify_PawnAdded(pawn);
        }

        public override void Notify_PawnLost(Pawn pawn, PawnLostCondition condition)
        {
            base.Notify_PawnLost(pawn, condition);
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
            Scribe_Values.Look<bool>(ref this.aggressive, "aggressive", false);
            Scribe_Values.Look<float>(ref this.radius, "radius");
            Scribe_Values.Look<float>(ref this.aggressiveradius, "aggressiveradius");
            Scribe_Values.Look<IntVec3>(ref this.flagLoc, "flagLoc", IntVec3.Zero);
            Scribe_Collections.Look<Pawn>(ref this.XenoList, "XenoList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.ScoutList, "ScoutList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.WorkerList, "WorkerList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.GuardList, "GuardList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.RunnerList, "RunnerList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.DroneList, "DroneList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.WarriorList, "WarriorList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.PreatorianList, "PreatorianList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.PredalienList, "PredalienList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.QueenList, "QueenList", LookMode.Reference, new object[0]);
            Scribe_Collections.Look<Pawn>(ref this.ThrumbomorphList, "ThrumbomorphList", LookMode.Reference, new object[0]);
        }

		// Token: 0x04000346 RID: 838
		private bool aggressive;
        private float radius = 40f;
        private float aggressiveradius = 80f;
        private Faction faction;
        private IntVec3 flagLoc;
        public List<Pawn> XenoList;
        public List<Pawn> ScoutList;
        public List<Pawn> WorkerList;
        public List<Pawn> GuardList;
        public List<Pawn> RunnerList;
        public List<Pawn> DroneList;
        public List<Pawn> WarriorList;
        public List<Pawn> PreatorianList;
        public List<Pawn> PredalienList;
        public List<Pawn> QueenList;
        public List<Pawn> ThrumbomorphList;
    }
}
