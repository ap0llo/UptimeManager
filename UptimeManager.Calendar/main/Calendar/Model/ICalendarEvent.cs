// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Calendar.Model
{
    public interface ICalendarEvent
    {
        string Id { get; }

        string Name { get; set; }

        ICalendarUser CreatedBy { get; }

        bool FromSeries { get; }

        DateTime Start { get; set; }

        DateTime End { get; set; }
    }
}