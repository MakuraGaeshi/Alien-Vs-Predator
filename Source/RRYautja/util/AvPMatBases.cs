using System;
using UnityEngine;
using Verse;

namespace RRYautja
{
    // Token: 0x02000087 RID: 135
    [StaticConstructorOnStartup]
    public static class AvP_MatPool
    {
        // Token: 0x0400034F RID: 847
        public static readonly Material hiveMaterial = MaterialPool.MatFrom("Other/HiveMat", true);
        
    //    public static readonly Texture2D monkIcon = ContentFinder<Texture2D>.Get("Other/ClassTextures/monkPsiMark", true);
    }
}
