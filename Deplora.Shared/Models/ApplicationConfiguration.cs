﻿using Deplora.XML.Models;
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

        public void AddDeployConfig(DeployConfiguration.IUpdateParam param)
        {
            var createParam = new DeployConfigurationCreateParam(param);
            var id = ApplicationConfiguration.GetValidId(Guid.NewGuid(), this.DeployConfigurations);
            createParam.ID = id;
            this.DeployConfigurations.Add(new DeployConfiguration(createParam));
        }

        public void UpdateDeployConfig(DeployConfiguration.IUpdateParam param, Guid id)
        {
            var config = this.DeployConfigurations.SingleOrDefault(dc => dc.ID == id);
            if (config != null)
            {
                config.Update(param);
            }
        }

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
