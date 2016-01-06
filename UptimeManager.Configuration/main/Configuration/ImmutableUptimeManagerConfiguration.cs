// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace UptimeManager.Configuration
{
    class ImmutableUptimeManagerConfiguration : IUptimeManagerConfiguration
    {

        public string ConfigurationPath
        {
            get;             
        }

        public IEnumerable<IDeviceConfiguration> Devices 
        { 
            get;            
        }


        public ImmutableUptimeManagerConfiguration(string configurationPath, IEnumerable<IDeviceConfiguration> devices)
        {
            if (configurationPath == null)
            {
                throw new ArgumentNullException(nameof(configurationPath));
            }
            if (devices == null)
            {
                throw new ArgumentNullException(nameof(devices));
            }
            this.ConfigurationPath = configurationPath;
            this.Devices = devices.ToList();
        }
    }
}
