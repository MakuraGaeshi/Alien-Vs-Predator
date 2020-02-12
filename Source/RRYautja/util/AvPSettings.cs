using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using HunterMarkingSystem;
using RRYautja.ExtensionMethods;
using UnityEngine;
using Verse;

namespace RRYautja.settings
{
    static internal class SettingsHelper
    {
        public static AvPSettings latest;
    }

    class AvPMod : Mod
    {
        private AvPSettings settings;
        public AvPMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<AvPSettings>();
            SettingsHelper.latest = this.settings;
        }

        public override string SettingsCategory() => "Aliens Vs Predator";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            /*
            this.settings.astartePunchingFactor = Widgets.HorizontalSlider(
                inRect.TopHalf().TopHalf().TopHalf().ContractedBy(4),
                this.settings.astartePunchingFactor, 0f, 5f, true,
                "Astarte Punching Power : " + this.settings.astartePunchingFactor * 100 +
                "% : [" + 15f * this.settings.astartePunchingFactor + "]\nDefault possible in single attack (Punch 15 at 100%)"
                , "0%", "500%");

            this.settings.astarteSplitFactor = Widgets.HorizontalSlider(
                inRect.TopHalf().TopHalf().BottomHalf().ContractedBy(4),
                this.settings.astarteSplitFactor, 0f, 5f, true,
                "Astarte Spit Power : " + this.settings.astarteSplitFactor * 100 +
                "% : [" + 80f * this.settings.astarteSplitFactor +
                "]\nDefault possible in single attack (Caustic Spit 80 at 100%)"
                , "0%", "500%");
                
            this.settings.scale = Widgets.HorizontalSlider(inRect.TopHalf().BottomHalf().TopHalf().ContractedBy(4),
                this.settings.scale, 0f, 2f, true,
                "Astarte Size Scaler: " + this.settings.astartePunchingFactor * 100 +
                "% for size of " + 3f * this.settings.scale
                , "0%", "200%");

                */

            Widgets.Label(inRect.TopHalf().TopHalf().TopHalf().TopHalf().ContractedBy(4),
                "Restart before playing to ensure your changes take effect.");
            Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().TopHalf().BottomHalf().ContractedBy(4), "RRY_AllowYautjaFaction".Translate(), ref settings.AllowYautjaFaction);
            Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().BottomHalf().TopHalf().ContractedBy(4), "RRY_AllowXenomorphFaction".Translate(), ref settings.AllowXenomorphFaction);
            Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().BottomHalf().BottomHalf().LeftHalf().ContractedBy(4), "RRY_AllowHiddenInfections".Translate(), ref settings.AllowHiddenInfections);
            Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().BottomHalf().BottomHalf().RightHalf().ContractedBy(4), "RRY_AllowPredalienImpregnations".Translate(), ref settings.AllowHiddenInfections);

            this.settings.fachuggerRemovalFailureDeathChance = Widgets.HorizontalSlider(inRect.TopHalf().BottomHalf().TopHalf().TopHalf().ContractedBy(4),
                this.settings.fachuggerRemovalFailureDeathChance, 0f, 1f, true,
                "RRY_FacehuggerRemovalDeathChance".Translate(this.settings.fachuggerRemovalFailureDeathChance * 100)
                , "0%", "100%");

            this.settings.embryoRemovalFailureDeathChance = Widgets.HorizontalSlider(inRect.TopHalf().BottomHalf().TopHalf().BottomHalf().ContractedBy(4),
                this.settings.embryoRemovalFailureDeathChance, 0f, 1f, true,
                "RRY_EmbryoRemovalDeathChance".Translate(this.settings.embryoRemovalFailureDeathChance * 100)
                , "0%", "100%");

            //    Widgets.BeginScrollView(inRect.BottomHalf().BottomHalf().BottomHalf().LeftHalf().ContractedBy(4), ref );


            float num = 400f;
            float x = inRect.x;
            float num2 = inRect.y;
            float num3 = inRect.y;
            List<ThingDef> suitablehostDefs = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(xx => XenomorphUtil.isInfectableThing(xx));
            Widgets.Label(inRect.TopHalf().BottomHalf().BottomHalf().BottomHalf().LeftHalf().ContractedBy(4), "RRY_SuitableHostKinds".Translate(suitablehostDefs.Count));
            Widgets.BeginScrollView(inRect.BottomHalf().LeftHalf().ContractedBy(4), ref this.pos, new Rect(inRect.x, inRect.y, num, suitablehostDefs.Count * 22f), true);
            foreach (ThingDef pkd in suitablehostDefs.OrderBy(xy=> xy.label))
            {
                string text = pkd.LabelCap;
                /*
                text += " possible Xenoforms:";
                foreach (var item in pkd.resultingXenomorph())
                {
                    text += " "+item.LabelCap;
                }
                */
                Widgets.Label(new Rect(x, num2, num, 32f), text);
                num2 += 22f;
            }
            Widgets.EndScrollView();
            /*
            List<PawnKindDef> WorthyKillDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(xx => HMSUtility.WorthyKill(xx));
            Widgets.Label(inRect.TopHalf().BottomHalf().BottomHalf().BottomHalf().RightHalf().ContractedBy(4), "RRY_WorthyKillKinds".Translate(WorthyKillDefs.Count));
            Widgets.BeginScrollView(inRect.BottomHalf().RightHalf().ContractedBy(4), ref this.pos2, new Rect(inRect.x, inRect.y, num, WorthyKillDefs.Count * 22f), true);
            foreach (PawnKindDef pkd in WorthyKillDefs.OrderBy(xz=> HMSUtility.GetMark(xz).stages[0].label))
            {
                Widgets.Label(new Rect(x, num3, num, 32f), HMSUtility.GetMark(pkd).stages[0].label + " : "+ pkd.LabelCap);
                num3 += 22f;
            }
            Widgets.EndScrollView();
            */

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
    
    class AvPSettings : ModSettings
    {
        public bool AllowXenomorphFaction = true, AllowYautjaFaction = true, AllowHiddenInfections = true, AllowPredalienImpregnations = true;
        public float fachuggerRemovalFailureDeathChance= 0.35f, embryoRemovalFailureDeathChance = 0.35f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.AllowXenomorphFaction, "AllowXenomorphFaction", true);
            Scribe_Values.Look(ref this.AllowYautjaFaction, "AllowYautjaFaction", true);
            Scribe_Values.Look(ref this.AllowHiddenInfections, "AllowHiddenInfections", true);
            Scribe_Values.Look(ref this.AllowPredalienImpregnations, "AllowPredalienImpregnations", true);
            Scribe_Values.Look<float>(ref this.fachuggerRemovalFailureDeathChance, "fachuggerRemovalFailureDeathChance", 0.35f);
            Scribe_Values.Look<float>(ref this.embryoRemovalFailureDeathChance, "embryoRemovalFailureDeathChance", 0.35f);
        }


    }
}