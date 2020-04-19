using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x0200029B RID: 667
    public class DefaultOutfitDef : Def
    {
        // Token: 0x06000B5D RID: 2909 RVA: 0x0005A917 File Offset: 0x00058D17
        public static DefaultOutfitDef Named(string defName)
        {
            return DefDatabase<DefaultOutfitDef>.GetNamed(defName, true);
        }

        [MustTranslate]
        public string pawnsPlural = "members";

        [NoTranslate]
        public List<string> UsesTags = new List<string>();
        public ThingDef raceDef = null;
    }
}
