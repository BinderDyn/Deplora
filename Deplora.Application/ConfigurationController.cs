﻿using Deplora.Shared.Models;
using Deplora.XML;
using Deplora.XML.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Deplora.Application
{
    /// <summary>
    /// Manages the application settings
    /// </summary>
    public static class ConfigurationController
    {
        /// <summary>
        /// Gets the current configuration or creates a new
        /// </summary>
        /// <returns></returns>
        public static ApplicationConfiguration GetCurrentSettings(string customPath = null)
        {
            var xmlManager = new XMLManager();
            ApplicationConfiguration applicationConfiguration = null;
            try
            {
                applicationConfiguration = xmlManager.GetApplicationConfiguration(customPath);
            }
            catch (IOException)
            {
                xmlManager.SaveApplicationConfigurationToFile(CreateDefaultConfiguration(), customPath);
                applicationConfiguration = xmlManager.GetApplicationConfiguration(customPath);
            }
            return applicationConfiguration;
        }

        /// <summary>
        /// Returns all available deploy configurations
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DeployConfiguration> GetDeployConfigurations()
        {
            var xmlManager = new XMLManager();
            ApplicationConfiguration applicationConfiguration = null;
            try
            {
                applicationConfiguration = xmlManager.GetApplicationConfiguration();
                return applicationConfiguration.DeployConfigurations.OrderBy(dc => dc.Name);
            }
            catch (IOException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets deploy configuration by id
        /// </summary>
        /// <param name="deployConfigurationId"></param>
        /// <returns></returns>
        public static DeployConfiguration GetDeployConfiguration(Guid id, string customConfigFilePath = null)
        {
            var xmlManager = new XMLManager();
            var currentConfig = xmlManager.GetApplicationConfiguration(customConfigFilePath);
            return currentConfig.DeployConfigurations.SingleOrDefault(config => config.ID == id);
        }

        /// <summary>
        /// Creates a new deploy configuration and saves it to the application config file
        /// </summary>
        /// <param name="param"></param>
        /// <param name="customConfigPath"></param>
        public static Guid CreateDeployConfiguration(DeployConfigurationCreateParam param, string customConfigPath = null)
        {
            var xmlManager = new XMLManager();
            var currentConfig = xmlManager.GetApplicationConfiguration(customConfigPath);
            var id = currentConfig.AddDeployConfig(param, param.ExcludedPaths, param.ExcludedPathsForBackup);
            xmlManager.SaveApplicationConfigurationToFile(currentConfig, customConfigPath);
            return id;
        }

        /// <summary>
        /// Updates existing deploy configuration and saves it to the application config file
        /// </summary>
        /// <param name="param"></param>
        /// <param name="configurationId"></param>
        /// <param name="customConfigPath"></param>
        public static void UpdateDeployConfiguration(DeployConfigurationUpdateParam param, Guid configurationId, string customConfigPath = null)
        {
            var xmlManager = new XMLManager();
            var currentConfig = xmlManager.GetApplicationConfiguration(customConfigPath);
            currentConfig.UpdateDeployConfig(param, configurationId, param.ExcludedPaths, param.ExcludedPathsForBackup);
            xmlManager.SaveApplicationConfigurationToFile(currentConfig, customConfigPath);
        }

        /// <summary>
        /// Creates a default configuration at the specific path
        /// </summary>
        /// <param name="customPath"></param>
        public static void CreateDefaultConfigurationWithCustomPath(string customPath)
        {
            var xmlManager = new XMLManager();
            xmlManager.SaveApplicationConfigurationToFile(CreateDefaultConfiguration(), customPath);
        }

        /// <summary>
        /// Saves the configuration to the xml file
        /// </summary>
        /// <returns></returns>
        public static void SaveApplicationConfiguration(ApplicationConfiguration.ICreateParam param)
        {
            var xmlManager = new XMLManager();
            xmlManager.SaveApplicationConfigurationToFile(param);
        }

        /// <summary>
        /// Deletes all configurations with given guids
        /// </summary>
        /// <param name="guids"></param>
        public static void DeleteDeployConfigurations(IEnumerable<Guid> guids)
        {
            var currentSettings = GetCurrentSettings();
            foreach (var guid in guids)
            {
                currentSettings.DeleteDeployConfig(guid);
            }
            SaveApplicationConfiguration(currentSettings);
        }

        private static ApplicationConfiguration CreateDefaultConfiguration()
        {
            string defaultIISPath = null;
            const string predictedDefaultIISPath = "C:\\Windows\\System32\\inetsrv\\";
            if (Directory.Exists(predictedDefaultIISPath))
            {
                defaultIISPath = predictedDefaultIISPath;
            }
            return new ApplicationConfiguration()
            {
                IISPath = defaultIISPath
            };
        }
    }
}
