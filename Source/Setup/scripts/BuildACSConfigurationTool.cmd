@echo off

echo.
echo ========= Building ACSConfigurationTool solution =========

set solutionDir="%~dp0..\..\Assets\ACSConfiguration\ACSConfigurationTool.sln"
set buildType=Release
set verbosity=quiet

SET msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

call %msBuildDir% %solutionDir% /t:Rebuild /p:Configuration=%buildType% /verbosity:%verbosity% /p:Platform="Any CPU"

echo.
echo Done