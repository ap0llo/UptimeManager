// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using UptimeManager.Configuration.Xml;

namespace UptimeManager.Configuration
{
    /// <summary>
    /// Public interface to instantiate configuration readers
    /// </summary>
    public class ConfigurationReaderFactory
    {
        /// <summary>
        /// Gets a new configuration reader for the specified configuratin type
        /// </summary>
        public IConfigurationReader GetConfigurationReader(ConfigurationType configurationType)
        {
            switch (configurationType)
            {
                case ConfigurationType.Xml:
                    return new XmlConfigurationReader();

                default:
                    throw new ConfigurationException("Unknown configuration type: " + configurationType);
            }
        }
    }
}