using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using AmbientSoundsTuner2.CommonShared.Extensions;
using AmbientSoundsTuner2.CommonShared.Proxies.Plugins;
using ColossalFramework.Plugins;
using ICities;

namespace CommonShared.Utils
{
    /// <summary>
    /// Contains various utilities regarding plugins.
    /// </summary>
    public static class PluginUtils
    {
        private static IPluginManagerInteractor pluginManagerInteractor = PluginManagerProxy.instance;
        /// <summary>
        /// Gets or sets the plugin manager interactor that will be used.
        /// By default this is <see cref="PluginManagerProxy"/>.
        /// </summary>
        public static IPluginManagerInteractor PluginManagerInteractor
        {
            get { return pluginManagerInteractor; }
            set { pluginManagerInteractor = value; }
        }

        /// <summary>
        /// Gets the plugin info of the calling mod.
        /// </summary>
        /// <param name="modInstance">The mod instance.</param>
        /// <returns>The plugin info.</returns>
        public static IPluginInfoInteractor GetPluginInfo(IUserMod modInstance)
        {
            return PluginManagerInteractor.GetPluginsInfo().FirstOrDefault(i => i.UserModInstance == modInstance);
        }

        /// <summary>
        /// Gets the plugin infos of a given hash set of workshop IDs.
        /// </summary>
        /// <param name="workshopIds">The workshop IDs.</param>
        /// <returns>A dictionary with the plugins that have been found.</returns>
        public static IDictionary<ulong, IPluginInfoInteractor> GetPluginInfosOf(IEnumerable<ulong> workshopIds)
        {
            return PluginManagerInteractor.GetPluginsInfo()
                .Where(i => workshopIds.Contains(i.PublishedFileID.AsUInt64))
                .ToDictionary(i => i.PublishedFileID.AsUInt64, i => i);
        }


        private static IDictionary<string, bool> pluginEnabledList = new Dictionary<string, bool>();
        private static IDictionary<string, HashSet<Action<bool>>> pluginStateChangeCallbacks = new Dictionary<string, HashSet<Action<bool>>>();

        /// <summary>
        /// Subscribes to the event when the plugin state changes.
        /// </summary>
        /// <param name="modInstance">The mod instance.</param>
        /// <param name="callback">The callback that will be used when the state changes, with a boolean parameter that is true when the plugin is enabled, and false otherwise.</param>
        public static void SubscribePluginStateChange(IUserMod modInstance, Action<bool> callback)
        {
            SubscribePluginStateChange(GetPluginInfo(modInstance), callback);
        }

        /// <summary>
        /// Subscribes to the event when the plugin state changes.
        /// </summary>
        /// <param name="pluginInfo">The plugin info.</param>
        /// <param name="callback">The callback that will be used when the state changes, with a boolean parameter that is true when the plugin is enabled, and false otherwise.</param>
        public static void SubscribePluginStateChange(IPluginInfoInteractor pluginInfo, Action<bool> callback)
        {
            string pluginName = pluginInfo.Name;
            if (pluginStateChangeCallbacks.Count == 0)
            {
                PluginManagerInteractor.OnPluginsStateChanged += PluginManager_OnPluginsStateChanged;
                foreach (var pluginInfo_ in PluginManagerInteractor.GetPluginsInfo())
                {
                    pluginEnabledList[pluginInfo_.Name] = pluginInfo_.IsEnabled;
                }
            }

            if (!pluginStateChangeCallbacks.ContainsKey(pluginName))
            {
                pluginStateChangeCallbacks.Add(pluginName, new HashSet<Action<bool>>());
            }

            pluginStateChangeCallbacks[pluginName].Add(callback);
        }

        private static void PluginManager_OnPluginsStateChanged()
        {
            foreach (var pluginInfo in PluginManagerInteractor.GetPluginsInfo())
            {
                bool isEnabled;
                if (pluginEnabledList.TryGetValueOrDefault(pluginInfo.Name, false, out isEnabled) && pluginInfo.IsEnabled != isEnabled)
                {
                    pluginEnabledList[pluginInfo.Name] = pluginInfo.IsEnabled;
                    if (pluginStateChangeCallbacks.ContainsKey(pluginInfo.Name))
                    {
                        var callbacks = new HashSet<Action<bool>>(pluginStateChangeCallbacks[pluginInfo.Name]);
                        foreach (var callback in callbacks)
                        {
                            callback(pluginInfo.IsEnabled);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes from the event when the plugin state changes.
        /// </summary>
        /// <param name="modInstance">The mod instance.</param>
        /// <param name="callback">The callback that will be used when the state changes, with a boolean parameter that is true when the plugin is enabled, and false otherwise.</param>
        public static void UnsubscribePluginStateChange(IUserMod modInstance, Action<bool> callback)
        {
            UnsubscribePluginStateChange(GetPluginInfo(modInstance), callback);
        }

        /// <summary>
        /// Unsubscribes from the event when the plugin state changes.
        /// </summary>
        /// <param name="pluginInfo">The plugin info.</param>
        /// <param name="callback">The callback that will be used when the state changes, with a boolean parameter that is true when the plugin is enabled, and false otherwise.</param>
        public static void UnsubscribePluginStateChange(IPluginInfoInteractor pluginInfo, Action<bool> callback)
        {
            string pluginName = pluginInfo.Name;
            if (pluginStateChangeCallbacks.ContainsKey(pluginName))
            {
                pluginStateChangeCallbacks[pluginName].Remove(callback);
                if (pluginStateChangeCallbacks[pluginName].Count == 0)
                {
                    pluginStateChangeCallbacks.Remove(pluginName);
                }
            }

            if (pluginStateChangeCallbacks.Count == 0)
            {
                PluginManagerInteractor.OnPluginsStateChanged -= PluginManager_OnPluginsStateChanged;
            }
        }

        /// <summary>
        /// Cleans up this instance.
        /// </summary>
        public static void CleanUp()
        {
            pluginEnabledList.Clear();
            pluginStateChangeCallbacks.Clear();
        }
    }
}
