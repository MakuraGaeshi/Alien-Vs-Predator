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
    // Prevents disabled factions from generating in a new world
    [HarmonyPatch(typeof(FactionGenerator), "GenerateFactionsIntoWorld", null)]
    public static class AvP_FactionGenerator_GenerateFactionsIntoWorld_Patch
    {
        // Token: 0x06000012 RID: 18 RVA: 0x000027D0 File Offset: 0x000017D0
        public static bool Prefix()
        {
            int i = 0;
            int num = 0;
            foreach (FactionDef factionDef in DefDatabase<FactionDef>.AllDefs)
            {
                if (!factionDef.isPlayer)
                {
                    string defName = factionDef.defName;
                    if (factionDef == XenomorphDefOf.RRY_Xenomorph && !SettingsHelper.latest.AllowXenomorphFaction)
                    {
                        AvP_FactionGenerator_GenerateFactionsIntoWorld_Patch.UpdateDef(factionDef, 0);
                        //    return false;
                    }
                    if (defName.Contains("RRY_Yautja_") && !SettingsHelper.latest.AllowYautjaFaction)
                    {
                        AvP_FactionGenerator_GenerateFactionsIntoWorld_Patch.UpdateDef(factionDef, 0);
                        //    return false;
                    }
                }
            }
            return true;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002C2C File Offset: 0x00001C2C
        private static void UpdateDef(FactionDef def, int requiredCount)
        {
            def.requiredCountAtGameStart = requiredCount;
            if (def.requiredCountAtGameStart < 1)
            {
                def.maxCountAtGameStart = 0;
                return;
            }
            def.maxCountAtGameStart = 100;
        }
    }
}