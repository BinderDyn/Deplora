using Deplora.XML.Models;
using Deploy.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deploy.Shared.Models
{
    public class DeployConfigurationUpdateParam : DeployConfiguration.IUpdateParam
    {
        public bool HasSqlCommands { get; set; }
        public DatabaseAdapter DatabaseAdapter { get; set; }
        public string NewestVersionUrl { get; set; }
        public string APIKey { get; set; }
        public string DeployPath { get; set; }
        public string Name { get; set; }
        public string AppPoolName { get; set; }
        public string WebSiteName { get; set; }
    }
}
