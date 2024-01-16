using ColossalFramework;
using ColossalFramework.IO;
using CommonShared.Proxies.IO;

namespace AmbientSoundsTuner2.CommonShared.Proxies.IO
{
    /// <summary>
    /// The default proxy to interface with the <see cref="DataLocation" /> of Cities Skylines.
    /// </summary>
    public class DataLocationProxy : SingletonLite<DataLocationProxy>, IDataLocationInteractor
    {
        /// <summary>
        /// The company name.
        /// </summary>
        public string CompanyName
        {
            get { return DataLocation.companyName; }
        }

        /// <summary>
        /// The product version.
        /// </summary>
        public uint ProductVersion
        {
            get { return DataLocation.productVersion; }
        }

        /// <summary>
        /// The product version string.
        /// </summary>
        public string ProductVersionString
        {
            get { return DataLocation.productVersionString; }
        }

        /// <summary>
        /// The product name.
        /// </summary>
        public string ProductName
        {
            get { return DataLocation.productName; }
        }

        /// <summary>
        /// The application base.
        /// </summary>
        public string ApplicationBase
        {
            get { return DataLocation.applicationBase; }
        }

        /// <summary>
        /// The game content path.
        /// </summary>
        public string GameContentPath
        {
            get { return DataLocation.gameContentPath; }
        }

        /// <summary>
        /// The add-ons path.
        /// </summary>
        public string AddonsPath
        {
            get { return DataLocation.addonsPath; }
        }

        /// <summary>
        /// The mods path.
        /// </summary>
        public string ModsPath
        {
            get { return DataLocation.modsPath; }
        }

        /// <summary>
        /// The assets path.
        /// </summary>
        public string AssetsPath
        {
            get { return DataLocation.assetsPath; }
        }

        /// <summary>
        /// The current directory.
        /// </summary>
        public string CurrentDirectory
        {
            get { return DataLocation.currentDirectory; }
        }

        /// <summary>
        /// The assembly directory.
        /// </summary>
        public string AssemblyDirectory
        {
            get { return DataLocation.assemblyDirectory; }
        }

        /// <summary>
        /// The executable directory.
        /// </summary>
        public string ExecutableDirectory
        {
            get { return DataLocation.executableDirectory; }
        }

        /// <summary>
        /// The temp folder.
        /// </summary>
        public string TempFolder
        {
            get { return DataLocation.tempFolder; }
        }

        /// <summary>
        /// The local application data directory.
        /// </summary>
        public string LocalApplicationData
        {
            get { return DataLocation.localApplicationData; }
        }

        /// <summary>
        /// The save location.
        /// </summary>
        public string SaveLocation
        {
            get { return DataLocation.saveLocation; }
        }

        /// <summary>
        /// The map location.
        /// </summary>
        public string MapLocation
        {
            get { return DataLocation.mapLocation; }
        }
    }
}
