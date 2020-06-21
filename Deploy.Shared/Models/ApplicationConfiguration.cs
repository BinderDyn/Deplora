using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deploy.Shared.Models
{
    public class ApplicationConfiguration
    {
        /// <summary>
        /// All the configurations set up so far
        /// </summary>
        public IEnumerable<DeployConfiguration> DeployConfigurations { get; set; }
    }
}
