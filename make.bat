@echo off

dotnet build rdpWrapper.sln -c Release -p:DebugType=None -p:Platform="Any CPU"
dotnet build rdpWrapper.sln -c ReleaseLite -p:DebugType=None -p:Platform="Any CPU"

dotnet build rdpWrapper.sln -c Release -p:DebugType=None -p:Platform="x64"
dotnet build rdpWrapper.sln -c ReleaseLite -p:DebugType=None -p:Platform="x64"

dotnet build rdpWrapper.sln -c Release -p:DebugType=None -p:Platform="x86"
dotnet build rdpWrapper.sln -c ReleaseLite -p:DebugType=None -p:Platform="x86"
