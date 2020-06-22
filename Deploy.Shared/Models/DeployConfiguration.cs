﻿using Deploy.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.XML.Models
{
    public class DeployConfiguration
    {
        public DeployConfiguration(ICreateParam param)
        {
            this.DeployPath = param.DeployPath;
            this.Name = param.Name;
            this.ID = param.ID;
            this.NewestVersionUrl = param.NewestVersionUrl;
            this.HasSqlCommands = param.HasSqlCommands;
            this.DatabaseAdapter = param.DatabaseAdapter;
            this.APIKey = param.APIKey;
        }

        public void Update(IUpdateParam param)
        {
            this.DeployPath = param.DeployPath;
            this.Name = param.Name;
            this.NewestVersionUrl = param.NewestVersionUrl;
            this.HasSqlCommands = param.HasSqlCommands;
            this.DatabaseAdapter = param.DatabaseAdapter;
            this.APIKey = param.APIKey;
        }

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
        /// <summary>
        /// The URL to the newest version (FTP or API-Call)
        /// </summary>
        public string NewestVersionUrl { get; set; }
        /// <summary>
        /// The key for the API-Call
        /// </summary>
        public string APIKey { get; set; }

        public interface ICreateParam : IUpdateParam
        {
            Guid ID { get; set; }
        }

        public interface IUpdateParam 
        {
            bool HasSqlCommands { get; set; }
            DatabaseAdapter DatabaseAdapter { get; set; }
            string NewestVersionUrl { get; set; }
            string APIKey { get; set; }
            string DeployPath { get; set; }
            string Name { get; set; }
        }
    }
}