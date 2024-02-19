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
            currentY += 31f;
            soundPackPresetDropDown = UIDropDowns.AddLabelledDropDown(MainPanel, Margin, currentY, "Sound Preset", MainPanel.width);
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

                foreach (var group in tabGroup.Value)
                {
                    foreach (var sound in group.Value)
                    {
                        this.CreateSoundSlider(sound);
                        currentY += 30f;
                    }
                    // Increment currentY by a fixed amount after all sliders in the current tab group have been added
                    currentY += 10f; // Add extra space between tab groups if needed
                }
            }
        }



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
                FileDebugge
                UnityEngine.Debug.LogError($"No volume configuration found for {sound.CategoryId}.{sound.Id}, using default value: {sound.DefaultVolume}");
                volume = sound.DefaultVolume;
            }

            // Add UI components to the main panel
            UISlider uiSlider = mainPanel.AddUIComponent<UISlider>();
            uiSlider.name = "SoundSlider";
            uiSlider.size = new Vector2(mainPanel.width, 16);
            uiSlider.relativePosition = new Vector3(Margin, currentY);
            uiSlider.minValue = 0;
            uiSlider.maxValue = sound.MaxVolume;
            uiSlider.stepSize = 0.01f;
            uiSlider.value = volume;
            uiSlider.eventValueChanged += (c, v) => this.SoundVolumeChanged(sound, v);
            currentY += 30f;

            // Increment currentY after adding each element
            currentY += 16f; // Height of the slider

            // Slider track.
            UISlicedSprite sliderSprite = uiSlider.AddUIComponent<UISlicedSprite>();
            sliderSprite.atlas = UITextures.InGameAtlas;
            sliderSprite.spriteName = "BudgetSlider";
            sliderSprite.size = new Vector2(uiSlider.width, 9f);
            sliderSprite.relativePosition = new Vector2(0f, 4f);

            currentY += 9f; // Height of the slider track

            // Slider thumb.
            UISlicedSprite sliderThumb = uiSlider.AddUIComponent<UISlicedSprite>();
            sliderThumb.atlas = UITextures.InGameAtlas;
            sliderThumb.spriteName = "SliderBudget";
            uiSlider.thumbObject = sliderThumb;

            currentY += 16f; // Height of the slider thumb

            UIPanel uiPanel = uiSlider.parent as UIPanel;
            UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
            uiLabel.text = sound.Name;
            uiLabel.autoSize = true;
            uiLabel.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;
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
                uiDropDown.items = new[] { "Default" }.Union(customAudioFiles.Select(kvp => kvp.Value.Name)).ToArray();
                uiDropDown.height = 28;
                uiDropDown.textFieldPadding.top = 4;
                if (configuration.ContainsKey(sound.Id) && !string.IsNullOrEmpty(configuration[sound.Id].SoundPack))
                    uiDropDown.selectedValue = configuration[sound.Id].SoundPack;
                else
                    uiDropDown.selectedIndex = 0;

                uiDropDown.eventSelectedIndexChanged += (c, i) => this.SoundPackChanged(sound, i > 0 ? ((UIDropDown)c).items[i] : null, uiSlider);
                this.soundSelections[string.Format("{0}.{1}", sound.CategoryId, sound.Id)] = uiDropDown;

                currentY += uiDropDown.height; // Increment currentY after adding the dropdown
            }

            // Configure UI components
            uiPanel.autoLayout = false;
            uiSlider.anchor = UIAnchorStyle.CenterVertical;
            uiSlider.width = 207;
            uiSlider.relativePosition = new Vector3(uiLabel.relativePosition.x + uiLabel.width + 20, 0);
            if (customAudioFiles.Count > 0)
            {
                uiDropDown.anchor = UIAnchorStyle.CenterVertical;
                uiDropDown.width = 180;
                uiDropDown.relativePosition = new Vector3(uiSlider.relativePosition.x + uiSlider.width + 20, 0);
                uiPanel.size = new Vector2(uiDropDown.relativePosition.x + uiDropDown.width, currentY);
            }
            else
            {
                uiPanel.size = new Vector2(uiSlider.relativePosition.x + uiSlider.width, currentY);
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


