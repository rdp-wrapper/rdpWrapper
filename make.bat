@echo off

FOR /F "tokens=* USEBACKQ" %%F IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) DO (
SET msbuild="%%F"
)
ECHO %msbuild%

@%msbuild% rdpWrapper.sln /t:restore /p:RestorePackagesConfig=true /p:Configuration=Release /p:Platform="Any CPU"
if errorlevel 1 goto error

rem dotnet build rdpWrapper.sln -c Release -p:DebugType=None -p:Platform="Any CPU"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:DebugType=None /p:Configuration=Release /p:Platform="Any CPU"
if errorlevel 1 goto error

if [%1]==[all]  (

rem dotnet build rdpWrapper.sln -c ReleaseLite -p:DebugType=None -p:Platform="Any CPU"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:DebugType=None /p:Configuration=ReleaseLite /p:Platform="Any CPU"
if errorlevel 1 goto error


@%msbuild% rdpWrapper.sln /t:restore /p:RestorePackagesConfig=true /p:Configuration=Release /p:Platform="x64"
if errorlevel 1 goto error

rem dotnet build rdpWrapper.sln -c Release -p:DebugType=None -p:Platform="x64"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:DebugType=None /p:Configuration=Release /p:Platform="x64"
if errorlevel 1 goto error

rem dotnet build rdpWrapper.sln -c ReleaseLite -p:DebugType=None -p:Platform="x64"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:DebugType=None /p:Configuration=ReleaseLite /p:Platform="x64"
if errorlevel 1 goto error


@%msbuild% rdpWrapper.sln /t:restore /p:RestorePackagesConfig=true /p:Configuration=Release /p:Platform="x86"
if errorlevel 1 goto error

rem dotnet build rdpWrapper.sln -c Release -p:DebugType=None -p:Platform="x86"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:DebugType=None /p:Configuration=Release /p:Platform="x86"
if errorlevel 1 goto error

rem dotnet build rdpWrapper.sln -c ReleaseLite -p:DebugType=None -p:Platform="x86"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:DebugType=None /p:Configuration=ReleaseLite /p:Platform="x86"
if errorlevel 1 goto error
)

if errorlevel 1 goto error

goto exit
:error
pause
:exit
