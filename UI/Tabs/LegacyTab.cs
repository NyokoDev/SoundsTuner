using AlgernonCommons.Translation;
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
        UIScrollablePanel ScrollPanel;
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

            MainPanel = UITabstrips.AddTextTab(tabStrip, Translations., tabIndex, out UIButton _);


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

        

            ScrollPanel = MainPanel.AddUIComponent<UIScrollablePanel>();
            var scrollPanel = ScrollPanel;

            scrollPanel.relativePosition = new Vector2(0, currentY);
            scrollPanel.autoSize = false;
            scrollPanel.autoLayout = false;
            scrollPanel.width = MainPanel.width;
            scrollPanel.backgroundSprite = "GenericTab";
            scrollPanel.height = MainPanel.height + 100000f;
            scrollPanel.clipChildren = true;
            scrollPanel.builtinKeyNavigation = true;
            scrollPanel.scrollWheelDirection = UIOrientation.Vertical;
            UIScrollbars.AddScrollbar(MainPanel, scrollPanel); // Assuming UIScrollbars is a class or function for adding scrollbars.


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
            UISlider uiSlider = ScrollPanel.AddUIComponent<UISlider>();
            currentY += 6f;
            uiSlider.name = "SoundSlider";
           
            uiSlider.width = 200f;
            uiSlider.relativePosition = new Vector3(200f, currentY);
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
            sliderSprite.relativePosition = new Vector2(0f, 4f);

            // Slider thumb.
            UISlicedSprite sliderThumb = uiSlider.AddUIComponent<UISlicedSprite>();
            sliderThumb.atlas = UITextures.InGameAtlas;
            sliderThumb.spriteName = "SliderBudget";
            uiSlider.thumbObject = sliderThumb;



            UILabel uiLabel = ScrollPanel.AddUIComponent<UILabel>();
            currentY += 6f;
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
                uiDropDown = ScrollPanel.AddUIComponent<UIDropDown>();
                currentY += 6f;

                // Calculate the position relative to the slider
                float dropdownX = uiSlider.relativePosition.x + uiSlider.width + 20;
                float dropdownY = currentY; // Keep it aligned with currentY

                // Create dropdown button.
                UIButton button = uiDropDown.AddUIComponent<UIButton>();
                uiDropDown.triggerButton = button;
                button.size = uiDropDown.size;
                button.text = string.Empty;
                button.relativePosition = new Vector2(0f, 0f);
                button.textVerticalAlignment = UIVerticalAlignment.Middle;
                button.textHorizontalAlignment = UIHorizontalAlignment.Left;
                button.normalFgSprite = "IconDownArrow";
                button.hoveredFgSprite = "IconDownArrowHovered";
                button.pressedFgSprite = "IconDownArrowPressed";
                button.focusedFgSprite = "IconDownArrowFocused";
                button.disabledFgSprite = "IconDownArrowDisabled";
                button.spritePadding = new RectOffset(3, 3, 3, 3);
                button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
                button.horizontalAlignment = UIHorizontalAlignment.Right;
                button.verticalAlignment = UIVerticalAlignment.Middle;
                button.zOrder = 0;

                uiDropDown.items = new[] { "Default" }.Union(customAudioFiles.Select(kvp => kvp.Value.Name)).ToArray();
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
            ScrollPanel.autoLayout = false;
            uiSlider.width = 150f;
            if (customAudioFiles.Count > 0)
            {
                // Calculate the position relative to the slider
                float dropdownX = uiSlider.relativePosition.x + 50f + uiSlider.width + 20;
                float dropdownY = currentY; // Keep it aligned with currentY

                uiDropDown.width = 100f;
                currentY += 6f;
                uiDropDown.relativePosition = new Vector3(dropdownX, dropdownY);

            }
            else
            {
               
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


