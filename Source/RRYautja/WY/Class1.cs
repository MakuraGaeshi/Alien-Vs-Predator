using System;
using RimWorld;
using Verse;

namespace RRYautja
{
    // Token: 0x02000002 RID: 2
    public class GetPawnThing : MoteThrown
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override void Tick()
        {
            bool flag = base.Map == null;
            if (flag)
            {
                this.Destroy(0);
            }
            PawnKindDef wildMan = PawnKindDefOf.WildMan;
            PawnGenerationRequest pawnGenerationRequest;
            pawnGenerationRequest = new PawnGenerationRequest(wildMan, null, (PawnGenerationContext)2, -1, true, false, false, false, true, false, 20f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            pawn.ageTracker.AgeBiologicalTicks = 70000000L;
            GenSpawn.Spawn(pawn, base.Position, base.Map, 0);
            this.Destroy(0);
        }
    }
}