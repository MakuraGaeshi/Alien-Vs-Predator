using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
    // Token: 0x020002AA RID: 682
    public class IncidentDef_FactionSpecific : IncidentDef
    {
        public FactionDef Faction;
        public List<FactionDef> Factions;
        public PawnsArrivalModeDef pawnsArrivalModeDef;
        public List<PawnsArrivalModeDef> pawnsArrivalModeDefs;
        public RaidStrategyDef raidStrategyDefDef;
        public List<RaidStrategyDef> raidStrategyDefDefs;

    }

    // Token: 0x020002AA RID: 682
    public class IncidentDef_FactionSpecific_WithCondition : IncidentDef_FactionSpecific
    {
        public GameConditionDef ConditionDef;
        public bool preRaid = false;
        public int delayTick;

    }
}
