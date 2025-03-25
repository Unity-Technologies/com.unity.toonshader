@echo off
setlocal enabledelayedexpansion

:: Base paths for symbolic links based on folder names
set hdrp_target=..\..\..\com.unity.toonshader\Samples~\HDRP
set urp_target=..\..\..\com.unity.toonshader\Samples~\URP
set builtin_target=..\..\..\com.unity.toonshader\Samples~\Legacy

set hdrp_sample_symlink_name=SamplesHDRP
set urp_sample_symlink_name=SamplesURP
set builtin_sample_symlink_name=SamplesBuiltIn

:: Directory to operate on
set test_projects_dir=TestProjects

:: Loop through all folders in the TestProjects directory
for /d %%F in ("%test_projects_dir%\*") do (
    :: Extract only the folder name (without the path)
    set folder_name=%%~nF

    :: Check if the folder name contains "HDRP", "Universal", or "Legacy"
    set target_path=
	set symlink_name=
    echo !folder_name! | findstr /i "HDRP" >nul
    if !errorlevel! equ 0 (
        set target_path=%hdrp_target%
        set symlink_name=%hdrp_sample_symlink_name%
    ) else (
        echo !folder_name! | findstr /i "Universal" >nul
        if !errorlevel! equ 0 (
            set target_path=%urp_target%
            set symlink_name=%urp_sample_symlink_name%
        ) else (
            echo !folder_name! | findstr /i "Legacy" >nul
            if !errorlevel! equ 0 (
                set target_path=%builtin_target%
                set symlink_name=%builtin_sample_symlink_name%
            )
        )
    )
		

    :: Create the symbolic link if a target path is defined
    if defined target_path (
	
        :: Store the full path to the symbolic link in a variable
        set symlink_full_path=%%F\Assets\!symlink_name!

        if exist "!symlink_full_path!" (
            rem echo Deleting existing link or folder: "!symlink_full_path!"
            rmdir "!symlink_full_path!"
        )
        rem echo Creating symbolic link "!symlink_full_path!" targeting "!target_path!"
        mklink /d "!symlink_full_path!" "!target_path!"
		
    )
)

echo All done!
