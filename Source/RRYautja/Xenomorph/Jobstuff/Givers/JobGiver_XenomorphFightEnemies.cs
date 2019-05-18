using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020000B4 RID: 180
    public class JobGiver_XenomorphFightEnemies : JobGiver_AIFightEnemy
    {
        // Token: 0x06000458 RID: 1112 RVA: 0x0002C7FC File Offset: 0x0002ABFC
        protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
        {
            Thing enemyTarget = pawn.mindState.enemyTarget;
            bool allowManualCastWeapons = !pawn.IsColonist;
            bool allowRanged = Rand.Chance(0.5f);
            Verb verb = allowRanged ? pawn.meleeVerbs.TryGetMeleeVerb(enemyTarget) : pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            if (verb == null)
            {
            //    Log.Message(string.Format("JobGiver_XenomorphFightEnemies Allow ranged: {0}, Verb: {1}", verb, allowRanged));
                dest = IntVec3.Invalid;
                return false;
            }
            return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = verb,
                maxRangeFromTarget = verb.verbProps.range,
                wantCoverFromTarget = (verb.verbProps.range > 5f)
            }, out dest);
        }
    }
}
