﻿using RRYautja;
using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001BE RID: 446
    public class ThinkNode_ConditionalFullyGrown : ThinkNode_Conditional
    {
        // Token: 0x06000956 RID: 2390 RVA: 0x0004D678 File Offset: 0x0004BA78
        protected override bool Satisfied(Pawn pawn)
        {
            LifeStageDef stage = pawn.ageTracker.CurLifeStage;
#if DEBUG
            bool selected = Find.Selector.SingleSelectedThing == pawn;
        //    if (selected&&pawn.kindDef!=XenomorphDefOf.RRY_Xenomorph_FaceHugger&&stage == pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def) Log.Message(string.Format("ThinkNode_ConditionalFullyGrown {0} \nCurLifeStage:{1} FinalLifeStage:{2}", pawn.Label, stage, pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def));
#endif
            return stage == pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def;
        }

    }

    public class ThinkNode_ConditionalNotGrown : ThinkNode_Conditional
    {
        // Token: 0x06000956 RID: 2390 RVA: 0x0004D678 File Offset: 0x0004BA78
        protected override bool Satisfied(Pawn pawn)
        {
            LifeStageDef stage = pawn.ageTracker.CurLifeStage;
#if DEBUG
            bool selected = Find.Selector.SingleSelectedThing == pawn;
         //   if (selected&&stage != pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def) Log.Message(string.Format("ThinkNode_ConditionalNotGrown {0} \nCurLifeStage:{1} FinalLifeStage:{2}", pawn.Label, stage, pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def));
#endif
            return stage != pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def;
        }

    }

    public class ThinkNode_ConditionalFacehuggerFertile : ThinkNode_Conditional
    {
        // Token: 0x06000956 RID: 2390 RVA: 0x0004D678 File Offset: 0x0004BA78
        protected override bool Satisfied(Pawn pawn)
        {
            Comp_Facehugger _Facehugger = pawn.TryGetComp<Comp_Facehugger>();
            return _Facehugger.Impregnations < _Facehugger.maxImpregnations;
        }

    }
}
