using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using RRYautja.settings;
using RRYautja.ExtensionMethods;

namespace RRYautja
{
    /*
    [HarmonyPatch(typeof(Building_XenoEgg), "get_DefaultGraphic")]
    public static class AvP_Building_XenoEgg_get_DefaultGraphic_Patch
    {
        
        [HarmonyPostfix]
        public static void RoyalEggSize(Thing __instance, ref Graphic __result)
        {
            Graphic value = Traverse.Create(__instance).Field("graphicInt").GetValue<Graphic>();
            bool flag = value != null;
            if (flag)
            {
                if (__instance is Building_XenoEgg)
                {
                //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nis Xeno Egg"));
                    CompXenoHatcher xenoHatcher = __instance.TryGetComp<CompXenoHatcher>();
                    if (xenoHatcher!=null && xenoHatcher.royalProgress>0f)
                    {
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nFound CompXenoHatcher"));
                        float num = (0.7f * xenoHatcher.royalProgress);
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nnum : {0}", num));
                        num += __instance.def.graphicData.drawSize.x;
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\nnum : {0}", num));
                        value = __result.GetCopy(new Vector2((num), (num)));
                    //    Log.Message(string.Format("Building_XenoEgg_get_DefaultGraphic_Patch\value.drawSize : {0}", value.drawSize));
                        __result = value;
                        
                    }
                }
            }
        }
    }
    */
}