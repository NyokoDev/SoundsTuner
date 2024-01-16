using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.PlatformServices;

namespace AmbientSoundsTuner2.CommonShared.Proxies.Plugins
{
    /// <summary>
    /// The PluginInfo interactor interface. Must be used when creating a custom proxy.
    /// See <see cref="PluginInfoProxy" /> for the default interaction with Cities Skylines.
    /// </summary>
    public interface IPluginInfoInteractor
    {
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>
        /// The list of assemblies.
        /// </value>
        IList<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets the assemblies string.
        /// </summary>
        /// <value>
        /// The assemblies string.
        /// </value>
        string AssembliesString { get; }

        /// <summary>
        /// Gets the number of assemblies.
        /// </summary>
        /// <value>
        /// The number of assemblies.
        /// </value>
        int AssemblyCount { get; }

        /// <summary>
        /// Gets a value indicating whether this plugin is built in.
        /// </summary>
        /// <value>
        /// <c>true</c> if this plugin is built in; otherwise, <c>false</c>.
        /// </value>
        bool IsBuiltIn { get; }

        /// <summary>
        /// Gets a value indicating whether this plugin is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this plugin is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the mod path.
        /// </summary>
        /// <value>
        /// The mod path.
        /// </value>
        string ModPath { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the published file Steam id.
        /// </summary>
        /// <value>
        /// The published file Steam id.
        /// </value>
        PublishedFileId PublishedFileID { get; }

        /// <summary>
        /// Gets the mod instance.
        /// </summary>
        /// <value>
        /// The mod instance.
        /// </value>
        object UserModInstance { get; }

        /// <summary>
        /// Adds an assembly.
        /// </summary>
        /// <param name="asm">The assembly.</param>
        void AddAssembly(Assembly asm);

        /// <summary>
        /// Gets the instances.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <returns>The instances.</returns>
        T[] GetInstances<T>() where T : class;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this plugin.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this plugin.
        /// </returns>
        string ToString();

        /// <summary>
        /// Unloads this plugin.
        /// </summary>
        void Unload();
    }
}
