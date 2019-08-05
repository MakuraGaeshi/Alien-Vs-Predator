using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x020001AB RID: 427
    public class RaidStrategyWorker_ImmediateAttackSmart_CutPower : RaidStrategyWorker
    {
        // Token: 0x060008F0 RID: 2288 RVA: 0x0004A6DD File Offset: 0x00048ADD
        protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
        {
            return new LordJob_AssaultColony_CutPower(parms.faction, true, true, false, true, true);
        }

        // Token: 0x060008F1 RID: 2289 RVA: 0x0004A6EF File Offset: 0x00048AEF
        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            return base.CanUseWith(parms, groupKind) && parms.faction.def.canUseAvoidGrid;
        }
    }
}
