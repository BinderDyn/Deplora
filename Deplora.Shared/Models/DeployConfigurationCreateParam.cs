using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.Shared.Models
{
    public class DeployConfigurationCreateParam : DeployConfigurationUpdateParam, DeployConfiguration.ICreateParam
    {
        public DeployConfigurationCreateParam()
        {
        }

        public DeployConfigurationCreateParam(DeployConfiguration.IUpdateParam param)
        {
            this.APIKey = param.APIKey;
            this.DatabaseAdapter = param.DatabaseAdapter;
            this.DeployPath = param.DeployPath;
            this.NewestVersionUrl = param.NewestVersionUrl;
            this.HasSqlCommands = param.HasSqlCommands;
            this.Name = param.Name;
        }

        public Guid ID { get; set; }
    }
}
