// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Calendar.Model
{
    public class CalendarEvent : ICalendarEvent
    {

        public string Id { get; }

        public string Name { get; set; }

        public bool FromSeries { get; }

        public ICalendarUser CreatedBy { get; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }


        public CalendarEvent(string id, ICalendarUser createdBy, bool fromSeries)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            this.Id = id;
            this.CreatedBy = createdBy;
            this.FromSeries = fromSeries;
        }


    }
}