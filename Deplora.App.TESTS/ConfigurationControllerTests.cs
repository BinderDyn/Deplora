using Deplora.Application;
using Deplora.Shared.Models;
using Deplora.XML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Deplora.App.TESTS
{
    [TestClass]
    public class ConfigurationControllerTests
    {
        private string testCustomPath { get => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }


        [TestInitialize]
        public void Initialize()
        {
            if (File.Exists(Path.Combine(testCustomPath, "DeploraConfig.xml")))
            {
                File.Delete(Path.Combine(testCustomPath, "DeploraConfig.xml"));
            }
        }


        [TestMethod]
        public void GetCurrentSettings_Test()
        {
            // ACT
            var currentSettings = ConfigurationController.GetCurrentSettings();

            // ASSERT
            Assert.IsNotNull(currentSettings);
        }

        [TestMethod]
        public void GetDeployConfigurations_Test()
        {
            // ARRANGE
            var xmlManager = new XMLManager();
            var applicationConfiguration = new ApplicationConfiguration()
            {
                DeployConfigurations = new System.Collections.Generic.List<XML.Models.DeployConfiguration>
                {
                    new XML.Models.DeployConfiguration { Name = "Test", ID = Guid.NewGuid() },
                    new XML.Models.DeployConfiguration { Name = "Test2", ID = Guid.NewGuid() }
                }
            };
            xmlManager.SaveApplicationConfigurationToFile(applicationConfiguration);

            // ACT
            var deployConfigurations = ConfigurationController.GetDeployConfigurations();

            // ASSERT
            Assert.AreEqual(2, deployConfigurations.Count());
        }

        [TestMethod]
        public void GetDeployConfiguration_Test()
        {
            // ARRANGE
            var xmlManager = new XMLManager();
            var guid = Guid.Parse("7ff27323-d1fd-4ec2-abad-b86ccb6f783f");
            var applicationConfiguration = new ApplicationConfiguration()
            {
                DeployConfigurations = new System.Collections.Generic.List<XML.Models.DeployConfiguration>
                {
                    new XML.Models.DeployConfiguration { Name = "Test", ID = guid },
                    new XML.Models.DeployConfiguration { Name = "Test2", ID = Guid.NewGuid() }
                }
            };
            xmlManager.SaveApplicationConfigurationToFile(applicationConfiguration);

            // ACT
            var configuration = ConfigurationController.GetDeployConfiguration(guid);

            // ASSERT
            Assert.IsNotNull(configuration);
            Assert.AreEqual("Test", configuration.Name);
        }

        [TestMethod]
        public void CreateDeployConfiguration_Test()
        {
            // ARRANGE
            var deployConfigurationParam = new DeployConfigurationCreateParam
            {
                Name = "Test",
                HasSqlCommands = true
            };
            var config = ConfigurationController.GetCurrentSettings();

            // ACT
            var id = ConfigurationController.CreateDeployConfiguration(deployConfigurationParam);

            // ASSERT
            Assert.AreNotEqual(Guid.Empty, id);
            var currentConfig = ConfigurationController.GetCurrentSettings();
            Assert.AreEqual(1, currentConfig.DeployConfigurations.Count);
        }

        [TestMethod]
        public void UpdateDeployConfiguration_Test()
        {
            // ARRANGE
            var xmlManager = new XMLManager();
            var guid = Guid.Parse("7ff27323-d1fd-4ec2-abad-b86ccb6f783f");
            var applicationConfiguration = new ApplicationConfiguration()
            {
                DeployConfigurations = new System.Collections.Generic.List<XML.Models.DeployConfiguration>
                {
                    new XML.Models.DeployConfiguration { Name = "Test", ID = guid },
                    new XML.Models.DeployConfiguration { Name = "Test2", ID = Guid.NewGuid() }
                },
                IISPath = "empty"
            };
            xmlManager.SaveApplicationConfigurationToFile(applicationConfiguration);
            var updateParam = new DeployConfigurationUpdateParam
            {
                Name = "Test_Update",
                ExcludedPaths = new string[] { "path1", "path2" }
            };

            // ACT
            ConfigurationController.UpdateDeployConfiguration(updateParam, guid);

            // ASSERT
            var configuration = ConfigurationController.GetDeployConfiguration(guid);
            Assert.AreEqual(updateParam.Name, configuration.Name);
            Assert.AreEqual(string.Join(",", updateParam.ExcludedPaths), string.Join(",", configuration.ExcludedPaths));
            var currentSettings = ConfigurationController.GetCurrentSettings();
            Assert.AreEqual(2, currentSettings.DeployConfigurations.Count);
        }


        [TestMethod]
        public void CreateDefaultConfigurationWithCustomPath_Test()
        {
            // ARRANGE
            var customPath = Path.Combine(testCustomPath, "testcustom");
            var customConfigPath = Path.Combine(customPath, "DeploraConfig.xml");
            Directory.CreateDirectory(customPath);

            // ACT
            ConfigurationController.CreateDefaultConfigurationWithCustomPath(customConfigPath);

            // ASSERT
            var currentSettings = ConfigurationController.GetCurrentSettings(customConfigPath);
            Assert.IsNotNull(currentSettings);
        }

        [TestMethod]
        public void SaveApplicationConfiguration_Test()
        {
            // ARRANGE
            var applicationConfiguration = new ApplicationConfiguration()
            {
            };

            // ACT
            ConfigurationController.SaveApplicationConfiguration(applicationConfiguration);

            // ASSERT
            Assert.IsNotNull(ConfigurationController.GetCurrentSettings());
        }

        [TestCleanup]
        public void Cleanup()
        {
            var customPath = Path.Combine(testCustomPath, "testcustom");
            if (Directory.Exists(customPath))
            {
                Directory.Delete(customPath, true);
            }

            if (File.Exists(Path.Combine(testCustomPath, "DeploraConfig.xml")))
            {
                File.Delete(Path.Combine(testCustomPath, "DeploraConfig.xml"));
            }
        }
    }
}
