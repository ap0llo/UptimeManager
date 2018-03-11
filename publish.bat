@ECHO OFF
SET LOGDIRPATH=%~dp0\build
SET LOGFILEPATH=%LOGDIRPATH%\publish.log
SET COMMONPARAMETERS=/p:Configuration=Release /M /fl /flp:verbosity=normal;Append;LogFile=%LOGFILEPATH% /t:Publish

IF NOT EXIST "%LOGDIRPATH%" mkdir "%LOGDIRPATH%"
IF EXIST "%LOGFILEPATH%" del "%LOGFILEPATH%"

CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Daemon\main\UptimeManager.Daemon.csproj /p:RuntimeIdentifier=linux-arm
IF %errorlevel% neq 0 EXIT /b %errorlevel%

CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     %~dp0\src\UptimeManager.Daemon\main\UptimeManager.Daemon.csproj /p:RuntimeIdentifier=win-x64
IF %errorlevel% neq 0 EXIT /b %errorlevel%

REM CALL %~dp0\msbuild.bat %COMMONPARAMETERS% ^
     REM %~dp0\src\UptimeManager.Client.Powershell\main\UptimeManager.Client.Powershell.csproj /p:RuntimeIdentifier=win
REM IF %errorlevel% neq 0 exit /b %errorlevel%