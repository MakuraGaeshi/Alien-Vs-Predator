﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HunterMarkingSystem.ExtensionMethods;
using UnityEngine;
using Verse;

namespace HunterMarkingSystem.Settings
{
    static internal class SettingsHelper
    {
        public static HMSSettings latest;
    }

    class HMSMod : Mod
    {
        private HMSSettings settings;
        public HMSMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<HMSSettings>();
            SettingsHelper.latest = this.settings;
        }

        public override string SettingsCategory() => "Hunter Marking System";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            this.settings.MinWorthyKill = Widgets.HorizontalSlider(inRect.TopHalf().BottomHalf().TopHalf().BottomHalf().ContractedBy(4),
                this.settings.MinWorthyKill, 0f, 3f, true,
                "HMS_MinScoreFactor".Translate(this.settings.MinWorthyKill * 100)
                , "0%", "300%");

            Widgets.TextFieldNumeric<float>(inRect.TopHalf().TopHalf().TopHalf().BottomHalf().LeftHalf().LeftHalf().ContractedBy(4), ref settings.MinWorthyKill, ref settings.MinWorthyKillBuffer, 0.001f, 10f);

            float num = 800f;
            float x = inRect.x;
            float num2 = inRect.y;
            float num3 = inRect.y;
            List<ThingDef> WorthyKillDefs = HunterMarkingSystem.RaceDefaultMarkDict.Keys.ToList();
            List<string> listed = new List<string>();
            Widgets.Label(inRect.TopHalf().BottomHalf().BottomHalf().BottomHalf().ContractedBy(4), "HMS_KillMarksScores".Translate(WorthyKillDefs.Count));
            Widgets.BeginScrollView(inRect.BottomHalf().ContractedBy(4), ref this.pos2, new Rect(inRect.x, inRect.y, num, WorthyKillDefs.Count * 22f), true);
            foreach (ThingDef td in WorthyKillDefs.OrderBy(xz=> HunterMarkingSystem.RaceDefaultMarkDict.TryGetValue(xz).MarkDef.stages[0].label))
            {
                if (!listed.Contains(td.label))
                {
                    listed.Add(td.label);
                    MarkData markData = HunterMarkingSystem.RaceDefaultMarkDict.TryGetValue(td);
                    Widgets.Label(new Rect(x, num3, num, 32f), markData.MarkDef.stages[0].label + " : " + markData.Label + " Score: " + markData.MarkScore);
                    num3 += 22f;
                }
            }
            Widgets.EndScrollView();

            /* 
        //    Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().BottomHalf().TopHalf().ContractedBy(4), "setting3: Desc", ref settings.setting3);
        //    Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().BottomHalf().BottomHalf().ContractedBy(4), "setting4: Desc", ref settings.setting4);

            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().TopHalf().TopHalf().ContractedBy(4), "setting5: Desc", ref settings.setting5);
            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().TopHalf().BottomHalf().ContractedBy(4), "setting6: Desc", ref settings.setting6);
            
            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().BottomHalf().TopHalf().ContractedBy(4), "setting7: Desc", ref settings.setting7);
            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().BottomHalf().BottomHalf().ContractedBy(4), "setting8: Desc", ref settings.setting8);
            */
            this.settings.Write();
        }
        private Vector2 pos = new Vector2(0f, 0f);
        private Vector2 pos2 = new Vector2(0f, 0f);

    }
    
    class HMSSettings : ModSettings
    {
        public float MinWorthyKill= 0.35f;
        public string MinWorthyKillString = string.Empty;
        public string MinWorthyKillBuffer;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.MinWorthyKill, "MinWorthyKill", 0.75f);
        }


    }
}