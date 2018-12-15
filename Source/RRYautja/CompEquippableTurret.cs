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


        // Token: 0x060053DC RID: 21468 RVA: 0x00264E1B File Offset: 0x0026321B
        public void ExposeData()
		{
			parent.ExposeData();
            Scribe_References.Look<Thing>(ref this.turret, "Turret", false);
            Scribe_Values.Look<bool>(ref this.turretIsOn, "TurretIsOn", false, false);
		}

		// Token: 0x060053DD RID: 21469 RVA: 0x00264E3D File Offset: 0x0026323D
		public override void CompTickRare()
        {
			base.CompTickRare();
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
            return GetWearer != null && !GetWearer.Dead && !GetWearer.Downed && GetWearer.Awake() && (this.turretMode == CompEquippableTurret.TurretMode.ForcedOn || (this.turretMode != CompEquippableTurret.TurretMode.ForcedOff && (GetWearer.Map != null && ((GetWearer.Position.Roofed(GetWearer.Map) && GetWearer.Map.glowGrid.PsychGlowAt(GetWearer.Position) <= PsychGlow.Lit) || (!GetWearer.Position.Roofed(GetWearer.Map) && GetWearer.Map.glowGrid.PsychGlowAt(GetWearer.Position) < PsychGlow.Overlit)))));
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        public void SwitchOnTurret()
        {
            IntVec3 intVec = GetWearer.DrawPos.ToIntVec3();
            if (!this.turret.DestroyedOrNull() && intVec != this.turret.Position)
            {
                this.SwitchOffTurret();
            }
            if (this.turret.DestroyedOrNull() && intVec.GetFirstThing(GetWearer.Map, Util_CompEquippableTurret.EquippableTurretDef) == null)
            {
                this.turret = GenSpawn.Spawn(Util_CompEquippableTurret.EquippableTurretDef, intVec, GetWearer.Map, WipeMode.Vanish);
                this.turret.SetFactionDirect(this.GetWearer.Faction);
            }
            this.turretIsOn = true;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void SwitchOffTurret()
        {
            if (!this.turret.DestroyedOrNull())
            {
                this.turret.DeSpawn();
                this.turret = null;
            }
            this.turretIsOn = false;
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
        public override IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag)
            {
                int num = 700000101;
                Command_Action command_Action = new Command_Action();
                switch (this.turretMode)
                {
                    case CompEquippableTurret.TurretMode.ForcedOff:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOff", true);
                        command_Action.defaultLabel = "Turret: off.";
                        break;
                    case CompEquippableTurret.TurretMode.ForcedOn:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOn", true);
                        command_Action.defaultLabel = "Turret: on.";
                        break;
                }
                command_Action.defaultDesc = "Switch mode.";
                command_Action.activateSound = SoundDef.Named("Click");
                command_Action.action = new Action(this.SwitchTurretMode);
                command_Action.groupKey = num + 1;
                yield return command_Action;


            }
        }
        /*
        // Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag)
            {
                int num = 700000101;
                Command_Action command_Action = new Command_Action();
                switch (this.turretMode)
                {
                    case CompEquippableTurret.TurretMode.ForcedOn:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOn", true);
                        command_Action.defaultLabel = "Turret: on.";
                        break;
                    case CompEquippableTurret.TurretMode.ForcedOff:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOff", true);
                        command_Action.defaultLabel = "Turret: off.";
                        break;
                }
                command_Action.defaultDesc = "Switch mode.";
                command_Action.activateSound = SoundDef.Named("Click");
                command_Action.action = new Action(this.SwitchTurretMode);
                command_Action.groupKey = num + 1;

                yield return command_Action;
            }
            yield break;
        }*/
        // Token: 0x0600000A RID: 10 RVA: 0x000023A4 File Offset: 0x000005A4
        public void SwitchTurretMode()
        {
            switch (this.turretMode)
            {
                case CompEquippableTurret.TurretMode.ForcedOn:
                    this.turretMode = CompEquippableTurret.TurretMode.ForcedOff;
                    break;
                case CompEquippableTurret.TurretMode.ForcedOff:
                    this.turretMode = CompEquippableTurret.TurretMode.ForcedOn;
                    break;
            }
            this.RefreshTurretState();
        }

        // Token: 0x04000001 RID: 1
        public const int updatePeriodInTicks = 60;

        // Token: 0x04000002 RID: 2
        public int nextUpdateTick;

        // Token: 0x04000003 RID: 3
        public Thing turret;

        // Token: 0x04000004 RID: 4
        public bool turretIsOn;

        // Token: 0x04000005 RID: 5
        public CompEquippableTurret.TurretMode turretMode;

        // Token: 0x02000004 RID: 4
        public enum TurretMode
        {
            // Token: 0x04000009 RID: 9
            ForcedOff,
            // Token: 0x04000008 RID: 8
            ForcedOn
        }
	}
}
