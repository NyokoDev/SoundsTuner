using AlgernonCommons.Notifications;
using AlgernonCommons.Patching;
using AmbientSoundsTuner2.SoundPack;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

       
    

        private UnityEngine.GameObject _gameObject;
        /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            base.LoadedActions(mode);
            SoundPacksManager.instance.InitSoundPacks();
            // Create logic instance.



        }
    }
}
