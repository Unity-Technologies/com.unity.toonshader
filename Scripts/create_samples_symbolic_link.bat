@echo off
setlocal enabledelayedexpansion

:: Base paths for symbolic links based on folder names
set hdrp_target=..\..\..\com.unity.toonshader\Samples~\HDRP
set urp_target=..\..\..\com.unity.toonshader\Samples~\URP
set builtin_target=..\..\..\com.unity.toonshader\Samples~\BuiltIn

:: Directory to operate on
set test_projects_dir=TestProjects

:: Loop through all folders in the TestProjects directory
for /d %%F in ("%test_projects_dir%\*") do (
    :: Get the folder name
    set folder_name=%%~nF

    :: Determine which target to use based on the folder name
    if /i "!folder_name!"=="HDRP" (
        set target_path=%hdrp_target%
    ) else if /i "!folder_name!"=="Universal" (
        set target_path=%urp_target%
    ) else if /i "!folder_name!"=="Legacy" (
        set target_path=%builtin_target%
    ) else (
        :: Skip folders that don't match the criteria
        echo Skipping folder %%F (no matching prefix)
        set target_path=
    )

    :: Create the symbolic link if a target path is defined
    if defined target_path (
        echo Creating symbolic link "Samples" in "%%F" targeting "!target_path!"
        
		rem mklink /d "%%F\Samples" "!target_path!"
    )
)

echo All done!
pause
