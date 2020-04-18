using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using AvP.settings;
using AvP.ExtensionMethods;

namespace AvP.HarmonyInstance
{
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
    public static class AvP_ApparelGraphicRecordGetter_TryGetGraphicApparel_YautjaSpecificHat_Patch
    {
        // Token: 0x0600004B RID: 75 RVA: 0x0000349C File Offset: 0x0000169C
        [HarmonyPostfix]
        public static void Yautja_SpecificHatPatch(ref Apparel apparel, ref BodyTypeDef bodyType, ref ApparelGraphicRecord rec)
        {
            bool flag = bodyType == YautjaDefOf.RRYYautjaFemale || bodyType == YautjaDefOf.RRYYautjaMale;
            if (flag)
            {
                bool flag2 = apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead;
                if (flag2)
                {
                    string text = apparel.def.apparel.wornGraphicPath + "_" + bodyType.defName;
                    bool flag3 = ContentFinder<Texture2D>.Get(text + "_north", false) == null || ContentFinder<Texture2D>.Get(text + "_east", false) == null || ContentFinder<Texture2D>.Get(text + "_south", false) == null;
                    if (!flag3)
                    {
                        Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(text, ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, apparel.DrawColor);
                        rec = new ApparelGraphicRecord(graphic, apparel);
                    }
                }
                else
                {
                    bool flag4 = !GenText.NullOrEmpty(apparel.def.apparel.wornGraphicPath);
                    if (flag4)
                    {
                        string text2 = apparel.def.apparel.wornGraphicPath + "_" + bodyType.defName;
                        bool flag5 = ContentFinder<Texture2D>.Get(text2 + "_north", false) == null || ContentFinder<Texture2D>.Get(text2 + "_east", false) == null || ContentFinder<Texture2D>.Get(text2 + "_south", false) == null;
                        if (flag5)
                        {
                            text2 = apparel.def.apparel.wornGraphicPath + "_Female";
                            Graphic graphic2 = GraphicDatabase.Get<Graphic_Multi>(text2, ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, apparel.DrawColor);
                            rec = new ApparelGraphicRecord(graphic2, apparel);
                        }
                    }
                }
            }
        }
    }
}