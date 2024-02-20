using AlgernonCommons.Notifications;
using AlgernonCommons.Patching;
using AlgernonCommons.UI;
using AmbientSoundsTuner2;
using AmbientSoundsTuner2.CommonShared;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Detour;
using AmbientSoundsTuner2.Migration;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds;
using AmbientSoundsTuner2.UI;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace POAIDBOX
{
    public sealed class Loading : PatcherLoadingBase<OptionsPanel, PatcherBase>
    {
        /// <summary>
        /// Gets a list of permitted loading modes.
        /// </summary>
        /// 
        public ColorCorrectionManager colorCorrectionManager;

      

        protected override List<AppMode> PermittedModes => new List<AppMode> { AppMode.Game, AppMode.MapEditor, AppMode.AssetEditor, AppMode.ThemeEditor, AppMode.ScenarioEditor };

        /// <summary>
        /// Called by the game when exiting a level.
        /// </summary>
        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
        }
        public bool ValidateSettings()
        {


            var nonExistingPacks = new HashSet<string>();
            ValidateSounds(Mod.Settings.AmbientNightSounds, nonExistingPacks, p => p.AmbientsNight);
            ValidateSounds(Mod.Settings.AmbientSounds, nonExistingPacks, p => p.Ambients);
            ValidateSounds(Mod.Settings.AnimalSounds, nonExistingPacks, p => p.Animals);
            ValidateSounds(Mod.Settings.BuildingSounds, nonExistingPacks, p => p.Buildings);
            ValidateSounds(Mod.Settings.MiscSounds, nonExistingPacks, p => p.Miscs);
            ValidateSounds(Mod.Settings.VehicleSounds, nonExistingPacks, p => p.Vehicles);

            if (nonExistingPacks.Count > 0)
            {
                Mod.Settings.SoundPackPreset = "Custom";

                Mod.Settings.SaveConfig(Mod.SettingsFilename);
                return false;
            }
            return nonExistingPacks.Count == 0;
        }


        public static void ValidateSounds(SerializableDictionary<string, ConfigurationV4.Sound> configuration, HashSet<string> nonExistingPacks, Func<SoundPacksFileV1.SoundPack, SoundPacksFileV1.Audio[]> selector)
        {
            configuration.ForEach(kvp =>
            {
                var soundName = kvp.Value.SoundPack;
                if (soundName == null)
                {
                    return;
                }
                if (SoundPacksManager.instance.SoundPacks.
                SelectMany(p => selector.Invoke(p.Value)).
                Any(a => a?.Name == soundName))
                {
                    return;
                }

                kvp.Value.SoundPack = null;
                nonExistingPacks.Add(soundName);
            });
        
    }

     /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            base.LoadedActions(mode);


            // Before we patch, we export the current game sounds as an example file
            var exampleFile = SoundManager.instance.GetCurrentSoundSettingsAsSoundPack();

        

            exampleFile.SaveConfig(Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "Example." + SoundPacksManager.SOUNDPACKS_FILENAME_XML));
            exampleFile.SaveConfig(Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "Example." + SoundPacksManager.SOUNDPACKS_FILENAME_YAML));
            FileDebugger.Debug("[SOUNDSTUNER] Entered in-game. Patching");
            SoundsTunerMod.ModInitializer.PatchSounds();
            FileDebugger.Debug("[SOUNDSTUNER] Entered in-game. Patch completed.");


        }
    }
}
