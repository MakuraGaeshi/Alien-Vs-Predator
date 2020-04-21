using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Text.RegularExpressions;
using System.Xml;

namespace AvP.HarmonyInstance
{
    [HarmonyPatch(typeof(BackCompatibility), "GetBackCompatibleType")]
    public static class AvP_BackCompatibility_GetBackCompatibleType_Patch
    {
        [HarmonyPostfix]
        public static void GetBackCompatibleType_Postfix(string providedClassName, ref Type __result)
        {
            if (providedClassName.Contains("RRYautja"))
            {
                string name = Regex.Replace(providedClassName, "RRYautja", "AvP");
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type!=null)
                {
                    __result = type;
                }
            }

            
            if (providedClassName.Contains("ThingDef_TunnelHiveLikeSpawner"))
            {
                string name = Regex.Replace(providedClassName, "ThingDef_TunnelHiveLikeSpawner", "ThingDef");
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type != null)
                {
                    __result = type;
                }
            }
            else
            if (providedClassName.Contains("TunnelHiveLikeSpawner"))
            {
                string name = Regex.Replace(providedClassName, "TunnelHiveLikeSpawner", "XenomorphTunnelSpawner");
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type != null)
                {
                    __result = type;
                }
            }
            if (providedClassName.Contains("ThingDef_HiveLike"))
            {
                string name = Regex.Replace(providedClassName, "ThingDef_HiveLike", "ThingDef");
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type != null)
                {
                    __result = type;
                }
            }
            else
            if (providedClassName.Contains("HiveLike"))
            {
                string name = Regex.Replace(providedClassName, "HiveLike", "XenomorphHive");
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type != null)
                {
                    __result = type;
                }
            }
            


        }
    }

}
