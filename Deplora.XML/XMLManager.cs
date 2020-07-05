using Deplora.Shared.Models;
using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Deplora.XML
{
    public class XMLManager
    {
        /// <summary>
        /// Gets the corresponding configuration by GUID
        /// </summary>
        /// <param name="configurationId"></param>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public DeployConfiguration GetConfigurationByID(Guid configurationId, string configFilePath = null)
        {
            if (string.IsNullOrEmpty(configFilePath)) configFilePath = GetDefaultPath();
            var appConfig = GetApplicationConfiguration(configFilePath);
            return appConfig.DeployConfigurations.FirstOrDefault(conf => conf.ID == configurationId);
        }

        /// <summary>
        /// Returns the applications configuration deserialized into an object from XML
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public ApplicationConfiguration GetApplicationConfiguration(string configFilePath = null)
        {
            if (string.IsNullOrEmpty(configFilePath)) configFilePath = GetDefaultPath();
            var serializer = new XmlSerializer(typeof(ApplicationConfiguration));
            ApplicationConfiguration appConfig = null;
            using (Stream reader = new FileStream(configFilePath, FileMode.Open))
            {
                appConfig = (ApplicationConfiguration)serializer.Deserialize(reader);
            }
            return appConfig;
        }

        /// <summary>
        /// Saves or creates a new Configuration for the app
        /// </summary>
        /// <param name="applicationConfigurationState"></param>
        /// <param name="configFilePath"></param>
        public void SaveApplicationConfigurationToFile(ApplicationConfiguration.ICreateParam applicationConfigurationState, string configFilePath = null)
        {
            if (string.IsNullOrEmpty(configFilePath)) configFilePath = GetDefaultPath();
            var serializer = new XmlSerializer(typeof(ApplicationConfiguration));
            using (Stream writer = new FileStream(configFilePath, FileMode.OpenOrCreate))
            {
                writer.SetLength(0);
                var xmlWriter = new XmlTextWriter(writer, Encoding.UTF8);
                serializer.Serialize(xmlWriter, applicationConfigurationState);
                xmlWriter.Close();
            }
        }

        /// <summary>
        /// Gets the default filepath of the executing assembly
        /// </summary>
        /// <returns></returns>
        private string GetDefaultPath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DEFAULT_FILE_NAME);
        }

        public const string DEFAULT_FILE_NAME = "DeploraConfig.xml";
    }
}
