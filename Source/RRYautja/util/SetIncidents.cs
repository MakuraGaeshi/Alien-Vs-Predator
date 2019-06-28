using System;
using RimWorld;
using RRYautja.settings;
using Verse;

namespace RRYautja
{
    // Token: 0x02000007 RID: 7
    public static class SetIncidents
    {
        // Token: 0x06000010 RID: 16 RVA: 0x000026AC File Offset: 0x000016AC
        public static void SetIncidentLevels()
        {
            foreach (IncidentDef incidentDef in DefDatabase<IncidentDef>.AllDefsListForReading)
            {
                if (incidentDef == XenomorphDefOf.RRY_Neomorph_FungusSprout)
                {
                    if (SettingsHelper.latest.AllowXenomorphFaction)
                    {
                           incidentDef.baseChance = 0.1f;
                    }
                    else
                    {
                        incidentDef.baseChance = 0f;
                    }
                }
                if (incidentDef == XenomorphDefOf.RRY_Neomorph_FungusSprout_Hidden)
                {
                    if (SettingsHelper.latest.AllowXenomorphFaction || SettingsHelper.latest.AllowXenomorphFaction)
                    {
                           incidentDef.baseChance = 0.1f;
                    }
                    else
                    {
                        incidentDef.baseChance = 0f;
                    }
                }
                if (incidentDef == XenomorphDefOf.RRY_XenomorphCrashedShipPartCrash)
                {
                    if (SettingsHelper.latest.AllowXenomorphFaction)
                    {
                        incidentDef.baseChance = 2.0f;
                    }
                    else
                    {
                        incidentDef.baseChance = 0f;
                    }
                }
                if (incidentDef == XenomorphDefOf.RRY_XenomorphInfestation)
                {
                    if (SettingsHelper.latest.AllowXenomorphFaction)
                    {
                           incidentDef.baseChance = 2.7f;
                    }
                    else
                    {
                        incidentDef.baseChance = 0f;
                    }
                }
            }
        }
    }
}
