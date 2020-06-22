using Deplora.XML.Models;
using Deploy.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deploy.Shared.Models
{
    public class DeployConfigurationCreateParam : DeployConfiguration.ICreateParam
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
        public bool HasSqlCommands { get; set; }
        public DatabaseAdapter DatabaseAdapter { get; set; }
        public string NewestVersionUrl { get; set; }
        public string APIKey { get; set; }
        public string DeployPath { get; set; }
        public string Name { get; set; }
    }
}
