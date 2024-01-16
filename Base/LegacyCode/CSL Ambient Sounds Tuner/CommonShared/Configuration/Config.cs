using System;
using System.IO;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using YamlDeserializer = YamlDotNet.Serialization.Deserializer;
using YamlSerializer = YamlDotNet.Serialization.Serializer;

namespace AmbientSoundsTuner2.CommonShared.Configuration
{
    /// <summary>
    /// An abstract class that provides basic implementation for loading and saving configuration files.
    /// Both XML and YAML files are supported, but in order to use them, the correct attributes have to be assigned.
    /// </summary>
    [XmlRoot("Configuration")]
    public abstract class Config
    {
        /// <summary>
        /// Loads the configuration from a file. XML and YAML are supported.
        /// </summary>
        /// <typeparam name="T">The config object type.</typeparam>
        /// <param name="filename">The name of the configuration file.</param>
        /// <returns>The configuration instance.</returns>
        public static T LoadConfig<T>(string filename) where T : new()
        {
            if (File.Exists(filename))
            {
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".xml":
                        return LoadConfigAsXml<T>(filename);
                    case ".yml":
                        return LoadConfigAsYaml<T>(filename);
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
        /// <returns>The configuration instance.</returns>
        private static T LoadConfigAsXml<T>(string filename) where T : new()
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(sr);
            }
        }

        /// <summary>
        /// Loads the configuration from a YAML file. Assuming that the file exists.
        /// </summary>
        /// <typeparam name="T">The config object type.</typeparam>
        /// <param name="filename">The name of the configuration file.</param>
        /// <returns>The configuration instance.</returns>
        private static T LoadConfigAsYaml<T>(string filename) where T : new()
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                return (T)new YamlDeserializer().Deserialize(sr, typeof(T));
            }
        }

        /// <summary>
        /// Saves the configuration to a file. XML and YAML are supported.
        /// </summary>
        /// <param name="filename">The name of the configuration file.</param>
        public virtual void SaveConfig(string filename)
        {
            string dirname = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(dirname) && !Directory.Exists(dirname))
            {
                Directory.CreateDirectory(dirname);
            }
            switch (Path.GetExtension(filename).ToLower())
            {
                case ".xml":
                    this.SaveConfigAsXml(filename);
                    break;
                case ".yml":
                    this.SaveConfigAsYaml(filename);
                    break;
                default:
                    throw new InvalidOperationException("The given filename is not an XML or YAML file.");
            }
        }

        /// <summary>
        /// Saves the configuration to an XML file. Assuming that the directory exists.
        /// </summary>
        /// <param name="filename">The name of the configuration file.</param>
        private void SaveConfigAsXml(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                new XmlSerializer(this.GetType()).Serialize(sw, this);
            }
        }

        /// <summary>
        /// Saves the configuration to a YAML file. Assuming that the directory exists.
        /// </summary>
        /// <param name="filename">The name of the configuration file.</param>
        private void SaveConfigAsYaml(string filename)
        {
            var serializer = new SerializerBuilder()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .Build();

            var yaml = serializer.Serialize(this);

            File.WriteAllText(filename, yaml);
        }
    }
}