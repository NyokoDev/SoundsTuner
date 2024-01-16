using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.PlatformServices;
using ColossalFramework.Plugins;
using CommonShared.Utils;

namespace AmbientSoundsTuner2.CommonShared.Proxies.Plugins
{
    /// <summary>
    /// The default proxy to interface with the <see cref="PluginManager.PluginInfo" /> of Cities Skylines.
    /// </summary>
    public class PluginInfoProxy : IPluginInfoInteractor
    {
        /// <summary>
        /// Gets the original plugin information.
        /// </summary>
        /// <value>
        /// The original plugin information.
        /// </value>
        public PluginManager.PluginInfo OriginalPluginInfo { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInfoProxy"/> class.
        /// </summary>
        /// <param name="originalPluginInfo">The original plugin information.</param>
        public PluginInfoProxy(PluginManager.PluginInfo originalPluginInfo)
        {
            this.OriginalPluginInfo = originalPluginInfo;
        }

        private IList<Assembly> assemblies;
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>
        /// The list of assemblies.
        /// </value>
        public IList<Assembly> Assemblies
        {
            get
            {
                if (this.assemblies == null)
                    this.assemblies = ReflectionUtils.GetPrivateField<List<Assembly>>(this.OriginalPluginInfo, "m_Assemblies");
                return this.assemblies;
            }
        }

        /// <summary>
        /// Gets the assemblies string.
        /// </summary>
        /// <value>
        /// The assemblies string.
        /// </value>
        public string AssembliesString
        {
            get { return this.OriginalPluginInfo.assembliesString; }
        }

        /// <summary>
        /// Gets the number of assemblies.
        /// </summary>
        /// <value>
        /// The number of assemblies.
        /// </value>
        public int AssemblyCount
        {
            get { return this.OriginalPluginInfo.assemblyCount; }
        }

        /// <summary>
        /// Gets a value indicating whether this plugin is built in.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this plugin is built in; otherwise, <c>false</c>.
        /// </value>
        public bool IsBuiltIn
        {
            get { return this.OriginalPluginInfo.isBuiltin; }
        }

        /// <summary>
        /// Gets a value indicating whether this plugin is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this plugin is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this.OriginalPluginInfo.isEnabled; }
        }

        /// <summary>
        /// Gets the mod path.
        /// </summary>
        /// <value>
        /// The mod path.
        /// </value>
        public string ModPath
        {
            get { return this.OriginalPluginInfo.modPath; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return this.OriginalPluginInfo.name; }
        }

        /// <summary>
        /// Gets the published file Steam id.
        /// </summary>
        /// <value>
        /// The published file Steam id.
        /// </value>
        public PublishedFileId PublishedFileID
        {
            get { return this.OriginalPluginInfo.publishedFileID; }
        }

        /// <summary>
        /// Gets the mod instance.
        /// </summary>
        /// <value>
        /// The mod instance.
        /// </value>
        public object UserModInstance
        {
            get { return this.OriginalPluginInfo.userModInstance; }
        }

        /// <summary>
        /// Adds an assembly.
        /// </summary>
        /// <param name="asm">The assembly.</param>
        public void AddAssembly(Assembly asm)
        {
            this.OriginalPluginInfo.AddAssembly(asm);
        }

        /// <summary>
        /// Gets the instances.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <returns>
        /// The instances.
        /// </returns>
        public T[] GetInstances<T>() where T : class
        {
            return this.OriginalPluginInfo.GetInstances<T>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this plugin.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this plugin.
        /// </returns>
        public override string ToString()
        {
            return this.OriginalPluginInfo.ToString();
        }

        /// <summary>
        /// Unloads this plugin.
        /// </summary>
        public void Unload()
        {
            this.OriginalPluginInfo.Unload();
        }
    }
}
