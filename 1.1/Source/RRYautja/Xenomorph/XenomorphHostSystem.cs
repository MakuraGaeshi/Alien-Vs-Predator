using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;
using HunterMarkingSystem.Settings;
using HunterMarkingSystem.ExtensionMethods;
using RRYautja.ExtensionMethods;

namespace RRYautja.Xenomorph
{
    [StaticConstructorOnStartup]
    public class XenomorphHostSystem
    {
        protected static List<ThingDef> XenomorphHostList = DefDatabase<ThingDef>.AllDefs.Where(x => x.race!=null && x.isPotentialHost()).ToList();
        static XenomorphHostSystem()
        {
            Log.Message(string.Format("Xenomorph Host System Loaded\n{0} Possible Host Races detected", XenomorphHostList.Count));
            /*
            DefDatabase<ThingDef>.AllDefsListForReading.ForEach(action: td => 
            {
                if (td.race!=null && td.isPotentialHost())
                {
                    string text = string.Format("{0}'s possible Xenoforms", td.LabelCap);
                    Log.Message(text);

                    foreach (var item in td.resultingXenomorph())
                    {
                        text = item.LabelCap;
                        Log.Message(text);
                    }
                }
            });
            */
        }
    }

}