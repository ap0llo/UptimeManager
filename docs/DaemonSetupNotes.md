Daemon Setup Notes
===================

Setup via SSH
--------------
The UptimeManager daemon should run on a computer that is powered on all the
time. This could for example be a Raspberry Pi.

As this computer has no keyboard, mouse or monitor attached, it is a little
bit tricky to setup remotely using SSH.

When you run the daemon for the first time, it will not have access to Google
Calendar and attempt to launch the browser which will most likely fail in
this scenario.
To avoid this, you should use the [Connect-UptimeManager Cmdlet](PowershellCmdlets.md)
to authenticate and then copy the configuration file and the calendar settings
directory (see [Configuration File](ConfigurationFile.md)) to the target computer.

If you run the UptimeManager daemon now, it should be able to access Google
Calendar without prompting for authentication.


Running on Mono
-----------------
If you run into SSL authentication issues when running under Mono,
it might help to run

    mozroots --import --sync
    
For details, please refer to the [Mono Documentation](http://www.mono-project.com/docs/faq/security/)
