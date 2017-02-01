// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Core
{
    [Serializable]
    public class CommandExecutionException : UptimeManagementException
    {

        public CommandExecutionException(string message) : base(message)
        {
            
        }

    }
}