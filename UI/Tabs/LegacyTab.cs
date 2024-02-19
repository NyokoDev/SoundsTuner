using AlgernonCommons.UI;
using AlgernonCommons.XML;
using AmbientSoundsTuner2;
using AmbientSoundsTuner2.CommonShared;
using AmbientSoundsTuner2.CommonShared.Defs;
using AmbientSoundsTuner2.CommonShared.UI.Extensions;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Detour;
using AmbientSoundsTuner2.Migration;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds;
using AmbientSoundsTuner2.UI;
using ColossalFramework.UI;
using Epic.OnlineServices;
using ICities;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Core.Tokens;

namespace POAIDBOX
{
    public sealed class LegacyTab
    {
        public float currentY = Margin;
        private const float Margin = 5f;
        OptionsPanel OptionsPanel;
        public string[] soundPacks;
        public bool isChangingSoundPackPreset = false;


        public UIHelper modSettingsGroup;
        private UIDropDown soundPackPresetDropDown;
        private UICheckBox debugLoggingCheckBox;
        private UIHelper soundSettingsGroup;
        private UILabel versionInfoLabel;
        public UISlider uiSlider;
        public UIPanel MainPanel;
        public UIScrollbar Scrollbar;
        Mod ModInitializer;

        public Dictionary<string, UIDropDown> soundSelections = new Dictionary<string, UIDropDown>();

        /// <summary>
        /// Creates a tabstrip to refresh the sound packs.
        /// </summary>
        /// <param name="tabStrip"></param>
        /// <param name="tabIndex"></param>
        internal LegacyTab(UITabstrip tabStrip, int tabIndex)
        {
            ModInitializer = new Mod();
            ModInitializer.OnModInitializing();

            MainPanel = UITabstrips.AddTextTab(tabStrip, "Legacy", tabIndex, out UIButton _);


            this.soundPacks = new[] { "Default", "Custom" }.Union(SoundPacksManager.instance.SoundPacks.Values.OrderBy(p => p.Name).Select(p => p.Name)).ToArray();

            soundPackPresetDropDown = UIDropDowns.AddLabelledDropDown(MainPanel, Margin, currentY, "Sound Preset", MainPanel.width);
            soundPackPresetDropDown.eventSelectedIndexChanged += (c, value) => SoundPackPresetDropDownSelectionChanged(value);

            currentY += 31f;
            if (soundPackPresetDropDown == null)
            {
                UnityEngine.Debug.Log("Failed to create dropdown.");
                return;
            }
            soundPackPresetDropDown.items = soundPacks;

            UnityEngine.Debug.Log("[SOUNDSTUNER] SetSliders() called");
            PopulateTabContainer();

        }

        /// <summary>
        /// Populates the Tab.
        /// </summary>
        protected void PopulateTabContainer()
        {
            // Parse all the available sounds first
            var sliders = new Dictionary<string, Dictionary<string, List<ISound>>>();
            foreach (var sound in SoundManager.instance.Sounds.Values)
            {
                if ((DlcUtils.InstalledDlcs & sound.RequiredDlc) != sound.RequiredDlc)
                    continue;

                if (!sliders.ContainsKey(sound.CategoryName))
                    sliders.Add(sound.CategoryName, new Dictionary<string, List<ISound>>());
                if (!sliders[sound.CategoryName].ContainsKey(sound.SubCategoryName))
                    sliders[sound.CategoryName].Add(sound.SubCategoryName, new List<ISound>());
                sliders[sound.CategoryName][sound.SubCategoryName].Add(sound);
            }



            foreach (var tabGroup in sliders)
            {
                // Add extra space between tab groups if needed
                foreach (var group in tabGroup.Value)
                {
                    // Add extra space between tab groups if needed
                    foreach (var sound in group.Value)
                    {
                        this.CreateSoundSlider(sound);

                    }
                    // Increment currentY by a fixed amount after all sliders in the current tab group have been added
                    // Add extra space between tab groups if needed
                }
            }
        }

        /// <summary>
        /// Dropdown change effect.
        /// </summary>
        /// <param name="value"></param>
        public void SoundPackPresetDropDownSelectionChanged(int value)
        {
            this.isChangingSoundPackPreset = true;

            if (value == 0)
            {
                // Default

                foreach (UIDropDown dropDown in this.soundSelections.Values)
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
                    foreach (var dropDown in this.soundSelections)
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
        }






        /// <summary>
        /// Creates sound sliders on command.
        /// </summary>
        /// <param name="sound"></param>
        protected void CreateSoundSlider(ISound sound)
        {

            UIPanel mainPanel = this.MainPanel;



            var configuration = Mod.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);
            var customAudioFiles = SoundPacksManager.instance.AudioFiles.Where(kvp => kvp.Key.StartsWith(string.Format("{0}.{1}", sound.CategoryId, sound.Id))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            float volume = 0;
            if (configuration.ContainsKey(sound.Id))
                volume = configuration[sound.Id].Volume;
            else
            {

                UnityEngine.Debug.LogError($"No volume configuration found for {sound.CategoryId}.{sound.Id}, using default value: {sound.DefaultVolume}");
                volume = sound.DefaultVolume;
            }

            // Add UI components to the main panel
            UISlider uiSlider = mainPanel.AddUIComponent<UISlider>();
            currentY += 6f;
            uiSlider.name = "SoundSlider";
           
            uiSlider.width = 120f;
            uiSlider.relativePosition = new Vector3(Margin, currentY);
            uiSlider.minValue = 0;
            uiSlider.maxValue = sound.MaxVolume;
            uiSlider.stepSize = 0.01f;
            uiSlider.value = volume;
            uiSlider.eventValueChanged += (c, v) => this.SoundVolumeChanged(sound, v);




            // Slider track.
            UISlicedSprite sliderSprite = uiSlider.AddUIComponent<UISlicedSprite>();
            sliderSprite.atlas = UITextures.InGameAtlas;
            sliderSprite.spriteName = "BudgetSlider";
            sliderSprite.size = new Vector2(uiSlider.width, 9f);



            // Slider thumb.
            UISlicedSprite sliderThumb = uiSlider.AddUIComponent<UISlicedSprite>();
            sliderThumb.atlas = UITextures.InGameAtlas;
            sliderThumb.spriteName = "SliderBudget";
            uiSlider.thumbObject = sliderThumb;



            UIPanel uiPanel = uiSlider.parent as UIPanel;
            uiPanel.maximumSize = new Vector2(100000f, 100000f);


            UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
            uiLabel.text = sound.Name;
            uiLabel.autoSize = true;

            uiLabel.relativePosition = new Vector3(0, currentY);
            currentY += uiLabel.height;

            // Increment currentY after adding the label

            UIDropDown uiDropDown = null;
            var maxVolume = sound.MaxVolume;

            if (customAudioFiles.Count > 0 && configuration.ContainsKey(sound.Id) && !string.IsNullOrEmpty(configuration[sound.Id].SoundPack))
            {
                // Custom sound, determine custom max volume
                var audioFile = SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, configuration[sound.Id].SoundPack);
                maxVolume = Mathf.Max(audioFile.AudioInfo.MaxVolume, audioFile.AudioInfo.Volume);
            }

            if (customAudioFiles.Count > 0)
            {
                // Setting up the dropdown if custom audio files are present
                uiDropDown = uiPanel.AddUIComponent<UIDropDown>();
                currentY += 6f;

                uiDropDown.items = new[] { "Default" }.Union(customAudioFiles.Select(kvp => kvp.Value.Name)).ToArray();
                uiDropDown.zOrder = 15;
                uiDropDown.atlas = UITextures.InGameAtlas;
                uiDropDown.normalBgSprite = "TextFieldPanel";
                uiDropDown.disabledBgSprite = "TextFieldPanelDisabled";
                uiDropDown.hoveredBgSprite = "TextFieldPanelHovered";
                uiDropDown.focusedBgSprite = "TextFieldPanelHovered";

                uiDropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
                uiDropDown.listBackground = "TextFieldPanel";
                uiDropDown.itemHover = "ListItemHover";
                uiDropDown.itemHighlight = "ListItemHighlight";
                uiDropDown.color = Color.white;
                uiDropDown.popupColor = Color.white;
                uiDropDown.textColor = Color.black;
                uiDropDown.popupTextColor = Color.black;
                uiDropDown.disabledColor = Color.black;
                uiDropDown.font = UIFonts.SemiBold;


                uiDropDown.height = 30f;
                uiDropDown.width = 80f;
                uiDropDown.textFieldPadding.top = 4;
                if (configuration.ContainsKey(sound.Id) && !string.IsNullOrEmpty(configuration[sound.Id].SoundPack))
                    uiDropDown.selectedValue = configuration[sound.Id].SoundPack;
                else
                    uiDropDown.selectedIndex = 0;

                uiDropDown.eventSelectedIndexChanged += (c, i) => this.SoundPackChanged(sound, i > 0 ? ((UIDropDown)c).items[i] : null, uiSlider);
                this.soundSelections[string.Format("{0}.{1}", sound.CategoryId, sound.Id)] = uiDropDown;


            }

            // Configure UI components
            uiPanel.autoLayout = false;
            uiSlider.width = 120f;
            if (customAudioFiles.Count > 0)
            {

                uiDropDown.width = 80f;
                currentY += 6f;
                uiDropDown.relativePosition = new Vector3(uiSlider.relativePosition.x + uiSlider.width + 20, 0);
             
            }
            else
            {
                uiPanel.size = new Vector2(100000f, 100000f);
            }
        }


        /// <summary>
        /// Method to call when volume is changed.
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="value"></param>
        private void SoundVolumeChanged(ISound sound, float value)
        {
            var configuration = Mod.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);

            if (!configuration.ContainsKey(sound.Id))
                configuration.Add(sound.Id, new ConfigurationV4.Sound());

            configuration[sound.Id].Volume = value;

            if (LoadingManager.instance.m_loadingComplete || !sound.IngameOnly)
                sound.PatchVolume(value);
            else

                Mod.Settings.SaveConfig(Mod.SettingsFilename);
        }

        /// <summary>
        /// Method to call when the sound pack is changed.
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="audioFileName"></param>
        /// <param name="uiSlider"></param>
        private void SoundPackChanged(ISound sound, string audioFileName, UISlider uiSlider)
        {
            var configuration = Mod.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);

            // Selected audio changed
            if (!configuration.ContainsKey(sound.Id))
                configuration.Add(sound.Id, new ConfigurationV4.Sound());

            // Set preset to custom
            if (!this.isChangingSoundPackPreset)
                this.soundPackPresetDropDown.selectedIndex = 1;

            if (!string.IsNullOrEmpty(audioFileName))
            {
                // Chosen audio is a custom audio
                configuration[sound.Id].SoundPack = audioFileName;
                var audioFile = SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, audioFileName);

                if (LoadingManager.instance.m_loadingComplete || !sound.IngameOnly)
                    sound.PatchSound(audioFile);
                else


                    uiSlider.maxValue = Mathf.Max(audioFile.AudioInfo.MaxVolume, audioFile.AudioInfo.Volume);
                uiSlider.value = audioFile.AudioInfo.Volume;
            }
            else
            {
                // Chosen audio is the default one
                configuration[sound.Id].SoundPack = "";

                if (LoadingManager.instance.m_loadingComplete || !sound.IngameOnly)
                    sound.RevertSound();
                else


                    uiSlider.maxValue = sound.MaxVolume;
                uiSlider.value = sound.DefaultVolume;
            }

            Mod.Settings.SaveConfig(Mod.SettingsFilename);
        }
    }
}


