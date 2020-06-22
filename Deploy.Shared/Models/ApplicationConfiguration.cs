using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deploy.Shared.Models
{
    public class ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            this.DeployConfigurations = new List<DeployConfiguration>();
        }

        /// <summary>
        /// All the configurations set up so far
        /// </summary>
        public IList<DeployConfiguration> DeployConfigurations { get; set; }

        public void AddConfiguration(DeployConfiguration.IUpdateParam param)
        {
            var createParam = new DeployConfigurationCreateParam(param);
            var id = GetValidId(Guid.NewGuid());
            createParam.ID = id;
            this.DeployConfigurations.Add(new DeployConfiguration(createParam));
        }

        private Guid GetValidId(Guid id)
        {
            if (!this.DeployConfigurations.Any(dc => dc.ID == id))
            {
                return id;
            }
            else
            {
                var newId = Guid.NewGuid();
                return GetValidId(newId);
            }
        }
    }
}
