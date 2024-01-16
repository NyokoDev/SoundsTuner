using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbientSoundsTuner2.CommonShared.Proxies.IO;

namespace CommonShared.Proxies.IO
{
    /// <summary>
    /// The DataLocation interactor interface. Must be used when creating a custom proxy.
    /// See <see cref="DataLocationProxy" /> for the default interaction with Cities Skylines.
    /// </summary>
    public interface IDataLocationInteractor
    {
        /// <summary>
        /// The company name.
        /// </summary>
        string CompanyName { get; }

        /// <summary>
        /// The product version.
        /// </summary>
        uint ProductVersion { get; }

        /// <summary>
        /// The product version string.
        /// </summary>
        string ProductVersionString { get; }

        /// <summary>
        /// The product name.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// The application base.
        /// </summary>
        string ApplicationBase { get; }

        /// <summary>
        /// The game content path.
        /// </summary>
        string GameContentPath { get; }

        /// <summary>
        /// The add-ons path.
        /// </summary>
        string AddonsPath { get; }

        /// <summary>
        /// The mods path.
        /// </summary>
        string ModsPath { get; }

        /// <summary>
        /// The assets path.
        /// </summary>
        string AssetsPath { get; }

        /// <summary>
        /// The current directory.
        /// </summary>
        string CurrentDirectory { get; }

        /// <summary>
        /// The assembly directory.
        /// </summary>
        string AssemblyDirectory { get; }

        /// <summary>
        /// The executable directory.
        /// </summary>
        string ExecutableDirectory { get; }

        /// <summary>
        /// The temp folder.
        /// </summary>
        string TempFolder { get; }

        /// <summary>
        /// The local application data directory.
        /// </summary>
        string LocalApplicationData { get; }

        /// <summary>
        /// The save location.
        /// </summary>
        string SaveLocation { get; }

        /// <summary>
        /// The map location.
        /// </summary>
        string MapLocation { get; }
    }
}
