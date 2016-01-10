UptimeManager
=============
Power-management using Google Calendar

Description
-----------
"UptimeManager" is a tool I wrote some time ago to manage when the server
in my local network that is supposed to collect backups of my computers is
powered on.
This can be achieved using traditional calendaring software in combination with
calendar back-ends (currently this works only with Google Calendar).

If a device is supposed to be on at a given time is is sufficient to create a
event in the calendar and UptimeManager will turn the computer on or off.
This also works for repeating events, for example you could configure the server
that is collecting backups to be powered every day 2pm to 6pm.


Components
----------
###UptimeManager Daemon
This is the core program that should run on an always-on computer (like a
[Raspberry Pi](docs/DaemonSetupNotes.md)).
It monitors one or more calendars, checks if the devices are currently running
and issues start or stop commands if necessary.

**Usage:**
The path of the configuration file has to be passed in as first (and only)
command line parameter

    UptimeManager.Daemon.exe CONFIGFILEPATH

    mono UptimeManager.Daemon.exe CONFIGFILEPATH


###UptimeManager Powershell Client
UptimeManager includes a set of Powershell Cmdlets to interact with the
calendar backend. This component is optional, the daemon can be used without
the client components if the calendar is managed using any other
calendaring software.

See [Powershell Client](docs/PowershellCmdlets.md) for details     


Configuration
-------------
UptimeManager uses a single configuration file in which all devices and the
associated calendars are specified.

See [Configuration File](docs/ConfigurationFile.md) for details


Build Instructions
-------------------
To access Google Calendar, UptimeManager requries a client-key
("OAuth client ID"). You need to generate your own key in order to use the
program as the key is not included in the source of UptimeManager.

You can generate your own client-key using the
[Google Developer Console](http://console.developers.google.com).

In the console you can download the key in the json format.
- If you name this file 'Google.ClientSecrets.json' and place is into the
  directory 'UptimeManager.Calendar\main\Calendar' the file will be embedded
  into the UptimeManager.Calendar assembly.

  ***Bear in mind that the client ID can be retrieved from the assembly if it is
     embedded in this way before distributing binaries***

- If you do not want to embed the client-key into the assembly, the file can be
  distributed separately. If the embedded resource cannot be found,
  UptimeManager will look for a file called
  'UptimeManager.Calender.Google.ClientSecrets.json' located in the same
  directory as UptimeManager.Calendar.dll and load the client-key from there.

Acknowledgements
----------------
UptimeManager uses a number of third party components
- [NLog](http://nlog-project.org/), licensed under the BSD-License
- [Google APIs client Library for .NET](https://github.com/google/google-api-dotnet-client/)
  licensed under the Apache License, version 2

For testing
- [xunit](http://xunit.github.io), licensed under the Apache License, version 2


License
-------
UptimeManager is licensed under the MIT License.
See LICENSE.txt file in the project root for full license information.
