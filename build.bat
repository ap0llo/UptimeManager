@echo off

for /f "usebackq tokens=*" %%i in (`tools\vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)

set MSBuildPath="%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe"

%MSBuildPath% ./src/UptimeManager.sln /t:Restore /p:Configuration=Release
%MSBuildPath% ./src/UptimeManager.sln /t:Build /p:Configuration=Release

