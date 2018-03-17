@ECHO OFF
SET LOGDIRPATH=%~dp0\build
SET LOGFILEPATH=%LOGDIRPATH%\publish.log
SET COMMONPARAMETERS=/p:Configuration=Release /M /fl /flp:verbosity=normal;Append;LogFile=%LOGFILEPATH% /t:Publish /p:ZipPublishDir=true

IF NOT EXIST "%LOGDIRPATH%" mkdir "%LOGDIRPATH%"
IF EXIST "%LOGFILEPATH%" del "%LOGFILEPATH%"

REM =======================================================================================================================
REM UptimeManager.Daemon
REM =======================================================================================================================
REM Publish as standalone application for Linux on ARM (Raspberry Pi)
CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Daemon\main\UptimeManager.Daemon.csproj /p:RuntimeIdentifier=linux-arm
IF %errorlevel% neq 0 EXIT /b %errorlevel%

REM Publish as standalone application for Windows
CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Daemon\main\UptimeManager.Daemon.csproj /p:RuntimeIdentifier=win-x64
IF %errorlevel% neq 0 EXIT /b %errorlevel%

REM =======================================================================================================================
REM UptimeManager.Client.Powershell
REM =======================================================================================================================
REM Publish Powershell client module for Powershell Core 
CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Client.Powershell\main\UptimeManager.Client.Powershell.csproj /p:TargetFramework=netstandard2.0
IF %errorlevel% neq 0 exit /b %errorlevel%


CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Client.Powershell\main\UptimeManager.Client.Powershell.csproj /p:TargetFramework=net461
IF %errorlevel% neq 0 exit /b %errorlevel%

