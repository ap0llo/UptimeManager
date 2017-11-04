// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Client
{
    [Serializable]
    class LockManagerException : Exception
    {
        public LockManagerException(string message) : base(message)
        {
        }

        public LockManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}