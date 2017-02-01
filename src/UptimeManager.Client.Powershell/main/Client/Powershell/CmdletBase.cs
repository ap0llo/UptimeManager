// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Management.Automation;
using UptimeManager.Configuration;

namespace UptimeManager.Client.Powershell
{
    public class CmdletBase : Cmdlet
    {
        const string s_AppDataDirectoryName = "UptimeManager";
        const string s_ConfigFileName = "Configuration.xml";


        [Parameter(Mandatory = false)]
        public string ConfigurationFile { get; set; }



        protected IUptimeManagerConfiguration GetConfiguration()
        {
            var configPath = GetConfigFilePath();

            var configReader = new ConfigurationReaderFactory().GetConfigurationReader(ConfigurationType.Xml);
            var config = configReader.ReadConfiguration(configPath);

            return config;
        }



        string GetConfigFilePath()
        {
            if (String.IsNullOrEmpty(ConfigurationFile))
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    s_AppDataDirectoryName,
                    s_ConfigFileName);
            }
            else
            {
                return ConfigurationFile;
            }
        }
    }
}