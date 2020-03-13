using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
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
            new Harmony("ogliss.rimworld.framework.extrabutchering").Patch(AccessTools.Method(typeof(Thing), "ButcherProducts", null, null), null, new HarmonyMethod(typeof(ExtraButcheringProducts), "MakeButcherProducts_PostFix", null), null);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002090 File Offset: 0x00000290
        private static void MakeButcherProducts_PostFix(Thing __instance, ref IEnumerable<Thing> __result, Pawn butcher, float efficiency)
        {
        //    Log.Message(string.Format("{0}", __instance));
            if (__instance is Pawn pawn)
            {
            //    Log.Message(string.Format("{0} is Pawn pawn", __instance));
                if (XenomorphUtil.IsXenomorph(pawn))
                {
                //    Log.Message(string.Format("{0} Pawn pawn is Xenomorph", __instance));
                    foreach (var item in pawn.health.hediffSet.GetNotMissingParts())
                    {
                        if (item.def == XenomorphDefOf.RRY_Xeno_TailSpike)
                        {
                            ThingDefCountClass thingDefCountClass = new ThingDefCountClass
                            {
                                thingDef = XenomorphDefOf.RRY_Xenomorph_TailSpike,
                                count = 1
                            };
                            Thing thing = ThingMaker.MakeThing(thingDefCountClass.thingDef, null);
                            thing.stackCount = thingDefCountClass.count;
                            __result = __result.AddItem(thing);
                        }
                        if (item.def == XenomorphDefOf.RRY_Xeno_Shell)
                        {
                            ThingDefCountClass thingDefCountClass = new ThingDefCountClass
                            {
                                thingDef = XenomorphDefOf.RRY_Xenomorph_HeadShell,
                                count = 1
                            };
                            Thing thing = ThingMaker.MakeThing(thingDefCountClass.thingDef, null);
                            thing.stackCount = thingDefCountClass.count;
                            __result = __result.AddItem(thing);
                        }
                    }
                }
            }
        }
    }
}
