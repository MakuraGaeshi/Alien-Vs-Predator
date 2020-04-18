using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace AvP
{
    public class XenomorphPawn : Pawn
    {
        public ThingDef HostRace;

        public Color HostBloodColour
        {
            get
            {
                if (HostRace == null)
                {
                    return base.DrawColorTwo;
                }
                Color color = HostRace.race.BloodDef.graphic.color;
                color.a = 1f;
                return color;
            }
        }

        public override Color DrawColorTwo => this.ageTracker.CurLifeStageIndex!=0 ? base.DrawColorTwo : HostBloodColour;
        public override Color DrawColor 
        { 
            get => base.DrawColor; 
            set => base.DrawColor = value; 
        }
    }
}
