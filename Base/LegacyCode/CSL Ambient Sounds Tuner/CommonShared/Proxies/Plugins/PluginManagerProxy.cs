using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;

namespace AmbientSoundsTuner2.CommonShared.Proxies.Plugins
{
    /// <summary>
    /// The default proxy to interface with the <see cref="PluginManager" /> of Cities Skylines.
    /// </summary>
    public class PluginManagerProxy : SingletonLite<PluginManagerProxy>, IPluginManagerInteractor
    {
        /// <summary>
        /// Gets the number of enabled mods.
        /// </summary>
        /// <value>
        /// The number of enabled mods.
        /// </value>
        public int EnabledModCount
        {
            get { return PluginManager.instance.enabledModCount; }
        }

        /// <summary>
        /// Gets the number of mods.
        /// </summary>
        /// <value>
        /// The number of mods.
        /// </value>
        public int ModCount
        {
            get { return PluginManager.instance.modCount; }
        }

        /// <summary>
        /// Occurs when plugins have changed.
        /// </summary>
        public event PluginManager.PluginsChangedHandler OnPluginsChanged
        {
            add
            {
                PluginManager.instance.eventPluginsChanged += value;
            }
            remove
            {
                PluginManager.instance.eventPluginsChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when a plugin state has changed.
        /// </summary>
        public event PluginManager.PluginsChangedHandler OnPluginsStateChanged
        {
            add
            {
                PluginManager.instance.eventPluginsStateChanged += value;
            }
            remove
            {
                PluginManager.instance.eventPluginsStateChanged -= value;
            }
        }

        /// <summary>
        /// Gets the implementations.
        /// </summary>
        /// <typeparam name="T">The type of the implementations.</typeparam>
        /// <returns>
        /// The list of implementations.
        /// </returns>
        public List<T> GetImplementations<T>() where T : class
        {
            return PluginManager.instance.GetImplementations<T>();
        }

        /// <summary>
        /// Gets the plugins information.
        /// </summary>
        /// <returns>
        /// The plugins information.
        /// </returns>
        public IEnumerable<IPluginInfoInteractor> GetPluginsInfo()
        {
            return PluginManager.instance.GetPluginsInfo().Select(i => (IPluginInfoInteractor)new PluginInfoProxy(i));
        }

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        public void LoadPlugins()
        {
            PluginManager.instance.LoadPlugins();
        }
    }
}
