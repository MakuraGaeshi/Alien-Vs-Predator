using RimWorld;
using System;
using Verse;

namespace RRYautja
{
	// Token: 0x02000D5B RID: 3419
	public class HediffComp_XenoFacehugger : HediffComp
    {
        public PawnKindDef pawnKindDef = YautjaDefOf.Xenomorph_FaceHugger;
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_XenoFacehugger Props
		{
			get
			{
				return (HediffCompProperties_XenoFacehugger)this.props;
			}
		}

        public override void CompPostPostRemoved()
        {
            PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, null, PawnGenerationContext.NonPlayer, -1, true, false, true, false, true, true, 20f);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            if (Props.spawnLive == true)
            {
                pawn.ageTracker.AgeBiologicalTicks = 0;
            }
            if (Props.spawnLive == false) { pawn.Kill(null); }
            GenSpawn.Spawn(pawn, base.parent.pawn.Position, base.parent.pawn.Map, 0);
        }
    }

    public class HediffComp_XenoSpawner : HediffComp
    {
        // Token: 0x17000BE6 RID: 3046
        // (get) Token: 0x06004C0F RID: 19471 RVA: 0x002370CE File Offset: 0x002354CE
        public HediffCompProperties_XenoSpawner Props
        {
            get
            {
                return (HediffCompProperties_XenoSpawner)this.props;
            }
        }

        public override void Notify_PawnDied()
        {
            PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(Props.pawnKindDef, null, PawnGenerationContext.NonPlayer, -1, true, false, true, false, true, true, 20f);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            pawn.ageTracker.AgeBiologicalTicks = 0;
            GenSpawn.Spawn(pawn, base.parent.pawn.PositionHeld, base.parent.pawn.MapHeld, 0);
        }
    }
}
