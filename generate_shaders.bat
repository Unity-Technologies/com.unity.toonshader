@echo off
echo Unity Toon Shader Generator
echo ==========================
echo.

echo Generating shader files from common properties...
python3 generate_shaders.py

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Shader generation completed successfully!
    echo Both UnityToon.shader and UnityToonTessellation.shader have been updated.
    echo Original files have been backed up with .backup extension.
) else (
    echo.
    echo Shader generation failed!
    echo Check the error messages above.
)

pause