@echo off
REM Creates directory symlinks in each TestProjects/*/Assets folder.
REM
REM Common to every project:
REM     Resources -> com.unity.toon-resources
REM     XR        -> com.unity.toon-xr-settings
REM     Textures  -> com.unity.toon-textures
REM
REM Render-pipeline specific (detected from the project folder name):
REM   Built-In:
REM     ToonSamples     -> com.unity.toonshader\Samples~\Built-In_RP
REM     ReferenceImages -> com.unity.toon-reference-images\Built-In
REM     Scenes          -> com.unity.toon-scenes\Built-In
REM   URP:
REM     ToonSamplesURP  -> com.unity.toonshader\Samples~\URP
REM     ReferenceImages -> com.unity.toon-reference-images\URP
REM     Scenes          -> com.unity.toon-scenes\URP
REM   HDRP:
REM     ToonSamplesHDRP -> com.unity.toonshader\Samples~\HDRP
REM     ReferenceImages -> com.unity.toon-reference-images\HDRP
REM     Scenes          -> com.unity.toon-scenes\HDRP
REM
REM Run from anywhere (double-click or from a cmd prompt). Must be run as a normal
REM user with Developer Mode enabled, or from an elevated prompt, for mklink to work.

setlocal enabledelayedexpansion

REM Repo root is the parent of this script's folder (Scripts\..)
set "REPO_ROOT=%~dp0.."
pushd "%REPO_ROOT%" || (echo Could not cd to repo root & exit /b 1)
set "REPO_ROOT=%CD%"
popd

set "TESTPROJECTS=%REPO_ROOT%\TestProjects"
if not exist "%TESTPROJECTS%" (
    echo ERROR: TestProjects folder not found at "%TESTPROJECTS%"
    exit /b 1
)

echo Repo root: %REPO_ROOT%
echo.

for /d %%P in ("%TESTPROJECTS%\*") do (
    set "PNAME=%%~nxP"
    set "ASSETS=%%~fP\Assets"
    if exist "!ASSETS!" (
        echo === !PNAME! ===

        REM --- common links ---
        call :makelink "!ASSETS!" Resources com.unity.toon-resources
        call :makelink "!ASSETS!" XR        com.unity.toon-xr-settings
        call :makelink "!ASSETS!" Textures  com.unity.toon-textures

        REM --- detect render pipeline from the project folder name ---
        set "PIPE="
        if not "!PNAME:HDRP=!"=="!PNAME!" (
            set "PIPE=HDRP"
        ) else if not "!PNAME:URP=!"=="!PNAME!" (
            set "PIPE=URP"
        ) else if not "!PNAME:BuiltIn=!"=="!PNAME!" (
            set "PIPE=BuiltIn"
        )

        REM --- pipeline specific links ---
        if /i "!PIPE!"=="BuiltIn" (
            call :makelink "!ASSETS!" ToonSamples     com.unity.toonshader\Samples~\Built-In_RP
            call :makelink "!ASSETS!" ReferenceImages com.unity.toon-reference-images\Built-In
            call :makelink "!ASSETS!" Scenes          com.unity.toon-scenes\Built-In
        ) else if /i "!PIPE!"=="URP" (
            REM The ECS project is peculiar - it does not get the ToonSamplesURP link.
            if "!PNAME:ECS=!"=="!PNAME!" (
                call :makelink "!ASSETS!" ToonSamplesURP  com.unity.toonshader\Samples~\URP
            ) else (
                echo   [skip] ToonSamplesURP not needed for ECS project
            )
            call :makelink "!ASSETS!" ReferenceImages com.unity.toon-reference-images\URP
            call :makelink "!ASSETS!" Scenes          com.unity.toon-scenes\URP
        ) else if /i "!PIPE!"=="HDRP" (
            call :makelink "!ASSETS!" ToonSamplesHDRP com.unity.toonshader\Samples~\HDRP
            call :makelink "!ASSETS!" ReferenceImages com.unity.toon-reference-images\HDRP
            call :makelink "!ASSETS!" Scenes          com.unity.toon-scenes\HDRP
        ) else (
            echo   [warn] could not detect render pipeline from folder name - skipping pipeline links
        )
    ) else (
        echo [no Assets] !PNAME!
    )
)

echo.
echo Done.
endlocal
exit /b 0

REM ---------------------------------------------------------------------------
REM :makelink <assetsDir> <linkName> <targetRelativeToRepoRoot>
REM Creates "<assetsDir>\<linkName>" -> ..\..\..\<target> if not present.
:makelink
set "_ASSETS=%~1"
set "_NAME=%~2"
set "_TARGET=%~3"
if exist "%_ASSETS%\%_NAME%" (
    echo   [skip] %_NAME% already exists
) else (
    pushd "%_ASSETS%"
    mklink /d "%_NAME%" "..\..\..\%_TARGET%"
    popd
)
goto :eof
