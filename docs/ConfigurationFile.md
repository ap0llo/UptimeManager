UptimeManager Configuration
============================
The configuration file is an xml file where all devices to be monitored by UptimeManager need to be specified


Example
-------

    <?xml version="1.0" encoding="utf-8" ?>
    <UptimeManagerConfiguration xmlns="http://grynwald.net/schemas/2016/UptimeManager/v1/Configuration/" Version="1.0">

        <Device Name="Device1">
        
            <UptimeProviders>
                <Calendar Name="Calendar1" SettingsDirectory="dir" Type="GoogleCalendar"/>
            </UptimeProviders>
        
            <IsRunningCommand>
                <PingCommand Address="device1"/>
            </IsRunningCommand>

            <StartCommand>
                <ShellCommand ProgramName="wakeonlan" Arguments="00:00:00:00:00:00" ExpectedReturnCode="0" />
            </StartCommand>

            <StopCommand>
                <NopCommand/>
            </StopCommand>
            
            <UptimeBufferInterval Minutes="5"/>
            
    </Device>
    
        
    </UptimeManagerConfiguration>


For every device you need to specify a name, the "Uptime Providers" (= the calendar backends) for the device
and the command to determine if a device is running and the commands that start/stop the server.

***The configuration file format was designed to be easily extensible for additional backends.
In the current version of UptimeManger however, you can only specify a single uptime provider, 
which only be a Google Calendar***

Optionally you can specify a value for "UptimeBufferInterval". This is the time UptimeManager will start
the device before the next calendar event begins and will stop the device after an event has ended.
If it is omitted from the configuration file, a default value of 10 minutes will be used.


Calendar Uptime Provider
------------------------
Specifies a calendar backend to determine if a device is supposed to be running.
It has three attributes
- *Type*: The type of the calendar backend. Currently, this always has to be 'GoogleCalendar'
- *Name*: The Name of the calendar to monitor. This has to be the same name that is visible in the 
  Google Calendar web interface 
- *SettingsDirectory*: UptimeManager has to store authentication information to interact with Google calendar.
   The directory specified here will be used to store this information. If a relative 
   path is specified, it will be interpreted relative to the directory the configuration file 
   is located in.


Commands
--------
Commands are used both to detemine the current state of a device and to 
turn a device on or off.

Commands return a boolean value that in the case of 'IsRunningCommand' is interpreted
as "On" (true) or "Off" (false)

Currently, these three commands are implemented

- NopCommand: Does nothing and always returns true
- PingCommand: Sends a ping request to the specified address and returns
  whether the target is reachable
- ShellCommand: Executes the specified program with the specified arguments and returns
  whether the exit code of the program has the expected value. 
  The 'ExpectedReturnCode' attribute is optional. If it is omitted, a return code of 0 will be expected
 

