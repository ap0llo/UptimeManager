// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System.Management.Automation;

namespace UptimeManager.Client.Powershell
{
    [Cmdlet(VerbsCommon.Clear, Nouns.UptimeManagerDeviceLock)]
    public class ClearUptimeManagerDeviceLockCmdlet : CmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string DeviceName { get; set; }


        protected override void ProcessRecord()
        {
            var config = GetConfiguration();
            var lockManager = LockManager.GetInstance(config);

            lockManager.UnlockShutdown(DeviceName);
        }
    }
}