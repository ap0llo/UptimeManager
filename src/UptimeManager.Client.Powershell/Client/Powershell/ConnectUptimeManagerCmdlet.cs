// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Management.Automation;
using UptimeManager.Calendar;

namespace UptimeManager.Client.Powershell
{
    [Cmdlet(VerbsCommunications.Connect, Nouns.UptimeManager)]
    public class ConnectUptimeManagerCmdlet : CmdletBase
    {
        protected override void ProcessRecord()
        {
            var config = GetConfiguration();
            var calendarFactory = new CalendarFactory();

            var calendars = config.Devices.Select(calendarFactory.GetCalendar);

            foreach (var calendar in  calendars)
            {
                calendar.ConnectToService();
            }
        }
    }
}