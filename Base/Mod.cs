using AlgernonCommons.Patching;
using AlgernonCommons.Translation;
using AlgernonCommons;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbientSoundsTuner2;
using System.Reflection;
using System.IO;

namespace POAIDBOX
{
   
        public sealed class SoundsTunerMod : PatcherMod<OptionsPanel, PatcherBase>, IUserMod
        {
            
            public override string BaseName => "Sounds Tuner";



            /// <summary>
            /// Gets the mod's unique Harmony identfier.
            /// </summary>
            public override string HarmonyID => "com.nyoko.soundstuner.patch";

        /// <summary>
        /// Gets the mod's description for display in the content manager.
        /// </summary>
        public string Description => "Customize ambient sounds.";

        public string SettingsFilename;

        public override void OnEnabled()
        {
            base.OnEnabled();

        }


        public override void LoadSettings()
            {
                // Enable detailed logging.
                Logging.DetailLogging = true;


            }

        public override void SaveSettings()
        {
            
        }
    }
    }


