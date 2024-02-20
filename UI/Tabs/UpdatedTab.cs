using AlgernonCommons.Translation;
using AlgernonCommons.UI;
using AmbientSoundsTuner2;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.UI;
using ColossalFramework.UI;
using LuminaTR;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core.Tokens;

namespace POAIDBOX
{
    public sealed class UpdatedTab
    {

        float currentY = Margin;
        private const float Margin = 5f;
       
        ModOptionsPanel LegacyPanel;
        public static UIDropDown dropdown;


        /// <summary>
        /// Creates a tabstrip to refresh the sound packs.
        /// </summary>
        /// <param name="tabStrip"></param>
        /// <param name="tabIndex"></param>
        internal UpdatedTab(UITabstrip tabStrip, int tabIndex)
        {
            if (tabStrip == null)
            {
                UnityEngine.Debug.Log("tabStrip is null.");
                return;
            }

            UIPanel panel = UITabstrips.AddTextTab(tabStrip, "Updated", tabIndex, out UIButton _);
            if (panel == null)
            {
                UnityEngine.Debug.Log("Failed to create panel.");
                return;
            }
            currentY += 30f;

          

            UIDropDown languageDropDown = UIDropDowns.AddPlainDropDown(panel, Margin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            currentY += 70f;

            dropdown = UIDropDowns.AddLabelledDropDown(panel, Margin, currentY, Translations.Translate(TranslationID.SOUND_PRESET), panel.width - 100f);
            dropdown.items = LegacyTab.soundPacks;
            currentY += 70f;
            if (dropdown == null)
            {
                UnityEngine.Debug.Log("Failed to create dropdown.");
                return;
            }

       
            // Assuming dropdown is an instance of your dropdown control
     


            UIButton supportbutton = UIButtons.AddSmallerButton(panel, Margin, currentY, Translations.Translate(TranslationID.SUPPORT));
            currentY += 30f;
            if (supportbutton == null)
            {
                UnityEngine.Debug.Log("Failed to create support button.");
                return;
            }

            supportbutton.eventClicked += (sender, args) =>
            {
                Process.Start("https://discord.gg/gdhyhfcj7A");
            };

            UILabel version = UILabels.AddLabel(panel, Margin, currentY, Assembly.GetExecutingAssembly().GetName().Version.ToString(), textScale: 0.7f, alignment: UIHorizontalAlignment.Center);
            if (version == null)
            {
                UnityEngine.Debug.Log("Failed to create version label.");
            }
        }

       
        public void PopulateDropdown()
        {
          
        }
    }
}


   