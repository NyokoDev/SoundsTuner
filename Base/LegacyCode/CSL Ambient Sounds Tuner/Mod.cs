using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using AmbientSoundsTuner2.CommonShared;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Compatibility;
using AmbientSoundsTuner2.Detour;
using AmbientSoundsTuner2.Migration;
using AmbientSoundsTuner2.SoundPack;
using AmbientSoundsTuner2.SoundPack.Migration;
using AmbientSoundsTuner2.Sounds;
using AmbientSoundsTuner2.Sounds.Exceptions;
using AmbientSoundsTuner2.UI;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CommonShared.Utils;
using ICities;
using POAIDBOX;
using UnityEngine;

namespace AmbientSoundsTuner2
{
    public class Mod
    {



        public static Configuration Settings { get; set; }

        public static string SettingsFilename { get; set; }

        public ModOptionsPanel OptionsPanel { get; set; }


        #region UserModBase members


        public void Initialize()
        {
            // Before patching, export the current game sounds as example files

            // Example for XML file
            var exampleFile = SoundManager.instance.GetCurrentSoundSettingsAsSoundPack();
            string xmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Example." + SoundPacksManager.SOUNDPACKS_FILENAME_XML);
            exampleFile.SaveConfig(xmlFilePath);

            // Example for YAML file
            string yamlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Example." + SoundPacksManager.SOUNDPACKS_FILENAME_YAML);
            exampleFile.SaveConfig(yamlFilePath);

            // Patch the sounds
            PatchSounds();
        }

        #endregion


        #region IUserModSettingsUI

   

        #endregion


        public string BuildVersion
        {
            get { return "dev version"; }
        }

        private static SoundsTunerMod instance;

        // Define the Instance property
        public static SoundsTunerMod Instance
        {
            get { return instance; }
            internal set { instance = value; }
        }



    #region Loading / Unloading

    public void Load()
        {
            // We have to properly migrate the outdated XML configuration file
            try
            {
                string oldXmlSettingsFilename = Path.Combine(Path.GetDirectoryName(SettingsFilename), Path.GetFileNameWithoutExtension(SettingsFilename)) + ".xml";
                if (File.Exists(oldXmlSettingsFilename) && !File.Exists(SettingsFilename))
                {
                    Settings = Configuration.LoadConfig(oldXmlSettingsFilename, new ConfigurationMigrator());
                    Settings.SaveConfig(SettingsFilename);
                    File.Delete(oldXmlSettingsFilename);
                }
                else
                {
                    Settings = Configuration.LoadConfig(SettingsFilename, new ConfigurationMigrator());
                }
            }
            catch (Exception ex)
            {
               
                Settings = new Configuration();
            }

           
            if (Settings.ExtraDebugLogging)
            {
                
            }

            // Initialize sounds
            SoundManager.instance.InitializeSounds();

            // Load sound packs
            SoundPacksManager.instance.InitSoundPacks();

            //verify config
            ValidateSettings();


            // Detour UI click sounds
            CustomPlayClickSound.Detour();
        }

        /// <summary>
        /// Redirects saving to Local App Data.
        /// </summary>
        public void OnModInitializing()
        {
            try
            {
                SettingsFilename = Path.Combine(FileUtils.GetStorageFolder(Instance), "AmbientSoundsTuner.yml");
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"An error occurred while combining file path: {ex.Message}");

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                // Combine the desktop path with the file name
                SettingsFilename = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "AmbientSoundsTuner.yml");
                FileDebugger.Debug("[SOUNDSTUNER] Redirected path to Local App Data.");

            }

            this.Load();
            this.PatchUISounds();
        }

        /// <summary>
        /// Validates settings. Creates settings file if it doesn't exist.
        /// </summary>
        /// <returns></returns>
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
      
                Mod     .Settings.SaveConfig(Mod.SettingsFilename);
                return false;
            }
            return nonExistingPacks.Count == 0;
        }

        /// <summary>
        /// Validates the sounds.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="nonExistingPacks"></param>
        /// <param name="selector"></param>
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

        public void Unload()
        {
            try
            {
                Settings.SaveConfig(SettingsFilename);
            }
            catch (Exception ex)
            {
            
            }
            CustomPlayClickSound.UnDetour();

            // Actually, to be consistent and nice, we should also revert the other sound patching here.
            // But since that sounds are only patched in-game, and closing that game conveniently resets the other sounds, it's not really needed.
            // If it's needed at some point in the future, we can add that logic here.
        }

        #endregion


        public void PatchSounds()
        {
            // Patch various sounds for compatibility first!
            switch (SoundDuplicator.PatchPoliceSiren())
            {
                case SoundDuplicator.PatchResult.Success:
                    
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
               
                    break;
                case SoundDuplicator.PatchResult.NotFound:
                 
                    break;
            }
            switch (SoundDuplicator.PatchScooterSound())
            {
                case SoundDuplicator.PatchResult.Success:
                 
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
                
                    break;
                case SoundDuplicator.PatchResult.NotFound:
                 
                    break;
            }
            switch (SoundDuplicator.PatchOilPowerPlant())
            {
                case SoundDuplicator.PatchResult.Success:
                 
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
               
                    break;
                case SoundDuplicator.PatchResult.NotFound:
             
                    break;
            }
            switch (SoundDuplicator.PatchWaterTreatmentPlant())
            {
                case SoundDuplicator.PatchResult.Success:
                   
                    break;
                case SoundDuplicator.PatchResult.AlreadyPatched:
                 
                    break;
                case SoundDuplicator.PatchResult.NotFound:
              
                    break;
            }

            // Try patching the sounds
            try
            {
                PatchGameSounds(true);
            }
            catch (Exception ex)
            {
           
            }
        }

        public void PatchUISounds()
        {
            // Try patching the sounds
            try
            {
                PatchGameSounds(false);
            }
            catch (Exception ex)
            {
               
            }
        }

        public void PatchGameSounds(bool ingame)
        {
            var backedUpSounds = new List<string>();
            var backedUpVolumes = new List<string>();
            var patchedSounds = new List<string>();
            var patchedVolumes = new List<string>();

            foreach (ISound sound in SoundManager.instance.Sounds.Values)
            {
                var soundName = string.Format("{0}.{1}", sound.CategoryId, sound.Id);
                if (sound.IngameOnly != ingame)
                    continue;

                try
                {
                    IDictionary<string, ConfigurationV4.Sound> soundConfig = null;
                    switch (sound.CategoryId)
                    {
                        case "Ambient":
                            soundConfig = Settings.AmbientSounds;
                            break;
                        case "AmbientNight":
                            soundConfig = Settings.AmbientNightSounds;
                            break;
                        case "Animal":
                            soundConfig = Settings.AnimalSounds;
                            break;
                        case "Building":
                            soundConfig = Settings.BuildingSounds;
                            break;
                        case "Vehicle":
                            soundConfig = Settings.VehicleSounds;
                            break;
                        case "Misc":
                            soundConfig = Settings.MiscSounds;
                            break;
                    }

                    try
                    {
                        sound.BackUpSound();
                        backedUpSounds.Add(soundName);
                    }
                    catch (SoundBackupException ex)
                    {
                      
                    }

                    try
                    {
                        sound.BackUpVolume();
                        backedUpVolumes.Add(soundName);
                    }
                    catch (SoundBackupException ex)
                    {
                        
                    }

                    if (soundConfig.ContainsKey(sound.Id))
                    {
                        if (!string.IsNullOrEmpty(soundConfig[sound.Id].SoundPack) && soundConfig[sound.Id].SoundPack != "Default")
                        {
                            try
                            {
                                sound.PatchSound(SoundPacksManager.instance.GetAudioFileByName(sound.CategoryId, sound.Id, soundConfig[sound.Id].SoundPack));
                                patchedSounds.Add(soundName);
                            }
                            catch (SoundPatchException ex)
                            {
                               
                            }
                        }

                        try
                        {
                            sound.PatchVolume(soundConfig[sound.Id].Volume);
                            patchedVolumes.Add(soundName);
                        }
                        catch (SoundPatchException ex)
                        {
                           
                        }
                    }
                }
                catch (Exception ex)
                {
                  
                }
            }

        }
    }

    /** List effect script 
            foreach(EffectInfo e in UnityEngine.Resources.FindObjectsOfTypeAll<EffectInfo>())
            {
                if (e is SoundEffect)
                {
                    SoundEffect sound = (SoundEffect) e;
                    AudioInfo audio = sound.m_audioInfo;
                    UnityEngine.Debug.Log("Sound Effect: name=" + sound.name + " volume=" + audio.m_volume + " pitch=" + audio.m_pitch + " isLoop=" + audio.m_loop + " 3d=" + audio.m_is3D);
                }
                if (e is MultiEffect)
                {
                    MultiEffect multi = (MultiEffect) e;
                    if (multi.m_effects == null)
                    {
                        continue;
                    }
                    foreach (MultiEffect.SubEffect sub in multi.m_effects)
                    {
                        EffectInfo e1 = sub.m_effect;
                        if (e1 is SoundEffect)
                        {
                            SoundEffect sound = (SoundEffect)e1;
                            AudioInfo audio = sound.m_audioInfo;
                            UnityEngine.Debug.Log("Sound Effect: parent=" + multi.name + " name=" + sound.name + " volume=" + audio.m_volume + " pitch=" + audio.m_pitch + " isLoop=" + audio.m_loop + " 3d=" + audio.m_is3D);
                        }
                    }
                }
            }
     */
}
