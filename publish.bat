@echo off
SET LOGDIRPATH=%~dp0\build
SET LOGFILEPATH=%LOGDIRPATH%\publish.log
SET COMMONPARAMETERS=/p:Configuration=Release /M /fl /flp:verbosity=normal;Append;LogFile=%LOGFILEPATH% /t:Publish

if not exist "%LOGDIRPATH%" mkdir "%LOGDIRPATH%"
if exist "%LOGFILEPATH%" del "%LOGFILEPATH%"

CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Daemon\main\UptimeManager.Daemon.csproj /p:RuntimeIdentifier=linux-arm

CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Daemon\main\UptimeManager.Daemon.csproj /p:RuntimeIdentifier=win-x64

REM CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     REM %~dp0\src\UptimeManager.Client.Powershell\main\UptimeManager.Client.Powershell.csproj /p:RuntimeIdentifier=win