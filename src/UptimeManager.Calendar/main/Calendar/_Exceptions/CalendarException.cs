// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Calendar
{
    /// <summary>
    /// The exception that is thrown when a error occurs while accessing a calendar
    /// </summary>
    public class CalendarException : Exception
    {
        public CalendarException(string message)
            : base(message)
        {
        }


        public CalendarException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}