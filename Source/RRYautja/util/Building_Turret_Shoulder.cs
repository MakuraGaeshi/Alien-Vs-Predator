using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RRYautja
{
    public class Building_Turret_Shoulder : Building_TurretGun
    {
        public Pawn Parental;

        public bool turretIsOn;

        public CompEquippableTurret comp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.Parental, "Parental", false);
            Scribe_Deep.Look<CompEquippableTurret>(ref this.comp, "comp", false);
            Scribe_Values.Look<bool>(ref this.turretIsOn, "TurretIsOn", false, false);
        }

        public override void Tick()
        {
            base.Tick();
            if (this.Parental==null||this.comp==null)
            {
                this.Destroy();
            }
        }
    }
}
