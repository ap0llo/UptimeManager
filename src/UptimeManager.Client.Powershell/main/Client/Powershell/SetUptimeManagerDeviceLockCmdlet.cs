// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;
using System.Management.Automation;

namespace UptimeManager.Client.Powershell
{
    [Cmdlet(VerbsCommon.Set, Nouns.UptimeManagerDeviceLock)]
    public class SetUptimeManagerDeviceLockCmdlet : CmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string DeviceName { get; set; }

        [Parameter(Mandatory = false)]
        public string Comment { get; set; }


        protected override void ProcessRecord()
        {
            var comment = String.IsNullOrEmpty(Comment) ? "" : Comment;

            var lockManager = LockManager.GetInstance(GetConfiguration());
            lockManager.LockShutdown(this.DeviceName, comment);
        }
    }
}