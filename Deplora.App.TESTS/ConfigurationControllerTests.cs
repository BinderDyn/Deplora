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
        private string defaultPath { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Deplora"); }


        [TestInitialize]
        public void Initialize()
        {
            if (File.Exists(Path.Combine(defaultPath, "DeploraConfig.xml")))
            {
                File.Delete(Path.Combine(defaultPath, "DeploraConfig.xml"));
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
            Cleanup();
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
        public void CreateDeployConfiguration_Test_WithExcludedPaths()
        {
            // ARRANGE
            Cleanup();
            var deployConfigurationParam = new DeployConfigurationCreateParam
            {
                Name = "Test",
                HasSqlCommands = true,
                ExcludedPaths = new string[] { "testExcluded" },
                ExcludedPathsForBackup = new string[] { "testExcludedForBackup" },
            };
            var config = ConfigurationController.GetCurrentSettings();

            // ACT
            var id = ConfigurationController.CreateDeployConfiguration(deployConfigurationParam);

            // ASSERT
            Assert.AreNotEqual(Guid.Empty, id);
            var currentConfig = ConfigurationController.GetCurrentSettings();
            Assert.AreEqual(1, currentConfig.DeployConfigurations.Count);
            Assert.AreEqual("testExcluded", currentConfig.DeployConfigurations.First().ExcludedPaths.First());
            Assert.AreEqual("testExcludedForBackup", currentConfig.DeployConfigurations.First().ExcludedForBackupPaths.First());
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

        [TestMethod]
        public void DeleteDeployConfigurations_Test()
        {
            // ARRANGE
            var xmlManager = new XMLManager();
            var guid = Guid.Parse("7ff27323-d1fd-4ec2-abad-b86ccb6f783e");
            var guid2 = Guid.Parse("7ff27323-d1fd-4ec2-abad-b86ccb6f783d");
            var guid3 = Guid.Parse("7ff27323-d1fd-4ec2-abad-b86ccb6f783c");
            var guid4 = Guid.Parse("7ff27323-d1fd-4ec2-abad-b86ccb6f783b");
            var guid5 = Guid.Parse("7ff27323-d1fd-4ec2-abad-b86ccb6f783a");
            var applicationConfiguration = new ApplicationConfiguration()
            {
                DeployConfigurations = new System.Collections.Generic.List<XML.Models.DeployConfiguration>
                {
                    new XML.Models.DeployConfiguration { Name = "Test", ID = guid },
                    new XML.Models.DeployConfiguration { Name = "Test2", ID = guid2 },
                    new XML.Models.DeployConfiguration { Name = "Test3", ID = guid3 },
                    new XML.Models.DeployConfiguration { Name = "Test4", ID = guid4 },
                    new XML.Models.DeployConfiguration { Name = "Test5", ID = guid5 },
                },
                IISPath = "empty"
            };
            ConfigurationController.SaveApplicationConfiguration(applicationConfiguration);

            // ACT
            var guidsToDelete = new Guid[] { guid3, guid4, guid5 };
            ConfigurationController.DeleteDeployConfigurations(guidsToDelete);

            // ASSERT
            var currentSettings = ConfigurationController.GetCurrentSettings();
            Assert.AreEqual(2, currentSettings.DeployConfigurations.Count);
            Assert.IsNotNull(currentSettings.DeployConfigurations.FirstOrDefault(dc => dc.ID == guid));
            Assert.IsNotNull(currentSettings.DeployConfigurations.FirstOrDefault(dc => dc.ID == guid2)); 
            Assert.IsNull(currentSettings.DeployConfigurations.FirstOrDefault(dc => dc.ID == guid3));
            Assert.IsNull(currentSettings.DeployConfigurations.FirstOrDefault(dc => dc.ID == guid4));
            Assert.IsNull(currentSettings.DeployConfigurations.FirstOrDefault(dc => dc.ID == guid5));
            
        }

        [TestCleanup]
        public void Cleanup()
        {
            var customPath = Path.Combine(defaultPath, "testcustom");
            if (Directory.Exists(customPath))
            {
                Directory.Delete(customPath, true);
            }

            if (File.Exists(Path.Combine(defaultPath, "DeploraConfig.xml")))
            {
                File.Delete(Path.Combine(defaultPath, "DeploraConfig.xml"));
            }
        }
    }
}
