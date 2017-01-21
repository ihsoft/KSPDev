@echo off
@REM Until new major version of C# is released the build number in the path will keep counting.
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild ..\Sources\LogConsole\KSPDev_LogConsole.csproj /t:Rebuild /p:Configuration=Release
