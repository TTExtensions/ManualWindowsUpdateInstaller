@echo off

REM This build script allows you to build the Manual Windows Update Installer.
REM For information about prerequisites, see the Wiki page at
REM https://github.com/TTExtensions/ManualWindowsUpdateInstaller/wiki/Running-the-Update-Installer


SetLocal ENABLEDELAYEDEXPANSION
REM Change the working directory to the script's directory.
REM E.g. if the user right-clicks on the script and selects "Run as Administrator",
REM the working directory would be the windows\system32 dir.
cd /d "%~dp0"

echo.Building the Manual Windows Update Installer...
echo.
REM MSBuild is always installed in the 32-Bit program files folder
if "!ProgramFiles(x86)!"=="" (
	set "ProgramFiles32Bit=!ProgramFiles!"
) else (
	set "ProgramFiles32Bit=!ProgramFiles(x86)!"
)
set "BuildExe=!ProgramFiles32Bit!\MSBuild\14.0\Bin\MSBuild.exe"

if not exist "!BuildExe!" (
	echo.ERROR: MSBuild not found at "!BuildExe!"^^!
	pause
	exit /b 1
)

REM Note that we need to specify both "Configuration" and "Platform" parameters, because
REM otherwise MSBuild will fill missing parameters from environment variables (and some
REM systems may have set a "Platform" variable).
"!BuildExe!" /v:minimal /nologo /p:Configuration=Release /p:Platform=AnyCPU "WindowsUpdateManualInstaller\WindowsUpdateManualInstaller.csproj"
if not errorlevel 1 (
	echo.
	echo.Build successful^^!
)
pause
exit /b !ERRORLEVEL!
