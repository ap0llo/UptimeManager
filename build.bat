@echo off
CALL ./msbuild ./src/UptimeManager.sln /t:Restore /p:Configuration=Release
CALL ./msbuild ./src/UptimeManager.sln /t:Build /p:Configuration=Release

