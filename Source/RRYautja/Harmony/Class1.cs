using System;
using System.Collections.Generic;
using Harmony;
using Verse;

namespace RRYautja
{
    // Token: 0x02000002 RID: 2
    [StaticConstructorOnStartup]
    internal static class ExtraButcheringProducts
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static ExtraButcheringProducts()
        {
            HarmonyInstance.Create("ogliss.rimworld.framework.extrabutchering").Patch(AccessTools.Method(typeof(Thing), "ButcherProducts", null, null), null, new HarmonyMethod(typeof(ExtraButcheringProducts), "MakeButcherProducts_PostFix", null), null);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002090 File Offset: 0x00000290
        private static void MakeButcherProducts_PostFix(Thing __instance, ref IEnumerable<Thing> __result, Pawn butcher, float efficiency)
        {
            CompSpecialButcherChance compSpecialButcherChance;
            if ((compSpecialButcherChance = ThingCompUtility.TryGetComp<CompSpecialButcherChance>(__instance)) != null)
            {
                if (GenList.NullOrEmpty<ThingDefCountWithChanceClass>(compSpecialButcherChance.Props.butcherProducts))
                {
                    return;
                }
                foreach (ThingDefCountWithChanceClass thingDefCountWithChanceClass in compSpecialButcherChance.Props.butcherProducts)
                {
                    if (Rand.Chance(thingDefCountWithChanceClass.chance))
                    {
                        ThingDefCountWithChanceClass thingDefCountWithChanceClass2 = new ThingDefCountWithChanceClass
                        {
                            thingDef = thingDefCountWithChanceClass.thingDef,
                            count = thingDefCountWithChanceClass.count
                        };
                        int num = GenMath.RoundRandom((float)thingDefCountWithChanceClass2.count * efficiency);
                        if (num > 0)
                        {
                            Thing thing = ThingMaker.MakeThing(thingDefCountWithChanceClass2.thingDef, null);
                            thing.stackCount = num;
                            __result = __result.Add(thing);
                        }
                    }
                }
            }
        }
    }
}
