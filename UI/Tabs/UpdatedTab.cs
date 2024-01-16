using AlgernonCommons.UI;
using AmbientSoundsTuner2;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.UI;
using ColossalFramework.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace POAIDBOX
{
    public sealed class UpdatedTab
    {
        Mod Mod;
        private AutoTabstrip tabStrip;
        private int tabindex;
        private int v;
        private const float LabelWidth = 40f;
        float currentY = Margin;
        private const float Margin = 5f;
        private string[] soundPacks;
        ModOptionsPanel LegacyPanel;
        UIDropDown dropdown;


        /// <summary>
        /// Creates a tabstrip to refresh the sound packs.
        /// </summary>
        /// <param name="tabStrip"></param>
        /// <param name="v"></param>
        internal UpdatedTab(AutoTabstrip tabStrip, int tabindex)
        {
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, "Test2", tabindex, out UIButton _,  250f) ;
     


          
            
            dropdown = UIDropDowns.AddLabelledDropDown(panel, Margin, currentY, "Sound Preset", 220f);
            dropdown.items = soundPacks;
            dropdown.selectedValue = Mod.Settings.SoundPackPreset;
            dropdown.eventSelectedIndexChanged += OnSelectedIndexChanged;
            currentY += 31f;


            UIButton supportbutton = UIButtons.AddSmallerButton(panel, LabelWidth, currentY, "Support");
            currentY += 50f;
            supportbutton.eventClicked += (sender, args) =>
            {
                Process.Start("https://discord.gg/gdhyhfcj7A");
            };





            UILabel version = UILabels.AddLabel(panel, LabelWidth, currentY, Assembly.GetExecutingAssembly().GetName().Version.ToString(), textScale: 0.7f, alignment: UIHorizontalAlignment.Center);




            this.tabStrip = tabStrip;
            this.tabindex = 1;

        }

        /// <summary>
        /// Changes the audio preset on dropdown change.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public void OnSelectedIndexChanged(UIComponent component, int value)
        {
            LegacyPanel.SoundPackPresetDropDownSelectionChanged(value);
        }

        /// <summary>
        /// Legacy method to change the audio preset.
        /// </summary>
        /// <param name="value"></param>
        public void SoundPackPresetDropDownSelectionChanged(int value)
        {
            if (value == 0)
          
            {
                // Default
    
                foreach (UIDropDown dropDown in LegacyPanel.soundSelections.Values)
                    dropDown.selectedIndex = 0;
            }
            else if (value == 1)
            {
                // Custom, don't do anything here
            }
            else if (value >= 2)
            {
                // Sound pack
                string soundPackName = this.soundPacks[value];
                SoundPacksFileV1.SoundPack soundPack = null;
              

                if (SoundPacksManager.instance.SoundPacks.TryGetValue(soundPackName, out soundPack))
                {
                    foreach (var dropDown in LegacyPanel.soundSelections)
                    {
                        var prefix = dropDown.Key.Substring(0, dropDown.Key.IndexOf('.'));
                        var id = dropDown.Key.Substring(dropDown.Key.IndexOf('.') + 1);
                        SoundPacksFileV1.Audio[] audios = null;
                        switch (prefix)
                        {
                            case "Ambient":
                                audios = soundPack.Ambients;
                                break;
                            case "AmbientNight":
                                audios = soundPack.AmbientsNight;
                                break;
                            case "Animal":
                                audios = soundPack.Animals;
                                break;
                            case "Building":
                                audios = soundPack.Buildings;
                                break;
                            case "Vehicle":
                                audios = soundPack.Vehicles;
                                break;
                            case "Misc":
                                audios = soundPack.Miscs;
                                break;
                        }
                        if (audios != null)
                        {
                            SoundPacksFileV1.Audio audio = audios.FirstOrDefault(a => a.Type == id);
                            if (audio != null)
                            {
                               
                                dropDown.Value.selectedValue = audio.Name;
                            }
                        }
                    }
                }
            }

            Mod.Settings.SoundPackPreset = dropdown.selectedValue;

        }
    }
}

