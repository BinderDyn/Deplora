using Deploy.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.XML.Models
{
    public class DeployConfiguration
    {
        /// <summary>
        /// The path to deploy the files to
        /// </summary>
        public string DeployPath { get; set; }
        /// <summary>
        /// The custom name for this configuration
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The system generated identifier
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// Whether the Deploy process will request SQL-Commands for AfterDeploy for changes on a database
        /// </summary>
        public bool HasSqlCommands { get; set; }
        /// <summary>
        /// Which database adapter the configuration will use for SQL-Commands
        /// </summary>
        public DatabaseAdapter DatabaseAdapter { get; set; }
    }
}
