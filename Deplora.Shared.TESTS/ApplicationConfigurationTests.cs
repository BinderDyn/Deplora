using Deplora.Shared.Models;
using Deplora.XML.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Deplora.Shared.TESTS
{
    [TestClass]
    public class ApplicationConfigurationTests
    {
        [TestMethod]
        public void AddDeployConfig_Test()
        {
            // ARRANGE
            var applicationConfiguration = new ApplicationConfiguration
            {
                DeployConfigurations = new List<DeployConfiguration>
                {
                    DeployConfigurationFactory.CreateDeployConfiguration("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c2")
                }
            };

            // ACT
            applicationConfiguration.AddDeployConfig(new DeployConfigurationUpdateParam());

            // ASSERT
            Assert.AreEqual(2, applicationConfiguration.DeployConfigurations.Count);
        }

        [TestMethod]
        public void UpdateDeployConfig_Test()
        {
            // ARRANGE
            var applicationConfiguration = new ApplicationConfiguration
            {
                DeployConfigurations = new List<DeployConfiguration>
                {
                    DeployConfigurationFactory.CreateDeployConfiguration("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c2")
                }
            };

            // ACT
            applicationConfiguration.UpdateDeployConfig(new DeployConfigurationUpdateParam(), Guid.Parse("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c2"));

            // ASSERT
            Assert.AreEqual(1, applicationConfiguration.DeployConfigurations.Count);
            Assert.IsNull(applicationConfiguration.DeployConfigurations.FirstOrDefault().Name);
        }

        [TestMethod]
        public void GetValidId_Test()
        {
            // ARRANGE
            Guid id = Guid.Parse("1b058142-cd99-4e66-9ec6-3b1a0bf3e3c2");
            var configurations = new List<DeployConfiguration>
            {
                DeployConfigurationFactory.CreateDeployConfiguration(id.ToString())
            };
            // ACT
            var validId = ApplicationConfiguration.GetValidId(id, configurations);

            // ASSERT
            Assert.IsTrue(configurations.Count(c => c.ID == id) == 1);
        }

        [TestMethod]
        public void GetValidId_Test_Empty()
        {
            // ARRANGE
            Guid id = Guid.Empty;
            var configurations = new List<DeployConfiguration>
            {
                DeployConfigurationFactory.CreateDeployConfiguration(id.ToString())
            };
            // ACT
            var validId = ApplicationConfiguration.GetValidId(id, configurations);

            // ASSERT
            Assert.IsTrue(configurations.Count(c => c.ID == id) == 1);
        }
    }

    public class DeployConfigurationFactory
    {
        internal static DeployConfiguration CreateDeployConfiguration(string guidString)
        {
            return new DeployConfiguration(new DeployConfigurationCreateParam
            {
                APIKey = "Testkey",
                AppPoolName = "Apppool",
                DatabaseAdapter = Deplora.Shared.Enums.DatabaseAdapter.None,
                DeployPath = Directory.GetCurrentDirectory(),
                HasSqlCommands = false,
                ID = new Guid(guidString),
                Name = "Test",
                NewestVersionUrl = string.Empty,
                WebSiteName = "Testwebsite"
            });
        }
    }
}
