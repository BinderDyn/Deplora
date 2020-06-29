using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deplora.Shared.Models
{
    public class ApplicationConfiguration : ApplicationConfiguration.ICreateParam
    {
        public ApplicationConfiguration()
        {
            this.DeployConfigurations = new List<DeployConfiguration>();
        }

        /// <summary>
        /// All the configurations set up so far
        /// </summary>
        public List<DeployConfiguration> DeployConfigurations { get; set; }

        /// <summary>
        /// Add new deploy config to the existing ones
        /// </summary>
        /// <param name="param"></param>
        /// <param name="excludedPaths"></param>
        /// <param name="excludedPathsForBackup"></param>
        public void AddDeployConfig(DeployConfiguration.IUpdateParam param, string[] excludedPaths = null, string[] excludedPathsForBackup = null)
        {
            var createParam = new DeployConfigurationCreateParam(param);
            var id = ApplicationConfiguration.GetValidId(Guid.NewGuid(), this.DeployConfigurations);
            createParam.ID = id;
            this.DeployConfigurations.Add(new DeployConfiguration(createParam));
        }

        /// <summary>
        /// Update existing deploy config if exists
        /// </summary>
        /// <param name="param"></param>
        /// <param name="id"></param>
        /// <param name="excludedPaths"></param>
        /// <param name="excludedPathsForBackup"></param>
        public void UpdateDeployConfig(DeployConfiguration.IUpdateParam param, Guid id, string[] excludedPaths = null, string[] excludedPathsForBackup = null)
        {
            var config = this.DeployConfigurations.SingleOrDefault(dc => dc.ID == id);
            if (config != null)
            {
                config.Update(param);
                config.UpdatePaths(excludedPaths, excludedPathsForBackup);
            }
        }

        /// <summary>
        /// Deletes an existing configuration if exists
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDeployConfig(Guid id)
        {
            var config = DeployConfigurations.SingleOrDefault(dc => dc.ID == id);
            if (config != null)
            {
                this.DeployConfigurations.Remove(config);
            }
        }

        /// <summary>
        /// Gets a valid ID for configurations
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deployConfigurations"></param>
        /// <returns></returns>
        public static Guid GetValidId(Guid id, IEnumerable<DeployConfiguration> deployConfigurations)
        {
            if (!deployConfigurations.Any(dc => dc.ID == id))
            {
                return id;
            }
            else
            {
                var newId = Guid.NewGuid();
                return ApplicationConfiguration.GetValidId(newId, deployConfigurations);
            }
        }

        public string IISPath { get; set; }

        public interface ICreateParam
        {
            string IISPath { get; set; }
        }
    }
}
