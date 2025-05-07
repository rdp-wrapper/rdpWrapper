<!-- : Begin batch script
@echo off
setLocal EnableExtensions
setlocal EnableDelayedExpansion

set rdpwrap_ini="%PROGRAMFILES%\RDP Wrapper\rdpwrap.ini"
set rdpwrap_templete_ini="%~dp0rdpwrap_templete.ini"
set rdpwrap_new_ini="%~dp0rdpwrap_new.ini"

REM ---------------------------------------------------------------
REM Get file version of %windir%\System32\termsrv.dll
REM ---------------------------------------------------------------
for /f "tokens=* usebackq" %%a in (
    `cscript //nologo "%~f0?.wsf" //job:fileVersion "%windir%\System32\termsrv.dll"`
) do (
    set termsrv_dll_ver=%%a
)
if "%termsrv_dll_ver%"=="" (
    echo [x] Error on getting the file version of "%windir%\System32\termsrv.dll"^^!
    exit /b 1
) else (
    echo [+] Installed "termsrv.dll" version: %termsrv_dll_ver%.
)

REM ---------------------------------------------------------------
REM Check if installed termsrv.dll version exists in rdpwrap.ini
REM ---------------------------------------------------------------
:check_update
if exist %rdpwrap_ini% (
    echo [*] Start searching [%termsrv_dll_ver%] version entry in file %rdpwrap_ini%...
    findstr /c:"[%termsrv_dll_ver%]" %rdpwrap_ini% >nul&&(
        echo [+] Found version entry [%termsrv_dll_ver%] in file %rdpwrap_ini%.
        echo [*] RDP Wrapper seems to be up-to-date and working...
        exit /b 0
    )||(
        echo [-] NOT found version entry [%termsrv_dll_ver%] in file %rdpwrap_ini%^^!
        )
    )
) else (
    echo [-] File NOT found: %rdpwrap_ini%.
)

REM --------------------------------------------------------------------
REM Autogenerate up-to-date version of rdpwrap.ini
REM --------------------------------------------------------------------
echo.
echo [*] Autogenerating latest version of rdpwrap.ini...
copy /Y %rdpwrap_templete_ini% %rdpwrap_new_ini%
"%~dp0RDPWrapOffsetFinder.exe" >> %rdpwrap_new_ini%
findstr /c:"ERROR" %rdpwrap_new_ini% >nul && (
    echo [-] FAILED to generate latest version of rdpwrap.ini^^!
    exit /b 1
) || (
    echo [+] Successfully generated latest version to %rdpwrap_new_ini%.
    net stop UmRdpService
    sc stop termservice
    sc config termservice start=disabled
    taskkill /F /FI "MODULES eq termsrv.dll"
    move /Y %rdpwrap_new_ini% %rdpwrap_ini%
    icacls %rdpwrap_ini% /inheritance:e
    sc config termservice start=demand
    sc start termservice
)
exit /b 0

--- Begin wsf script --- fileVersion --->
<package>
  <job id="fileVersion"><script language="VBScript">
    set args = WScript.Arguments
    Set fso = CreateObject("Scripting.FileSystemObject")
    WScript.Echo fso.GetFileVersion(args(0))
    Wscript.Quit
  </script></job>
</package>
