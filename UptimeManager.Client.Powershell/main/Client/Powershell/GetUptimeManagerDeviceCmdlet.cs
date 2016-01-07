// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Management.Automation;

namespace UptimeManager.Client.Powershell
{
    [Cmdlet(VerbsCommon.Get, Nouns.UptimeManagerDevice)]
    public class GetUptimeManagerDeviceCmdlet : CmdletBase
    {
        [Parameter(Mandatory = false, Position = 0)]
        public string Name { get; set; }


        protected override void ProcessRecord()
        {
            var config = GetConfiguration();

            var devices = config.Devices;
            if (!String.IsNullOrEmpty(Name))
            {
                var pattern = new WildcardPattern(Name, WildcardOptions.IgnoreCase);
                devices = devices.Where(d => pattern.IsMatch(d.Name));
            }

            foreach (var device in devices)
            {
                WriteObject(device);
            }
        }
    }
}