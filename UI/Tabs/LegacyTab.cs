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
using UnityEngine.UI;

namespace POAIDBOX
{
    public sealed class LegacyTab
    {
        public static float currentY = Margin;
        private const float Margin = 5f;
        OptionsPanel OptionsPanel;
        public string[] soundPacks = UpdatedTab.soundPacks;
        public bool isChangingSoundPackPreset = false;
        public Dictionary<string, UIDropDown> soundSelections = new Dictionary<string, UIDropDown>();

        public UIHelper modSettingsGroup;
        private UIDropDown soundPackPresetDropDown;
        private UICheckBox debugLoggingCheckBox;
        private UIHelper soundSettingsGroup;
        private UILabel versionInfoLabel;
        public static UISlider uiSlider;
        public static UIPanel MainPanel;



        /// <summary>
        /// Creates a tabstrip to refresh the sound packs.
        /// </summary>
        /// <param name="tabStrip"></param>
        /// <param name="tabIndex"></param>
        internal LegacyTab(UITabstrip tabStrip, int tabIndex)
        {
            MainPanel = UITabstrips.AddTextTab(tabStrip, "Legacy", tabIndex, out UIButton _, autoLayout: true);
            currentY += 31f;
            soundPackPresetDropDown = UIDropDowns.AddLabelledDropDown(MainPanel, Margin, currentY, "Sound Preset", MainPanel.width);
            if (soundPackPresetDropDown == null)
            {
                UnityEngine.Debug.Log("Failed to create dropdown.");
                return;
            }
            soundPackPresetDropDown.items = soundPacks;
            SetSliders(); // Call Sliders function
        }



        /// <summary>
        /// Dynamically creates sliders based on audio values.
        /// </summary>
        public void SetSliders()
        {
            /// Check for custom audio files with a valid sound pack
            foreach (var sound in SoundManager.instance.Sounds.Values)
            {
                var configuration = Mod.Settings.GetSoundsByCategoryId<string>(sound.CategoryId);

                var customAudioFiles = SoundPacksManager.instance.AudioFiles
                    .Where(kvp => kvp.Key.StartsWith($"{sound.CategoryId}.{sound.Id}"))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var maxVolume = sound.MaxVolume;

                if (customAudioFiles.Count > 0 && configuration.TryGetValue(sound.Id, out var soundConfig) && !string.IsNullOrEmpty(soundConfig.SoundPack))
                {
                    var audioFile = SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, configuration[sound.Id].SoundPack);
                    maxVolume = Mathf.Max(audioFile.AudioInfo.MaxVolume, audioFile.AudioInfo.Volume);
                }

                // Add the slider directly to the existing panel
                LegacyTab.uiSlider = UISliders.AddBudgetSlider(MainPanel, Margin, currentY, MainPanel.width, maxVolume, $"Volume - {sound.Id}");

                // Increment the Y position for the next UI element
                LegacyTab.currentY += 31f;
            }

        }
    }
}
    




       





      