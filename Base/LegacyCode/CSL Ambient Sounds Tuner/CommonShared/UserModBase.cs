using System.Collections.Generic;
using System.Linq;
using AmbientSoundsTuner2.CommonShared.Proxies.Plugins;
using ColossalFramework.PlatformServices;
using CommonShared.Utils;
using ICities;
using UnityEngine;

namespace AmbientSoundsTuner2.CommonShared
{
    /// <summary>
    /// Provides basic implementation for a mod.
    /// </summary>
    /// <typeparam name="T">The type of the inherited class of this abstract class.</typeparam>
    public abstract class UserModBase<T> : LoadingExtensionBase, IUserMod, IUserModStateChangeEvents where T : IUserMod
    {
        private bool isValid;

        /// <summary>
        /// Gets the workshop identifier. If set to 0, the mod validity check will be disabled.
        /// If you want to republish the mod, you have to change this value to the new workshop ID and recompile it.
        /// But please try to improve it first by submitting pull requests on GitHub instead.
        /// </summary>
        /// <value>
        /// The workshop identifier.
        /// </value>
        public virtual ulong WorkshopId { get { return 0; } }

        /// <summary>
        /// Gets the list of incompatible mods.
        /// Upon loading these mods will be checked if they are installed and enabled.
        /// If at least one is, a warning will be shown to the user.
        /// </summary>
        /// <value>
        /// The list of incompatible mods.
        /// </value>
        public virtual IEnumerable<ulong> IncompatibleMods { get { return new HashSet<ulong>(); } }


        /// <summary>
        /// Gets the instance of this mod.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static T Instance { get; private set; }

        /// <summary>
        /// Gets the plugin information.
        /// </summary>
        /// <value>
        /// The plugin information.
        /// </value>
        public IPluginInfoInteractor PluginInfo { get; private set; }

        /// <summary>
        /// Gets the logger for this mod.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public Logger Log { get; private set; }


        public bool CheckIncompatibility()
        {
            var list = PluginUtils.GetPluginInfosOf(this.IncompatibleMods);
            if (list.Count > 0)
            {
                string text = string.Join(", ",
                    list.Where(kvp => kvp.Value.IsEnabled)
                        .Select(kvp => string.Format("{0} ({1})", kvp.Value.GetInstances<IUserMod>()[0].Name, kvp.Value.PublishedFileID.ToString()))
                        .OrderBy(s => s)
                        .ToArray()
                    );

                if (!string.IsNullOrEmpty(text))
                    this.Log.Warning("You've got some known incompatible mods installed and enabled! It's possible that this mod doesn't work as expected.\nThe following incompatible mods are enabled: {0}.", text);
                return true;
            }
            return false;
        }

        public bool CheckValidity()
        {
            
            return true;
        }


        #region IUserMod members

        /// <summary>
        /// Gets the name of the mod.
        /// </summary>
        /// <value>
        /// The name of the mod.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the mod description.
        /// </summary>
        /// <value>
        /// The mod description.
        /// </value>
        public abstract string Description { get; }

        #endregion


        #region IUserModStateChangeEvents members

        /// <summary>
        /// Called when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            this.Log = new Logger(this.GetType().Assembly);
            this.PluginInfo = PluginUtils.GetPluginInfo(this);
            UserModBase<T>.Instance = (T)(object)this; // Make the compiler shut up about non-existing conversions

            this.isValid = this.CheckValidity();
            if (!this.isValid)
                return;

            this.CheckIncompatibility();
            this.OnModInitializing();
        }

        /// <summary>
        /// Called when the mod is disabled.
        /// </summary>
        public void OnDisabled()
        {
            this.OnModUninitializing();
        }

        #endregion


        #region LoadingExtensionBase members

        /// <summary>
        /// Called when the mod instance is created.
        /// </summary>
        /// <param name="loading">The loading instance.</param>
        public sealed override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (this.isValid)
                this.OnModCreated(loading);
        }

        /// <summary>
        /// Called when the mod instance is released.
        /// </summary>
        public sealed override void OnReleased()
        {
            base.OnReleased();
            if (this.isValid)
                this.OnModReleased();
        }

        /// <summary>
        /// Called when the game has loaded a level.
        /// </summary>
        /// <param name="mode">The game mode.</param>
        public sealed override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (this.isValid)
            {
                this.CheckIncompatibility();
                this.OnGameLoaded(mode);
            }
        }

        /// <summary>
        /// Called when the game has started unloading a level.
        /// </summary>
        public sealed override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (this.isValid)
                this.OnGameUnloading();
        }

        #endregion


        /// <summary>
        /// Called when this mod is being initialized.
        /// </summary>
        public virtual void OnModInitializing() { }

        /// <summary>
        /// Called when this mod is being uninitialized.
        /// </summary>
        public virtual void OnModUninitializing() { }

        /// <summary>
        /// Called when the mod instance is created.
        /// </summary>
        /// <param name="loading">The loading instance.</param>
        public virtual void OnModCreated(ILoading loading) { }

        /// <summary>
        /// Called when the mod instance is released.
        /// </summary>
        public virtual void OnModReleased() { }

        /// <summary>
        /// Called when the game has loaded a level.
        /// </summary>
        /// <param name="mode">The game mode.</param>
        public virtual void OnGameLoaded(LoadMode mode) { }

        /// <summary>
        /// Called when the game has started unloading a level.
        /// </summary>
        public virtual void OnGameUnloading() { }
    }
}
