@echo off

FOR /F "tokens=* USEBACKQ" %%F IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) DO (
SET msbuild="%%F"
)
ECHO %msbuild%

@%msbuild% rdpWrapper.sln /t:Rebuild /p:Configuration=Release /p:DebugType=None /p:Platform="Any CPU"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:Configuration=ReleaseLite /p:DebugType=None /p:Platform="Any CPU"

@%msbuild% rdpWrapper.sln /t:Rebuild /p:Configuration=Release /p:DebugType=None /p:Platform="x64"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:Configuration=ReleaseLite /p:DebugType=None /p:Platform="x64"

@%msbuild% rdpWrapper.sln /t:Rebuild /p:Configuration=Release /p:DebugType=None /p:Platform="x86"
@%msbuild% rdpWrapper.sln /t:Rebuild /p:Configuration=ReleaseLite /p:DebugType=None /p:Platform="x86"

@if errorlevel 1 goto error

@goto exit
:error
@pause

:exit
del /S /Q ".\rdpWrapper\bin\*.pdb"
