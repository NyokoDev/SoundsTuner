using AlgernonCommons.UI;
using AmbientSoundsTuner2;
using AmbientSoundsTuner2.CommonShared.Defs;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Migration;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds;
using AmbientSoundsTuner2.UI;
using ColossalFramework.UI;
using Epic.OnlineServices;
using ICities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace POAIDBOX
{
    public sealed class LegacyTab : UIHelperBase
    {
        float currentY = Margin;
        private const float Margin = 5f;
        OptionsPanel OptionsPanel;
        public string[] soundPacks = new[] { "Default", "Custom" }.Union(SoundPacksManager.instance.SoundPacks.Values.OrderBy(p => p.Name).Select(p => p.Name)).ToArray();
        public bool isChangingSoundPackPreset = false;
        public Dictionary<string, UIDropDown> soundSelections = new Dictionary<string, UIDropDown>();

        public UIHelper modSettingsGroup;
        private UIDropDown soundPackPresetDropDown;
        private UICheckBox debugLoggingCheckBox;
        private UIHelper soundSettingsGroup;
        private UILabel versionInfoLabel;



        /// <summary>
        /// Creates a tabstrip to refresh the sound packs.
        /// </summary>
        /// <param name="tabStrip"></param>
        /// <param name="tabIndex"></param>
        internal LegacyTab(UITabstrip tabStrip, int tabIndex)
        {
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, "Legacy", tabIndex, out UIButton _, autoLayout: true);
            currentY += 31f;
            // Assuming UIHelperBase is a class, not static




            this.modSettingsGroup = (UIHelper)AddGroup("Mod settings");
            this.soundPacks = new[] { "Default", "Custom" }.Union(SoundPacksManager.instance.SoundPacks.Values.OrderBy(p => p.Name).Select(p => p.Name)).ToArray();
            this.soundPackPresetDropDown = (UIDropDown)this.modSettingsGroup.AddDropdown("Sound pack preset", this.soundPacks, 0, this.SoundPackPresetDropDownSelectionChanged);
            this.soundPackPresetDropDown.selectedValue = Mod.Settings.SoundPackPreset;

        }



        void PopulateTabContainer()
        {
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


            {

            }
        }




        protected void CreateSoundSlider(UIHelperBase helper, ISound sound)
        {
            // Initialize variables
            var configuration = Mod.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);
            var customAudioFiles = SoundPacksManager.instance.AudioFiles.Where(kvp => kvp.Key.StartsWith(string.Format("{0}.{1}", sound.CategoryId, sound.Id))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            float volume = 0;
            if (configuration.ContainsKey(sound.Id))
                volume = configuration[sound.Id].Volume;
            else
            {

                volume = sound.DefaultVolume;
            }

            // Add UI components
            UISlider uiSlider = null;
            UIPanel uiPanel = null;
            UILabel uiLabel = null;
            UIDropDown uiDropDown = null;
            var maxVolume = sound.MaxVolume;

            if (customAudioFiles.Count > 0 && configuration.ContainsKey(sound.Id) && !string.IsNullOrEmpty(configuration[sound.Id].SoundPack))
            {
                // Custom sound, determine custom max volume
                var audioFile = SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, configuration[sound.Id].SoundPack);
                maxVolume = Mathf.Max(audioFile.AudioInfo.MaxVolume, audioFile.AudioInfo.Volume);
            }

            uiSlider = (UISlider)helper.AddSlider(sound.Name, 0, maxVolume, 0.01f, volume, v => this.SoundVolumeChanged(sound, v));
            uiPanel = (UIPanel)uiSlider.parent;
            uiLabel = uiPanel.Find<UILabel>("Label");

            if (customAudioFiles.Count > 0)
            {
                uiDropDown = uiPanel.AttachUIComponent(GameObject.Instantiate((UITemplateManager.Peek(UITemplateDefs.ID_OPTIONS_DROPDOWN_TEMPLATE) as UIPanel).Find<UIDropDown>("Dropdown").gameObject)) as UIDropDown;
                uiDropDown.items = new[] { "Default" }.Union(customAudioFiles.Select(kvp => kvp.Value.Name)).ToArray();
                uiDropDown.height = 28;
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
            uiLabel.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;
            uiLabel.width = 250;
            uiSlider.anchor = UIAnchorStyle.CenterVertical;
            uiSlider.builtinKeyNavigation = false;
            uiSlider.width = 207;
            uiSlider.relativePosition = new Vector3(uiLabel.relativePosition.x + uiLabel.width + 20, 0);
            if (customAudioFiles.Count > 0)
            {
                uiDropDown.anchor = UIAnchorStyle.CenterVertical;
                uiDropDown.width = 180;
                uiDropDown.relativePosition = new Vector3(uiSlider.relativePosition.x + uiSlider.width + 20, 0);
                uiPanel.size = new Vector2(uiDropDown.relativePosition.x + uiDropDown.width, 32);
            }
            else
            {
                uiPanel.size = new Vector2(uiSlider.relativePosition.x + uiSlider.width, 32);
            }
        }

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

            Mod.Settings.SoundPackPreset = this.soundPackPresetDropDown.selectedValue;
            this.isChangingSoundPackPreset = false;
        }

        public UIHelperBase AddGroup(string text)
        {
            return ((UIHelperBase)modSettingsGroup).AddGroup(text);
        }

        public object AddButton(string text, OnButtonClicked eventCallback)
        {
            return ((UIHelperBase)modSettingsGroup).AddButton(text, eventCallback);
        }

        public object AddSpace(int height)
        {
            return ((UIHelperBase)modSettingsGroup).AddSpace(height);
        }

        public object AddCheckbox(string text, bool defaultValue, OnCheckChanged eventCallback)
        {
            return ((UIHelperBase)modSettingsGroup).AddCheckbox(text, defaultValue, eventCallback);
        }

        public object AddSlider(string text, float min, float max, float step, float defaultValue, OnValueChanged eventCallback)
        {
            return ((UIHelperBase)modSettingsGroup).AddSlider(text, min, max, step, defaultValue, eventCallback);
        }

        public object AddDropdown(string text, string[] options, int defaultSelection, OnDropdownSelectionChanged eventCallback)
        {
            return ((UIHelperBase)modSettingsGroup).AddDropdown(text, options, defaultSelection, eventCallback);
        }

        public object AddTextfield(string text, string defaultContent, OnTextChanged eventChangedCallback, OnTextSubmitted eventSubmittedCallback = null)
        {
            return ((UIHelperBase)modSettingsGroup).AddTextfield(text, defaultContent, eventChangedCallback, eventSubmittedCallback);
        }
    }
}

    

      