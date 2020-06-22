using Deplora.Shared.Models;
using Deplora.Shared.TESTS;
using Deplora.XML.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Deplora.XML.TESTS
{
    [TestClass]
    public class XMLManagerTests
    {
        private Guid id = Guid.Parse("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c2");

        [TestInitialize]
        public void CreateXMLFile()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeploraConfig.xml");
            ApplicationConfiguration applicationConfigurationState = new ApplicationConfiguration
            {
                DeployConfigurations = new List<DeployConfiguration>
                {
                    DeployConfigurationFactory.CreateDeployConfiguration(id.ToString())
                }
            };
            var serializer = new XmlSerializer(typeof(ApplicationConfiguration));
            using (Stream writer = new FileStream(filePath, FileMode.Create))
            {
                var xmlWriter = new XmlTextWriter(writer, Encoding.Unicode);
                serializer.Serialize(xmlWriter, applicationConfigurationState);
                xmlWriter.Close();
            }
        }

        [TestMethod]
        public void GetConfigurationByID_Test()
        {
            // ARRANGE
            var xmlManager = new XMLManager();

            // ACT
            var config = xmlManager.GetConfigurationByID(id);

            // ASSERT
            Assert.IsNotNull(config);
            Assert.IsTrue(config.GetType() == typeof(DeployConfiguration));
        }

        [TestMethod]
        public void GetApplicationConfiguration_Test()
        {
            // ARRANGE
            var xmlManager = new XMLManager();

            // ACT
            var appConfig = xmlManager.GetApplicationConfiguration();

            // ASSERT
            Assert.IsNotNull(appConfig);
            Assert.IsTrue(appConfig.DeployConfigurations.Count == 1);
        }

        [TestMethod]
        public void SaveApplicationConfigurationToFile_Test()
        {
            // ARRANGE
            var appConfig = new ApplicationConfiguration
            {
                DeployConfigurations = new List<DeployConfiguration>
                {
                    DeployConfigurationFactory.CreateDeployConfiguration("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c2"),
                    DeployConfigurationFactory.CreateDeployConfiguration("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c3"),
                    DeployConfigurationFactory.CreateDeployConfiguration("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c4")
                }
            };
            var xmlManager = new XMLManager();

            // ACT
            xmlManager.SaveApplicationConfigurationToFile(appConfig);

            // ASSERT
            var savedConfig = xmlManager.GetApplicationConfiguration();
            Assert.IsNotNull(savedConfig);
            Assert.AreEqual(3, savedConfig.DeployConfigurations.Count);
        }

        [TestCleanup]
        public void Cleanup()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeploraConfig.xml");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
