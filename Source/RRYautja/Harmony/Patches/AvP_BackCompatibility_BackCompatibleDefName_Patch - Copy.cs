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
        public static void GetBackCompatibleType_Postfix(Type baseType, string providedClassName, XmlNode node, ref Type __result)
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
        }
    }

}
