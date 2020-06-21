using Deplora.XML.Models;
using Deploy.Shared.Models;
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
    public sealed class XMLManager
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
        public void SaveApplicationConfigurationToFile(ApplicationConfiguration applicationConfigurationState, string configFilePath = null)
        {
            if (string.IsNullOrEmpty(configFilePath)) configFilePath = GetDefaultPath();
            var serializer = new XmlSerializer(typeof(ApplicationConfiguration));
            using (Stream writer = new FileStream(configFilePath, FileMode.OpenOrCreate))
            {
                var xmlWriter = new XmlTextWriter(writer, Encoding.Unicode);
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
            const string DEFAULT_FILENAME = "DeploraConfig.xml";
            return Path.Combine(Assembly.GetExecutingAssembly().Location, DEFAULT_FILENAME);
        }
    }
}
