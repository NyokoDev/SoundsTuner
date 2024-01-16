using System;
using System.IO;

namespace AmbientSoundsTuner2.CommonShared.Configuration
{
    /// <summary>
    /// An interface for implementing configuration migrators.
    /// Typically you would want to use <see cref="T:ConfigMigratorBase`1" />.
    /// </summary>
    /// <typeparam name="T">The configuration object type.</typeparam>
    public interface IConfigMigrator<T> where T : VersionedConfig
    {
        /// <summary>
        /// Migrate an XML configuration file.
        /// </summary>
        /// <param name="version">The current version of the configuration file.</param>
        /// <param name="stream">The stream of the configuration file.</param>
        /// <returns>An up-to-date configuration object.</returns>
        [Obsolete("Use MigrateFromXml or MigrateFromYaml.")]
        T Migrate(uint version, Stream stream);

        /// <summary>
        /// Migrate an XML configuration file.
        /// </summary>
        /// <param name="version">The current version of the configuration file.</param>
        /// <param name="stream">The stream of the configuration file.</param>
        /// <returns>An up-to-date configuration object.</returns>
        T MigrateFromXml(uint version, Stream stream);

        /// <summary>
        /// Migrate a YAML configuration file.
        /// </summary>
        /// <param name="version">The current version of the configuration file.</param>
        /// <param name="stream">The stream of the configuration file.</param>
        /// <returns>An up-to-date configuration object.</returns>
        T MigrateFromYaml(uint version, Stream stream);
    }
}
