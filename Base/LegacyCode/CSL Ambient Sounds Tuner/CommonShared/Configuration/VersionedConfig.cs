using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace AmbientSoundsTuner2.CommonShared.Configuration
{
    /// <summary>
    /// A class that provides basic implementation for loading and saving versioned configuration files.
    /// Both XML and YAML files are supported, but in order to use them, the correct attributes have to be assigned.
    /// </summary>
    [XmlRoot("Configuration")]
    public abstract class VersionedConfig : Config
    {
        /// <summary>
        /// Gets or sets the version of this configuration file.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [XmlAttribute("version")]
        [YamlMember(Alias = "ConfigVersion", Order = -1)]
        public virtual uint Version { get; set; }

        /// <summary>
        /// Loads the configuration from a file. XML and YAML are supported.
        /// </summary>
        /// <typeparam name="T">The config object type.</typeparam>
        /// <param name="filename">The name of the configuration file.</param>
        /// <param name="migrator">The config migrator object.</param>
        /// <returns>The configuration instance.</returns>
        public static T LoadConfig<T>(string filename, IConfigMigrator<T> migrator) where T : VersionedConfig, new()
        {
            if (File.Exists(filename))
            {
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".xml":
                        return LoadConfigAsXml<T>(filename, migrator);
                    case ".yml":
                        return LoadConfigAsYaml<T>(filename, migrator);
                    default:
                        throw new InvalidOperationException("The given filename is not an XML or YAML file.");
                }
            }
            return new T();
        }

        /// <summary>
        /// Loads the configuration from an XML file. Assuming that the file exists.
        /// </summary>
        /// <typeparam name="T">The config object type.</typeparam>
        /// <param name="filename">The name of the configuration file.</param>
        /// <param name="migrator">The config migrator object.</param>
        /// <returns>The configuration instance.</returns>
        private static T LoadConfigAsXml<T>(string filename, IConfigMigrator<T> migrator) where T : VersionedConfig, new()
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fs);
                uint version = 0;
                if (doc.DocumentElement.HasAttribute("version"))
                    version = uint.Parse(doc.DocumentElement.Attributes["version"].Value);
                fs.Position = 0;
                return migrator.MigrateFromXml(version, fs);
            }

        }

        /// <summary>
        /// Loads the configuration from a YAML file. Assuming that the file exists.
        /// </summary>
        /// <typeparam name="T">The config object type.</typeparam>
        /// <param name="filename">The name of the configuration file.</param>
        /// <param name="migrator">The config migrator object.</param>
        /// <returns>The configuration instance.</returns>
        private static T LoadConfigAsYaml<T>(string filename, IConfigMigrator<T> migrator) where T : VersionedConfig, new()
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                YamlStream doc = new YamlStream();
                doc.Load(sr);
                var mapping = (YamlMappingNode)doc.Documents[0].RootNode;
                uint version = 0;
                if (mapping.Children.ContainsKey(new YamlScalarNode("ConfigVersion")))
                    version = uint.Parse(((YamlScalarNode)mapping.Children[new YamlScalarNode("ConfigVersion")]).Value);
                fs.Position = 0;
                return migrator.MigrateFromYaml(version, fs);
            }
        }
    }
}
