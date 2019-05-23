﻿using RimWorld;
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

        public CompEquippableTurret comp
        {
            get
            {
                if (Parental!=null)
                {
                    return Parental.TryGetComp<CompEquippableTurret>();
                }
                return null;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        //    Log.Message(string.Format("turret spawned"));
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
        //    Log.Message(string.Format("turret despawned"));
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
        //    Log.Message(string.Format("turret destroyed"));
        }
        public override void ExposeData()
        {
            base.ExposeData();
        //    Scribe_References.Look<Pawn>(ref this.Parental, "Parental", false);
            Scribe_Values.Look<bool>(ref this.turretIsOn, "TurretIsOn");
        }

        public override void Tick()
        {
            base.Tick();
            if (this.Parental==null||(this.Parental is Pawn pawn && (pawn.Dead || pawn.Downed)))//||this.comp==null)
            {
                this.Destroy();
            }
            /*
            else
            {
                if (Parental.apparel.WornApparelCount>0)
                {
                    foreach (var app in Parental.apparel.WornApparel)
                    {
                        CompEquippableTurret turretcomp = this.Parental.TryGetComp<CompEquippableTurret>();
                        if (turretcomp!=null)
                        {
                            if (this!= turretcomp.turret)
                            {
                                this.Destroy();
                            }
                        }
                    }
                }
                
            }
            */
        }
        public override LocalTargetInfo CurrentTarget => Parental.TargetCurrentlyAimingAt != null ? Parental.TargetCurrentlyAimingAt : base.CurrentTarget;
    }
}
