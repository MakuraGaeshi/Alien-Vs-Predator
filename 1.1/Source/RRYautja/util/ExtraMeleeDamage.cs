using System;

namespace Verse
{
    // Token: 0x02000B67 RID: 2919
    public class ExtraMeleeDamage
    {
        // Token: 0x06004080 RID: 16512 RVA: 0x001E3B01 File Offset: 0x001E1F01
        public float AdjustedDamageAmount(Verb verb, Pawn caster)
        {
            return (float)this.amount * verb.verbProps.GetDamageFactorFor(verb, caster);
        }

        // Token: 0x06004081 RID: 16513 RVA: 0x001E3B18 File Offset: 0x001E1F18
        public float AdjustedArmorPenetration(Verb verb, Pawn caster)
        {
            if (this.armorPenetration < 0f)
            {
                return this.AdjustedDamageAmount(verb, caster) * 0.015f;
            }
            return this.armorPenetration;
        }

        // Token: 0x04002A38 RID: 10808
        public DamageDef def;

        // Token: 0x04002A39 RID: 10809
        public int amount;

        // Token: 0x04002A3A RID: 10810
        public float armorPenetration = -1f;
    }
}
