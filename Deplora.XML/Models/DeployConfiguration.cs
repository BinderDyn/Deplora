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
        public string Designation { get; set; }
    }
}
