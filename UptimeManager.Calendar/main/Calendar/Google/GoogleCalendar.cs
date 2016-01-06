// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using UptimeManager.Calendar.Model;

namespace UptimeManager.Calendar.Google
{
    /// <summary>
    /// Implementation of <see cref="ICalendar" /> that uses the Google Calendar as backend
    /// </summary>
    class GoogleCalendar : ICalendar
    {

        const string s_Cancelled = "cancelled";
        const string s_ClientSecretsFileName = "UptimeManager.Calender.Google.ClientSecrets.json";

        const string s_TlsException = "TlsException";

        //lock object to synchronize accesses to the calendar
        static readonly object s_CalendarLock = new object();

        readonly string m_CalendarSettingsPath;



        public GoogleCalendar(string calendarSettingsPath, string calendarName)
        {
            if (String.IsNullOrEmpty(calendarSettingsPath))
            {
                throw new ArgumentNullException(nameof(calendarSettingsPath));
            }

            if (String.IsNullOrEmpty(calendarName))
            {
                throw new ArgumentNullException(nameof(calendarName));
            }

            this.m_CalendarSettingsPath = calendarSettingsPath;
            this.Name = calendarName;
        }



        public string Name { get; }

        public void ConnectToService()
        {
            lock (s_CalendarLock)
            {
                using (var calendarService = GetCalendarService())
                {
                    GetCalendar(calendarService);
                }
            }
        }

        public IEnumerable<ICalendarEvent> GetEvents(DateTime startDate)
        {
            lock (s_CalendarLock)
            {
                //get calendar service
                using (var calendarService = GetCalendarService())
                {
                    //get calendar
                    var calendar = GetCalendar(calendarService);

                    //get events from calendar
                    var request = calendarService.Events.List(calendar.Id);
                    request.TimeMin = startDate;
                    request.ShowDeleted = false;

                    var allEvents = DoCalendarAction(() => request.Execute().Items.Where(ev => ev.Summary != null).ToList());

                    var singleInstance = allEvents.Where(ev => ev.Recurrence == null && ev.Status != s_Cancelled);
                    var recurring = allEvents.Where(ev => ev.Recurrence != null);


                    //for recurring events, get the next occurrence
                    var recurringInstances = recurring
                        .Select(recurringEvent => GetNextOccurrenceOfRecurringEvent(calendarService, calendar, recurringEvent, startDate))
                        .Where(ev => ev != null);

                    return singleInstance.Select(ev => ev.ToCalendarEvent())
                        .Union(recurringInstances)
                        .ToList();
                }
            }
        }

        public ICalendarEvent GetEvent(string eventId)
        {
            lock (s_CalendarLock)
            {
                using (var calendarService = GetCalendarService())
                {
                    var calendar = GetCalendar(calendarService);

                    var request = calendarService.Events.Get(calendar.Id, eventId);
                    var response = DoCalendarAction(() => request.Execute());
                    return response.ToCalendarEvent();
                }
            }
        }

        public ICalendarEvent AddEvent(string eventName, DateTime start, DateTime end)
        {
            lock (s_CalendarLock)
            {
                using (var calendarService = GetCalendarService())
                {
                    var calendar = GetCalendar(calendarService);

                    var calendarEvent = new Event();
                    calendarEvent.Summary = eventName;
                    calendarEvent.Start = new EventDateTime {DateTime = start};
                    calendarEvent.End = new EventDateTime {DateTime = end};

                    var createdEvent = DoCalendarAction(() => calendarService.Events.Insert(calendarEvent, calendar.Id));

                    return DoCalendarAction(() => createdEvent.Execute().ToCalendarEvent());
                }
            }
        }

        public ICalendarEvent UpdateEvent(ICalendarEvent eventToUpdate)
        {
            //updating instances of recurring events is not supported 
            if (eventToUpdate.FromSeries)
            {
                throw new NotSupportedException("Updates to recurring events are not supported");
            }

            lock (s_CalendarLock)
            {
                using (var calendarService = GetCalendarService())
                {
                    var calendar = GetCalendar(calendarService);

                    //get the current version of the event from Google Calendar
                    var currentEvent = DoCalendarAction(() => calendarService.Events.Get(calendar.Id, eventToUpdate.Id).Execute());

                    //update the event's properties
                    currentEvent.Summary = eventToUpdate.Name;
                    currentEvent.Start = new EventDateTime {DateTime = eventToUpdate.Start};
                    currentEvent.End = new EventDateTime {DateTime = eventToUpdate.End};

                    //send updated event back to Google Calendar
                    var updatedEvent = DoCalendarAction(() => calendarService.Events.Update(currentEvent, calendar.Id, currentEvent.Id));

                    //return the updated instance of the event
                    return DoCalendarAction(() => updatedEvent.Execute().ToCalendarEvent());
                }
            }
        }


        /// <summary>
        /// Gets a new instance of CalendarService to access Google Calendar
        /// </summary>
        CalendarService GetCalendarService()
        {
            //make sure only a single thread is initializing a calendar service at any time
            lock (s_CalendarLock)
            {
                UserCredential credential = null;

                //get credential for Google Calendar
                DoCalendarAction(() =>
                {
                    using (var stream = OpenClientSecretsStream())
                    {
                        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            new List<string> {CalendarService.Scope.Calendar},
                            "user",
                            CancellationToken.None,
                            new FileDataStore(m_CalendarSettingsPath)).Result;
                    }
                });

                // Create the calendar service using an initializer instance
                var initializer = new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "UptimeManager"
                };

                //initialize CalendarService and store instance in cache
                var calendarService = new CalendarService(initializer);
                return calendarService;
            }
        }

        /// <summary>
        /// Gets the uptime calendar from the specified CalendarService
        /// </summary>
        CalendarListEntry GetCalendar(CalendarService calendarService)
        {
            //prevent concurrent access to the CalendarService 
            lock (s_CalendarLock)
            {
                var calendars = DoCalendarAction(() => calendarService.CalendarList.List().Execute().Items.ToList());

                var calendarQuery = calendars.Where(calendar => calendar.Summary.Equals(Name)).ToList();
                if (!calendarQuery.Any())
                {
                    throw new CalendarNotFoundException(String.Format("Calendar '{0}' could not found", Name));
                }
                return calendarQuery.First();
            }
        }

        /// <summary>
        /// Gets the next occurrence of the specified recurring event after 'startDate'
        /// </summary>
        ICalendarEvent GetNextOccurrenceOfRecurringEvent(CalendarService calendarService, CalendarListEntry calendar, Event recurringEvent, DateTime startDate)
        {
            lock (s_CalendarLock)
            {
                var request = calendarService.Events.Instances(calendar.Id, recurringEvent.Id);
                request.TimeMin = startDate;

                var events = DoCalendarAction(() => request.Execute().Items.ToList());

                if (events.Any())
                {
                    return events.First().ToCalendarEvent(true);
                }
                //no more instances found
                //to make sure we do not miss a event, return event itself if it fits the startDate criterion
                //(might be only occurrence of recurring event)
                if (recurringEvent.Start.ToDateTime() >= startDate)
                {
                    return recurringEvent.ToCalendarEvent(true);
                }
                return null;
            }
        }

        /// <summary>
        /// Executes the specified function and returns its result
        /// Exceptions that can occur while accessing Google calendar are caught and rethrown as
        /// <see cref="CalendarException" />
        /// </summary>
        T DoCalendarAction<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (GoogleApiException ex)
            {
                throw new CalendarException("Error accessing Google Calendar", ex);
            }
            catch (SocketException ex)
            {
                throw new CalendarException("Error accessing Google Calendar", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new CalendarException("Error accessing Google Calendar", ex);
            }
            catch (IOException ex)
            {
                throw new CalendarException("Error accessing Google Calendar", ex);
            }
            catch (NullReferenceException ex)
            {
                throw new CalendarException("Error accessing Google Calendar", ex);
            }
            catch (WebException ex)
            {
                throw new CalendarException("Error accessing Google Calendar", ex);
            }
                //mono specific exception, cannot be referenced directly
            catch (Exception ex) when (ex.GetType().Name == s_TlsException)
            {
                throw new CalendarException("Error accessing Google Calendar", ex);
            }
        }

        /// <summary>
        /// Executes the specified action
        /// Exceptions that can occur while accessing Google calendar are caught and rethrown as
        /// <see cref="CalendarException" />
        /// </summary>
        void DoCalendarAction(Action action)
        {
            //ignore return value
            DoCalendarAction<object>(() =>
            {
                action();
                return null;
            });
        }

        Stream OpenClientSecretsStream()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);            
            var file = Path.Combine(directory, s_ClientSecretsFileName);
            return File.Open(file, FileMode.Open);
        }

    }
}