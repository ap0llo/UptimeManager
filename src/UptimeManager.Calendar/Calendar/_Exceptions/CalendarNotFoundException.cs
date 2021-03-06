﻿// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Calendar
{
    /// <summary>
    /// The exception that is thrown when a calendar could not be located (e.g. name in configuration does not match the name
    /// in the calendar backend)
    /// </summary>
    public class CalendarNotFoundException : CalendarException
    {
        public CalendarNotFoundException(string message) : base(message)
        {
        }
    }
}