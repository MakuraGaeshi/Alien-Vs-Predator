using System.Linq;
using System.Reflection;
using Harmony;
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

            Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().TopHalf().TopHalf().ContractedBy(4), "RRY_AllowHiddenInfections".Translate(), ref settings.AllowHiddenInfections);
            //    Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().TopHalf().BottomHalf().ContractedBy(4), "setting2: Desc", ref settings.setting2);
            /*
            Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().BottomHalf().TopHalf().ContractedBy(4), "setting3: Desc", ref settings.setting3);
            Widgets.CheckboxLabeled(inRect.TopHalf().TopHalf().BottomHalf().BottomHalf().ContractedBy(4), "setting4: Desc", ref settings.setting4);

            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().TopHalf().TopHalf().ContractedBy(4), "setting5: Desc", ref settings.setting5);
            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().TopHalf().BottomHalf().ContractedBy(4), "setting6: Desc", ref settings.setting6);
            
            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().BottomHalf().TopHalf().ContractedBy(4), "setting7: Desc", ref settings.setting7);
            Widgets.CheckboxLabeled(inRect.TopHalf().BottomHalf().BottomHalf().BottomHalf().ContractedBy(4), "setting8: Desc", ref settings.setting8);
            */
            Widgets.Label(inRect.BottomHalf().BottomHalf().BottomHalf(),
                "Restart before playing to ensure your changes take effect.");
            this.settings.Write();
        }
    }


    class AvPSettings : ModSettings
    {
        public bool AllowHiddenInfections, setting2;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.AllowHiddenInfections, "AllowHiddenInfections", true);
            Scribe_Values.Look(ref this.setting2, "setting2", false);
        }
    }
}