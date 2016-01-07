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

        public string FilePath { get; }

        public IEnumerable<IDeviceConfiguration> Devices { get; }


        public ImmutableUptimeManagerConfiguration(string filePath, IEnumerable<IDeviceConfiguration> devices)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (devices == null)
            {
                throw new ArgumentNullException(nameof(devices));
            }
            this.FilePath = filePath;
            this.Devices = devices.ToList();
        }
    }
}