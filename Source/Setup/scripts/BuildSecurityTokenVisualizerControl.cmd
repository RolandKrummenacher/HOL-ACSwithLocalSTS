@echo off

echo.
echo ========= Building SecurityTokenVisualizerControl solution =========

set solutionDir="%~dp0..\..\Assets\SecurityTokenVisualizerControl\SecurityTokenVisualizerControl.sln"
set buildType=Release
set verbosity=quiet

SET msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

call %msBuildDir% %solutionDir% /t:Rebuild /p:Configuration=%buildType% /verbosity:%verbosity% /p:Platform="Any CPU"

echo.
echo ========= Installing SecurityTokenVisualizerControl =========

"%~dp0ToolboxInstaller.exe" uninstall "DPE Identity Samples" "SecurityTokenVisualizerControl"
"%~dp0ToolboxInstaller.exe" install "DPE Identity Samples" "%~dp0..\..\Assets\SecurityTokenVisualizerControl\Microsoft.Samples.DPE.Identity.Controls\bin\%buildType%\SecurityTokenVisualizerControl.dll"

REM Copy SecurityTokenVisualizerControl.dll to end solutions
echo.
md "%~dp0..\..\Ex01-ACSLabsV2Federation\End\WebSiteAdvancedACS\Bin\"
copy "%~dp0..\..\Assets\SecurityTokenVisualizerControl\Microsoft.Samples.DPE.Identity.Controls\bin\%buildType%\SecurityTokenVisualizerControl.dll" "%~dp0..\..\Ex01-ACSLabsV2Federation\End\WebSiteAdvancedACS\Bin\"

echo.
echo Done