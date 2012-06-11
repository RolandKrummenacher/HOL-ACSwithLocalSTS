@echo off

set OS_VERSION=
set OS_MAJOR=
set OS_MINOR=
set OS_BUILD=
set IsVistaOrHigher=
set IsWin7OrHigher=
set IsWin2K3=
set OS_ARCHITECTURE=
Set Script_Path=

for /f "skip=1" %%i in ( 'wmic os get version' ) do ( 
    set OS_VERSION=%%i 
    goto:__ver_done
)
:__ver_done

for /f "delims=. tokens=1,2,3" %%i in ("%OS_VERSION%") do ( 
    set OS_MAJOR=%%i&set OS_MINOR=%%j&set OS_BUILD=%%k  
    goto :__ver_split_done
)
:__ver_split_done

if "%OS_MAJOR%" GEQ "6" (
    set IsVistaOrHigher=true
    if "%OS_MINOR%" == "1" (
        set IsWin7OrHigher=true
        goto :__ver_set_done
    )
    if "%OS_MAJOR%" GTR "6" (
        set IsWin7OrHigher=true
        goto :__ver_set_done
    )
)

if "%OS_MAJOR%" == "5" (
    if "%OS_MINOR%" == "2" (
        set IsWin2K3=true
    )
    goto :__ver_set_done
)

:__ver_set_done

for /f "skip=1" %%i in ( 'wmic os get OSArchitecture' ) do ( 
    set OS_ARCHITECTURE=%%i 
    goto:__arch_done
)
:__arch_done

set OS_ARCHITECTURE=%OS_ARCHITECTURE: =%
set Script_Path=Scripts\x64

if "%OS_ARCHITECTURE%" == "32-bit" (    
    set Script_Path=Scripts\x86
    goto :__script_path_done
)
:__script_path_done

echo.
echo This script will perform the following steps:
echo.
echo STEP 1: Delete the virtual directories if they exist:FabrikamWebSite, ContosoAuthenticationServer and  CustomerService .
echo.
echo STEP 2: Create the virtual directories.
echo.


echo ----------------------------------------------------------------------
echo --- STEP 1: Delete the virtual directories if they exist. ---
echo ----------------------------------------------------------------------

 "%systemroot%\system32\inetsrv\AppCmd.exe" delete app "Default Web Site/FabrikamWebSite"
 "%systemroot%\system32\inetsrv\AppCmd.exe" delete app "Default Web Site/ContosoAuthenticationServer"
 "%systemroot%\system32\inetsrv\AppCmd.exe" delete app "Default Web Site/CustomersService"   

echo ----------------------------------------------------------------------
echo --- STEP 2: Create the virtual directories. ---
echo ----------------------------------------------------------------------


setlocal

chdir /d "%~dp0..\.."
"%systemroot%\system32\inetsrv\AppCmd.exe" add app /site.name:"Default Web Site" /path:/FabrikamWebSite /physicalPath:"%CD%\Ex02-RestServiceWithACS\Begin\FabrikamWebSite"   /applicationPool:"ASP.NET v4.0"
"%systemroot%\system32\inetsrv\AppCmd.exe" add app /site.name:"Default Web Site" /path:/ContosoAuthenticationServer /physicalPath:"%CD%\Ex02-RestServiceWithACS\Begin\ContosoAuthenticationServer" /applicationPool:"ASP.NET v4.0"
"%systemroot%\system32\inetsrv\AppCmd.exe" add app /site.name:"Default Web Site" /path:/CustomersService /physicalPath:"%CD%\Ex02-RestServiceWithACS\Begin\CustomerService" /applicationPool:"ASP.NET v4.0"   

echo Done.

:end

