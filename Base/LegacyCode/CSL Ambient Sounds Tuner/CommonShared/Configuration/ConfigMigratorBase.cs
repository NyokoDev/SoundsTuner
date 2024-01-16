using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using YamlDeserializer = YamlDotNet.Serialization.Deserializer;

namespace AmbientSoundsTuner2.CommonShared.Configuration
{
    /// <summary>
    /// An abstract class that implements basic functionality of <see cref="T:IConfigMigrator`1"/>.
    /// </summary>
    /// <typeparam name="T">The configuration object type.</typeparam>
    public abstract class ConfigMigratorBase<T> : IConfigMigrator<T> where T : VersionedConfig, new()
    {
        /// <summary>
        /// Gets or sets the migration methods that are used for different versions.
        /// </summary>
        protected virtual IDictionary<uint, Func<object, object>> MigrationMethods { get; set; }

        /// <summary>
        /// Gets or sets the configuration object types used for different versions.
        /// </summary>
        protected virtual IDictionary<uint, Type> VersionTypes { get; set; }

        /// <summary>
        /// Migrate an XML configuration file.
        /// </summary>
        /// <param name="version">The current version of the configuration file.</param>
        /// <param name="stream">The stream of the configuration file.</param>
        /// <returns>An up-to-date configuration object.</returns>
        [Obsolete("Please use MigrateFromXml or MigrateFromYaml instead.")]
        public T Migrate(uint version, Stream stream)
        {
            return this.MigrateFromXml(version, stream);
        }

        /// <summary>
        /// Migrate an XML configuration file.
        /// </summary>
        /// <param name="version">The current version of the configuration file.</param>
        /// <param name="stream">The stream of the configuration file.</param>
        /// <returns>An up-to-date configuration object.</returns>
        public T MigrateFromXml(uint version, Stream stream)
        {
            T currentConfig = new T();
            if (version >= currentConfig.Version)
            {
                // Using latest version
                return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
            }
            else
            {
                // Using an outdated version
                object config = new XmlSerializer(this.VersionTypes[version]).Deserialize(stream);
                while (version < currentConfig.Version)
                {
                    config = this.MigrationMethods[version](config);
                    version++;
                }
                return (T)config;
            }
        }

        /// <summary>
        /// Migrate a YAML configuration file.
        /// </summary>
        /// <param name="version">The current version of the configuration file.</param>
        /// <param name="stream">The stream of the configuration file.</param>
        /// <returns>An up-to-date configuration object.</returns>
        public T MigrateFromYaml(uint version, Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                T currentConfig = new T();
                if (version >= currentConfig.Version)
                {
                    // Using latest version
                    return (T)new YamlDeserializer().Deserialize(sr, typeof(T));
                }
                else
                {
                    // Using an outdated version
                    object config = new YamlDeserializer().Deserialize(sr, this.VersionTypes[version]);
                    while (version < currentConfig.Version)
                    {
                        config = this.MigrationMethods[version](config);
                        version++;
                    }
                    return (T)config;
                }
            }
        }
    }
}
