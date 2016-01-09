PowerShell Cmdlets
==================
The PowerShell Module "UptimeManager.Client.Powershell" includes some PowerShell
Cmdlets that can be useful when devices are managed using UptimeManager.

The following Cmdlets are included
- Get-UptimeManagerDevice
- Connect-UptimeManager
- Set-UptimeManagerDeviceLock
- Clear-UptimeManagerDeviceLock

Configuration
------------------
For all cmdlets, a path to the configuration file can be specified using the
*ConfigurationFile* parameter. If this parameter is omitted, the configuration
file will be loaded from %appdata%\UptimeManager\Configuration.xml.

Get-UptimeManagerDevice
-------------------------
Lists all devices found in the configuration file.

Connect-UptimeManager
------------------------
Connects to the Google Calendar backend. If no authentication data is present
in the configuration directory the browser will be launched automatically where
you'll need to grant access to your Google Calendar. If authentication has
already been done, this cmdlet will have no user-visible effect

Set-UptimeManagerDeviceLock
---------------------------
"Locks" a device. It will create an event in the calendar and start a background
thread that automatically extends the end-time of the event to keep the device
running.
The thread will run as long as the PowerShell process is active or the thread is
stopped using the Clear-UptimeManagerDeviceLock cmdlet.

The cmdlet can only be called once per device and PowerShell process
(and configuration file)

If a value for the optional 'Comment' parameter is specified, this value will be
appended to the title of the event created in the calendar

**Example**

    Set-UptimeManagerDeviceLock "Device1" -Comment "This is a comment"

Clear-UptimeManagerDeviceLock
-----------------------------
"Unlocks" a device. This will stop the background thread that automatically
prolongs the event create by the Set-UptimeManagerDeviceLock cmdlet and sets
the end-time of the calendar entry to the current time.
