using UnityEngine;
using Verse;

namespace AvP
{
    // Token: 0x02000002 RID: 2
    public class TrailThrower
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00001050
        public static void ThrowSmokeTrail(Vector3 loc, float size, Map map, string DefName)
        {
            MoteCounter moteCounter = new MoteCounter();
            bool flag = !loc.ShouldSpawnMotesAt(map) || moteCounter.SaturatedLowPriority;
            if (!flag)
            {
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named(DefName), null);
                moteThrown.Scale = Rand.Range(2f, 3f) * size;
                moteThrown.exactPosition = loc;
                moteThrown.rotationRate = Rand.Range(-0.5f, 0.5f);
                moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.008f, 0.012f));
                GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            }
        }
    }
}
