@echo off
./msbuild ./src/UptimeManager.sln /t:Restore /p:Configuration=Release
./msbuild ./src/UptimeManager.sln /t:Build /p:Configuration=Release

