using System.Collections.Generic;
using ColossalFramework.Plugins;

namespace AmbientSoundsTuner2.CommonShared.Proxies.Plugins
{
    /// <summary>
    /// The PluginManager interactor interface. Must be used when creating a custom proxy.
    /// See <see cref="PluginManagerProxy" /> for the default interaction with Cities Skylines.
    /// </summary>
    public interface IPluginManagerInteractor
    {
        /// <summary>
        /// Gets the number of enabled mods.
        /// </summary>
        /// <value>
        /// The number of enabled mods.
        /// </value>
        int EnabledModCount { get; }

        /// <summary>
        /// Gets the number of mods.
        /// </summary>
        /// <value>
        /// The number of mods.
        /// </value>
        int ModCount { get; }

        /// <summary>
        /// Occurs when plugins have changed.
        /// </summary>
        event PluginManager.PluginsChangedHandler OnPluginsChanged;

        /// <summary>
        /// Occurs when a plugin state has changed.
        /// </summary>
        event PluginManager.PluginsChangedHandler OnPluginsStateChanged;

        /// <summary>
        /// Gets the implementations.
        /// </summary>
        /// <typeparam name="T">The type of the implementations.</typeparam>
        /// <returns>The list of implementations.</returns>
        List<T> GetImplementations<T>() where T : class;

        /// <summary>
        /// Gets the plugins information.
        /// </summary>
        /// <returns>The plugins information.</returns>
        IEnumerable<IPluginInfoInteractor> GetPluginsInfo();

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        void LoadPlugins();
    }
}
