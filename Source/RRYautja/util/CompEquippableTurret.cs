using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RRYautja
{
    public class CompProperties_EquippableTurret : CompProperties
    {
        public CompProperties_EquippableTurret()
        {
            this.compClass = typeof(CompEquippableTurret);
        }
    }

    // Token: 0x02000E58 RID: 3672
    public class CompEquippableTurret : CompWearable
    {
        public CompProperties_EquippableTurret Props => (CompProperties_EquippableTurret)props;

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_ApparelTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else if (ParentHolder != null && ParentHolder is Pawn_EquipmentTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }

        // Determine if this ThingComp is being worn presently. Returns True/False
        protected virtual bool IsWorn => (GetWearer != null);
        protected virtual bool IsTurnedOn => (GetWearer != null && GetWearer.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_HeDiff_TurretIsOn) != null);

        public bool turretIsOn => (IsTurnedOn);
        // Token: 0x060053DC RID: 21468 RVA: 0x00264E1B File Offset: 0x0026321B
        public void ExposeData()
        {// Building_Turret_Shoulder
            parent.ExposeData();
            Scribe_References.Look<Thing>(ref this.turret, "TurretThing", false);
            Scribe_Deep.Look<Building_Turret_Shoulder>(ref this.turret_Shoulder, "TurretBuilding", false);
        //    Scribe_Values.Look<bool>(ref this.turretIsOn, "TurretIsOn", IsTurnedOn);
        }

		// Token: 0x060053DD RID: 21469 RVA: 0x00264E3D File Offset: 0x0026323D
		public override void CompTick()
        {
			base.CompTick();
            if (!PlayerKnowledgeDatabase.IsComplete(YautjaConceptDefOf.RRY_Concept_Plasmacaster) && GetWearer.IsColonist)
            {
                LessonAutoActivator.TeachOpportunity(YautjaConceptDefOf.RRY_Concept_Plasmacaster, OpportunityType.GoodToKnow);
            }
            if (this.turretIsOn || Find.TickManager.TicksGame >= this.nextUpdateTick)
            {
                this.nextUpdateTick = Find.TickManager.TicksGame + 60;
                this.RefreshTurretState();
            }
        }

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            parent.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.nextUpdateTick = Find.TickManager.TicksGame + Rand.Range(0, 60);
            }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020EE File Offset: 0x000002EE
        public void RefreshTurretState()
        {
            if (this.ComputeTurretState())
            {
                this.SwitchOnTurret();
                return;
            }
            this.SwitchOffTurret();
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002108 File Offset: 0x00000308
        public bool ComputeTurretState()
        {
            return GetWearer != null && !GetWearer.Dead && !GetWearer.Downed && GetWearer.Awake() && (this.turretIsOn);
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        public void SwitchOnTurret()
        {
            IntVec3 intVec = GetWearer.DrawPos.ToIntVec3();
            if (!this.turret.DestroyedOrNull() && intVec != this.turret.Position)
            {
                this.MoveTurret(intVec);
            }
            if ((this.turret.DestroyedOrNull()|| !this.turret.Spawned) && intVec.GetFirstThing(GetWearer.Map, Util_CompEquippableTurret.EquippableTurretDef) == null)
            {
                this.turret = GenSpawn.Spawn(Util_CompEquippableTurret.EquippableTurretDef, intVec, GetWearer.Map, WipeMode.Vanish);
                this.turret.SetFactionDirect(this.GetWearer.Faction);
                ((Building_Turret_Shoulder)this.turret).Parental = GetWearer;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void SwitchOffTurret()
        {
            if (!this.turret.DestroyedOrNull() && this.turret.Spawned)
            {
                this.turret.DeSpawn();
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void MoveTurret(IntVec3 intVec)
        {
            if (!this.turret.DestroyedOrNull())
            {
                this.turret.Position = intVec;
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
        public override IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            bool flag = Find.Selector.SelectedObjects.Contains(GetWearer);
            if (flag)
            {
                int num = 700000101;
                if (!this.turretIsOn)
                {
                    yield return new Command_Action
                    {
                        icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_TurretModeOff", true),
                        defaultLabel = "Turret: off.",
                        defaultDesc = "Switch mode.",
                        activateSound = SoundDef.Named("Click"),
                        action = new Action(this.SwitchTurretMode),
                        groupKey = num + 1,
                    };
                }
                if (this.turretIsOn)
                {
                    yield return new Command_Action
                    {
                        icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_TurretModeOn", true),
                        defaultLabel = "Turret: on.",
                        defaultDesc = "Switch mode.",
                        activateSound = SoundDef.Named("Click"),
                        action = new Action(this.SwitchTurretMode),
                        groupKey = num + 1,
                    };
                }
            }
            yield break;
        }


        // Token: 0x0600000A RID: 10 RVA: 0x000023A4 File Offset: 0x000005A4
        public void SwitchTurretMode()
        {
            Hediff hediff;
            switch (this.turretIsOn)
            {
                case true:
                    hediff = GetWearer.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_HeDiff_TurretIsOn);
                    if (hediff != null)
                    {
                        GetWearer.health.RemoveHediff(hediff);
                    }
                    break;
                case false:
                    hediff = GetWearer.health.hediffSet.GetFirstHediffOfDef(YautjaDefOf.RRY_HeDiff_TurretIsOn);
                    if (hediff == null)
                    {
                        GetWearer.health.AddHediff(YautjaDefOf.RRY_HeDiff_TurretIsOn);
                    }
                    break;
            }
            this.RefreshTurretState();
        }

        // Token: 0x04000001 RID: 1
        public const int updatePeriodInTicks = 60;

        // Token: 0x04000002 RID: 2
        public int nextUpdateTick;

        // Token: 0x04000003 RID: 3 Building_Turret_Shoulder
        public Thing turret;
        public Building_Turret_Shoulder turret_Shoulder;

        // Token: 0x04000004 RID: 4
        
	}
}
