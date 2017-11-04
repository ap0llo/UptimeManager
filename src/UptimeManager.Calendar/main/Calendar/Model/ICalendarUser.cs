// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

namespace UptimeManager.Calendar.Model
{
    public interface ICalendarUser
    {
        string Name { get; }

        string Email { get; }
    }
}